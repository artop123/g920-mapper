using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using g920_mapper.Actions;
using g920_mapper.Models;
using g920_mapper.Services;
using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.Drivers;
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
		private readonly List<View> _pages = [];
		private readonly List<Button> _navigationButtons = [];

		private readonly PropertyInfo[] _settingProperties;
		private readonly PropertyInfo[] _mappingProperties;
		private int _activePage;

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

			BorderStyle = LineStyle.None;

			BuildLayout();
			RefreshSettings();
			RefreshMappings();

			KeyDown += (_, key) =>
			{
				if (key.NoShift.NoCtrl.NoAlt.KeyCode != KeyCode.Esc)
					return;

				if (_activePage != 0)
					ShowPage(0);

				key.Handled = true;
			};

			_joystickReader.StateChanged += OnJoystickStateChanged;
			_joystickReader.StatusChanged += OnJoystickStatusChanged;
		}

		private void BuildLayout()
		{
			const int sidebarWidth = 25;

			var overviewPage = new View
			{
				X = sidebarWidth,
				Y = 0,
				Width = Dim.Fill(),
				Height = Dim.Fill()
			};

			var statusFrame = new FrameView
			{
				Title = " Wheel status ",
				SchemeName = "Base",
				X = 0,
				Y = 0,
				Width = Dim.Percent(50),
				Height = Dim.Fill()
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
				Title = " Active wheel inputs ",
				SchemeName = "Accent",
				X = Pos.Right(statusFrame),
				Y = 0,
				Width = Dim.Fill(),
				Height = Dim.Fill()
			};

			_activeInputList.X = 1;
			_activeInputList.Y = 1;
			_activeInputList.Width = Dim.Fill(1);
			_activeInputList.Height = Dim.Fill(1);
			_activeInputList.SetSource(_activeInputs);
			activeFrame.Add(_activeInputList);
			overviewPage.Add(statusFrame, activeFrame);

			var settingsPage = new FrameView
			{
				Title = " Settings  •  Enter to edit  •  Esc to return ",
				SchemeName = "Accent",
				X = sidebarWidth,
				Y = 0,
				Width = Dim.Fill(),
				Height = Dim.Fill(),
				Visible = false
			};

			_settingsList.X = 1;
			_settingsList.Y = 1;
			_settingsList.Width = Dim.Fill(1);
			_settingsList.Height = Dim.Fill(1);
			_settingsList.SetSource(_settingItems);
			_settingsList.Accepting += (_, args) =>
			{
				EditSelectedSetting();
				args.Handled = true;
			};
			settingsPage.Add(_settingsList);

			var mappingsPage = new FrameView
			{
				Title = " Key mappings  •  Enter to capture a key  •  Esc to return ",
				SchemeName = "Accent",
				X = sidebarWidth,
				Y = 0,
				Width = Dim.Fill(),
				Height = Dim.Fill(),
				Visible = false
			};

			_mappingList.X = 1;
			_mappingList.Y = 1;
			_mappingList.Width = Dim.Fill(1);
			_mappingList.Height = Dim.Fill(1);
			_mappingList.SetSource(_mappingItems);
			_mappingList.Accepting += (_, args) =>
			{
				EditSelectedMapping();
				args.Handled = true;
			};
			mappingsPage.Add(_mappingList);

			var menuFrame = new FrameView
			{
				Title = " Menu ",
				SchemeName = "Base",
				X = 0,
				Y = 0,
				Width = sidebarWidth - 1,
				Height = Dim.Fill()
			};

			var overviewButton = CreateButton("_Overview", 1, () => ShowPage(0));
			overviewButton.Y = 0;
			overviewButton.Width = Dim.Fill(1);

			var settingsButton = CreateButton("_Settings", 1, () => ShowPage(1));
			settingsButton.Y = 1;
			settingsButton.Width = Dim.Fill(1);

			var mappingsButton = CreateButton("_Key mappings", 1, () => ShowPage(2));
			mappingsButton.Y = 2;
			mappingsButton.Width = Dim.Fill(1);

			var startButton = CreateButton("_Start wheel", 1, () =>
				_joystickReader.StartAsync(CancellationToken.None).GetAwaiter().GetResult());
			startButton.Y = 4;
			startButton.Width = Dim.Fill(1);

			var stopButton = CreateButton("S_top wheel", 1, () =>
				_joystickReader.StopAsync(CancellationToken.None).GetAwaiter().GetResult());
			stopButton.Y = 5;
			stopButton.Width = Dim.Fill(1);

			var quitButton = CreateButton("_Quit", 1, _application.RequestStop);
			quitButton.Y = 7;
			quitButton.Width = Dim.Fill(1);

			menuFrame.Add(
				overviewButton,
				settingsButton,
				mappingsButton,
				startButton,
				stopButton,
				quitButton);

			_pages.AddRange([overviewPage, settingsPage, mappingsPage]);
			_navigationButtons.AddRange([overviewButton, settingsButton, mappingsButton]);

			Add(overviewPage, settingsPage, mappingsPage, menuFrame);
			ShowPage(0);
		}

		private void ShowPage(int pageIndex)
		{
			if (pageIndex < 0 || pageIndex >= _pages.Count)
				return;

			_activePage = pageIndex;

			for (var index = 0; index < _pages.Count; index++)
				_pages[index].Visible = index == _activePage;

			_navigationButtons[0].Title = _activePage == 0 ? "› _Overview" : "  _Overview";
			_navigationButtons[1].Title = _activePage == 1 ? "› _Settings" : "  _Settings";
			_navigationButtons[2].Title = _activePage == 2 ? "› _Key mappings" : "  _Key mappings";

			switch (_activePage)
			{
				case 1:
					_settingsList.SetFocus();
					break;
				case 2:
					_mappingList.SetFocus();
					break;
				default:
					_activeInputList.SetFocus();
					break;
			}
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
				X = 1,
				Y = row + 1,
				Width = 13,
				Height = 1
			});

			valueLabel.Text = initialValue;
			valueLabel.X = 15;
			valueLabel.Y = row + 1;
			valueLabel.Width = Dim.Fill(1);
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
				Title = $" {title} ",
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
				Title = $" {title} ",
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
			=> MessageBox.ErrorQuery(_application, " Error ", message, "_OK");

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
