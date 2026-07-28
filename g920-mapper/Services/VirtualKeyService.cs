using System.Globalization;

namespace g920_mapper.Services
{
	public static class VirtualKeyService
	{
		private static readonly IReadOnlyDictionary<string, byte> NamedKeys =
			new Dictionary<string, byte>(StringComparer.OrdinalIgnoreCase)
			{
				["BACKSPACE"] = 0x08,
				["TAB"] = 0x09,
				["ENTER"] = 0x0D,
				["RETURN"] = 0x0D,
				["SHIFT"] = 0x10,
				["CTRL"] = 0x11,
				["CONTROL"] = 0x11,
				["ALT"] = 0x12,
				["ESC"] = 0x1B,
				["ESCAPE"] = 0x1B,
				["SPACE"] = 0x20,
				["PAGEUP"] = 0x21,
				["PAGEDOWN"] = 0x22,
				["END"] = 0x23,
				["HOME"] = 0x24,
				["LEFT"] = 0x25,
				["UP"] = 0x26,
				["RIGHT"] = 0x27,
				["DOWN"] = 0x28,
				["INSERT"] = 0x2D,
				["DELETE"] = 0x2E,
				["F1"] = 0x70,
				["F2"] = 0x71,
				["F3"] = 0x72,
				["F4"] = 0x73,
				["F5"] = 0x74,
				["F6"] = 0x75,
				["F7"] = 0x76,
				["F8"] = 0x77,
				["F9"] = 0x78,
				["F10"] = 0x79,
				["F11"] = 0x7A,
				["F12"] = 0x7B
			};

		private static readonly IReadOnlyDictionary<byte, string> DisplayNames =
			NamedKeys
				.Where(pair => pair.Key is not "RETURN" and not "CONTROL" and not "ESC")
				.GroupBy(pair => pair.Value)
				.ToDictionary(group => group.Key, group => group.First().Key);

		public static bool TryParse(string? input, out byte value)
		{
			value = 0;
			var text = input?.Trim();

			if (string.IsNullOrEmpty(text))
				return false;

			if (NamedKeys.TryGetValue(text, out value))
				return true;

			if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
			{
				return byte.TryParse(text[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
			}

			if (byte.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
				return true;

			if (text.Length == 1)
			{
				var character = char.ToUpperInvariant(text[0]);
				if (character <= byte.MaxValue)
				{
					value = (byte)character;
					return true;
				}
			}

			return false;
		}

		public static string Format(byte value)
		{
			if (value == 0)
				return "Not mapped (0x00)";

			if (DisplayNames.TryGetValue(value, out var name))
				return $"{name} (0x{value:X2})";

			if (value is >= 0x30 and <= 0x39 or >= 0x41 and <= 0x5A)
				return $"{(char)value} (0x{value:X2})";

			return $"0x{value:X2}";
		}
	}
}
