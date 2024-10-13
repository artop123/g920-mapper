using System.Runtime.InteropServices;

namespace g920_mapper.Services
{
	public class KeyboardService
	{
		[DllImport("user32.dll")]
		private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

		private const uint KEYEVENTF_KEYDOWN = 0x0000;
		private const uint KEYEVENTF_KEYUP = 0x0002;

		private List<byte> _oldKeys = [];

		public void HandleKeys(List<byte> keys)
		{
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

				keybd_event(key, 0, flags, UIntPtr.Zero);
			}

			_oldKeys = keys;
		}
	}
}
