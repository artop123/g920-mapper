using g920_mapper.Interfaces;

namespace g920_mapper.Services
{
	public class KeyboardService
	{
		private readonly IKeyboardInput? _keyboardInput;

		private const uint KEYEVENTF_KEYDOWN = 0x0000;
		private const uint KEYEVENTF_KEYUP = 0x0002;

		private List<byte> _oldKeys = [];

		public KeyboardService(IKeyboardInput keyboardInput)
		{
			_keyboardInput = keyboardInput;
		}

		public void HandleKeys(List<byte> keys)
		{
			ArgumentNullException.ThrowIfNull(_keyboardInput);

			var allkeys = keys
				.Concat(_oldKeys)
				.Where(key => key != 0x0)
				.Distinct()
				.ToList();

			foreach (var key in allkeys)
			{
				var flags = keys.Contains(key)
						? KEYEVENTF_KEYDOWN
						: KEYEVENTF_KEYUP;

				_keyboardInput.KeyPress(key, flags);
			}

			_oldKeys = keys;
		}
	}
}
