using System;
using Xunit;
using MiniPL.scanner;

namespace MiniPL.Tests.scanner.Tests {
  
  public class TokenScannerTest { 

    private ITokenScanner tokenScanner;

    private IScanner scanner;

    public TokenScannerTest() {
      this.scanner = new Scanner();
      this.tokenScanner = new TokenScanner(this.scanner);
    }

    [Fact]
    public void tokenScannerExists() {
      Assert.True(this.tokenScanner != null);
    }

    [Fact]
    public void settingSourceTest() {
      String source = "This is the source!";
      this.tokenScanner.setSource(source);
      Assert.Equal(source, this.tokenScanner.getSource());
    }
  }

}