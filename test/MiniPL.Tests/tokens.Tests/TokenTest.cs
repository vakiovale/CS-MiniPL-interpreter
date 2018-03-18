using System;
using MiniPL.tokens;
using Xunit;

namespace MiniPL.Tests {

  public class TokenTest {

    [Fact]
    public void setupMiniPLTokensTest() {
      Token<MiniPLTokenType> token = new Token<MiniPLTokenType>(MiniPLTokenType.STRING_LITERAL, "This is a string literal");
      dynamic type = token.getType();
      Assert.Equal(typeof(MiniPLTokenType), type.GetType());
    }

    [Fact]
    public void stringLiteralHasCorrectLexeme() {
      Token<MiniPLTokenType> token = new Token<MiniPLTokenType>(MiniPLTokenType.STRING_LITERAL, "This is a string literal");
      String lexeme = token.getLexeme();
      Assert.Equal("This is a string literal", lexeme);
    }

    [Fact]
    public void semicolonLiteralDoesNotHaveLexeme() {
      Token<MiniPLTokenType> token = new Token<MiniPLTokenType>(MiniPLTokenType.SEMICOLON);
      Assert.True(token.getLexeme() == null);
    }
  }
}