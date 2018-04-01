using System;
using MiniPL.io;
using MiniPL.parser;
using MiniPL.scanner;

namespace MiniPL.Tests {

  public class TestHelpers {

    public static String sampleProgram = "var nTimes : int := 0;\n"
                           + "print \"How many times?\";\n"
                           + "read nTimes;\n"
                           + "var x : int;\n"
                           + "for x in 0..nTimes-1 do\n"
                           + "\tprint x;\n"
                           + "\tprint \" : Hello, World!\\n\";\n"
                           + "end for;\n"
                           + "assert (x = nTimes);\n"
                           + "assert ((1 + (2 * 3)) = ((6 - 1) + 1));";

    public static MiniPLParser getParser(string source) {
      return new MiniPLParser(new TokenReader(ScannerFactory.createMiniPLScanner(source)), new TestIO());
    }

    public static MiniPLParser getParser(string source, IInputOutput io) {
      return new MiniPLParser(new TokenReader(ScannerFactory.createMiniPLScanner(source)), io);
    }
  }

}