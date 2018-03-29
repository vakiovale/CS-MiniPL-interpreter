using System;
using System.Collections.Generic;
using MiniPL.parser.AST;

namespace MiniPL.semantics {

  public interface ISemanticAnalyzer {

    bool analyze(IAST ast);
    int getInt(string variable);
    string getString(string variable);
    bool variableExists(string variable);
    bool getBool(string v);
  }

}