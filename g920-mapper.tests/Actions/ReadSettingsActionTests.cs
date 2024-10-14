using FluentAssertions;
using Xunit;

namespace g920_mapper.Actions.Tests
{
	public class ReadSettingsActionTests
	{
		[Fact]
		public void Execute_ShouldReadFromFile_WhenFileExists()
		{
			// Arrange
			var filePath = "Data/wheelkeys1.json";
			var action = new ReadSettingsAction(filePath);

			// Act
			var result = action.Execute();

			// Assert
			result.Should().NotBeNull();
			result.DefaultRotation.Should().Be(1);
			result.RotationMinDiff.Should().Be(2);
			result.LoopDuration.Should().Be(3);
			result.PedalsAccelerationValue.Should().Be(4);
			result.PedalsBrakeValue.Should().Be(5);
			result.PedalsClutchValue.Should().Be(6);
			result.DefaultValue.Should().Be(7);
			result.Debug.Should().BeTrue();
		}

		[Fact]
		public void Execute_ShouldReadKeysFromFile_WhenFileExists()
		{
			// Arrange
			var filePath = "Data/wheelkeys1.json";
			var action = new ReadSettingsAction(filePath);

			// Act
			var result = action.Execute();

			// Assert
			result.Should().NotBeNull();
			result.Keys.Should().NotBeNull();
			result.Keys.WHEEL_A.Should().Be(0x41); 
			result.Keys.WHEEL_B.Should().Be(0x42); 
			result.Keys.WHEEL_ACTION_RIGHT.Should().Be(0x0D);
			result.Keys.WHEEL_ACTION_LEFT.Should().Be(0x1B);
		}
	}
}