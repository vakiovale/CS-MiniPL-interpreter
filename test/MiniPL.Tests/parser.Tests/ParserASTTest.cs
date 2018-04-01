using System;
using MiniPL.parser;
using MiniPL.parser.AST;
using MiniPL.scanner;
using MiniPL.syntax;
using MiniPL.tokens;
using Xunit;

namespace MiniPL.Tests {

  public class ParserASTTest {

    private IParser parser;

    public ParserASTTest() {
      this.parser = TestHelpers.getParser(TestHelpers.sampleProgram);
    }

    [Fact]
    public void checkSampleProgramSyntax() {
      Assert.True(this.parser.processAndBuildAST());
    }

    [Fact]
    public void sampleProgramShouldHaveCorrectAST() {
      this.parser.processAndBuildAST();
      IAST ast = this.parser.getAST();
      INode statementList = ast.getProgram().getChildren()[0];
      INode firstVarDeclarationStatement = statementList.getChildren()[0];
      INode printStatement = statementList.getChildren()[1];
      INode readStatement = statementList.getChildren()[2];
      INode secondVarDeclarationStatement = statementList.getChildren()[3];
      INode forLoopStatement = statementList.getChildren()[4];
      INode firstAssertStatement = statementList.getChildren()[5];
      INode secondAssertStatement = statementList.getChildren()[6];
      Assert.Equal(MiniPLSymbol.STATEMENT_LIST, statementList.getValue());
      Assert.Equal(MiniPLSymbol.VAR_DECLARATION, firstVarDeclarationStatement.getValue());
      Assert.Equal(MiniPLSymbol.PRINT_PROCEDURE, printStatement.getValue());
      Assert.Equal(MiniPLSymbol.READ_PROCEDURE, readStatement.getValue());
      Assert.Equal(MiniPLSymbol.VAR_DECLARATION, secondVarDeclarationStatement.getValue());
      Assert.Equal(MiniPLSymbol.FOR_LOOP, forLoopStatement.getValue());
      Assert.Equal(MiniPLSymbol.ASSERT_PROCEDURE, firstAssertStatement.getValue());
      Assert.Equal(MiniPLSymbol.ASSERT_PROCEDURE, secondAssertStatement.getValue());
    }

    [Fact]
    public void checkASTexists() {
      this.parser.processAndBuildAST();
      IAST ast = this.parser.getAST();
      Assert.True(ast != null);
    }

    [Fact]
    public void checkASTDoesNotExistForIllegalProgram() {
      this.parser = TestHelpers.getParser("This program is NOT ok!; print \"Bye!\";");
      this.parser.processAndBuildAST();
      Assert.True(this.parser.getAST() == null);
    }

    [Fact]
    public void checkASTForPrintStatement() {
      this.parser = TestHelpers.getParser("print \"Hello World!\";");
      this.parser.processAndBuildAST();


      IAST ast = this.parser.getAST();
      INode program = ast.getProgram();

      INode current = program;
      INode statementList = program.getChildren()[0];
      INode print = statementList.getChildren()[0];
      INode expression = print.getChildren()[0];
      INode stringLiteral = expression.getChildren()[0];

      Assert.Equal("Hello World!", stringLiteral.getValue()); 
    }

    [Fact]
    public void shouldHaveTwoStringsAddedTogether() {
      this.parser = TestHelpers.getParser("print \"Hello \" + \"World!\";");
      this.parser.processAndBuildAST();

      IAST ast = this.parser.getAST();
      INode program = ast.getProgram();

      INode statementList = program.getChildren()[0];
      INode print = statementList.getChildren()[0];
      INode expression = print.getChildren()[0];
      INode plusOperation = expression.getChildren()[0];
      INode leftString = plusOperation.getChildren()[0];
      INode rightString = plusOperation.getChildren()[1];

      Assert.Equal("Hello ", leftString.getValue()); 
      Assert.Equal("World!", rightString.getValue()); 
    }

