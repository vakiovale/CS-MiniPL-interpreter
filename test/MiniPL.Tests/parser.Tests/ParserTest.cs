using System;
using MiniPL.logger;
using MiniPL.parser;
using MiniPL.scanner;
using MiniPL.tokens;
using Xunit;

namespace MiniPL.Tests {

  public class ParserTest {

    private ITokenScanner<MiniPLTokenType> scanner;

    private String sampleProgram = "var nTimes : int := 0;\n"
                           + "print \"How many times?\";\n"
                           + "read nTimes;\n"
                           + "var x : int;\n"
                           + "for x in 0..nTimes-1 do\n"
                           + "\tprint x;\n"
                           + "\tprint \" : Hello, World!\\n\";\n"
                           + "end for;\n"
                           + "assert (x = nTimes);";

    private IParser parser;

    private TestLogger logger;

    public ParserTest() {
      this.logger = new TestLogger();
      this.parser = getParser(sampleProgram);
    }

    private MiniPLParser getParser(string source) {
      return new MiniPLParser(new TokenReader(ScannerFactory.createMiniPLScanner(source)), logger);
    }

    [Fact]
    public void checkSimpleSyntax() {
      this.parser = getParser("read word;");
      Assert.True(this.parser.checkSyntax());
    }

    [Fact]
    public void checkSimpleSyntaxForTwoLineProgram() {
      this.parser = getParser("read word;\nread word;");
      Assert.True(this.parser.checkSyntax());
    }

    [Fact]
    public void checkSimpleFailingSyntax() {
      this.parser = getParser("read read;");
      Assert.False(this.parser.checkSyntax());
    }

    [Theory]
    [InlineData(";")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("var := 10")]
    [InlineData("var .= 10;")]
    public void checkIllegalPrograms(string source) {
      this.parser = getParser(source);
      Assert.False(this.parser.checkSyntax());
    }

    [Fact]
    public void checkSimpleFailingSyntaxForTwoLineProgram() {
      this.parser = getParser("read word;\nread read;");
      Assert.False(this.parser.checkSyntax());
    }

    [Theory]
    [InlineData("\"Hello World!\";")]
    [InlineData("end;")]
    [InlineData("100;")]
    public void testIllegalStartOfAStatement(string source) {
      this.parser = getParser(source);
      Assert.False(this.parser.checkSyntax());
    }

    [Theory]
    [InlineData("print \"Hello World!\";")]
    [InlineData("print (\"Hello World!\");")]
    [InlineData("print \"Hello\" + \"World\";")]
    [InlineData("print 1;")]
    [InlineData("print !1;")]
    [InlineData("print (1);")]
    [InlineData("print 1 + 2;")]
    [InlineData("print (1 + 2) + 3;")]
    [InlineData("print (1 + 2) + (3);")]
    [InlineData("print (1 + 2) + (3 + 4);")]
    [InlineData("print (1 - 2) / (3 * 4);")]
    [InlineData("print 2 = 3;")]
    [InlineData("print (1 + 2) = 3;")]
    [InlineData("print 3 = (1 + 2);")]
    [InlineData("print (3 = 3) = (!(3 = 2));")]
    [InlineData("print trueValue;")]
    [InlineData("print !trueValue;")]
    [InlineData("print (!trueValue) & trueValue;")]
    [InlineData("print (!(trueValue)) = 3;")]
    [InlineData("print (!trueValue) = 3;")]
    [InlineData("print 2 < 3;")]
    [InlineData("print ((2 < 3));")]
    [InlineData("print ((2 < (3)));")]
    [InlineData("print (((2 < 3)));")]
    [InlineData("print (((2 < 3)));\nprint (!trueValue) = 3;")]
    public void testDifferentExpressions(string source) {
      this.parser = getParser(source);
      Assert.True(this.parser.checkSyntax());
    }

    [Theory]
    [InlineData("print \"Hello World!\"")]
    [InlineData("print 2 > 3;")]
    [InlineData("print !2 = 3;")]
    [InlineData("print !trueValue = 3;")]
    [InlineData("print !(trueValue) = 3;")]
    [InlineData("print 1 + 2 + 3;")]
    [InlineData("print 1 + - 2;")]
    [InlineData("print -1;")]
    [InlineData("print 1 + (-1);")]
    [InlineData("print ((1 + 2);")]
    [InlineData("print (1 + 2));")]
    public void checkIllegalExpressionsWithPrintStatement(string source) {
      this.parser = getParser(source);
      Assert.False(this.parser.checkSyntax());
    }

    [Theory]
    [InlineData("read apina;")]
    [InlineData("read gorilla2;")]
    public void checkReadStatementAndIdentifiers(string source) {
      this.parser = getParser(source);
      Assert.True(this.parser.checkSyntax());
    }
    
    [Theory]
    [InlineData("read 12;")]
    [InlineData("read !variable;")]
    [InlineData("read \"WORD!\";")]
    public void checkIllegalReadStatements(string source) {
      this.parser = getParser(source);
      Assert.False(this.parser.checkSyntax());
    }

