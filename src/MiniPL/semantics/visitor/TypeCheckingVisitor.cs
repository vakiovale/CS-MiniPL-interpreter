using System;
using System.Collections.Generic;
using MiniPL.exceptions;
using MiniPL.parser.AST;
using MiniPL.tokens;

namespace MiniPL.semantics.visitor {

  public class TypeCheckingVisitor : INodeVisitor {

    private ISymbolTable symbolTable;

    private Stack<MiniPLTokenType> typeStack;

    public TypeCheckingVisitor(ISymbolTable symbolTable) {
      this.symbolTable = symbolTable;
    }

    public void visitDivision(DivisionOperationNode node) {
      accessChildren(node);
      MiniPLTokenType left = this.typeStack.Pop();
      MiniPLTokenType right = this.typeStack.Pop();
      if(left != right) {
        throw new SemanticException("Wrong type. Expected an integer.");
      }
      this.typeStack.Push(MiniPLTokenType.TYPE_IDENTIFIER_INTEGER);
    }

    public void visitEqualityOperator(EqualityOperationNode node) {
      accessChildren(node);
      MiniPLTokenType left = this.typeStack.Pop();
      MiniPLTokenType right = this.typeStack.Pop();
      if(left != right) {
        throw new SemanticException("Equality operator has different types on both sides.");
      }
      this.typeStack.Push(MiniPLTokenType.TYPE_IDENTIFIER_BOOL);
    }

    public void visitLessThanOperator(LessThanOperationNode node) {
      accessChildren(node);
      MiniPLTokenType left = this.typeStack.Pop();
      MiniPLTokenType right = this.typeStack.Pop();
      if(left != right) {
        throw new SemanticException("Less than operator has different types on both sides.");
      }
      this.typeStack.Push(MiniPLTokenType.TYPE_IDENTIFIER_BOOL);
    }

    public void visitExpression(ExpressionNode node) {
      accessChildren(node);
    }

    public void visitIdentifier(IdentifierNode node) {
      if(!this.symbolTable.hasVariable(node.getVariableName())) {
        throw new SemanticException("Variable has not been declared.");
      } else {
        if(this.symbolTable.hasInteger(node.getVariableName())) {
          this.typeStack.Push(MiniPLTokenType.TYPE_IDENTIFIER_INTEGER);
        }
        if(this.symbolTable.hasString(node.getVariableName())) {
          this.typeStack.Push(MiniPLTokenType.TYPE_IDENTIFIER_STRING);
        }
        if(this.symbolTable.hasBool(node.getVariableName())) {
          this.typeStack.Push(MiniPLTokenType.TYPE_IDENTIFIER_BOOL);
        }
      }
    }

    public void visitIntegerLiteral(IntegerLiteralNode node) {
      this.typeStack.Push(MiniPLTokenType.TYPE_IDENTIFIER_INTEGER);
    }

    public void visitMinus(MinusOperationNode node) {
      accessChildren(node);
      MiniPLTokenType left = this.typeStack.Pop();
      MiniPLTokenType right = this.typeStack.Pop();
      if(left != right) {
        throw new SemanticException("Wrong type. Expected an integer.");
      }
      this.typeStack.Push(MiniPLTokenType.TYPE_IDENTIFIER_INTEGER);
    }

    public void visitMultiplication(MultiplicationOperationNode node) {
      accessChildren(node);
      MiniPLTokenType left = this.typeStack.Pop();
      MiniPLTokenType right = this.typeStack.Pop();
      if(left != right) {
        throw new SemanticException("Wrong type. Expected an integer.");
      }
      this.typeStack.Push(MiniPLTokenType.TYPE_IDENTIFIER_INTEGER);
    }

    public void visitPlus(PlusOperationNode node) {
      accessChildren(node);
      MiniPLTokenType left = this.typeStack.Pop();
      MiniPLTokenType right = this.typeStack.Pop();
      if(left == MiniPLTokenType.TYPE_IDENTIFIER_STRING) {
        if(left != right) {
          throw new SemanticException("Wrong type. Expected a string.");
        }
        this.typeStack.Push(MiniPLTokenType.TYPE_IDENTIFIER_STRING);
      } else if(left == MiniPLTokenType.TYPE_IDENTIFIER_INTEGER) {
        if(left != right) {
          throw new SemanticException("Wrong type. Expected a string.");
        }
        this.typeStack.Push(MiniPLTokenType.TYPE_IDENTIFIER_INTEGER);
      }
    }

