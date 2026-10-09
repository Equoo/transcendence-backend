using System.Globalization;
using System.Text;
using KeepGrouped.API.AiBackend.AiClient;
using KeepGrouped.API.AiBackend.Delete;
using KeepGrouped.API.AiBackend.Ingest;
using KeepGrouped.API.Events;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Rag;

public sealed class RagEvent(KeepGroupedDb db, IApiClient client) : RagHandler
{
    public override string Kind => RagItem.EventKey;

    public override async ValueTask Upsert(string id, CancellationToken ct = default)
    {
        await Delete(id, ct);

        var ev = await db.Events
            .AsNoTracking()
            .Include(e => e.Organizer)
            .Include(e => e.Registrations).ThenInclude(r => r.User)
            .Include(e => e.Registrations).ThenInclude(r => r.Role)
            .Include(e => e.EventRoles)
            .SingleOrDefaultAsync(e => e.Id == id, ct);

        if (ev is null)
        {
            return;
        }
        var bytes = Encoding.UTF8.GetBytes(ToText(ev));
        using var stream = new MemoryStream(bytes);
        await client.PostFileAsync<CreateRagResponse>($"documents/{id}", stream, $"Event_{ev.Name}.txt", ct);
    }

    public override async ValueTask Delete(string id, CancellationToken ct = default)
    {
        await client.DeleteFileAsync<DeleteRagResponse>($"documents/{id}", ct);
    }

    private static string ToText(Event ev)
    {
        var culture = CultureInfo.InvariantCulture;
        var sb = new StringBuilder();

        var registered = ev.Registrations.Count;
        var roles = ev.EventRoles.Where(r => r.Name != EventRole.Implicit).Select(r => r.Name).ToList();

        sb.AppendLine($"# Event: {ev.Name}");
        sb.AppendLine();
        sb.AppendLine($"The event \"{ev.Name}\" takes place on {ev.Date.ToString("dddd d MMMM yyyy 'at' HH:mm", culture)} ({ev.Date.ToString("yyyy-MM-dd HH:mm", culture)}).");
        sb.AppendLine($"Location of \"{ev.Name}\": {ev.Location}.");
        sb.AppendLine($"Organizer of \"{ev.Name}\": {ev.Organizer.UserName}.");

        if (ev.Tags.Count > 0)
        {
            sb.AppendLine($"Tags of \"{ev.Name}\": {string.Join(", ", ev.Tags)}.");
        }

        sb.AppendLine();
        sb.AppendLine("## Description");
        sb.AppendLine(string.IsNullOrWhiteSpace(ev.Description)
            ? $"No description was provided for \"{ev.Name}\"."
            : ev.Description.Trim());

        sb.AppendLine();
        sb.AppendLine("## Capacity and availability");
        sb.AppendLine($"\"{ev.Name}\" accepts a maximum of {ev.Size} participants.");
        sb.AppendLine($"{registered} {(registered == 1 ? "person is" : "people are")} currently registered.");
        sb.AppendLine(registered >= ev.Size
            ? $"\"{ev.Name}\" is full: no places are left."
            : $"{ev.Size - registered} places are still available for \"{ev.Name}\".");

        sb.AppendLine();
        sb.AppendLine("## Roles");
        sb.AppendLine(roles.Count > 0
            ? $"Participants of \"{ev.Name}\" can register with one of these roles: {string.Join(", ", roles)}."
            : $"\"{ev.Name}\" has no specific roles: every participant registers without a role.");

        sb.AppendLine();
        sb.AppendLine("## Participants");
        if (registered == 0)
        {
            sb.AppendLine($"Nobody is registered to \"{ev.Name}\" yet.");
        }
        else
        {
            foreach (var reg in ev.Registrations.OrderBy(r => r.RegisteredAt))
            {
                var role = reg.Role.Name != EventRole.Implicit ? $" as {reg.Role.Name}" : string.Empty;
                sb.AppendLine($"- {reg.User.UserName} is registered to \"{ev.Name}\"{role} (since {reg.RegisteredAt.ToString("yyyy-MM-dd", culture)}).");
            }
        }

        return sb.ToString();
    }
}
