using g920_mapper.Interfaces;
using Moq;
using Xunit;

namespace g920_mapper.Services.Tests
{
	public class KeyboardServiceTests
	{
		[Fact]
		public void HandleKeys_ShouldCallKeyPress_WithCorrectParameters()
		{
			// Arrange
			var keyboardInputMock = new Mock<IKeyboardInput>();
			var keyboardService = new KeyboardService(keyboardInputMock.Object);

			var keys = new List<byte> { 0x41, 0x42 }; // A, B keys

			// Act
			keyboardService.HandleKeys(keys);

			// Assert
			keyboardInputMock.Verify(k => k.KeyPress(0x41, 0x0000), Times.Once); // A key down
			keyboardInputMock.Verify(k => k.KeyPress(0x42, 0x0000), Times.Once); // B key down
		}

		[Fact]
		public void HandleKeys_ShouldReleaseOldKeys()
		{
			// Arrange
			var keyboardInputMock = new Mock<IKeyboardInput>();
			var keyboardService = new KeyboardService(keyboardInputMock.Object);

			var initialKeys = new List<byte> { 0x41 }; // A key
			var newKeys = new List<byte> { 0x42 }; // B key

			// Act
			keyboardService.HandleKeys(initialKeys);
			keyboardService.HandleKeys(newKeys);

			// Assert
			keyboardInputMock.Verify(k => k.KeyPress(0x41, 0x0000), Times.Once); // A key down
			keyboardInputMock.Verify(k => k.KeyPress(0x41, 0x0002), Times.Once); // A key up
			keyboardInputMock.Verify(k => k.KeyPress(0x42, 0x0000), Times.Once); // B key down
		}
	}
}