    [Fact]
    public void shouldHaveCorrectExpressionWithLogicalNot() {
      this.parser = TestHelpers.getParser("print !(1 = 2);");
      this.parser.processAndBuildAST();

      IAST ast = this.parser.getAST();
      INode program = ast.getProgram();

      INode statementList = program.getChildren()[0];
      INode print = statementList.getChildren()[0];
      INode expression = print.getChildren()[0];
      INode logicalNot = expression.getChildren()[0];
      INode innerExpression = logicalNot.getChildren()[0];
      INode equality = innerExpression.getChildren()[0];
      INode leftHandSide = equality.getChildren()[0];
      INode rightHandSide = equality.getChildren()[1];

      Assert.Equal(1, leftHandSide.getValue());
      Assert.Equal(2, rightHandSide.getValue());
    }

    [Fact]
    public void shouldHaveValidASTWithAssertStatement() {
      this.parser = TestHelpers.getParser("assert (1 < 2);");
      this.parser.processAndBuildAST();

      IAST ast = this.parser.getAST();
      INode program = ast.getProgram();

      INode statementList = program.getChildren()[0];
      INode assert = statementList.getChildren()[0];
      INode expression = assert.getChildren()[0];
      INode lessThanEquality = expression.getChildren()[0];
      INode leftHandSide = lessThanEquality.getChildren()[0];
      INode rightHandSide = lessThanEquality.getChildren()[1];

      Assert.Equal(1, leftHandSide.getValue());
      Assert.Equal(2, rightHandSide.getValue());
      Assert.Equal(MiniPLTokenType.LESS_THAN_COMPARISON, lessThanEquality.getValue());
    }

    [Fact]
    public void shouldHaveValidASTWithReadStatement() {
      this.parser = TestHelpers.getParser("read goodVariable;");
      this.parser.processAndBuildAST();

      IAST ast = this.parser.getAST();
      INode program = ast.getProgram();

      INode statementList = program.getChildren()[0];
      INode read = statementList.getChildren()[0];
      INode identifier = read.getChildren()[0];

      Assert.Equal(MiniPLSymbol.READ_PROCEDURE, read.getValue());
      Assert.Equal("goodVariable", identifier.getValue());
    }

    [Fact]
    public void shouldHaveValidASTWithToStatements() {
      this.parser = TestHelpers.getParser("print 1 + 2; read goodVariable;");
      this.parser.processAndBuildAST();

      IAST ast = this.parser.getAST();
      INode program = ast.getProgram();

      INode statementList = program.getChildren()[0];
      INode print = statementList.getChildren()[0];
      INode read = statementList.getChildren()[1];
      INode expression = print.getChildren()[0];
      INode plusOperator = expression.getChildren()[0];

      Assert.Equal(1, plusOperator.getChildren()[0].getValue());
      Assert.Equal(2, plusOperator.getChildren()[1].getValue());
      Assert.Equal("goodVariable", read.getChildren()[0].getValue());
    }

    [Fact]
    public void shouldHaveValidASTWithVarAssignment() {
      this.parser = TestHelpers.getParser("car := formula1Car;");
      this.parser.processAndBuildAST();

      IAST ast = this.parser.getAST();
      INode program = ast.getProgram();

      INode statementList = program.getChildren()[0];
      INode varAssignment = statementList.getChildren()[0];
      INode car = varAssignment.getChildren()[0];
      INode formula1CarExpression = varAssignment.getChildren()[1];
      INode formula1Car = formula1CarExpression.getChildren()[0];

      Assert.Equal(MiniPLSymbol.VAR_ASSIGNMENT, varAssignment.getValue());
      Assert.Equal("car", car.getValue());
      Assert.Equal("formula1Car", formula1Car.getValue());
    }

    [Fact]
    public void shouldHaveValidASTWithVarAssignmentHavingExpression() {
      this.parser = TestHelpers.getParser("theBoss := (\"Yes, \" + \"sir!\");");
      this.parser.processAndBuildAST();

      IAST ast = this.parser.getAST();
      INode program = ast.getProgram();

      INode statementList = program.getChildren()[0];
      INode varAssignment = statementList.getChildren()[0];
      INode theBoss = varAssignment.getChildren()[0];
      INode expression = varAssignment.getChildren()[1];
      INode innerExpression = expression.getChildren()[0];
      INode plusOperation = innerExpression.getChildren()[0];
      INode yesString = plusOperation.getChildren()[0];
      INode sirString = plusOperation.getChildren()[1];

      Assert.Equal(MiniPLSymbol.VAR_ASSIGNMENT, varAssignment.getValue());
      Assert.Equal("Yes, ", yesString.getValue());
      Assert.Equal("sir!", sirString.getValue());
    }

