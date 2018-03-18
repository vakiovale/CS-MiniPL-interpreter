using System;

namespace MiniPL.scanner {
  public class TokenScanner : ITokenScanner {

    private IScanner characterScanner;

    public TokenScanner(IScanner characterScanner) {
      this.characterScanner = characterScanner;
    }

    public String getSource() {
      return this.characterScanner.getSource();
    }

    public void setSource(String source) {
      this.characterScanner.setSource(source);
    }
  }
}