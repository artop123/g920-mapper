using System.Reflection;
using System.Text.Json;
using g920_mapper.Models;

namespace g920_mapper.Actions
{
	public class ReadSettingsAction
	{
		private readonly string _path;

		public ReadSettingsAction(string path)
		{
			_path = path;
		}

		private byte? ReadByteSetting(string prompt)
		{
			Console.WriteLine(prompt);
			var key = Console.ReadKey(intercept: true);

			return key.Key switch
			{
				ConsoleKey.LeftArrow => (byte?)0x25,
				ConsoleKey.RightArrow => (byte?)0x27,
				ConsoleKey.UpArrow => (byte?)0x26,
				ConsoleKey.DownArrow => (byte?)0x28,
				ConsoleKey.Enter => (byte?)0x0D,
				ConsoleKey.Escape => (byte?)0x1B,
				ConsoleKey.Spacebar => (byte?)0x20,
				ConsoleKey.Tab => (byte?)0x09,
				ConsoleKey.Backspace => (byte?)0x08,
				_ => (byte?)char.ToUpper(key.KeyChar),
			};
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
			var properties = typeof(WheelSettings)
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(p => p.PropertyType == typeof(int));

			Console.WriteLine("No settings found. Please enter the following settings. Leave empty to use default values.");

			foreach (var property in properties)
			{
				var currentValue = (int?)property.GetValue(settings);
				var newValue = ReadSetting($"Enter value for {property.Name} (current {currentValue}): ");

				if (newValue.HasValue)
				{
					property.SetValue(settings, newValue.Value);
				}
			}

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

		private WheelSettings? ReadFromFile(bool writeError = true)
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
				if (writeError)
					Console.WriteLine($"{ex.Message}");
			}

			return null;
		}

		private void SaveToFile(WheelSettings settings)
		{
			var json = JsonSerializer.Serialize(settings);
			File.WriteAllText(_path, json);
		}

		public WheelSettings LoadOrDefault()
			=> ReadFromFile(writeError: false) ?? new WheelSettings();

		public void Save(WheelSettings settings)
		{
			ArgumentNullException.ThrowIfNull(settings);
			SaveToFile(settings);
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
