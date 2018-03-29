using System;
using System.Collections.Generic;
using MiniPL.parser.AST;
using MiniPL.semantics.visitor;

namespace MiniPL.semantics {

  public class MiniPLSemanticAnalyzer : ISemanticAnalyzer {

    private IAST ast; 

    private IDictionary<string, int> integerVariables;

    public MiniPLSemanticAnalyzer() {
      this.ast = null;
      this.integerVariables = new Dictionary<string, int>();
    }

    public bool analyze(IAST ast) {
      this.ast = ast;
      return checkVariablesDeclaredExactlyOnceBeforeUsage();
      //return checkTypes(ast);
    }

    private bool checkVariablesDeclaredExactlyOnceBeforeUsage() {
      ProgramNode program = (ProgramNode)this.ast.getProgram();
      INodeVisitor varDeclarationVisitor = new VarDeclarationVisitor(integerVariables);
      program.accept(varDeclarationVisitor);
      return false;
    }

    private bool checkTypes(IAST ast) {
      INode program = this.ast.getProgram();
      INodeVisitor typeChecker = new TypeCheckingVisitor();
      program.accept(typeChecker);
      return false;
    }

    public int getInt(string variable) {
      return this.integerVariables[variable];
    }

    public string getString(string variable) {
      throw new NotImplementedException();
    }
  }

}