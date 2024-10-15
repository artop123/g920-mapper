using System.Reflection;
using g920_mapper.Models;

namespace g920_mapper.Actions
{
	public class DebugAction
	{
		private List<byte> _keys;
		private WheelState? _wheelState;
		private bool _clearConsole;

		public DebugAction()
		{
			_keys = [];
			_clearConsole = true;
		}

		public DebugAction SetClearConsole(bool clearConsole)
		{
			_clearConsole = clearConsole;

			return this;
		}

		public DebugAction SetWheelstate(WheelState? state)
		{
			_wheelState = state;

			return this;
		}

		public DebugAction SetKeys(List<byte> keys)
		{
			ArgumentNullException.ThrowIfNull(keys);

			_keys = keys;

			return this;
		}

		private string GetPaddedValue(object? value, int pad = 50)
		{
			var text = value switch
			{
				null => string.Empty,
				bool boolValue => (boolValue ? "true" : string.Empty),
				_ => value.ToString()
			};

			return text!.PadLeft(pad);
		}

		public void Execute()
		{
			ArgumentNullException.ThrowIfNull(_wheelState);

			if (_clearConsole)
			{
				Console.SetCursorPosition(0, 0);
			}

			var rowWidth = 64;
			var fieldWith = 50;
			var valueWidth = rowWidth - fieldWith;
			var line = new string('-', rowWidth);
			var keys = string.Join(", ", _keys.Select(k => k.ToString()));

			Console.WriteLine($"{DateTime.Now.ToString("G").PadRight(rowWidth)}");
			Console.WriteLine($"{line}");

			var fields = typeof(WheelState).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (var field in fields)
			{
				var value = field.GetValue(_wheelState);
				Console.WriteLine($"{field.Name.PadRight(fieldWith)}{GetPaddedValue(value, valueWidth)}");
			}

			Console.WriteLine($"{line}");
			Console.WriteLine($"{"Sending keys".PadRight(fieldWith)}{GetPaddedValue(keys, valueWidth)}");
		}
	}
}
