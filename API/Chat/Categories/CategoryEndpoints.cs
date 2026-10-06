namespace KeepGrouped.API.Chat;

public static class CategoryEndpoints
{
	public static void MapCategories(this IEndpointRouteBuilder app)
	{
		var categories = app.MapGroup("/categories").WithTags("Categories");

		categories.MapCreateCategory();
		categories.MapListCategories();
		categories.MapGetCategory();
		categories.MapUpdateCategory();
		categories.MapDeleteCategory();
	}
}
