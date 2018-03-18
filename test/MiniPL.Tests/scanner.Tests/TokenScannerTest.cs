using System;
using Xunit;
using MiniPL.scanner;
using MiniPL.tokens;

namespace MiniPL.Tests.scanner.Tests {
  
  public class TokenScannerTest { 

    private ITokenScanner<MiniPLTokenType> tokenScanner;

    private IScanner scanner;

    public TokenScannerTest() {
      String sampleProgram = "var nTimes : int := 0;\n"
                           + "print \"How many times?\";\n"
                           + "read nTimes;\n"
                           + "var x : int;\n"
                           + "for x in 0..nTimes-1 do\n"
                           + "\tprint x;\n"
                           + "\tprint \" : Hello, World!\\n\";\n"
                           + "end for;\n"
                           + "assert (x = nTimes);";
      this.scanner = new Scanner(sampleProgram);
      this.tokenScanner = new MiniPLTokenScanner(this.scanner);
    }

    [Fact]
    public void tokenScannerExists() {
      Assert.True(this.tokenScanner != null);
    }

    [Theory]
    [InlineData(";")]
    [InlineData("=")]
    [InlineData("+")]
    [InlineData("-")]
    [InlineData("*")]
    [InlineData("/")]
    [InlineData("&")]
    [InlineData("!")]
    [InlineData("<")]
    [InlineData("(")]
    [InlineData(")")]
    [InlineData("\"")]
    [InlineData("\\")]
    public void readsSingleCharacterTokenFromSource(String source) {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner(source));
      dynamic token = this.tokenScanner.readNextToken();
      MiniPLTokenType type = MiniPLTokenType.INVALID_TOKEN;
      switch(source) {
        case ";":
          type = MiniPLTokenType.SEMICOLON;
          break;
        case "=":
          type = MiniPLTokenType.EQUALITY_COMPARISON;
          break;
        case "+":
          type = MiniPLTokenType.PLUS;
          break;
        case "-":
          type = MiniPLTokenType.MINUS;
          break;
        case "*":
          type = MiniPLTokenType.ASTERISK;
          break;
        case "/":
          type = MiniPLTokenType.SLASH;
          break;
        case "&":
          type = MiniPLTokenType.LOGICAL_AND;
          break;
        case "!":
          type = MiniPLTokenType.LOGICAL_NOT;
          break;
        case "<":
          type = MiniPLTokenType.LESS_THAN_COMPARISON;
          break;
        case "(":
          type = MiniPLTokenType.LEFT_PARENTHESIS;
          break;
        case ")":
          type = MiniPLTokenType.RIGHT_PARENTHESIS;
          break;
        case "\"":
          type = MiniPLTokenType.QUOTE;
          break;
        case "\\":
          type = MiniPLTokenType.BACKSLASH;
          break;
      }
      Assert.Equal(type, token.getType());
    }
  }

}