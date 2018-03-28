using System;
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

    private ISemanticAnalyzer analyzer;

    public SemanticAnalysisTest() {
      this.logger = new TestLogger();
      this.parser = getParser(sampleProgram);
      this.analyzer = new MiniPLSemanticAnalyzer();
    }

    private MiniPLParser getParser(string source) {
      return new MiniPLParser(new TokenReader(ScannerFactory.createMiniPLScanner(source)), logger);
    }

    [Fact]
    public void checkSampleProgramSyntax() {
      Assert.True(this.parser.processAndBuildAST());
    }
        
  }
}