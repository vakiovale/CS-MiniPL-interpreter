using System;
using MiniPL.scanner;

namespace MiniPL.tokens {

  public class MiniPLTokenScanner : TokenScanner<MiniPLTokenType> {

    public MiniPLTokenScanner(IScanner characterScanner) : base(characterScanner) {}

    public override Token<MiniPLTokenType> readNextToken() { 
      char nextCharacter = this.characterScanner.readNextCharacter();
      switch(nextCharacter) {
        case ';':
          return getSingleCharacterToken(MiniPLTokenType.SEMICOLON);
        case '=':
          return getSingleCharacterToken(MiniPLTokenType.EQUALITY_COMPARISON);
        case '<':
          return getSingleCharacterToken(MiniPLTokenType.LESS_THAN_COMPARISON);
        case '+':
          return getSingleCharacterToken(MiniPLTokenType.PLUS);
        case '-':
          return getSingleCharacterToken(MiniPLTokenType.MINUS);
        case '*':
          return getSingleCharacterToken(MiniPLTokenType.ASTERISK);
        case '/':
          return getSingleCharacterToken(MiniPLTokenType.SLASH);
        case '&':
          return getSingleCharacterToken(MiniPLTokenType.LOGICAL_AND);
        case '!':
          return getSingleCharacterToken(MiniPLTokenType.LOGICAL_NOT);
        case '(':
          return getSingleCharacterToken(MiniPLTokenType.LEFT_PARENTHESIS);
        case ')':
          return getSingleCharacterToken(MiniPLTokenType.RIGHT_PARENTHESIS);
        case '\"':
          return getSingleCharacterToken(MiniPLTokenType.QUOTE);
        case '\\':
          return getSingleCharacterToken(MiniPLTokenType.BACKSLASH);
        default:
          return new Token<MiniPLTokenType>(MiniPLTokenType.INVALID_TOKEN);
      }
    }

    private Token<MiniPLTokenType> getSingleCharacterToken(MiniPLTokenType type) {
      return new Token<MiniPLTokenType>(type);
    }
  }

}