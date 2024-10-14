using System.Text.Json;

namespace g920_mapper.Models
{
	public class JsonStringToByteConverter : System.Text.Json.Serialization.JsonConverter<byte>
	{
		public override byte Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType == JsonTokenType.Number)
			{
				int intValue = reader.GetInt32();
				return (byte)intValue;
			}
			else if (reader.TokenType == JsonTokenType.String)
			{
				var stringValue = reader.GetString();

				if (stringValue != null)
				{
					if (stringValue.StartsWith("0x"))
					{
						return Convert.ToByte(stringValue, 16);
					}
					else if (stringValue.Length == 1)
					{
						return (byte)char.ToUpper(stringValue[0]);
					}

					return Convert.ToByte(stringValue);
				}
			}

			throw new JsonException();
		}

		public override void Write(Utf8JsonWriter writer, byte value, JsonSerializerOptions options)
		{
			writer.WriteNumberValue(value);
		}
	}
}
