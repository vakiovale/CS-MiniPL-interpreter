using System;
using System.Collections.Generic;
using MiniPL.parser.AST;
using MiniPL.semantics.visitor;

namespace MiniPL.semantics {

  public class MiniPLSemanticAnalyzer : ISemanticAnalyzer {

    private IAST ast; 

    private ISymbolTable symbolTable;

    public MiniPLSemanticAnalyzer() {
      this.ast = null;
      this.symbolTable = new SymbolTable();
    }

    public bool analyze(IAST ast) {
      this.ast = ast;
      return checkVariableDeclarations();
      //return checkTypes(ast);
    }

    private bool checkVariableDeclarations() {
      ProgramNode program = (ProgramNode)this.ast.getProgram();
      INodeVisitor varDeclarationVisitor = new VarDeclarationVisitor(symbolTable);
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
      return this.symbolTable.getInt(variable);
    }

    public string getString(string variable) {
      return this.symbolTable.getString(variable);
    }

    public bool getBool(string variable) {
      return this.symbolTable.getBool(variable);
    }

    public bool variableExists(string variable) {
      return this.symbolTable.hasVariable(variable);
    }

  }

}