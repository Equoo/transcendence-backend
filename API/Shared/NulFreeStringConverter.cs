using System.Text.Json;
using System.Text.Json.Serialization;

namespace KeepGrouped.API;

/// <summary>
/// PostgreSQL text columns cannot store the NUL (U+0000) character.
/// Rejecting it while reading the JSON body turns it into a 400 instead of a database error.
/// </summary>
public sealed class NulFreeStringConverter : JsonConverter<string>
{
	/// <summary>
	/// Same rule for inputs that are not read from JSON (form fields).
	/// </summary>
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

	// A custom string converter is also used for dictionary keys (ProblemDetails errors, headers...).
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
