﻿# Logitech G920 keyboard mapper

![Logitech G920 keyboard mapper](assets/header.jpg)

This project maps Logitech G920 racing wheel inputs to keyboard keys, enabling the wheel to be used for older games that require traditional keyboard input (e.g., arrow keys).

The application reads input data from the controller using DirectInput (DirectX API). While designed for the G920, it may work with other DirectInput-compatible controllers, though adjustments might be needed for different layouts.

## Configuration File (`wheelkeys.json`)

The configuration file should be in JSON format and allows you to specify the key bindings for various G920 inputs. Below is a complete example JSON structure with descriptions of each field:

```json
{
    "DefaultRotation": 32767,
    "RotationMinDiff": 1000,
    "LoopDuration": 100
    "PedalsAccelerationValue": 50000,
    "PedalsBrakeValue": 50000,
    "PedalsClutchValue": 50000,
    "DefaultValue": 32767,
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
        "WHEEL_ARROW_RIGHT": 39,
        "WHEEL_ACCELERATOR": 38,
        "WHEEL_BRAKE": 40,
        "WHEEL_CLUTCH": 90
    }
}
```

### Field Descriptions
- **`DefaultRotation`**: Default rotation value for the wheel (no action).
- **`RotationMinDiff`**: How much rotation is required to trigger a keyboard event.
- **`LoopDuration`**: Loop interval for polling input, in milliseconds.
- **`PedalsAccelerationValue`**: Acceleration pedal rotation to trigger a keyboard event.
- **`PedalsBrakeValue`**: Brake pedal rotation to trigger a keyboard event.
- **`PedalsClutchValue`**: Clutch pedal rotation to trigger a keyboard event.
- **`DefaultValue`**: Ignored pedal value (initial pedal rotation).
- **`Debug`**: Enable or disable debug messages.
- **`Keys`**: Contains key-value pairs that map G920 inputs to keyboard keys. Values can be provided as hexadecimal strings (`"0x41"`), integers (`65`), or characters (`"A"`). All these formats will map to the `A` key.

### Key Mapping
- `WHEEL_ROTATION_LEFT` / `WHEEL_ROTATION_RIGHT`: Mapped to arrow left/right keys (`37`, `39`).
- `WHEEL_A`, `WHEEL_B`, `WHEEL_X`, `WHEEL_Y`: Correspond to the `A`, `B`, `X`, `Y` buttons on the wheel and can be mapped to different keyboard keys (`65` for `A`, etc.).
- `WHEEL_LB` / `WHEEL_RB`: Left and right bumper buttons on the wheel.
- `WHEEL_LSB` / `WHEEL_RSB`: Left and right stick buttons on the wheel.
- `WHEEL_ACTION_RIGHT` / `WHEEL_ACTION_LEFT`: Action buttons, such as ENTER (`13`) and ESC (`27`).
- `WHEEL_ARROW_*`: D-pad arrow buttons mapped to arrow keys (`38`, `40`, `37`, `39`).

## Reference for Key Codes

- [ASCII Codes](https://www.asciitable.com/)
- [Hexadecimal Virtual-Key Codes (Windows)](https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes)

## Usage
1. You need to have the [.NET 8 runtime](https://dotnet.microsoft.com/download/dotnet/8.0) installed
2. [Download the latest release](https://github.com/artop123/g920-mapper/releases/latest)
3. Create a JSON configuration file (`wheelkeys.json`) with the desired key bindings. If the JSON file is missing, the application will prompt you to enter the settings manually, and they will be saved as a new JSON configuration file.
4. Place the JSON file in the same directory as the executable.
5. Run the application, and it will use the key mappings specified in the JSON to emulate keyboard inputs from the G920 controller.

Running the application will not make any permanent modifications to the system. The application must always be running on the background while playing.

Antivirus programs may prevent the application from running if downloaded from GitHub. Consider building from the source or adding an exception.

## Debugging

With debugging enabled (`Debug: true`), you can view the values parsed from the controller. This helps you determine the optimal settings for steering and pedals.

![Debugging example](assets/debugging.jpg)

## Development

You need to have the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed

   **Clone the Repository**
   ```sh
   git clone https://github.com/artop123/g920-mapper
   cd g920-mapper
   ```

   **Restore Dependencies**
   ```sh
   dotnet restore
   ```

   **Build the Project**
   ```sh
   dotnet build
   ```

   **Run Tests**
   ```sh
   dotnet test
   ```

   **Run the Application**
   ```sh
   dotnet run --project g920-mapper
   ```

   **Publish the Application**

   ```sh
   dotnet publish g920-mapper/g920-mapper.csproj -p:PublishProfile=Release
   ```
   
   The application (win-x64) will be published to `/g920-mapper/publish/` folder
