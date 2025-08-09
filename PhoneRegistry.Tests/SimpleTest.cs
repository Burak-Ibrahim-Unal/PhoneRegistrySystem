using FluentAssertions;

namespace PhoneRegistry.Tests;

public class SimpleTest
{
    [Fact]
    public void Simple_Test_Should_Pass()
    {
        // Arrange
        var expected = "Hello World";
        
        // Act
        var actual = "Hello World";
        
        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(5, 5, 10)]
    [InlineData(-1, 1, 0)]
    public void Add_Should_Return_Correct_Sum(int a, int b, int expected)
    {
        // Act
        var result = a + b;
        
        // Assert
        result.Should().Be(expected);
    }
}
