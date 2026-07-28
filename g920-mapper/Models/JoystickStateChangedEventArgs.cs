namespace g920_mapper.Models
{
	public sealed class JoystickStateChangedEventArgs : EventArgs
	{
		public JoystickStateChangedEventArgs(WheelState wheelState, IReadOnlyList<byte> keys)
		{
			WheelState = wheelState;
			Keys = keys;
		}

		public WheelState WheelState { get; }
		public IReadOnlyList<byte> Keys { get; }
	}
}
