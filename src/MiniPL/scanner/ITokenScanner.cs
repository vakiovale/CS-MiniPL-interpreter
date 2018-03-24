using System;
using MiniPL.tokens;

namespace MiniPL.scanner {

  public interface ITokenScanner<T> {
      
    Token<T> readNextToken();

  }
}