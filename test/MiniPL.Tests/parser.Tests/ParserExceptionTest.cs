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

    [Fact]
    public void shouldGetOnlyOneSyntaxErrorAfterRecovery() {
      this.parser = getParser("for x in 0..1 do print ) \"Hello!\"; end for;");
      this.parser.checkSyntax();
      Assert.Equal(1, this.logger.getLogs().Count);
      Assert.True(contains("Illegal start of an expression"));
    }

    [Fact]
    public void shouldGetOnlyOneSyntaxErrorAndRecoverToHandleOtherStatements() {
      this.parser = getParser("read 1; print \"OK!\"; var x : int := 10; for x in 0..1 do print \"Hello!\"; end for;");
      this.parser.checkSyntax();
      foreach(string err in this.logger.getLogs()) {
        Console.WriteLine(err);
      }
      Assert.Equal(1, this.logger.getLogs().Count);
      Assert.True(contains("Expected an identifier"));
    }

    [Fact]
    public void shouldGetOneLexicalErrorInVarDeclaration() {
      this.parser = getParser("var $dollars : string; print dollars;");
      this.parser.checkSyntax();
      Assert.Equal(1, this.logger.getLogs().Count);
      Assert.True(contains("$dollars"));
    }

    [Fact]
    public void shouldGetOneLexialErrorInStartOfAStatement() {
      this.parser = getParser("print\"Time to make some money\"; $DOLLARBOY := 12; assert(DOLLARBOY = 12);");
      this.parser.checkSyntax();
      Assert.Equal(1, this.logger.getLogs().Count);
      Assert.True(contains("$DOLLARBOY"));
    }

    [Fact]
    public void shouldTwoLexicalErrorsInStartOfAStatementAndInAssert() {
      this.parser = getParser("var123 := ^abba ; assert ( var123 = $12 );print\"Time to make some money\";");
      this.parser.checkSyntax();
      Assert.Equal(2, this.logger.getLogs().Count);
      Assert.True(contains("^abba"));
      Assert.True(contains("$12"));
      Assert.False(contains("SYNTAX ERROR"));
      Assert.True(contains("LEXICAL ERROR"));
    }
  }

}