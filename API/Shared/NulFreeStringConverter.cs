using System.Text.Json;
using System.Text.Json.Serialization;

namespace KeepGrouped.API;

public sealed class NulFreeStringConverter : JsonConverter<string>
{
	public const string Pattern = @"^[^\0]*$";

	public const string ErrorMessage = "The NUL (U+0000) character is not allowed.";

	public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return Check(reader.GetString());
	}

	public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value);
	}

	public override string ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return Check(reader.GetString())!;
	}

	public override void WriteAsPropertyName(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
	{
		writer.WritePropertyName(value);
	}

	private static string? Check(string? value)
	{
		if (value is not null && value.Contains('\0'))
		{
			throw new JsonException(ErrorMessage);
		}

		return value;
	}
}
