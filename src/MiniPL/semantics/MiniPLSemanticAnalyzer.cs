using System;
using System.Collections.Generic;
using MiniPL.parser.AST;
using MiniPL.semantics.visitor;

namespace MiniPL.semantics {

  public class MiniPLSemanticAnalyzer : ISemanticAnalyzer {

    private IAST ast; 

    private IDictionary<string, int> integerVariables;

    private IDictionary<string, string> stringVariables;

    public MiniPLSemanticAnalyzer() {
      this.ast = null;
      this.integerVariables = new Dictionary<string, int>();
      this.stringVariables = new Dictionary<string, string>();
    }

    public bool analyze(IAST ast) {
      this.ast = ast;
      return checkVariableDeclarations();
      //return checkTypes(ast);
    }

    private bool checkVariableDeclarations() {
      ProgramNode program = (ProgramNode)this.ast.getProgram();
      INodeVisitor varDeclarationVisitor = new VarDeclarationVisitor(integerVariables, stringVariables);
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
      return this.stringVariables[variable];
    }

    public bool variableExists(string variable) {
      return this.integerVariables.ContainsKey(variable);
    }
  }

}