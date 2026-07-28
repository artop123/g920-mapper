using g920_mapper.Actions;
using g920_mapper.Services;
using g920_mapper.UI;
using Terminal.Gui.App;

class Program
{
	private static string _filePath = "wheelkeys.json";

	static void Main(string[] args)
	{
		var settingsAction = new ReadSettingsAction(_filePath);
		var settings = settingsAction.LoadOrDefault();
		using var joystickReader = new JoystickReaderService(settings);
		using IApplication application = Application.Create();

		try
		{
			application.Init();
			using var window = new MapperWindow(application, joystickReader, settings, settingsAction);
			joystickReader.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
			application.Run(window);
		}
		finally
		{
			joystickReader.StopAsync(CancellationToken.None).GetAwaiter().GetResult();
		}
	}
}
