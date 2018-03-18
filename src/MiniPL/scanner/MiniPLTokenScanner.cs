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
        default:
          return new Token<MiniPLTokenType>(MiniPLTokenType.INVALID_TOKEN);
      }
    }

    private Token<MiniPLTokenType> getSingleCharacterToken(MiniPLTokenType type) {
      return new Token<MiniPLTokenType>(type);
    }
  }

}