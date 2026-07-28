using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using g920_mapper.Actions;
using g920_mapper.Models;
using g920_mapper.Services;
using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace g920_mapper.UI
{
	public sealed class MapperWindow : Window
	{
		private readonly IApplication _application;
		private readonly JoystickReaderService _joystickReader;
		private readonly WheelSettings _settings;
		private readonly ReadSettingsAction _settingsAction;

		private readonly Label _statusValue = new();
		private readonly Label _wheelValue = new();
		private readonly Label _acceleratorValue = new();
		private readonly Label _brakeValue = new();
		private readonly Label _clutchValue = new();
		private readonly Label _sentKeysValue = new();
		private readonly Label _saveStatusValue = new();

		private readonly ObservableCollection<string> _activeInputs = [];
		private readonly ObservableCollection<string> _settingItems = [];
		private readonly ObservableCollection<string> _mappingItems = [];

		private readonly ListView _activeInputList = new();
		private readonly ListView _settingsList = new();
		private readonly ListView _mappingList = new();

		private readonly PropertyInfo[] _settingProperties;
		private readonly PropertyInfo[] _mappingProperties;

		public MapperWindow(
			IApplication application,
			JoystickReaderService joystickReader,
			WheelSettings settings,
			ReadSettingsAction settingsAction)
		{
			_application = application;
			_joystickReader = joystickReader;
			_settings = settings;
			_settingsAction = settingsAction;

			_settingProperties = typeof(WheelSettings)
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(property => property.PropertyType == typeof(int))
				.OrderBy(property => property.MetadataToken)
				.ToArray();

			_mappingProperties = typeof(WheelKeys)
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(property => property.PropertyType == typeof(byte))
				.OrderBy(property => property.MetadataToken)
				.ToArray();

			Title = "G920 Mapper – wheel status and key mappings (Esc to quit)";

			BuildLayout();
			RefreshSettings();
			RefreshMappings();

			_joystickReader.StateChanged += OnJoystickStateChanged;
			_joystickReader.StatusChanged += OnJoystickStatusChanged;
		}

		private void BuildLayout()
		{
			var statusFrame = new FrameView
			{
				Title = "Wheel status",
				X = 0,
				Y = 0,
				Width = Dim.Percent(50),
				Height = 12
			};

			AddValueRow(statusFrame, "Connection:", _statusValue, 0, "Starting…");
			AddValueRow(statusFrame, "Steering:", _wheelValue, 2);
			AddValueRow(statusFrame, "Accelerator:", _acceleratorValue, 3);
			AddValueRow(statusFrame, "Brake:", _brakeValue, 4);
			AddValueRow(statusFrame, "Clutch:", _clutchValue, 5);
			AddValueRow(statusFrame, "Sent keys:", _sentKeysValue, 7, "–");
			AddValueRow(statusFrame, "Settings:", _saveStatusValue, 8, "Loaded");

			var activeFrame = new FrameView
			{
				Title = "Active wheel inputs",
				X = Pos.Right(statusFrame),
				Y = 0,
				Width = Dim.Fill(),
				Height = 12
			};

			_activeInputList.X = 0;
			_activeInputList.Y = 0;
			_activeInputList.Width = Dim.Fill();
			_activeInputList.Height = Dim.Fill();
			_activeInputList.SetSource(_activeInputs);
			activeFrame.Add(_activeInputList);

			var settingsFrame = new FrameView
			{
				Title = "Settings (Enter or Edit)",
				X = 0,
				Y = Pos.Bottom(statusFrame),
				Width = Dim.Percent(50),
				Height = Dim.Fill(3)
			};

			_settingsList.X = 0;
			_settingsList.Y = 0;
			_settingsList.Width = Dim.Fill();
			_settingsList.Height = Dim.Fill();
			_settingsList.SetSource(_settingItems);
			_settingsList.Accepting += (_, args) =>
			{
				EditSelectedSetting();
				args.Handled = true;
			};
			settingsFrame.Add(_settingsList);

			var mappingsFrame = new FrameView
			{
				Title = "Key mappings (Enter or Edit)",
				X = Pos.Right(settingsFrame),
				Y = Pos.Bottom(activeFrame),
				Width = Dim.Fill(),
				Height = Dim.Fill(3)
			};

			_mappingList.X = 0;
			_mappingList.Y = 0;
			_mappingList.Width = Dim.Fill();
			_mappingList.Height = Dim.Fill();
			_mappingList.SetSource(_mappingItems);
			_mappingList.Accepting += (_, args) =>
			{
				EditSelectedMapping();
				args.Handled = true;
			};
			mappingsFrame.Add(_mappingList);

			var buttonBar = new FrameView
			{
				X = 0,
				Y = Pos.AnchorEnd(3),
				Width = Dim.Fill(),
				Height = 3
			};

			var startButton = CreateButton("_Start", 0, () =>
				_joystickReader.StartAsync(CancellationToken.None).GetAwaiter().GetResult());
			var stopButton = CreateButton("S_top", Pos.Right(startButton) + 1, () =>
				_joystickReader.StopAsync(CancellationToken.None).GetAwaiter().GetResult());
			var editSettingButton = CreateButton("Edit s_etting", Pos.Right(stopButton) + 1, EditSelectedSetting);
			var editMappingButton = CreateButton("Edit _mapping", Pos.Right(editSettingButton) + 1, EditSelectedMapping);
			var saveButton = CreateButton("Sa_ve", Pos.Right(editMappingButton) + 1, SaveSettings);
			var quitButton = CreateButton("_Quit", Pos.Right(saveButton) + 1, _application.RequestStop);

			buttonBar.Add(startButton, stopButton, editSettingButton, editMappingButton, saveButton, quitButton);
			Add(statusFrame, activeFrame, settingsFrame, mappingsFrame, buttonBar);
		}

		private static void AddValueRow(
			View parent,
			string name,
			Label valueLabel,
			int row,
			string initialValue = "–")
		{
			parent.Add(new Label
			{
				Text = name,
				X = 0,
				Y = row,
				Width = 13,
				Height = 1
			});

			valueLabel.Text = initialValue;
			valueLabel.X = 14;
			valueLabel.Y = row;
			valueLabel.Width = Dim.Fill();
			valueLabel.Height = 1;
			parent.Add(valueLabel);
		}

		private static Button CreateButton(string title, Pos x, Action action)
		{
			var button = new Button
			{
				Title = title,
				X = x,
				Y = 0
			};

			button.Accepting += (_, args) =>
			{
				action();
				args.Handled = true;
			};

			return button;
		}

		private void OnJoystickStatusChanged(object? sender, JoystickStatusChangedEventArgs args)
			=> _application.Invoke(() => _statusValue.Text = args.Status);

		private void OnJoystickStateChanged(object? sender, JoystickStateChangedEventArgs args)
		{
			_application.Invoke(() =>
			{
				var state = args.WheelState;
				_wheelValue.Text = state.RAW_WHEEL_ROTATION.ToString(CultureInfo.InvariantCulture);
				_acceleratorValue.Text = state.RAW_PEDAL_ACCELERATION.ToString(CultureInfo.InvariantCulture);
				_brakeValue.Text = state.RAW_PEDAL_BRAKE.ToString(CultureInfo.InvariantCulture);
				_clutchValue.Text = state.RAW_PEDAL_CLUTCH.ToString(CultureInfo.InvariantCulture);
				_sentKeysValue.Text = args.Keys.Count == 0
					? "–"
					: string.Join(", ", args.Keys.Select(VirtualKeyService.Format));

				_activeInputs.Clear();
				foreach (var property in typeof(WheelState)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance)
					.Where(property => property.PropertyType == typeof(bool) && property.GetValue(state) is true))
				{
					var keyProperty = typeof(WheelKeys).GetProperty(property.Name);
					var key = keyProperty?.GetValue(_settings.Keys) is byte value ? value : (byte)0;
					_activeInputs.Add($"{property.Name}  →  {VirtualKeyService.Format(key)}");
				}

				if (_activeInputs.Count == 0)
					_activeInputs.Add("No active wheel inputs");
			});
		}

		private void EditSelectedSetting()
		{
			if (_settingsList.SelectedItem is not int selectedIndex ||
				selectedIndex < 0 ||
				selectedIndex >= _settingProperties.Length)
			{
				ShowError("Select a setting to edit.");
				return;
			}

			var property = _settingProperties[selectedIndex];
			var currentValue = property.GetValue(_settings)?.ToString() ?? string.Empty;
			var input = Prompt(
				$"Edit setting {property.Name}",
				"Enter a new integer value:",
				currentValue);

			if (input == null)
				return;

			if (!TrySetSetting(property, input, out var error))
			{
				ShowError(error);
				return;
			}

			RefreshSettings(selectedIndex);
			SaveSettings();
		}

		private bool TrySetSetting(PropertyInfo property, string input, out string error)
		{
			error = string.Empty;
			var text = input.Trim();

			if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
			{
				error = "The value must be an integer.";
				return false;
			}

			if (property.Name == nameof(WheelSettings.LoopDuration) && intValue <= 0)
			{
				error = "LoopDuration must be greater than zero.";
				return false;
			}

			if (property.Name == nameof(WheelSettings.RotationMinDiff) && intValue < 0)
			{
				error = "RotationMinDiff cannot be negative.";
				return false;
			}

			property.SetValue(_settings, intValue);
			return true;
		}

		private void EditSelectedMapping()
		{
			if (_mappingList.SelectedItem is not int selectedIndex ||
				selectedIndex < 0 ||
				selectedIndex >= _mappingProperties.Length)
			{
				ShowError("Select a key mapping to edit.");
				return;
			}

			var property = _mappingProperties[selectedIndex];
			var currentValue = property.GetValue(_settings.Keys) is byte key ? key : (byte)0;
			var newValue = CaptureMappingKey($"Edit mapping {property.Name}", currentValue);

			if (!newValue.HasValue)
				return;

			property.SetValue(_settings.Keys, newValue.Value);
			RefreshMappings(selectedIndex);
			SaveSettings();
		}

		private string? Prompt(string title, string description, string currentValue)
		{
			using var dialog = new Dialog
			{
				Title = title,
				Width = 72,
				Height = 10
			};

			var descriptionLabel = new Label
			{
				Text = description,
				X = 1,
				Y = 1,
				Width = Dim.Fill(1),
				Height = 2
			};

			var field = new TextField
			{
				Text = currentValue,
				X = 1,
				Y = 4,
				Width = Dim.Fill(1),
				Height = 1
			};

			dialog.Add(descriptionLabel, field);
			dialog.AddButton(new Button { Title = "_Cancel" });
			dialog.AddButton(new Button { Title = "_OK" });
			field.SetFocus();

			_application.Run(dialog);
			return dialog.Result == 1 ? field.Text : null;
		}

		private byte? CaptureMappingKey(string title, byte currentValue)
		{
			using var dialog = new Dialog
			{
				Title = title,
				Width = 64,
				Height = 9
			};

			var descriptionLabel = new Label
			{
				Text = $"Current: {VirtualKeyService.Format(currentValue)}\nPress the key to map. It will be saved immediately.",
				X = 1,
				Y = 1,
				Width = Dim.Fill(1),
				Height = 2
			};

			var captureField = new TextField
			{
				Text = "Waiting for a key…",
				X = 1,
				Y = 4,
				Width = Dim.Fill(1),
				Height = 1
			};

			byte? capturedValue = null;
			captureField.KeyDown += (_, key) =>
			{
				if (!VirtualKeyService.TryFromTerminalKey(key, out var virtualKey))
					return;

				capturedValue = virtualKey;
				key.Handled = true;
				_application.RequestStop(dialog);
			};

			var clearButton = new Button { Title = "C_lear mapping" };
			clearButton.Accepting += (_, args) =>
			{
				capturedValue = 0;
				args.Handled = true;
				_application.RequestStop(dialog);
			};

			dialog.Add(descriptionLabel, captureField);
			dialog.AddButton(clearButton);
			dialog.AddButton(new Button { Title = "_Cancel" });
			captureField.SetFocus();

			_application.Run(dialog);
			return capturedValue;
		}

		private void SaveSettings()
		{
			try
			{
				_settingsAction.Save(_settings);
				_joystickReader.ApplySettings();
				_saveStatusValue.Text = $"Saved {DateTime.Now:HH:mm:ss}";
			}
			catch (Exception ex)
			{
				ShowError($"Unable to save settings:\n{ex.Message}");
			}
		}

		private void RefreshSettings(int? selectedIndex = null)
		{
			var oldSelection = selectedIndex ?? _settingsList.SelectedItem ?? 0;
			_settingItems.Clear();

			foreach (var property in _settingProperties)
			{
				var value = property.GetValue(_settings);
				_settingItems.Add($"{property.Name,-27} {value}");
			}

			if (_settingItems.Count > 0)
				_settingsList.SelectedItem = Math.Clamp(oldSelection, 0, _settingItems.Count - 1);
		}

		private void RefreshMappings(int? selectedIndex = null)
		{
			var oldSelection = selectedIndex ?? _mappingList.SelectedItem ?? 0;
			_mappingItems.Clear();

			foreach (var property in _mappingProperties)
			{
				var value = property.GetValue(_settings.Keys) is byte key ? key : (byte)0;
				_mappingItems.Add($"{property.Name,-27} {VirtualKeyService.Format(value)}");
			}

			if (_mappingItems.Count > 0)
				_mappingList.SelectedItem = Math.Clamp(oldSelection, 0, _mappingItems.Count - 1);
		}

		private void ShowError(string message)
			=> MessageBox.ErrorQuery(_application, "Error", message, "_OK");

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_joystickReader.StateChanged -= OnJoystickStateChanged;
				_joystickReader.StatusChanged -= OnJoystickStatusChanged;
			}

			base.Dispose(disposing);
		}
	}
}
