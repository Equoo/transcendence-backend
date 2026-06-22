using System.ComponentModel.DataAnnotations;
namespace KeepGrouped.API.Events;

public class EventRole
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = null!;
}

public class CreateRole
{
    [Required]
    public string Name = null!;
}