    [Theory]
    [InlineData("assert (apina = gorilla);")]
    [InlineData("assert (1 + 2);")]
    [InlineData("assert (gorilla + \"BAD MONKEY!\");")]
    public void checkCorrectAssertStatements(string source) {
      this.parser = getParser(source);
      Assert.True(this.parser.checkSyntax());
    }

    [Theory]
    [InlineData("assert apina = gorilla;")]
    [InlineData("assert (1 / !(BAD));")]
    [InlineData("assert gorilla + \"BAD MONKEY!\");")]
    public void checkIllegalAssertStatements(string source) {
      this.parser = getParser(source);
      Assert.False(this.parser.checkSyntax());
    }

    [Theory]
    [InlineData("var a : int := 10;")]
    [InlineData("var b : int;")]
    [InlineData("var b : string;")]
    [InlineData("var b : string := \"Live long and\" + \" prosper!\";")]
    [InlineData("var trueValue : bool;")]
    [InlineData("var trueValue : bool := pointsFromThisCourse < 1000;")]
    public void checkCorrectDeclarationAndAssignment(string source) {
      this.parser = getParser(source);
      Assert.True(this.parser.checkSyntax());
    }

    [Theory]
    [InlineData("var x;")]
    [InlineData("var x := 10;")]
    [InlineData("x : int := 10;")]
    [InlineData("var : int;")]
    [InlineData("var b : int := 1000 := 99;")]
    [InlineData("var $DOLLARBOY := 10;")]
    public void checkIllegalVariableDeclarations(string source) {
      this.parser = getParser(source);
      Assert.False(this.parser.checkSyntax());
    }

    [Theory]
    [InlineData("variable := 2;")]
    [InlineData("variable := \"Jeah\";")]
    [InlineData("variable := \"Jeah\" + \"Jeah\";")]
    [InlineData("variable := otherVariable;")]
    [InlineData("variable := otherVariable - 3;")]
    [InlineData("variable := (otherVariable - (3));")]
    [InlineData("variable := !otherVariable;")]
    [InlineData("variable := left < right;")]
    [InlineData("variable := left = right;")]
    [InlineData("variable := (left / right);")]
    [InlineData("variable:=10;")]
    public void checkCorrectVariableAssignment(string source) {
      this.parser = getParser(source);
      Assert.True(this.parser.checkSyntax());
    }

    [Theory]
    [InlineData(":= 10;")]
    [InlineData("variable := !;")]
    [InlineData("varx := !;")]
    [InlineData("var := 10;")]
    [InlineData("varx := ;")]
    [InlineData("varx:=;")]
    public void checkIllegalVariableAssignment(string source) {
      this.parser = getParser(source);
      Assert.False(this.parser.checkSyntax());
    }

    [Theory]
    [InlineData("for x in 0..nTimes-1 do\n"
                + "\tprint x;\n"
                + "\tprint \" : Hello, World!\\n\";\n"
                + "end for;\n")]
    [InlineData("for word in ((x - 13)*word) .. hello < 3 do assert(word); end for;")]
    [InlineData("for i in 1..10 do print \"Moi!\"; end for;")]
    [InlineData("for i in 1..10 do print \"Hello!\"; end for;")]
    [InlineData("for i in 1..10 do print \"Hello!\"; end for;for i in 1..10 do print \"Hello!\"; end for;")]
    [InlineData("for i in 1..10 do print \"Hello!\"; end for; \nfor i in 1..10 do print \"Hello!\"; end for;")]
    [InlineData("for i in 1..10 do for i in 1..10 do print \"Hello!\"; end for; end for; \nfor i in 1..10 do print \"Hello!\"; for i in 1..10 do for i in 1..10 do print \"Hello!\"; end for; end for; end for;")]
    public void checkCorrectForLoop(string source) {
      this.parser = getParser(source);
      Assert.True(this.parser.checkSyntax());
    }

    [Theory]
    [InlineData("for i in 1..10 do ; end for;")]
    [InlineData("for i in 1.10 do print \"Hello!\"; end for;")]
    [InlineData("for i 1..10 do print \"Hello!\"; end for;")]
    [InlineData("for i in 1..10 do print \"Hello!\" end for;")]
    [InlineData("for i in 1..10 do print \"Hello!\"; for;")]
    [InlineData("for i in 1..10 do print \"Hello!\"; end;")]
    [InlineData("for i in 1..10 do for i in 1..10 do print \"Hello!\"; end for; end for; \nfor i in 1..10 do print \"Hello!\"; for i in 1..10 do for i in 1..10 do print \"Hello!\"; end for; end for; print \"Missing end for!\";")]
    public void checkIllegalForLoop(string source) {
      this.parser = getParser(source);
      Assert.False(this.parser.checkSyntax());
    }

    [Fact]
    public void checkSyntaxOfASampleProgram() {
      Assert.True(this.parser.checkSyntax());
    }

  }

}