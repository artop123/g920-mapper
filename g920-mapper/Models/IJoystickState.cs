namespace g920_mapper.Models
{
	public interface IJoystickState
	{
		int X { get; }
		int Y { get; }
		int[] Sliders { get; }
		int RotationZ { get; }
		bool[] Buttons { get; }
		int[] PointOfViewControllers { get; }
	}
}
