using System;
using System.IO;
using System.Text;
using MiniPL.interpreter;
using MiniPL.io;
using MiniPL.parser.AST;

namespace MiniPL {

  public class MiniPLInterpreterProgram {

    public static void Main(string[] args) {

      if(args != null && args.Length > 0) {
        string filePath = args[0];
        if(File.Exists(filePath)) {
          TextReader textReader = File.OpenText(filePath);
          StringBuilder source = new StringBuilder();
          string line = textReader.ReadLine();
          while(line != null) {
            source.AppendLine(line);
            line = textReader.ReadLine();
          }
          IInterpreter interpreter = new MiniPLInterpreter(source.ToString(), new SymbolTable(), new ConsoleIO());
          interpreter.interpret();
        }
      }    
    }

  }

}