    public void visitStringLiteral(StringLiteralNode node) {
      this.typeStack.Push(MiniPLTokenType.TYPE_IDENTIFIER_STRING);
    }

    public void visitVarAssignment(VarAssignmentNode node) {
      IdentifierNode identifier = (IdentifierNode)node.getChildren()[0];
      string variableName = identifier.getVariableName();
      INode expression = node.getChildren()[1];
      this.typeStack = new Stack<MiniPLTokenType>();
      MiniPLTokenType type;
      if(this.symbolTable.hasInteger(variableName)) {
        type = MiniPLTokenType.TYPE_IDENTIFIER_INTEGER;
        typeCheck(expression, type);
      } else if(this.symbolTable.hasString(variableName)) {
        type = MiniPLTokenType.TYPE_IDENTIFIER_STRING;
        typeCheck(expression, type);
      } else if(this.symbolTable.hasBool(variableName)) {
        type = MiniPLTokenType.TYPE_IDENTIFIER_BOOL;
        typeCheck(expression, type);
      }
    }

    public void visitVarDeclaration(VarDeclarationNode node) {
      IdentifierNode identifier = (IdentifierNode)node.getChildren()[0];
      string variableName = identifier.getVariableName();
      TypeNode typeNode = (TypeNode)node.getChildren()[1];
      MiniPLTokenType type = (MiniPLTokenType)typeNode.getValue();
      this.typeStack = new Stack<MiniPLTokenType>();
      typeCheck(typeNode, type);
    }

    public void visitForLoop(ForLoopNode node) {
      RangeOperatorNode rangeOperatorNode = (RangeOperatorNode)node.getChildren()[1];
      this.typeStack = new Stack<MiniPLTokenType>();
      INode leftExpression = rangeOperatorNode.getChildren()[0];
      this.typeStack.Push(MiniPLTokenType.TYPE_IDENTIFIER_INTEGER);
      accessChildren(leftExpression);
      if(this.typeStack.Pop() != MiniPLTokenType.TYPE_IDENTIFIER_INTEGER) {
        throw new SemanticException("Expected an integer. Wrong type in the left hand side of range operator.");
      }
      INode rightExpression = rangeOperatorNode.getChildren()[1];
      this.typeStack.Clear();
      this.typeStack.Push(MiniPLTokenType.TYPE_IDENTIFIER_INTEGER);
      accessChildren(rightExpression);
      if(this.typeStack.Pop() != MiniPLTokenType.TYPE_IDENTIFIER_INTEGER) {
        throw new SemanticException("Expected an integer. Wrong type in the right hand side of range operator.");
      }
    }

    private void typeCheck(INode node, MiniPLTokenType type) {
      if(type == MiniPLTokenType.TYPE_IDENTIFIER_INTEGER) {
        this.typeStack.Push(MiniPLTokenType.TYPE_IDENTIFIER_INTEGER);
        accessChildren(node);
        if(this.typeStack.Pop() != MiniPLTokenType.TYPE_IDENTIFIER_INTEGER) {
          throw new SemanticException("Wrong type. Expected an integer.");
        }
      } else if(type == MiniPLTokenType.TYPE_IDENTIFIER_STRING) {
        this.typeStack.Push(MiniPLTokenType.TYPE_IDENTIFIER_STRING);
        accessChildren(node);
        if(this.typeStack.Pop() != MiniPLTokenType.TYPE_IDENTIFIER_STRING) {
          throw new SemanticException("Wrong type. Expected a string.");
        }
      } else if(type == MiniPLTokenType.TYPE_IDENTIFIER_BOOL) {
        this.typeStack.Push(MiniPLTokenType.TYPE_IDENTIFIER_BOOL);
        accessChildren(node);
        if(this.typeStack.Pop() != MiniPLTokenType.TYPE_IDENTIFIER_BOOL) {
          throw new SemanticException("Wrong type. Expected a bool.");
        }
      }
    }

    private void accessChildren(INode node) {
      foreach(INode innerNode in node.getChildren()) {
        innerNode.accept(this);
      }
    }

    public void visitPrint(PrintNode printNode) {
      //throw new NotImplementedException();
    }
  }

}