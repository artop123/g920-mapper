using g920_mapper.Actions;
using g920_mapper.Models;
using SharpDX.DirectInput;

namespace g920_mapper.Services
{
	public class JoystickReaderService : IDisposable
	{
		private readonly DirectInput _directInput;
		private Joystick _joystick;
		private Timer _timer;
		private WheelSettings _settings;
		private KeyboardService _keyboardService;

		public JoystickReaderService(WheelSettings settings)
		{
			_directInput = new DirectInput();
			_keyboardService = new KeyboardService();
			_settings = settings;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			var joystickGuid = Guid.Empty;

			foreach (var deviceInstance in _directInput.GetDevices(DeviceType.Driving, DeviceEnumerationFlags.AttachedOnly))
			{
				joystickGuid = deviceInstance.InstanceGuid;
				break;
			}

			if (joystickGuid == Guid.Empty)
			{
				Console.WriteLine("Wheel not found");
				return Task.CompletedTask;
			}

			_joystick = new Joystick(_directInput, joystickGuid);
			_joystick.Acquire();

			Console.WriteLine("Ready to read keys from the wheel. Enjoy!");

			_timer = new Timer(ReadJoystickState, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(_settings.LoopDuration));

			return Task.CompletedTask;
		}

		private void ReadJoystickState(object state)
		{
			_joystick.Poll();
			var joystickState = _joystick.GetCurrentState();
			var handleWheelAction = new HandleWheelAction()
				.SetSettings(_settings);

			if (joystickState != null)
			{
				var downKeys = handleWheelAction
					.SetJoystick(joystickState)
					.Execute();

				_keyboardService.HandleKeys(downKeys);
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_timer?.Change(Timeout.Infinite, 0);
			return Task.CompletedTask;
		}

		public void Dispose()
		{
			_timer?.Dispose();
			_joystick?.Unacquire();
			_directInput?.Dispose();
		}
	}
}
