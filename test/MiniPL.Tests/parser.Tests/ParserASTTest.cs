using System;
using MiniPL.logger;
using MiniPL.parser;
using MiniPL.parser.AST;
using MiniPL.scanner;
using MiniPL.syntax;
using MiniPL.tokens;
using Xunit;

namespace MiniPL.Tests {

  public class ParserASTTest {

    private ITokenScanner<MiniPLTokenType> scanner;

    private String sampleProgram = "var nTimes : int := 0;\n"
                           + "print \"How many times?\";\n"
                           + "read nTimes;\n"
                           + "var x : int;\n"
                           + "for x in 0..nTimes-1 do\n"
                           + "\tprint x;\n"
                           + "\tprint \" : Hello, World!\\n\";\n"
                           + "end for;\n"
                           + "assert (x = nTimes);\n"
                           + "assert ((1 + (2 * 3)) = ((6 - 1) + 1));";

    private IParser parser;

    private TestLogger logger;

    public ParserASTTest() {
      this.logger = new TestLogger();
      this.parser = getParser(sampleProgram);
    }

    private MiniPLParser getParser(string source) {
      return new MiniPLParser(new TokenReader(ScannerFactory.createMiniPLScanner(source)), logger);
    }

    [Fact]
    public void checkSampleProgramSyntax() {
      Assert.True(this.parser.checkSyntax());
    }

    [Fact]
    public void checkASTexists() {
      this.parser.checkSyntax();
      IAST ast = this.parser.getAST();
      Assert.True(ast != null);
    }

    [Fact]
    public void checkASTDoesNotExistForIllegalProgram() {
      this.parser = getParser("This program is NOT ok!; print \"Bye!\";");
      this.parser.checkSyntax();
      Assert.True(this.parser.getAST() == null);
    }

    [Fact]
    public void checkASTForPrintStatement() {
      this.parser = getParser("print \"Hello World!\";");
      this.parser.checkSyntax();


      IAST ast = this.parser.getAST();
      INode program = ast.getProgram();

      INode current = program;
      INode statementList = program.getChildren()[0];
      INode statement = statementList.getChildren()[0];
      INode print = statement.getChildren()[0];
      INode expression = print.getChildren()[0];
      INode stringLiteral = expression.getChildren()[0];

      Assert.Equal("Hello World!", stringLiteral.getValue()); 
    }

    [Fact]
    public void shouldHaveTwoStringsAddedTogether() {
      this.parser = getParser("print \"Hello \" + \"World!\";");
      this.parser.checkSyntax();

      IAST ast = this.parser.getAST();
      INode program = ast.getProgram();

      INode statementList = program.getChildren()[0];
      INode statement = statementList.getChildren()[0];
      INode print = statement.getChildren()[0];
      INode expression = print.getChildren()[0];
      INode plusOperation = expression.getChildren()[0];
      INode leftString = plusOperation.getChildren()[0];
      INode rightString = plusOperation.getChildren()[1];

      Assert.Equal("Hello ", leftString.getValue()); 
      Assert.Equal("World!", rightString.getValue()); 
    }

    [Fact]
    public void shouldHaveCorrectExpressionWithLogicalNot() {
      this.parser = getParser("print !(1 = 2);");
      this.parser.checkSyntax();

      IAST ast = this.parser.getAST();
      INode program = ast.getProgram();

      INode statementList = program.getChildren()[0];
      INode statement = statementList.getChildren()[0];
      INode print = statement.getChildren()[0];
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
      this.parser = getParser("assert (1 < 2);");
      this.parser.checkSyntax();

      IAST ast = this.parser.getAST();
      INode program = ast.getProgram();

      INode statementList = program.getChildren()[0];
      INode statement = statementList.getChildren()[0];
      INode assert = statement.getChildren()[0];
      INode expression = assert.getChildren()[0];
      INode lessThanEquality = expression.getChildren()[0];
      INode leftHandSide = lessThanEquality.getChildren()[0];
      INode rightHandSide = lessThanEquality.getChildren()[1];

      Assert.Equal(1, leftHandSide.getValue());
      Assert.Equal(2, rightHandSide.getValue());
      Assert.Equal("<", lessThanEquality.getValue());
    }

    [Fact]
    public void shouldHaveValidASTWithReadStatement() {
      this.parser = getParser("read goodVariable;");
      this.parser.checkSyntax();

      IAST ast = this.parser.getAST();
      INode program = ast.getProgram();

      INode statementList = program.getChildren()[0];
      INode statement = statementList.getChildren()[0];
      INode read = statement.getChildren()[0];
      INode identifier = read.getChildren()[0];

      Assert.Equal(MiniPLSymbol.READ_PROCEDURE, read.getValue());
      Assert.Equal("goodVariable", identifier.getValue());
    }

    [Fact]
    public void shouldHaveValidASTWithToStatements() {
      this.parser = getParser("print 1 + 2; read goodVariable;");
      this.parser.checkSyntax();

      IAST ast = this.parser.getAST();
      INode program = ast.getProgram();

      INode statementList = program.getChildren()[0];
      INode firstStatement = statementList.getChildren()[0];
      INode secondStatement = statementList.getChildren()[1];
      INode print = firstStatement.getChildren()[0];
      INode expression = print.getChildren()[0];
      INode plusOperator = expression.getChildren()[0];
      
      INode read = secondStatement.getChildren()[0];

      Assert.Equal(1, plusOperator.getChildren()[0].getValue());
      Assert.Equal(2, plusOperator.getChildren()[1].getValue());
      Assert.Equal("goodVariable", read.getChildren()[0].getValue());
    }

    [Fact]
    public void shouldHaveValidASTWithVarAssignment() {
      this.parser = getParser("car := formula1Car;");
      this.parser.checkSyntax();

      IAST ast = this.parser.getAST();
      INode program = ast.getProgram();

      INode statementList = program.getChildren()[0];
      INode statement = statementList.getChildren()[0];
      INode varAssignment = statement.getChildren()[0];
      INode car = varAssignment.getChildren()[0];
      INode formula1CarExpression = varAssignment.getChildren()[1];
      INode formula1Car = formula1CarExpression.getChildren()[0];

      Assert.Equal(MiniPLSymbol.VAR_ASSIGNMENT, varAssignment.getValue());
      Assert.Equal("car", car.getValue());
      Assert.Equal("formula1Car", formula1Car.getValue());
    }
  }
}