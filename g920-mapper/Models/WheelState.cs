using System.Reflection;

namespace g920_mapper.Models
{
	public class WheelState
	{
		public int RAW_WHEEL_ROTATION { get; set; }
		public int RAW_PEDAL_ACCELERATION { get; set; }
		public int RAW_PEDAL_BRAKE { get; set; }
		public int RAW_PEDAL_CLUTCH { get; set; }

		public bool WHEEL_ROTATION_LEFT { get; set; }
		public bool WHEEL_ROTATION_RIGHT { get; set; }
		public bool WHEEL_A { get; set; }
		public bool WHEEL_B { get; set; }
		public bool WHEEL_X { get; set; }
		public bool WHEEL_Y { get; set; }
		public bool WHEEL_LB { get; set; }
		public bool WHEEL_RB { get; set; }
		public bool WHEEL_LSB { get; set; }
		public bool WHEEL_RSB { get; set; }
		public bool WHEEL_ACTION_RIGHT { get; set; }
		public bool WHEEL_ACTION_LEFT { get; set; }
		public bool WHEEL_ARROW_UP { get; set; }
		public bool WHEEL_ARROW_DOWN { get; set; }
		public bool WHEEL_ARROW_LEFT { get; set; }
		public bool WHEEL_ARROW_RIGHT { get; set; }
		public bool WHEEL_ACCELERATOR { get; set; }
		public bool WHEEL_BRAKE { get; set; }
		public bool WHEEL_CLUTCH { get; set; }

		public bool GetValue(string key)
		{
			var field = typeof(WheelState).GetProperty(key, BindingFlags.Public | BindingFlags.Instance);

			if (field != null && field.GetValue(this) is bool value)
			{
				return value;
			}

			throw new ArgumentException($"Field {key} not found");
		}
	}
}
