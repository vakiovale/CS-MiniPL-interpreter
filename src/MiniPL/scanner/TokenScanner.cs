using System;
using MiniPL.tokens;

namespace MiniPL.scanner {

  public abstract class TokenScanner<T> : ITokenScanner<T> {

    protected IScanner characterScanner;

    public TokenScanner(IScanner characterScanner) {
      this.characterScanner = characterScanner;
    }

    public abstract Token<T> readNextToken();

  }
}