namespace g920_mapper.Models
{
	public sealed class JoystickStatusChangedEventArgs : EventArgs
	{
		public JoystickStatusChangedEventArgs(string status, bool isRunning)
		{
			Status = status;
			IsRunning = isRunning;
		}

		public string Status { get; }
		public bool IsRunning { get; }
	}
}
