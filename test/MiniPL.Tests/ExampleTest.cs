using System;
using Xunit;
using MiniPL;

namespace MiniPL.Tests {

  public class ExampleTest {

    [Fact]
    public void exampleMethodShouldReturnTrue() { 
      Example example = new Example();
      Assert.True(example.isTrue(), "Should be true");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void naturalNumbersShouldBeGreaterThanZero(int value) {
      Example example = new Example();
      bool assertion = example.greaterThanZero(value);
      Assert.True(assertion);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void zeroAndNegativeValuesShouldBeEqualOrLessThanZero(int value) {
      Example example = new Example();
      bool assertion = example.greaterThanZero(value);
      Assert.False(assertion);
    }

  }
}
