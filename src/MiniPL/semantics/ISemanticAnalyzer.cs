using System;
using MiniPL.parser.AST;

namespace MiniPL.semantics {

  public interface ISemanticAnalyzer {

    bool analyze(IAST ast);

  }

}