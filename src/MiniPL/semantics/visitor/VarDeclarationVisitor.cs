using System;
using System.Collections;
using System.Collections.Generic;
using MiniPL.exceptions;
using MiniPL.parser.AST;
using MiniPL.tokens;

namespace MiniPL.semantics.visitor {

  public class VarDeclarationVisitor : INodeVisitor
  {

    private ISymbolTable symbolTable;

    private Stack<string> forLoopControlVariables;

    public VarDeclarationVisitor(ISymbolTable symbolTable) {
      this.symbolTable = symbolTable;
      this.forLoopControlVariables = new Stack<string>();
    }

    public void visitExpression(ExpressionNode node) {
      node.getChildren()[0].accept(this);
    }

    public void visitIdentifier(IdentifierNode node) {
      string variableName = node.getVariableName();
      if(!this.symbolTable.hasVariable(variableName)) {
        throw new SemanticException("Variable '" + variableName + "' has not been declared.");
      }
    }

    public void visitIntegerLiteral(IntegerLiteralNode node) {
    }

    public void visitStringLiteral(StringLiteralNode node) {
    }

    public void visitPlus(PlusOperationNode node) {
    }

    public void visitMinus(MinusOperationNode node) {
    }

    public void visitDivision(DivisionOperationNode node) {
    }

    public void visitMultiplication(MultiplicationOperationNode node) {
    }

    public void visitLessThanOperator(LessThanOperationNode node) {
    }

    public void visitEqualityOperator(EqualityOperationNode node) {
    }

    public void visitLogicalNotOperator(LogicalNotOperationNode logicalNotOperationNode) {
      logicalNotOperationNode.getChildren()[0].accept(this);
    }

    public void visitLogicalAndOperator(LogicalAndOperationNode logicalAndOperationNode) {
    }

    public void visitVarAssignment(VarAssignmentNode node) {
      IdentifierNode identifier = (IdentifierNode)node.getChildren()[0];
      string variableName = identifier.getVariableName();
      if(!variableAlreadyDeclared(variableName)) {
        throw new SemanticException("Variable '" + variableName + "' has not been declared.");
      }
      if(this.forLoopControlVariables.Contains(variableName)) {
        throw new SemanticException("Control variable '" + variableName + "' cannot be assigned a new value inside for loop.");
      }
    }

    public void visitForLoop(ForLoopNode node) {
      IdentifierNode identifier = (IdentifierNode)node.getChildren()[0];
      string variableName = identifier.getVariableName();
      if(!variableAlreadyDeclared(variableName)) {
        throw new SemanticException("The control variable '" + variableName + "' in for loop has not been declared.");
      }
      this.forLoopControlVariables.Push(variableName);
      node.getChildren()[2].accept(this);
      this.forLoopControlVariables.Pop();
    }

    public void visitVarDeclaration(VarDeclarationNode node) {
      IdentifierNode identifier = (IdentifierNode)node.getChildren()[0];
      string variableName = identifier.getVariableName();
      if(this.forLoopControlVariables.Count > 0) {
        throw new SemanticException("Declaring variables inside for loop is not allowed.");
      }
      if(variableAlreadyDeclared(variableName)) {
        throw new SemanticException("Variable '" + variableName + "' already declared.");
      }
      TypeNode typeNode = (TypeNode)node.getChildren()[1];
      
      MiniPLTokenType type = (MiniPLTokenType)typeNode.getValue();

      if(type == MiniPLTokenType.TYPE_IDENTIFIER_INTEGER) {
        this.symbolTable.addVariable(variableName, 0);
      } else if(type == MiniPLTokenType.TYPE_IDENTIFIER_STRING) {
        this.symbolTable.addVariable(variableName, "");
      } else if(type == MiniPLTokenType.TYPE_IDENTIFIER_BOOL) {
        this.symbolTable.addVariable(variableName, false);
      } else {
        throw new Exception("Unknown type usage in semantic analyzer.");
      }
    }

    private bool variableAlreadyDeclared(string variableName) {
      return symbolTable.hasVariable(variableName); 
    }

    public void visitPrint(PrintNode node) {
      foreach(INode child in node.getChildren()) {
        child.accept(this);
      }
    }

    public void visitAssert(AssertNode node) {
      foreach(INode child in node.getChildren()) {
        child.accept(this);
      }
    }

    public void visitRead(ReadNode node) {
      foreach(INode child in node.getChildren()) {
        child.accept(this);
      }
    }
  }

}