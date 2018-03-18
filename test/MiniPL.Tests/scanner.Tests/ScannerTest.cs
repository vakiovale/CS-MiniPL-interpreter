using System;
using Xunit;
using MiniPL.scanner;

namespace MiniPL.Tests.scanner.Tests {

  public class ScannerTest {

    private IScanner scanner;

    public ScannerTest() {
      scanner = new Scanner();
    }

    [Fact]
    public void scannerExists() {
      Assert.True(scanner != null);
    }

    [Theory]
    [InlineData("Source file!")]
    [InlineData("\nThis is some code!\n\nEOF")]
    public void settingSourceTest(String source) {
      scanner.setSource(source);
      Assert.Equal(source, scanner.getSource());
    }

    [Theory]
    [InlineData("This is a test source")]
    public void readCharacterTest(String source) {
      scanner.setSource(source);
      char firstCharacter = scanner.readNextCharacter();
      Assert.Equal("T", firstCharacter.ToString());
    }

    [Theory]
    [InlineData("\nSource")]
    public void readMultipleCharactersTest(String source) {
      scanner.setSource(source);
      char char1 = scanner.readNextCharacter();
      char char2 = scanner.readNextCharacter();
      char char3 = scanner.readNextCharacter();
      char char4 = scanner.readNextCharacter();
      char char5 = scanner.readNextCharacter();
      char char6 = scanner.readNextCharacter();
      char char7 = scanner.readNextCharacter();
      Assert.Equal("\n", char1.ToString());
      Assert.Equal("S", char2.ToString());
      Assert.Equal("o", char3.ToString());
      Assert.Equal("u", char4.ToString());
      Assert.Equal("r", char5.ToString());
      Assert.Equal("c", char6.ToString());
      Assert.Equal("e", char7.ToString());
    }

    [Fact]
    public void readingTooManyCharactersThrowsException() {
      scanner.setSource("A");
      scanner.readNextCharacter();
      Assert.Throws<IndexOutOfRangeException>(() => scanner.readNextCharacter());
    }

    [Fact]
    public void checkingIfNextCharacterExistsIfWeAreNotAtTheEnd() {
      scanner.setSource("abc");
      Assert.True(scanner.hasNext());
      scanner.readNextCharacter();
      Assert.True(scanner.hasNext());
      scanner.readNextCharacter();
      Assert.True(scanner.hasNext());
      scanner.readNextCharacter();
      Assert.False(scanner.hasNext(), "There should be no more characters");
    }

    [Fact]
    public void peekingCharacterShouldShowNextCharacter() {
      scanner.setSource("ab");
      Assert.Equal('a', scanner.peek());
      Assert.Equal('a', scanner.peek());
    }

    [Theory]
    [InlineData("abba")]
    [InlineData("Gorilla!")]
    [InlineData("\t\n\\A")]
    public void peekingNextCharacterShouldBeSameWhenReadingNextCharacter(String source) {
      scanner.setSource(source);
      char peekedCharacter = scanner.peek();
      Assert.Equal(scanner.readNextCharacter(), peekedCharacter);
    }
  }


}