using System;
using MiniPL.exceptions;
using MiniPL.logger;
using MiniPL.parser;
using MiniPL.parser.AST;
using MiniPL.scanner;
using MiniPL.semantics;
using MiniPL.syntax;
using MiniPL.tokens;
using Xunit;

namespace MiniPL.Tests.semantics.Tests {

  public class SemanticAnalysisTest {

    private IParser parser;

    private ISemanticAnalyzer analyzer;

    public SemanticAnalysisTest() {
      this.parser = TestHelpers.getParser(TestHelpers.sampleProgram);
      this.analyzer = new MiniPLSemanticAnalyzer();
    }

    [Fact]
    public void checkIntegerIsDeclaredWithDefaultValue() {
      this.parser = TestHelpers.getParser("var x : int;");
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.Equal(0, this.analyzer.getInt("x"));
    }

    [Fact]
    public void checkStringIsDeclaredWithDefaultValue() {
      this.parser = TestHelpers.getParser("var x : string;");
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.Equal("", this.analyzer.getString("x"));
    }

    [Fact]
    public void checkBoolIsDeclaredWithDefaultValue() {
      this.parser = TestHelpers.getParser("var trueValue : bool;");
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.True(this.analyzer.variableExists("trueValue"));
      Assert.False(this.analyzer.getBool("trueValue"));
    }

    [Fact]
    public void checkVariableIsDeclared() {
      this.parser = TestHelpers.getParser("var x : int;");
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.True(this.analyzer.variableExists("x"));
    }

    [Fact]
    public void checkVariableIsNotDeclared() {
      this.parser = TestHelpers.getParser("var x : int; var y : int;");
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.False(this.analyzer.variableExists("z"));
    }

    [Fact]
    public void checkThreeIntegersAreDeclaredWithDefaultValue() {
      this.parser = TestHelpers.getParser("var x : int; var y : int; var z : int;");
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.Equal(0, this.analyzer.getInt("x"));
      Assert.Equal(0, this.analyzer.getInt("y"));
      Assert.Equal(0, this.analyzer.getInt("z"));
    }

    [Fact]
    public void twoSameVariablesCantBeDeclared() {
      this.parser = TestHelpers.getParser("var x : int; var x : string;");
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      Assert.Throws<SemanticException>(() => this.analyzer.analyze(ast));
    }

    [Fact]
    public void shouldHaveThreeDifferentTypesDeclared() {
      this.parser = TestHelpers.getParser("var x : bool; var y : int; var z : string;");
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.Equal(false, this.analyzer.getBool("x"));
      Assert.Equal(0, this.analyzer.getInt("y"));
      Assert.Equal("", this.analyzer.getString("z"));
    }

    [Theory]
    [InlineData("var x : bool; var y : int; var z : string;")]
    [InlineData("var x : bool := (1 < 2) < (2 < 3); var y : int := (1 + 3) + (4 - 5); var z : string := \"Hello\" + \"World!\";")]
    [InlineData("var x : bool := \"Hello\" < \"World\"; var y : int := (3 / 7) * 2; var z : string := \"\";")]
    [InlineData("var x : bool := (1 < 2) = (\"abba\" < \"gorilla\");")]
    [InlineData("var x : bool := 1 < 2;")]
    [InlineData("var x : bool := 1 = 2;")] 
    [InlineData("var y : int := 2; var x : bool := 1 = y;")] 
    [InlineData("var y : string := \"apina\"; var x : bool := \"gorilla\" = y;")] 
    [InlineData("var y : bool := 2 < 3; var x : bool := (1 < 2) = y;")] 
    [InlineData("var x : bool := (((1 / 2) = 30) < (\"abba\" = \"gorilla\")) = (5 < (7 + 9));")]
    [InlineData("var y : string := \"apina\"; var x : string := \"13\" + y;")] 
    public void analyzeShouldFinishWithDifferentLegalTypes(string source) {
      this.parser = TestHelpers.getParser(source);
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      Assert.True(this.analyzer.analyze(ast));
    }

