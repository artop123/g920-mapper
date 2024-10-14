using System.Reflection;

namespace g920_mapper.Models
{
	public class WheelKeys
	{
		public byte WHEEL_ROTATION_LEFT { get; set; } = 0x0;
		public byte WHEEL_ROTATION_RIGHT { get; set; } = 0x0;
		public byte WHEEL_A { get; set; } = 0x0;
		public byte WHEEL_B { get; set; } = 0x0;
		public byte WHEEL_X { get; set; } = 0x0;
		public byte WHEEL_Y { get; set; } = 0x0;
		public byte WHEEL_LB { get; set; } = 0x0;
		public byte WHEEL_RB { get; set; } = 0x0;
		public byte WHEEL_LSB { get; set; } = 0x0;
		public byte WHEEL_RSB { get; set; } = 0x0;
		public byte WHEEL_ACTION_RIGHT { get; set; } = 0x0;
		public byte WHEEL_ACTION_LEFT { get; set; } = 0x0;
		public byte WHEEL_ARROW_UP { get; set; } = 0x0;
		public byte WHEEL_ARROW_DOWN { get; set; } = 0x0;
		public byte WHEEL_ARROW_LEFT { get; set; } = 0x0;
		public byte WHEEL_ARROW_RIGHT { get; set; } = 0x0;
		public byte WHEEL_ACCELERATOR { get; set; } = 0x0;
		public byte WHEEL_BRAKE { get; set; } = 0x0;
		public byte WHEEL_CLUTCH { get; set; } = 0x0;

		public byte GetValue(string key)
		{
			var field = typeof(WheelKeys).GetProperty(key, BindingFlags.Public | BindingFlags.Instance);

			if (field != null && field.GetValue(this) is byte value)
			{
				return value;
			}

			throw new ArgumentException($"Field {key} not found");
		}
	}
}
