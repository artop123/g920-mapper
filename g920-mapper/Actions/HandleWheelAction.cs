using g920_mapper.Interfaces;
using g920_mapper.Models;
using SharpDX.DirectInput;
using System.Reflection;

namespace g920_mapper.Actions
{
	public class HandleWheelAction
	{
		private IJoystickState? _joystickState;
		private WheelState? _wheelState;
		private WheelKeys? _wheelKeys;

		private int _wheelDiff;
		private int _wheelDefaultRotation;
		private int _pedalsAccelerationValue;
		private int _pedalsBrakeValue;
		private int _pedalsClutchValue;
		private int _defaultInputValue;

		public HandleWheelAction()
		{
		}

		public HandleWheelAction SetJoystick(IJoystickState joystickState)
		{
			ArgumentNullException.ThrowIfNull(joystickState);

			_joystickState = joystickState;

			return this;
		}

		public HandleWheelAction SetJoystick(JoystickState joystickState)
		{
			ArgumentNullException.ThrowIfNull(joystickState);

			_joystickState = new JoystickStateWrapper(joystickState);

			return this;
		}

		public HandleWheelAction SetSettings(WheelSettings settings)
		{
			ArgumentNullException.ThrowIfNull(settings);
			ArgumentNullException.ThrowIfNull(settings.Keys);

			_wheelDefaultRotation = settings.DefaultRotation;
			_wheelDiff = settings.RotationMinDiff;
			_pedalsAccelerationValue = settings.PedalsAccelerationValue;
			_pedalsBrakeValue = settings.PedalsBrakeValue;
			_pedalsClutchValue = settings.PedalsClutchValue;
			_pedalsClutchValue = settings.PedalsClutchValue;
			_defaultInputValue = settings.DefaultValue;
			_wheelKeys = settings.Keys;

			return this;
		}

		private byte GetKeyValue(string key)
		{
			return _wheelKeys != null
				? _wheelKeys.GetValue(key)
				: (byte)0x0;
		}

		private bool GetButtonState(bool[] buttons, int index)
		{
			return buttons.Length > index && buttons[index];
		}

		public HandleWheelAction ParseWheelstate()
		{
			ArgumentNullException.ThrowIfNull(_joystickState);

			int wheelValue = _joystickState.X;
			int accelerator = _joystickState.Y;
			int brake = _joystickState.Sliders.Length > 0 ? _joystickState.Sliders[0] : 0;
			int clutch = _joystickState.RotationZ;
			var buttons = _joystickState.Buttons;
			var povs = _joystickState.PointOfViewControllers;

			int diff = wheelValue - _wheelDefaultRotation;
			var rotated = Math.Abs(diff) > _wheelDiff;

			_wheelState = new WheelState()
			{
				RAW_PEDAL_ACCELERATION = accelerator,
				RAW_PEDAL_BRAKE = brake,
				RAW_PEDAL_CLUTCH = clutch,
				RAW_WHEEL_ROTATION = wheelValue,

				WHEEL_ROTATION_LEFT = rotated && diff < 0,
				WHEEL_ROTATION_RIGHT = rotated && diff > 0,

				WHEEL_A = GetButtonState(buttons, (int)WheelButtonIndex.A),
				WHEEL_B = GetButtonState(buttons, (int)WheelButtonIndex.B),
				WHEEL_X = GetButtonState(buttons, (int)WheelButtonIndex.X),
				WHEEL_Y = GetButtonState(buttons, (int)WheelButtonIndex.Y),
				WHEEL_RB = GetButtonState(buttons, (int)WheelButtonIndex.RB),
				WHEEL_LB = GetButtonState(buttons, (int)WheelButtonIndex.LB),
				WHEEL_ACTION_RIGHT = GetButtonState(buttons, (int)WheelButtonIndex.ActionRight),
				WHEEL_ACTION_LEFT = GetButtonState(buttons, (int)WheelButtonIndex.ActionLeft),
				WHEEL_RSB = GetButtonState(buttons, (int)WheelButtonIndex.RSB),
				WHEEL_LSB = GetButtonState(buttons, (int)WheelButtonIndex.LSB),

				WHEEL_ARROW_UP = povs[0] == (int)POVDirection.Up,
				WHEEL_ARROW_RIGHT = povs[0] == (int)POVDirection.Right,
				WHEEL_ARROW_DOWN = povs[0] == (int)POVDirection.Down,
				WHEEL_ARROW_LEFT = povs[0] == (int)POVDirection.Left,

				WHEEL_ACCELERATOR = accelerator < _pedalsAccelerationValue && accelerator != _defaultInputValue,
				WHEEL_BRAKE = brake < _pedalsBrakeValue && brake != _defaultInputValue,
				WHEEL_CLUTCH = clutch < _pedalsClutchValue && clutch != _defaultInputValue
			};

			return this;
		}

		public WheelState? GetWheelState()
			=> _wheelState;

		public List<byte> Execute()
		{
			var result = new List<byte>();

			var state = GetWheelState();

			if (state != null)
			{
				var keys = typeof(WheelKeys)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance)
					.Select(k => k.Name);

				foreach (var key in keys)
				{
					var stateValue = state.GetValue(key);

					if (stateValue)
					{
						var keyValue = GetKeyValue(key);
						result.Add(keyValue);
					}
				}
			}

			return result
				.Distinct()
				.ToList();
		}
	}
}
