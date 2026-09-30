
namespace KeepGrouped.API.AiBackend.Delete;

public static class DeleteEndpoint
{
	public static void DeleteFileEndpoint(this IEndpointRouteBuilder aibackend)
	{
		var DeleteFile = aibackend.MapGroup("/delete/{id}");
	}
}