using System;
using MiniPL.tokens;

namespace MiniPL.scanner {

  public class ScannerFactory {
        
    public static ITokenScanner<MiniPLTokenType> createMiniPLScanner(string source) {
      IScanner scanner = new Scanner(source);
      ITokenScanner<MiniPLTokenType> tokenScanner = new MiniPLTokenScanner(scanner);
      return tokenScanner;
    }

  }
}