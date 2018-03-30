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

    public void visitVarDeclaration(VarDeclarationNode node) {
      IdentifierNode identifier = (IdentifierNode)node.getChildren()[0];
      string variableName = identifier.getVariableName();
      if(variableAlreadyDeclared(variableName)) {
        throw new SemanticException("Variable '" + variableName + "' already declared.");
      }
      TypeNode typeNode = (TypeNode)node.getChildren()[1];
      
      MiniPLTokenType type = (MiniPLTokenType)typeNode.getValue();
      ExpressionVisitor expressionVisitor = new ExpressionVisitor(symbolTable);
      expressionVisitor.initializeExpressionVariables();

      if(type == MiniPLTokenType.TYPE_IDENTIFIER_INTEGER) {
        accessInnerNodes(typeNode, expressionVisitor);
        int value = expressionVisitor.getInt();
        this.symbolTable.addVariable(variableName, value);
      } else if(type == MiniPLTokenType.TYPE_IDENTIFIER_STRING) {
        accessInnerNodes(typeNode, expressionVisitor);
        string value = expressionVisitor.getString();
        this.symbolTable.addVariable(variableName, value);
      } else if(type == MiniPLTokenType.TYPE_IDENTIFIER_BOOL) {
        expressionVisitor.setBoolFlag();
        accessInnerNodes(typeNode, expressionVisitor);
        bool value = expressionVisitor.getBool();
        this.symbolTable.addVariable(variableName, value);
      } else {
        throw new Exception("Unknown type usage in semantic analyzer.");
      }
    }

    private void accessInnerNodes(TypeNode node, ExpressionVisitor expressionVisitor) {
      foreach(INode innerNode in node.getChildren()) {
        innerNode.accept(expressionVisitor);
      }
    }

    private bool variableAlreadyDeclared(string variableName) {
      return symbolTable.hasVariable(variableName); 
    }

  }

}