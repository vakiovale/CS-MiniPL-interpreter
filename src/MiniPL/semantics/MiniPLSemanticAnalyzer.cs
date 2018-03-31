using System;
using System.Collections.Generic;
using MiniPL.exceptions;
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
      return analyzeSemantics();
    }

    private bool analyzeSemantics() {
      try {
        ProgramNode program = (ProgramNode)this.ast.getProgram();
        INodeVisitor varDeclarationVisitor = new VarDeclarationVisitor(symbolTable);
        INodeVisitor typeCheckingVisitor = new TypeCheckingVisitor(symbolTable);
        program.accept(varDeclarationVisitor);
        program.accept(typeCheckingVisitor);
        return true;
      } catch(SemanticException exception) {
        throw exception;
      }
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