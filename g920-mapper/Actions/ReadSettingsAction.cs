using g920_mapper.Models;
using System.Reflection;
using System.Text.Json;

namespace g920_mapper.Actions
{
	public class ReadSettingsAction
	{
		private string _path;

		public ReadSettingsAction(string path)
		{
			_path = path;
		}

		private byte? ReadByteSetting(string prompt)
		{
			Console.WriteLine(prompt);
			var key = Console.ReadKey(intercept: true);

			// Handle arrow keys and other special keys
			switch (key.Key)
			{
				case ConsoleKey.LeftArrow:
					return 0x25;
				case ConsoleKey.RightArrow:
					return 0x27;
				case ConsoleKey.UpArrow:
					return 0x26;
				case ConsoleKey.DownArrow:
					return 0x28;
				case ConsoleKey.Enter:
					return 0x0D;
				case ConsoleKey.Escape:
					return 0x1B;
				case ConsoleKey.Spacebar:
					return 0x20;
				case ConsoleKey.Tab:
					return 0x09;
				case ConsoleKey.Backspace:
					return 0x08;
				default:
					return (byte?)char.ToUpper(key.KeyChar);
			}
		}

		private int? ReadSetting(string prompt)
		{
			Console.Write(prompt);
			var defaultRotation = Console.ReadLine();

			return (!string.IsNullOrEmpty(defaultRotation) && int.TryParse(defaultRotation.Trim(), out int res))
				? res
				: null;
		}

		private WheelSettings ReadFromUser()
		{
			var settings = new WheelSettings();

			Console.WriteLine("No settings found. Please enter the following settings. Leave empty to use default values.");

			settings.DefaultRotation = ReadSetting($"Enter the DefaultRotation value (current: {settings.DefaultRotation}): ")
				?? settings.DefaultRotation;

			settings.RotationMinDiff = ReadSetting($"Enter the RotationMinDiff value (current: {settings.RotationMinDiff}): ")
				?? settings.RotationMinDiff;

			settings.PedalsAccelerationValue = ReadSetting($"Enter the PedalsAccelerationValue value (current: {settings.PedalsAccelerationValue}): ")
				?? settings.PedalsAccelerationValue;

			settings.PedalsBrakeValue = ReadSetting($"Enter the PedalsBrakeValue value (current: {settings.PedalsBrakeValue}): ")
				?? settings.PedalsBrakeValue;

			settings.PedalsClutchValue = ReadSetting($"Enter the PedalsClutchValue value (current: {settings.PedalsClutchValue}): ")
				?? settings.PedalsClutchValue;

			settings.DefaultValue = ReadSetting($"Enter the DefaultValue that is ignored (current: {settings.DefaultValue}): ")
				?? settings.DefaultValue;

			settings.LoopDuration = ReadSetting($"Enter the LoopDuration value (current: {settings.LoopDuration}): ")
				?? settings.LoopDuration;

			settings.Debug = ReadSetting($"Enter the Debug value ({(settings.Debug ? 1 : 0)}): ") == 1;

			foreach (var property in typeof(WheelKeys).GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				var currentValue = (byte)(property.GetValue(settings.Keys) ?? 0);
				var newValue = ReadByteSetting($"Enter value for {property.Name} (default 0x{currentValue:X2}): ");

				if (newValue.HasValue)
				{
					property.SetValue(settings.Keys, newValue.Value);
				}
			}

			return settings;
		}

		private WheelSettings? ReadFromFile()
		{
			try
			{
				var json = File.ReadAllText(_path);
				var options = new JsonSerializerOptions
				{
					Converters = { new JsonStringToByteConverter() }
				};

				return JsonSerializer.Deserialize<WheelSettings>(json, options);
			}

			catch (Exception ex)
			{
				Console.WriteLine($"{ex.Message}");
			}

			return null;
		}

		private void SaveToFile(WheelSettings settings)
		{
			var json = JsonSerializer.Serialize(settings);
			File.WriteAllText(_path, json);
		}

		public WheelSettings Execute()
		{
			var settings = ReadFromFile();

			if (settings == null)
			{
				settings = ReadFromUser();
				SaveToFile(settings);
			}

			return settings;
		}
	}
}
