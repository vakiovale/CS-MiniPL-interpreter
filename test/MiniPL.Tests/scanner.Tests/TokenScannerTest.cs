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
    
    [Fact]
    public void readMultipleSingleCharTokens() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner(";;;"));
      Assert.Equal(MiniPLTokenType.SEMICOLON, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.SEMICOLON, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.SEMICOLON, this.tokenScanner.readNextToken().getType());
    }

    [Fact]
    public void ifNoMoreTokensThenReturnNullToken() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("!"));
      this.tokenScanner.readNextToken();
      dynamic nullToken = this.tokenScanner.readNextToken();
      Assert.True(nullToken == null);
    }

    [Fact]
    public void readMultipleDifferentSingleCharTokensSeparatedWithWhiteSpace() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("\" \\ !\t+\n\n&"));
      Assert.Equal(MiniPLTokenType.QUOTE, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.BACKSLASH, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.LOGICAL_NOT, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.PLUS, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.LOGICAL_AND, this.tokenScanner.readNextToken().getType());
    }

    [Fact]
    public void readMultipleDifferentSingleCharTokens() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("(\"!&<=\")"));
      Assert.Equal(MiniPLTokenType.LEFT_PARENTHESIS, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.QUOTE, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.LOGICAL_NOT, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.LOGICAL_AND, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.LESS_THAN_COMPARISON, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.EQUALITY_COMPARISON, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.QUOTE, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.RIGHT_PARENTHESIS, this.tokenScanner.readNextToken().getType());
    }

    [Fact]
    public void readStringLiteralMiddleOfSpecialCharacters() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("(!goodVariable_1)"));
      dynamic leftParenthesis = this.tokenScanner.readNextToken();
      dynamic logicalNot = this.tokenScanner.readNextToken();
      dynamic identifier = this.tokenScanner.readNextToken();
      dynamic rightParenthesis = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.LEFT_PARENTHESIS, leftParenthesis.getType());
      Assert.Equal(MiniPLTokenType.LOGICAL_NOT, logicalNot.getType());
      Assert.Equal(MiniPLTokenType.RIGHT_PARENTHESIS, rightParenthesis.getType());
      Assert.Equal(MiniPLTokenType.STRING_LITERAL, identifier.getType());
      Assert.Equal("goodVariable_1", identifier.getLexeme());
    }

    [Theory]
    [InlineData("variable_1")]
    [InlineData("var2")]
    [InlineData("MysticalVariable")]
    [InlineData("A_b_123_assert")]
    [InlineData("variable9_")]
    public void readStringTokenTest(String source) {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner(source));
      dynamic token = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.STRING_LITERAL, token.getType());
      Assert.Equal(source, token.getLexeme());
    }

    [Fact]
    public void readStringLiterals() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("token1 token2 token_3"));
      dynamic firstToken = this.tokenScanner.readNextToken();
      dynamic secondToken = this.tokenScanner.readNextToken();
      dynamic thirdToken = this.tokenScanner.readNextToken();
      Assert.Equal("token1", firstToken.getLexeme());
      Assert.Equal("token2", secondToken.getLexeme());
      Assert.Equal("token_3", thirdToken.getLexeme());
    }

    [Fact]
    public void readVarKeyword() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("var"));
      dynamic token = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.KEYWORD_VAR, token.getType());
    }

    [Fact]
    public void readForKeyword() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("for"));
      dynamic token = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.KEYWORD_FOR, token.getType());
    }

  }

}