namespace g920_mapper.Models
{
	public class WheelSettings
	{
		public int DefaultRotation { get; set; }
		public int RotationMinDiff { get; set; }
		public int LoopDuration { get; set; }
		public int PedalsAccelerationValue { get; set; }
		public int PedalsBrakeValue { get; set; }
		public int PedalsClutchValue { get; set; }
		public int DefaultValue { get; set; }
		public bool Debug { get; set; }
		public WheelKeys Keys { get; set; }

		public WheelSettings()
		{
			Keys = new WheelKeys();

			DefaultRotation = 32767;
			PedalsAccelerationValue = 32767;
			DefaultValue = 32767;
			PedalsBrakeValue = 32767;
			PedalsClutchValue = 65535;
			RotationMinDiff = 1000;
			LoopDuration = 100;
			Debug = false;
		}
	}
}