    [Theory]
    [InlineData("var x : bool := 1 = gorilla;")] 
    [InlineData("var x : bool := 1 + 2; var y : int; var z : string;")]
    [InlineData("var x : bool; var y : int := \"abba\"; var z : string;")]
    [InlineData("var x : bool; var y : int; var z : string := \"Hello\" + 10;")]
    [InlineData("var x : bool := (1 < 2) = (\"abba\" + \"gorilla\");")]
    [InlineData("var x : bool := (1 - 2) = (\"abba\" < \"gorilla\");")]
    [InlineData("var x : bool := (1 * 2) = (\"abba\" < \"gorilla\");")]
    [InlineData("var x : bool := (1 / 2) = (\"abba\" < \"gorilla\");")]
    [InlineData("var x : bool := (1 + 2) = (\"abba\" < \"gorilla\");")]
    [InlineData("var y : string := \"apina\"; var x : int := 13 + y;")] 
    public void analyzeShouldThrowSemanticExceptionWithUsageOfWrongTypes(string source) {
      this.parser = TestHelpers.getParser(source);
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      Assert.Throws<SemanticException>(() => this.analyzer.analyze(ast));
    }

    [Theory]
    [InlineData("var x : int; x := 10;")]
    [InlineData("var x : bool; x := 1 < 3;")]
    [InlineData("var x : string; x := \"Hello\" + x;")]
    public void variableAssignmentShouldBeOkWithCorrectTypes(string source) {
      this.parser = TestHelpers.getParser(source);
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      Assert.True(this.analyzer.analyze(ast));
    }

    [Theory]
    [InlineData("var x : string; x := 10;")]
    [InlineData("var x : int; x := 1 < 3;")]
    [InlineData("var x : string; x := 3 + x;")]
    public void variableAssignmentShouldThrowAnExceptionWithWrongTypes(string source) {
      this.parser = TestHelpers.getParser(source);
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      Assert.Throws<SemanticException>(() => this.analyzer.analyze(ast));
    }

    [Theory]
    [InlineData("var x : int; for x in 0..10 do print x; end for;")]
    [InlineData("var endIndex : int := 10; var x : int; for x in 0..endIndex do print x; end for;")]
    [InlineData("var endIndex : int := 10; var x : int; for x in (0 + 1)..((endIndex * 2) + 3) do print x; end for;")]
    [InlineData("var x : int; var y : int; for x in 0..10 do print x; for y in 0..10 do print y*x; end for; end for;")]
    [InlineData("var z : int; var x : int; for x in 0..10 do print x; z := 5; end for;")]
    public void checkCorrectTypeInForLoopsRangeOperator(string source) {
      this.parser = TestHelpers.getParser(source);
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      Assert.True(this.analyzer.analyze(ast));
    }

    [Theory]
    [InlineData("for x in 0..10 do print x; end for;")]
    [InlineData("var endIndex : bool; var x : int; for x in 0..endIndex do print x; end for;")]
    [InlineData("var endIndex : string := \"HA!\"; var x : int; for x in (0 + 1)..((endIndex * 2) + 3) do print x; end for;")]
    [InlineData("var x : int; for x in 0..GO do print x; end for;")]
    [InlineData("var GO : int; var x : int; for x in dummy..GO do print x; end for;")]
    [InlineData("var x : int; for x in GO..10 do print x; end for;")]
    public void forLoopShouldThrowAnExceptionWithWrongTypesOrMissingDeclarations(string source) {
      this.parser = TestHelpers.getParser(source);
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      Assert.Throws<SemanticException>(() => this.analyzer.analyze(ast));
    }

    [Theory]
    [InlineData("var x : int; for x in 0..10 do print x; var z : int; end for;")]
    [InlineData("var x : int; var y : int; for x in 0..10 do print x; for y in 0..10 do print y*x; var z : bool; end for; end for;")]
    public void declaringVariablesInsideForLoopShouldBeIllegal(string source) {
      this.parser = TestHelpers.getParser(source);
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      Assert.Throws<SemanticException>(() => this.analyzer.analyze(ast));
    }

    [Theory]
    [InlineData("var x : int; for x in 0..10 do print x; x := 5; end for;")]
    [InlineData("var z : int; var x : int; for x in 0..10 do print x; for z in 0..10 do print x; x := 5; end for; end for;")]
    [InlineData("var x : int; var y : int; for x in 0..10 do print x; for y in 0..10 do print y*x; var z : bool; end for; end for;")]
    public void assigningValuesForControlVariablesShouldBeIllegalInsideForLoops(string source) {
      this.parser = TestHelpers.getParser(source);
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      Assert.Throws<SemanticException>(() => this.analyzer.analyze(ast));
    }
  }
}