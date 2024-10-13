# G920 Controller Input Configuration

This project provides a way to map Logitech G920 racing wheel inputs to keyboard keys by reading settings from a JSON configuration file. This allows for easy customization of key bindings for different racing wheel buttons and actions.

## Configuration File (`wheelkeys.json`)

The configuration file should be in JSON format and allows you to specify the key bindings for various G920 inputs. Below is an example JSON structure with descriptions of each field:

```json
{
    "DefaultRotation": 32767,
    "RotationMinDiff": 1000,
    "LoopDuration": 100,
    "Debug": false,
    "Keys": {
        "WHEEL_ROTATION_LEFT": 37,
        "WHEEL_ROTATION_RIGHT": 39,
        "WHEEL_A": 65,
        "WHEEL_B": 66,
        "WHEEL_X": 88,
        "WHEEL_Y": 89,
        "WHEEL_LB": 81,
        "WHEEL_RB": 87,
        "WHEEL_LSB": 69,
        "WHEEL_RSB": 82,
        "WHEEL_ACTION_RIGHT": 13,
        "WHEEL_ACTION_LEFT": 27,
        "WHEEL_ARROW_UP": 38,
        "WHEEL_ARROW_DOWN": 40,
        "WHEEL_ARROW_LEFT": 37,
        "WHEEL_ARROW_RIGHT": 39
    }
}
```

### Field Descriptions
- **`DefaultRotation`**: Default rotation value for the wheel (no action).
- **`RotationMinDiff`**: Minimum difference in rotation to trigger a keyboard event.
- **`LoopDuration`**: Loop interval for polling input, in milliseconds.
- **`Debug`**: Enable or disable debug messages.
- **`Keys`**: Contains key-value pairs that map G920 inputs to keyboard keys. The values can be given as either hexadecimal strings (e.g., `"0x41"`) or as integers (e.g., `65` for the `A` key).

### Key Mapping
- `WHEEL_ROTATION_LEFT` / `WHEEL_ROTATION_RIGHT`: Mapped to arrow left/right keys (`37`, `39`).
- `WHEEL_A`, `WHEEL_B`, `WHEEL_X`, `WHEEL_Y`: Correspond to the `A`, `B`, `X`, `Y` buttons on the wheel and can be mapped to different keyboard keys (`65` for `A`, etc.).
- `WHEEL_LB` / `WHEEL_RB`: Left and right bumper buttons on the wheel.
- `WHEEL_LSB` / `WHEEL_RSB`: Left and right stick buttons on the wheel.
- `WHEEL_ACTION_RIGHT` / `WHEEL_ACTION_LEFT`: Action buttons, such as ENTER (`13`) and ESC (`27`).
- `WHEEL_ARROW_*`: D-pad arrow buttons mapped to arrow keys (`38`, `40`, `37`, `39`).

## Reference for Key Codes
You can find a complete list of virtual key codes on the Microsoft documentation page:

- [Virtual-Key Codes (Windows)](https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes)

This link provides the hexadecimal codes that can be used in the JSON configuration to map G920 buttons to specific keys on your keyboard.

## Usage
1. Create a JSON configuration file (`wheelkeys.json`) with the desired key bindings. If the JSON file is missing, the application will prompt you to enter the settings manually, and they will be saved as a new JSON configuration file.
2. Place the JSON file in the same directory as the executable.
3. Run the application, and it will use the key mappings specified in the JSON to emulate keyboard inputs from the G920 controller.

## Notes
- You can now provide key values as either hexadecimal strings (e.g., `"0x41"`) or as integers (e.g., `65`), and both formats will be correctly interpreted by the application.
- If the configuration file is missing, the program will prompt for user input to gather all necessary settings and save them to `wheelkeys.json` for future use.
- Make sure the key codes in the JSON file are correct; incorrect values may lead to unexpected behavior.
- Use the provided Microsoft documentation to get the correct values for different keys.

## Development

To set up the project for development, follow these steps:

1. **Clone the Repository**
   ```sh
   git clone <repository-url>
   cd <repository-folder>
   ```

2. **Restore Dependencies**
   Ensure that you have .NET SDK installed. Run the following command to restore dependencies:
   ```sh
   dotnet restore
   ```

3. **Build the Project**
   To build the project, use the following command:
   ```sh
   dotnet build
   ```

4. **Run the Application**
   You can run the application with:
   ```sh
   dotnet run
   ```
