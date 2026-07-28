using FluentAssertions;
using g920_mapper.Services;
using Xunit;

namespace g920_mapper.Tests.Services
{
	public class VirtualKeyServiceTests
	{
		[Theory]
		[InlineData("A", 0x41)]
		[InlineData("a", 0x41)]
		[InlineData("LEFT", 0x25)]
		[InlineData("escape", 0x1B)]
		[InlineData("0x5A", 0x5A)]
		[InlineData("90", 0x5A)]
		[InlineData("0", 0x00)]
		public void TryParse_WhenValueIsSupported_ShouldReturnVirtualKey(string input, byte expected)
		{
			var result = VirtualKeyService.TryParse(input, out var value);

			result.Should().BeTrue();
			value.Should().Be(expected);
		}

		[Theory]
		[InlineData("")]
		[InlineData("UNKNOWN")]
		[InlineData("0x100")]
		[InlineData("256")]
		public void TryParse_WhenValueIsInvalid_ShouldReturnFalse(string input)
		{
			var result = VirtualKeyService.TryParse(input, out _);

			result.Should().BeFalse();
		}

		[Theory]
		[InlineData(0x00, "Not mapped (0x00)")]
		[InlineData(0x25, "LEFT (0x25)")]
		[InlineData(0x41, "A (0x41)")]
		[InlineData(0xBA, "0xBA")]
		public void Format_ShouldReturnReadableVirtualKey(byte value, string expected)
		{
			VirtualKeyService.Format(value).Should().Be(expected);
		}
	}
}
