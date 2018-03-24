using System;
using MiniPL.scanner;
using MiniPL.tokens;

namespace MiniPL.parser {

  public class MiniPLParser : IParser {

    private ITokenScanner<MiniPLTokenType> scanner;

    public MiniPLParser(ITokenScanner<MiniPLTokenType> scanner) {
      this.scanner = scanner;
    }

    public void parse() { 
      throw new NotImplementedException();
    }
  }

}