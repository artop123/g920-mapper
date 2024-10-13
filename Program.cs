using g920_mapper.Actions;
using g920_mapper.Services;

class Program
{
	private static string _filePath = "wheelkeys.json";

	static async Task Main(string[] args)
	{
		var settings = new ReadSettingsAction(_filePath)
			.Execute();

		var joystickReader = new JoystickReaderService(settings);

		await joystickReader.StartAsync(CancellationToken.None);

		Console.WriteLine("Press ESC to stop");

		while (true)
		{
			if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
			{
				await joystickReader.StopAsync(CancellationToken.None);
				break;
			}
		}

		joystickReader.Dispose();
	}
}