    [Fact]
    public void shouldHaveValidASTWithVarDeclaration() {
      this.parser = TestHelpers.getParser("var x : int := 1;");
      this.parser.processAndBuildAST();

      IAST ast = this.parser.getAST();
      INode program = ast.getProgram();

      INode statementList = program.getChildren()[0];
      INode varDeclaration = statementList.getChildren()[0];
      INode x = varDeclaration.getChildren()[0];
      INode type = varDeclaration.getChildren()[1];
      INode expression = type.getChildren()[0];
      INode value = expression.getChildren()[0];

      Assert.Equal(MiniPLSymbol.VAR_DECLARATION, varDeclaration.getValue());
      Assert.Equal(MiniPLTokenType.TYPE_IDENTIFIER_INTEGER, type.getValue());
      Assert.Equal(1, value.getValue());
    }

    [Fact]
    public void shouldHaveValidASTWithVarDeclarationAndMoreComplexExpression() {
      this.parser = TestHelpers.getParser("var x : string := (\"HELLO\" + (\"WORLD!\"));");
      this.parser.processAndBuildAST();

      IAST ast = this.parser.getAST();
      INode program = ast.getProgram();

      INode statementList = program.getChildren()[0];
      INode varDeclaration = statementList.getChildren()[0];
      INode x = varDeclaration.getChildren()[0];
      INode type = varDeclaration.getChildren()[1];
      INode expression = type.getChildren()[0];
      INode plusOperation = expression.getChildren()[0].getChildren()[0];
      INode leftString = plusOperation.getChildren()[0];
      INode innerExpression = plusOperation.getChildren()[1];
      INode rightString = innerExpression.getChildren()[0];

      Assert.Equal(MiniPLSymbol.VAR_DECLARATION, varDeclaration.getValue());
      Assert.Equal(MiniPLTokenType.TYPE_IDENTIFIER_STRING, type.getValue());
      Assert.Equal("HELLO", leftString.getValue());
      Assert.Equal("WORLD!", rightString.getValue());
    }

    [Fact]
    public void shouldHaveValidASTWithVarDeclarationWithOnlyType() {
      this.parser = TestHelpers.getParser("var x : int;");
      this.parser.processAndBuildAST();

      IAST ast = this.parser.getAST();
      INode program = ast.getProgram();

      INode statementList = program.getChildren()[0];
      INode varDeclaration = statementList.getChildren()[0];
      INode x = varDeclaration.getChildren()[0];
      INode type = varDeclaration.getChildren()[1];

      Assert.Equal(MiniPLSymbol.VAR_DECLARATION, varDeclaration.getValue());
      Assert.Equal(MiniPLTokenType.TYPE_IDENTIFIER_INTEGER, type.getValue());
      Assert.True(type.getChildren().Count == 0);
    }

    [Fact]
    public void shouldHaveValidASTWithForLoop() {
      this.parser = TestHelpers.getParser("for x in 0..10 do print \"We are in!\"; end for;");
      this.parser.processAndBuildAST();

      IAST ast = this.parser.getAST();
      INode program = ast.getProgram();

      INode statementList = program.getChildren()[0];
      INode forLoop = statementList.getChildren()[0];
      INode identifier = forLoop.getChildren()[0]; 
      INode range = forLoop.getChildren()[1];
      INode startExpression = range.getChildren()[0];
      INode endExpression = range.getChildren()[1];
      INode forStatement = forLoop.getChildren()[2].getChildren()[0];
      INode printText = forStatement.getChildren()[0].getChildren()[0];

      Assert.Equal(MiniPLSymbol.FOR_LOOP, forLoop.getValue());
      Assert.Equal(MiniPLTokenType.RANGE_OPERATOR, range.getValue());
      Assert.Equal("x", identifier.getValue());
      Assert.Equal(0, startExpression.getChildren()[0].getValue());
      Assert.Equal(10, endExpression.getChildren()[0].getValue());
      Assert.Equal("We are in!", printText.getValue());
    }
  }
}