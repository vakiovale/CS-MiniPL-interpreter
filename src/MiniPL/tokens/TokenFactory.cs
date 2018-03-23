using System;

namespace MiniPL.tokens {

  public class TokenFactory {

    public static Token<MiniPLTokenType> createIdentifier(String lexeme) {
      return createToken(MiniPLTokenType.IDENTIFIER, lexeme);
    }

    public static Token<MiniPLTokenType> createStringLiteral(String lexeme) {
      return createToken(MiniPLTokenType.STRING_LITERAL, lexeme);
    }

    public static Token<MiniPLTokenType> createIntegerLiteral(String lexeme) {
      return createToken(MiniPLTokenType.INTEGER_LITERAL, lexeme);
    }

    public static Token<MiniPLTokenType> createInvalidToken(String lexeme) {
      return createToken(MiniPLTokenType.INVALID_TOKEN, lexeme);
    }

    public static Token<MiniPLTokenType> createToken(MiniPLTokenType type, String lexeme) {
      return new Token<MiniPLTokenType>(type, lexeme);
    }
  }

}