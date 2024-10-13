namespace g920_mapper.Models
{
	public class WheelSettings
	{
		public int DefaultRotation { get; set; }
		public int RotationMinDiff { get; set; }
		public int LoopDuration { get; set; }
		public bool Debug { get; set; }
		public WheelKeys Keys { get; set; }

		public WheelSettings()
		{
			Keys = new WheelKeys();

			DefaultRotation = 32767;
			RotationMinDiff = 1000;
			LoopDuration = 100;
			Debug = false;
		}
	}
}
