using FluentAssertions;
using g920_mapper.Models;
using Moq;
using Xunit;
using g920_mapper.Interfaces;

namespace g920_mapper.Actions.Tests
{
	public class HandleWheelActionTests
	{
		[Fact()]
		public void ParseWheelstate_WhenJoystickStateIsProvided_ShouldParseWheelStateCorrectly()
		{
			// Arrange
			var joystickStateMock = new Mock<IJoystickState>();
			joystickStateMock.Setup(js => js.X).Returns(35000);
			joystickStateMock.Setup(js => js.Y).Returns(20000);
			joystickStateMock.Setup(js => js.Sliders).Returns(new[] { 10000 });
			joystickStateMock.Setup(js => js.RotationZ).Returns(15000);
			joystickStateMock.Setup(js => js.Buttons).Returns(new bool[12] { true, false, false, false, false, false, false, false, false, false, false, false });
			joystickStateMock.Setup(js => js.PointOfViewControllers).Returns(new[] { 0 });

			var settings = new WheelSettings
			{
				DefaultRotation = 32767,
				RotationMinDiff = 1000,
				PedalsAccelerationValue = 30000,
				PedalsBrakeValue = 20000,
				PedalsClutchValue = 25000,
				DefaultValue = 32767,
				Keys = new WheelKeys
				{
					WHEEL_ROTATION_LEFT = 0x25,
					WHEEL_ROTATION_RIGHT = 0x27,
					WHEEL_A = 0x41,
					WHEEL_B = 0x42,
					WHEEL_X = 0x58,
					WHEEL_Y = 0x59
				}
			};

			var handleWheelAction = new HandleWheelAction()
				.SetJoystick(joystickStateMock.Object)
				.SetSettings(settings);

			// Act
			handleWheelAction.ParseWheelstate();
			var wheelState = handleWheelAction.GetWheelState();

			// Assert
			wheelState.Should().NotBeNull();
			wheelState!.WHEEL_ROTATION_RIGHT.Should().BeTrue();
			wheelState.WHEEL_ACCELERATOR.Should().BeTrue();
			wheelState.WHEEL_BRAKE.Should().BeTrue();
			wheelState.WHEEL_CLUTCH.Should().BeTrue();
			wheelState.WHEEL_A.Should().BeTrue();
		}

		[Fact]
		public void Execute_WhenWheelStateIsParsed_ShouldReturnCorrectKeyValues()
		{
			// Arrange
			var joystickStateMock = new Mock<IJoystickState>();
			joystickStateMock.Setup(js => js.X).Returns(35000);
			joystickStateMock.Setup(js => js.Y).Returns(20000);
			joystickStateMock.Setup(js => js.Sliders).Returns(new[] { 10000 });
			joystickStateMock.Setup(js => js.RotationZ).Returns(15000);
			joystickStateMock.Setup(js => js.Buttons).Returns(new bool[12] { true, false, false, false, false, false, false, false, false, false, false, false });
			joystickStateMock.Setup(js => js.PointOfViewControllers).Returns(new[] { 0 });
			
			var settings = new WheelSettings
			{
				DefaultRotation = 32767,
				RotationMinDiff = 1000,
				PedalsAccelerationValue = 30000,
				PedalsBrakeValue = 20000,
				PedalsClutchValue = 25000,
				DefaultValue = 32767,
				Keys = new WheelKeys
				{
					WHEEL_ROTATION_LEFT = 0x25,
					WHEEL_ROTATION_RIGHT = 0x27,
					WHEEL_A = 0x41,
					WHEEL_B = 0x42,
					WHEEL_X = 0x58,
					WHEEL_Y = 0x59
				}
			};

			var handleWheelAction = new HandleWheelAction()
				.SetJoystick(joystickStateMock.Object)
				.SetSettings(settings);

			// Act
			handleWheelAction.ParseWheelstate();
			var result = handleWheelAction.Execute();

			// Assert
			result.Should().Contain(new byte[] { 0x27, 0x41 }); // WHEEL_ROTATION_RIGHT and WHEEL_A
		}


		[Fact]
		public void Execute_WhenWheelStateIsParsed_NoDuplicates()
		{
			// Arrange
			var joystickStateMock = new Mock<IJoystickState>();
			joystickStateMock.Setup(js => js.X).Returns(35000);
			joystickStateMock.Setup(js => js.Y).Returns(35000);
			joystickStateMock.Setup(js => js.Sliders).Returns(new[] { 35000 });
			joystickStateMock.Setup(js => js.RotationZ).Returns(35000);
			joystickStateMock.Setup(js => js.PointOfViewControllers).Returns(new[] { 9000 });

			var settings = new WheelSettings
			{
				DefaultRotation = 32767,
				RotationMinDiff = 1000,
				PedalsAccelerationValue = 30000,
				PedalsBrakeValue = 20000,
				PedalsClutchValue = 25000,
				DefaultValue = 32767,
				Keys = new WheelKeys
				{
					WHEEL_ROTATION_LEFT = 0x25,
					WHEEL_ROTATION_RIGHT = 0x27,
					WHEEL_A = 0x41,
					WHEEL_B = 0x42,
					WHEEL_X = 0x58,
					WHEEL_Y = 0x59,
					WHEEL_ARROW_RIGHT = 0x27,
					WHEEL_ARROW_LEFT = 0x25
				}
			};

			var handleWheelAction = new HandleWheelAction()
				.SetJoystick(joystickStateMock.Object)
				.SetSettings(settings);

			// Act
			handleWheelAction.ParseWheelstate();
			var result = handleWheelAction.Execute();

			// Assert
			result.Count.Should().Be(1);
			result.Should().Contain(new byte[] { 0x27 }); // WHEEL_ROTATION_RIGHT and WHEEL_ARROW_RIGHT
		}
	}
}