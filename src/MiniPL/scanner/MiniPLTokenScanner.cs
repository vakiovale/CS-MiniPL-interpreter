using System;
using MiniPL.scanner;

namespace MiniPL.tokens {

  public class MiniPLTokenScanner : TokenScanner<MiniPLTokenType> {

    public MiniPLTokenScanner(IScanner characterScanner) : base(characterScanner) {}

    public override Token<MiniPLTokenType> readNextToken() { 
      if(!hasNext()) {
        return null;
      } else {
        return readToken();
      }
    }

    private bool hasNext() {
      return this.characterScanner.hasNext();
    }

    private bool hasWhitespace() {
      return Char.IsWhiteSpace(this.characterScanner.peek());
    }

    private void removeWhitespace() {
      this.characterScanner.readNextCharacter();
    }

    private Token<MiniPLTokenType> readToken() {
      while(hasNext() && hasWhitespace()) {
        removeWhitespace();
      }
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