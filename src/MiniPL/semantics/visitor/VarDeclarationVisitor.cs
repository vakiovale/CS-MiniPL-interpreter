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

    public VarDeclarationVisitor(ISymbolTable symbolTable) {
      this.symbolTable = symbolTable;
    }

    public void visitExpression(ExpressionNode node) {
      node.getChildren()[0].accept(this);
    }

    public void visitIdentifier(IdentifierNode node) {
      throw new NotImplementedException();
    }

    public void visitIntegerLiteral(IntegerLiteralNode node) {
      throw new NotImplementedException();
    }

    public void visitStringLiteral(StringLiteralNode node) {
      throw new NotImplementedException();
    }

    public void visitPlus(PlusOperationNode node) {
      throw new NotImplementedException();
    }

    public void visitMinus(MinusOperationNode node) {
      throw new NotImplementedException();
    }

    public void visitDivision(DivisionOperationNode node) {
      throw new NotImplementedException();
    }

    public void visitMultiplication(MultiplicationOperationNode node) {
      throw new NotImplementedException();
    }

    public void visitLessThanOperator(LessThanOperationNode node) {
      throw new NotImplementedException();
    }

    public void visitEqualityOperator(EqualityOperationNode node) {
      throw new NotImplementedException();
    }

    public void visitVarAssignment(VarAssignmentNode node) {
      IdentifierNode identifier = (IdentifierNode)node.getChildren()[0];
      string variableName = identifier.getVariableName();
      if(!variableAlreadyDeclared(variableName)) {
        throw new SemanticException("Variable '" + variableName + "' has not been declared.");
      }
    }

    public void visitVarDeclaration(VarDeclarationNode node) {
      IdentifierNode identifier = (IdentifierNode)node.getChildren()[0];
      string variableName = identifier.getVariableName();
      if(variableAlreadyDeclared(variableName)) {
        throw new SemanticException("Variable '" + variableName + "' already declared.");
      }
      TypeNode typeNode = (TypeNode)node.getChildren()[1];
      
      MiniPLTokenType type = (MiniPLTokenType)typeNode.getValue();

      //ExpressionVisitor expressionVisitor = new ExpressionVisitor(symbolTable);

      if(type == MiniPLTokenType.TYPE_IDENTIFIER_INTEGER) {
        //accessInnerNodes(typeNode, expressionVisitor);
        //int value = expressionVisitor.getInt();
        //this.symbolTable.addVariable(variableName, value);
        this.symbolTable.addVariable(variableName, 0);
      } else if(type == MiniPLTokenType.TYPE_IDENTIFIER_STRING) {
        //accessInnerNodes(typeNode, expressionVisitor);
        //string value = expressionVisitor.getString();
        //this.symbolTable.addVariable(variableName, value);
        this.symbolTable.addVariable(variableName, "");
      } else if(type == MiniPLTokenType.TYPE_IDENTIFIER_BOOL) {
        //expressionVisitor.setBoolFlag();
        //accessInnerNodes(typeNode, expressionVisitor);
        //bool value = expressionVisitor.getBool();
        //this.symbolTable.addVariable(variableName, value);
        this.symbolTable.addVariable(variableName, false);
      } else {
        throw new Exception("Unknown type usage in semantic analyzer.");
      }
    }

/* 
    private void accessInnerNodes(TypeNode node, ExpressionVisitor expressionVisitor) {
      foreach(INode innerNode in node.getChildren()) {
        innerNode.accept(expressionVisitor);
      }
    }
    */

    private bool variableAlreadyDeclared(string variableName) {
      return symbolTable.hasVariable(variableName); 
    }
  }

}