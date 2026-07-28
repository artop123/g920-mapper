using System.Globalization;

using Terminal.Gui.Drivers;
using Terminal.Gui.Input;

namespace g920_mapper.Services
{
	public static class VirtualKeyService
	{
		private static readonly IReadOnlyDictionary<KeyCode, byte> TerminalSpecialKeys =
			new Dictionary<KeyCode, byte>
			{
				[KeyCode.Backspace] = 0x08,
				[KeyCode.Tab] = 0x09,
				[KeyCode.Enter] = 0x0D,
				[KeyCode.Esc] = 0x1B,
				[KeyCode.Space] = 0x20,
				[KeyCode.PageUp] = 0x21,
				[KeyCode.PageDown] = 0x22,
				[KeyCode.End] = 0x23,
				[KeyCode.Home] = 0x24,
				[KeyCode.CursorLeft] = 0x25,
				[KeyCode.CursorUp] = 0x26,
				[KeyCode.CursorRight] = 0x27,
				[KeyCode.CursorDown] = 0x28,
				[KeyCode.Insert] = 0x2D,
				[KeyCode.Delete] = 0x2E,
				[KeyCode.F1] = 0x70,
				[KeyCode.F2] = 0x71,
				[KeyCode.F3] = 0x72,
				[KeyCode.F4] = 0x73,
				[KeyCode.F5] = 0x74,
				[KeyCode.F6] = 0x75,
				[KeyCode.F7] = 0x76,
				[KeyCode.F8] = 0x77,
				[KeyCode.F9] = 0x78,
				[KeyCode.F10] = 0x79,
				[KeyCode.F11] = 0x7A,
				[KeyCode.F12] = 0x7B
			};

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

		public static bool TryFromTerminalKey(Key key, out byte value)
		{
			value = key.ModifierKey switch
			{
				ModifierKey.Shift or ModifierKey.LeftShift or ModifierKey.RightShift => 0x10,
				ModifierKey.Ctrl or ModifierKey.LeftCtrl or ModifierKey.RightCtrl => 0x11,
				ModifierKey.Alt or ModifierKey.LeftAlt or ModifierKey.RightAlt or ModifierKey.AltGr => 0x12,
				_ => 0
			};

			if (value != 0)
				return true;

			var unmodifiedKey = key.NoShift.NoCtrl.NoAlt;
			if (TerminalSpecialKeys.TryGetValue(unmodifiedKey.KeyCode, out value))
				return true;

			var rune = unmodifiedKey.AsRune;
			return rune.Value is > 0 and <= char.MaxValue &&
				TryParse(((char)rune.Value).ToString(), out value);
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
