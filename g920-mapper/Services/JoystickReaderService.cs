using g920_mapper.Actions;
using g920_mapper.Models;
using SharpDX.DirectInput;

namespace g920_mapper.Services
{
	public class JoystickReaderService : IDisposable
	{
		private readonly DirectInput _directInput;
		private Joystick? _joystick;
		private Timer? _timer;
		private readonly WheelSettings? _settings;
		private readonly KeyboardService? _keyboardService;

		public JoystickReaderService(WheelSettings settings)
		{
			_directInput = new DirectInput();
			_keyboardService = new KeyboardService(new KeyboardInput());
			_settings = settings;
		}

		public event EventHandler<JoystickStateChangedEventArgs>? StateChanged;
		public event EventHandler<JoystickStatusChangedEventArgs>? StatusChanged;

		public bool IsRunning { get; private set; }

		public Task StartAsync(CancellationToken cancellationToken)
		{
			if (IsRunning)
				return Task.CompletedTask;

			if (_joystick != null && _timer != null && _settings != null)
			{
				_timer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(_settings.LoopDuration));
				IsRunning = true;
				NotifyStatus("Connected – reading wheel input", true);
				return Task.CompletedTask;
			}

			var joystickGuid = _directInput
				.GetDevices(DeviceType.Driving, DeviceEnumerationFlags.AttachedOnly)
				.FirstOrDefault()?
				.InstanceGuid;

			if (joystickGuid.HasValue == false)
			{
				NotifyStatus("Wheel not found", false);
				return Task.CompletedTask;
			}

			if (_settings == null)
			{
				NotifyStatus("Invalid settings", false);
				return Task.CompletedTask;
			}

			_joystick = new Joystick(_directInput, joystickGuid.Value);
			_joystick.Acquire();

			_timer = new Timer(ReadJoystickState, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(_settings.LoopDuration));
			IsRunning = true;
			NotifyStatus("Connected – reading wheel input", true);

			return Task.CompletedTask;
		}

		private void ReadJoystickState(object? state)
		{
			if (_joystick == null || _keyboardService == null || _settings == null)
				return;

			_joystick.Poll();
			var joystickState = _joystick.GetCurrentState();
			var handleWheelAction = new HandleWheelAction()
				.SetSettings(_settings);

			if (joystickState != null)
			{
				var downKeys = handleWheelAction
					.SetJoystick(joystickState)
					.ParseWheelstate()
					.Execute();

				_keyboardService.HandleKeys(downKeys);

				var currentWheelState = handleWheelAction.GetWheelState();
				if (currentWheelState != null)
				{
					StateChanged?.Invoke(this, new JoystickStateChangedEventArgs(currentWheelState, downKeys));
				}
			}
		}

		public void ApplySettings()
		{
			if (IsRunning && _settings != null)
			{
				_timer?.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(_settings.LoopDuration));
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_keyboardService?.HandleKeys([]);
			_timer?.Change(Timeout.Infinite, 0);
			IsRunning = false;
			NotifyStatus("Stopped", false);
			return Task.CompletedTask;
		}

		private void NotifyStatus(string status, bool isRunning)
			=> StatusChanged?.Invoke(this, new JoystickStatusChangedEventArgs(status, isRunning));

		public void Dispose()
		{
			_timer?.Dispose();
			_joystick?.Unacquire();
			_directInput?.Dispose();
		}
	}
}
