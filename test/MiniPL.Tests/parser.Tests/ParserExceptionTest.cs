using System;
using MiniPL.logger;
using MiniPL.parser;
using MiniPL.scanner;
using MiniPL.tokens;
using Xunit;

namespace MiniPL.Tests {

  public class ParserExceptionTest {

    private IParser parser;

    private TestLogger logger;

    public ParserExceptionTest() {
      this.parser = TestHelpers.getParser("");
      this.logger = new TestLogger();
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
      this.parser = TestHelpers.getParser("read word;", logger);
      this.parser.processAndBuildAST();
      Assert.Equal(0, this.logger.getLogs().Count);
    }

    [Fact]
    public void shouldGetTwoLexicalErrorsContainingInvalidLexemes() {
      this.parser = TestHelpers.getParser("read _kjsdflök; assert(^BAD)", logger);
      this.parser.processAndBuildAST();
      Assert.Equal(2, this.logger.getLogs().Count);
      Assert.True(contains("_kjsdflök"));
      Assert.True(contains("^BAD"));
    }

    [Fact]
    public void shouldGetOnlyOneSyntaxErrorAfterRecovery() {
      this.parser = TestHelpers.getParser("for x in 0..1 do print ) \"Hello!\"; end for;", logger);
      this.parser.processAndBuildAST();
      Assert.Equal(1, this.logger.getLogs().Count);
      Assert.True(contains("Illegal start of an expression"));
    }

    [Fact]
    public void shouldGetOnlyOneSyntaxErrorAndRecoverToHandleOtherStatements() {
      this.parser = TestHelpers.getParser("read 1; print \"OK!\"; var x : int := 10; for x in 0..1 do print \"Hello!\"; end for;", logger);
      this.parser.processAndBuildAST();
      Assert.Equal(1, this.logger.getLogs().Count);
      Assert.True(contains("Expected an identifier"));
    }

    [Fact]
    public void shouldGetOneLexicalErrorInVarDeclaration() {
      this.parser = TestHelpers.getParser("var $dollars : string; print dollars;", logger);
      this.parser.processAndBuildAST();
      Assert.Equal(1, this.logger.getLogs().Count);
      Assert.True(contains("$dollars"));
    }

    [Fact]
    public void shouldGetOneLexialErrorInStartOfAStatement() {
      this.parser = TestHelpers.getParser("print\"Time to make some money\"; $DOLLARBOY := 12; assert(DOLLARBOY = 12);", logger);
      this.parser.processAndBuildAST();
      Assert.Equal(1, this.logger.getLogs().Count);
      Assert.True(contains("$DOLLARBOY"));
    }

    [Fact]
    public void shouldTwoLexicalErrorsInStartOfAStatementAndInAssert() {
      this.parser = TestHelpers.getParser("var123 := ^abba ; assert ( var123 = $12 );print\"Time to make some money\";", logger);
      this.parser.processAndBuildAST();
      Assert.Equal(2, this.logger.getLogs().Count);
      Assert.True(contains("^abba"));
      Assert.True(contains("$12"));
      Assert.False(contains("SYNTAX ERROR"));
      Assert.True(contains("LEXICAL ERROR"));
    }
  }

}