using FluentAssertions;
using g920_mapper.Actions;
using g920_mapper.Models;
using Xunit;

namespace g920_mapper.Tests
{
	public class DebugActionTests
	{
		[Fact]
		public void Execute_WhenWheelStateAndKeysAreSet_ShouldOutputDebugInfo()
		{
			// Arrange
			var wheelState = new WheelState
			{
				RAW_WHEEL_ROTATION = 32767,
				RAW_PEDAL_ACCELERATION = 15000,
				RAW_PEDAL_BRAKE = 10000,
				RAW_PEDAL_CLUTCH = 5000,
				WHEEL_ROTATION_LEFT = true,
				WHEEL_A = true,
				WHEEL_BRAKE = true
			};

			var keys = new List<byte> { 0x41, 0x42 }; // A, B keys

			var debugAction = new DebugAction()
				.SetClearConsole(false) // Console.Clear / Cursor position not working in tests
				.SetWheelstate(wheelState)
				.SetKeys(keys);

			var output = new StringWriter();
			Console.SetOut(output);

			// Act
			debugAction.Execute();

			// Assert
			var consoleOutput = output.ToString();
			consoleOutput.Should().Contain("RAW_WHEEL_ROTATION");
			consoleOutput.Should().Contain("32767");
			consoleOutput.Should().Contain("WHEEL_ROTATION_LEFT");
			consoleOutput.Should().Contain("true");
		}

		[Fact]
		public void Execute_WhenWheelStateIsNull_ShouldThrowArgumentNullException()
		{
			// Arrange
			var debugAction = new DebugAction();

			// Act
			Action act = () => debugAction.Execute();

			// Assert
			act.Should().Throw<ArgumentNullException>();
		}

		[Fact]
		public void SetKeys_WhenKeysAreNull_ShouldThrowArgumentNullException()
		{
			// Arrange
			var debugAction = new DebugAction();

			// Act
			Action act = () => debugAction.SetKeys(null!);

			// Assert
			act.Should().Throw<ArgumentNullException>();
		}
	}
}
