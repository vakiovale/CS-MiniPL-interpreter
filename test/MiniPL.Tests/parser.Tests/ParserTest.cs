using System;
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

    public ParserTest() {
      this.parser = getParser(sampleProgram);
    }

    private MiniPLParser getParser(string source) {
      return new MiniPLParser(ScannerFactory.createMiniPLScanner(source));
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
    public void checkCorrectDeclarationAndAssignment(string source) {
      Console.WriteLine(":" + source);
      this.parser = getParser(source);
      Assert.True(this.parser.checkSyntax());
      Console.WriteLine(source);
    }


  }

}