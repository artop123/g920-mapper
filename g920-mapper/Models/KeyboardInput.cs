using System.Runtime.InteropServices;
using g920_mapper.Interfaces;

namespace g920_mapper.Models
{
	public class KeyboardInput : IKeyboardInput
	{
		[DllImport("user32.dll")]
		private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

		public void KeyPress(byte key, uint flags)
		{
			keybd_event(key, 0, flags, UIntPtr.Zero);
		}
	}
}
