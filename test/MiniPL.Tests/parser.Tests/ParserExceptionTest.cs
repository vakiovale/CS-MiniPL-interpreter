using System;
using MiniPL.logger;
using MiniPL.parser;
using MiniPL.scanner;
using MiniPL.tokens;
using Xunit;

namespace MiniPL.Tests {

  public class ParserExceptionTest {

    private ITokenScanner<MiniPLTokenType> scanner;

    private IParser parser;

    private TestLogger logger;

    public ParserExceptionTest() {
      this.logger = new TestLogger();
      this.parser = getParser("");
    }

    private MiniPLParser getParser(string source) {
      return new MiniPLParser(new TokenReader(ScannerFactory.createMiniPLScanner(source)), logger);
    }

    private bool contains(string sentence) {
      foreach(string errorLog in this.logger.getLogs()) {
        if(errorLog.Contains(sentence)) {
          return true;
        }
      }
      return false;
    }

    [Fact]
    public void checkNoErrors() {
      this.parser = getParser("read word;");
      this.parser.checkSyntax();
      Assert.Equal(0, this.logger.getLogs().Count);
    }

    [Fact]
    public void shouldGetTwoLexicalErrorsContainingInvalidLexemes() {
      this.parser = getParser("read _kjsdflök; assert(^BAD)");
      this.parser.checkSyntax();
      Assert.Equal(2, this.logger.getLogs().Count);
      Assert.True(contains("_kjsdflök"));
      Assert.True(contains("^BAD"));
    }
  }

}