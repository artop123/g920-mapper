using SharpDX.DirectInput;

namespace g920_mapper.Models
{
	public class JoystickStateWrapper : IJoystickState
	{
		private readonly JoystickState _joystickState;

		public JoystickStateWrapper(JoystickState joystickState)
		{
			_joystickState = joystickState;
		}

		public int X => _joystickState.X;
		public int Y => _joystickState.Y;
		public int[] Sliders => _joystickState.Sliders;
		public int RotationZ => _joystickState.RotationZ;
		public bool[] Buttons => _joystickState.Buttons;
		public int[] PointOfViewControllers => _joystickState.PointOfViewControllers;
	}

}
