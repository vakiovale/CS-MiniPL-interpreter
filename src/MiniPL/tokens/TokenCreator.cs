using System;

namespace MiniPL.tokens {

  public class TokenCreator {

    private int rowNumber;

    private int columnNumber;

    public TokenCreator() {
      rowNumber = 1;
      columnNumber = 0;
    }

    public void update(char character) {
      if(Char.Equals(character, '\n')) {
        this.rowNumber++;
        this.columnNumber = 0;
      } else {
        this.columnNumber++;
      }
    }

    public Token<MiniPLTokenType> createIdentifier(String lexeme) {
      return createToken(MiniPLTokenType.IDENTIFIER, lexeme, rowNumber, columnNumber);
    }

    public Token<MiniPLTokenType> createStringLiteral(String lexeme) {
      return createToken(MiniPLTokenType.STRING_LITERAL, lexeme, rowNumber, columnNumber);
    }

    public Token<MiniPLTokenType> createIntegerLiteral(String lexeme) {
      return createToken(MiniPLTokenType.INTEGER_LITERAL, lexeme, rowNumber, columnNumber);
    }

    public Token<MiniPLTokenType> createInvalidToken(String lexeme) {
      return createToken(MiniPLTokenType.INVALID_TOKEN, lexeme, rowNumber, columnNumber);
    }

    public Token<MiniPLTokenType> createToken(MiniPLTokenType type, String lexeme) {
      return createToken(type, lexeme, rowNumber, columnNumber);
    }

    private Token<MiniPLTokenType> createToken(MiniPLTokenType type, String lexeme, int rowNumber, int columnNumber) {
      return new Token<MiniPLTokenType>(type, lexeme, rowNumber, columnNumber-lexeme.Length+1);
    }
  }

}