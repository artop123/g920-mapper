using g920_mapper.Models;
using SharpDX.DirectInput;

namespace g920_mapper.Actions
{
	public class HandleWheelAction
	{
		private List<byte> _keys;
		private JoystickState? _joystickState;
		private WheelKeys? _wheelKeys;
		private bool _debug;
		private int _wheelDiff;
		private int _wheelDefaultRotation;

		public HandleWheelAction()
		{
			_keys = [];
			_debug = false;
			_wheelDiff = 0;
			_wheelDefaultRotation = 32767;
		}

		public HandleWheelAction SetJoystick(JoystickState joystickState)
		{
			ArgumentNullException.ThrowIfNull(joystickState);

			_joystickState = joystickState;

			return this;
		}

		public HandleWheelAction SetSettings(WheelSettings settings)
		{
			ArgumentNullException.ThrowIfNull(settings);
			ArgumentNullException.ThrowIfNull(settings.Keys);

			_wheelDefaultRotation = settings.DefaultRotation;
			_wheelDiff = settings.RotationMinDiff;
			_debug = settings.Debug;
			_wheelKeys = settings.Keys;

			return this;
		}

		private byte HandleKey(string key)
		{
			if (_debug)
			{
				Console.WriteLine($"WHEEL KEY: {key}");
			}

			return _wheelKeys != null
				? _wheelKeys.GetValue(key)
				: (byte)0x0;
		}

		private void HandleWheel()
		{
			if (_joystickState == null)
			{
				return;
			}

			int wheelValue = _joystickState.X;
			int diff = wheelValue - _wheelDefaultRotation;

			if (Math.Abs(diff) > _wheelDiff)
			{
				if (diff < 0)
				{
					_keys.Add(HandleKey("WHEEL_ROTATION_LEFT"));
				}
				else
				{
					_keys.Add(HandleKey("WHEEL_ROTATION_RIGHT"));
				}
			}
		}

		private void HandleButtons()
		{
			if (_joystickState == null)
			{
				return;
			}

			var buttons = _joystickState.Buttons;

			if (buttons.Length >= 8)
			{
				if (buttons[0])
					_keys.Add(HandleKey("WHEEL_A"));

				if (buttons[1])
					_keys.Add(HandleKey("WHEEL_B"));

				if (buttons[2])
					_keys.Add(HandleKey("WHEEL_X"));

				if (buttons[3])
					_keys.Add(HandleKey("WHEEL_Y"));

				if (buttons[4])
					_keys.Add(HandleKey("WHEEL_RB"));

				if (buttons[5])
					_keys.Add(HandleKey("WHEEL_LB"));

				if (buttons[6])
					_keys.Add(HandleKey("WHEEL_ACTION_RIGHT"));

				if (buttons[7])
					_keys.Add(HandleKey("WHEEL_ACTION_LEFT"));

				if (buttons[8])
					_keys.Add(HandleKey("WHEEL_RSB"));

				if (buttons[9])
					_keys.Add(HandleKey("WHEEL_LSB"));
			}
		}

		private void HandlePOV()
		{
			if (_joystickState == null)
			{
				return;
			}

			var povs = _joystickState.PointOfViewControllers;

			if (povs.Length > 0)
			{
				var key = povs[0] switch
				{
					0 => "WHEEL_ARROW_UP",
					9000 => "WHEEL_ARROW_RIGHT",
					18000 => "WHEEL_ARROW_DOWN",
					27000 => "WHEEL_ARROW_LEFT",
					_ => ""
				};

				if (string.IsNullOrEmpty(key) == false)
					_keys.Add(HandleKey(key));
			}
		}

		public List<byte> Execute()
		{
			_keys = [];

			HandleWheel();
			HandleButtons();
			HandlePOV();

			return _keys.Distinct().ToList();
		}
	}
}
