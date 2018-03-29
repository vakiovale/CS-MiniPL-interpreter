using System;
using MiniPL.parser.AST;
using MiniPL.semantics.visitor;

namespace MiniPL.semantics {

  public class MiniPLSemanticAnalyzer : ISemanticAnalyzer {

    private IAST ast; 

    public MiniPLSemanticAnalyzer() {
      this.ast = null;
    }

    public bool analyze(IAST ast) {
      this.ast = ast;
      return checkTypes(ast);
    }

    private bool checkTypes(IAST ast) {
      INode program = this.ast.getProgram();
      INodeVisitor typeChecker = new TypeCheckingVisitor();
      program.accept(typeChecker);
      return false;
    }
  }

}