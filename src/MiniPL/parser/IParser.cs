using System;
using MiniPL.parser.AST;
using MiniPL.semantics;

namespace MiniPL.parser {

  public interface IParser {

    bool processAndBuildAST();
    IAST getAST();
    bool doSemanticAnalysis(ISemanticAnalyzer semanticAnalyzer); 

  }
}