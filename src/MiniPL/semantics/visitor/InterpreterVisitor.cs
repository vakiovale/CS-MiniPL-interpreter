using System;
using System.Collections.Generic;
using MiniPL.logger;
using MiniPL.parser.AST;
using MiniPL.tokens;

namespace MiniPL.semantics.visitor {

  public class InterpreterVisitor : INodeVisitor
  {
    private ISymbolTable symbolTable;

    private ILogger logger;

    private bool strType = false;

    private bool intType = false;

    private Stack<int> intStack;

    private Stack<string> strStack;

    private Stack<bool> boolStack;

    public InterpreterVisitor(ISymbolTable symbolTable, ILogger logger) {
      this.symbolTable = symbolTable;
      this.logger = logger;
      this.intStack = new Stack<int>();
      this.strStack = new Stack<string>();
      this.boolStack = new Stack<bool>();
      this.intStack.Clear();
      this.intStack.Push(0);
      this.strStack.Clear();
      this.strStack.Push("");
      this.boolStack.Clear();
      this.boolStack.Push(false);
      this.intType = false;
      this.strType = false;
    }

    public void visitExpression(ExpressionNode node) {
      node.getChildren()[0].accept(this);
    }

    public void visitIdentifier(IdentifierNode node) {
      string variableName = node.getVariableName();
      if(this.symbolTable.hasInteger(variableName)) {
        this.intStack.Push(this.symbolTable.getInt(variableName));
      } else if(this.symbolTable.hasString(variableName)) {
        this.strStack.Push(this.symbolTable.getString(variableName));
      } else if(this.symbolTable.hasBool(variableName)) {
        this.boolStack.Push(this.symbolTable.getBool(variableName));
      }
    }

    public void visitIntegerLiteral(IntegerLiteralNode node) {
      this.intType = true;
      this.intStack.Push(node.getInt());
    }

    public void visitStringLiteral(StringLiteralNode node) {
      this.strType = true;
      this.strStack.Push(node.getString());
    }

    public void visitPlus(PlusOperationNode node) {
      readValues(node);
      if(intType) {
        int value = this.intStack.Pop() + this.intStack.Pop();
        this.intStack.Push(value);
      } else if(strType) {
        string value = this.strStack.Pop() + this.strStack.Pop();
        this.strStack.Push(value);
      }
    }

    private void readValues(INode node) { 
      INode rhs = node.getChildren()[1];
      rhs.accept(this);
      INode lhs = node.getChildren()[0];
      lhs.accept(this);
    }


    public void visitMinus(MinusOperationNode node) {
      readValues(node);
      int value = this.intStack.Pop() - this.intStack.Pop();
      this.intStack.Push(value);
    }

    public void visitDivision(DivisionOperationNode node) {
      readValues(node);
      int value = this.intStack.Pop() / this.intStack.Pop();
      this.intStack.Push(value);
    }

    public void visitMultiplication(MultiplicationOperationNode node) {
      readValues(node);
      int value = this.intStack.Pop() * this.intStack.Pop();
      this.intStack.Push(value);
    }

    public void visitLessThanOperator(LessThanOperationNode node) {
      readValues(node);
      if(intType) {
        bool value = this.intStack.Pop() < this.intStack.Pop();
        this.boolStack.Push(value);
        this.intType = false;
      } else if(strType) {
        bool value = String.Compare(this.strStack.Pop(), this.strStack.Pop()) < 0;
        this.boolStack.Push(value);
        this.strType = false;
      } else {
        bool lhs = this.boolStack.Pop();
        bool rhs = this.boolStack.Pop();
        bool value = (!lhs && rhs) ? true : false; 
        this.boolStack.Push(value);
      }
    }

    public void visitEqualityOperator(EqualityOperationNode node) {
      readValues(node);
      if(intType) {
        bool value = this.intStack.Pop() == this.intStack.Pop();
        this.boolStack.Push(value);
        this.intType = false;
      } else if(strType) {
        bool value = String.Equals(this.strStack.Pop(), this.strStack.Pop());
        this.boolStack.Push(value);
        this.strType = false;
      } else {
        bool lhs = this.boolStack.Pop();
        bool rhs = this.boolStack.Pop();
        bool value = lhs == rhs; 
        this.boolStack.Push(value);
      }
    }

    public void visitVarDeclaration(VarDeclarationNode node) {
      IdentifierNode identifier = (IdentifierNode)node.getChildren()[0];
      string variableName = identifier.getVariableName();
      foreach(INode innerNode in node.getChildren()[1].getChildren()) {
        innerNode.accept(this);
      } 
      if(this.symbolTable.hasInteger(variableName)) {
        this.symbolTable.updateVariable(variableName, this.intStack.Pop());
      } else if(this.symbolTable.hasString(variableName)) {
        this.symbolTable.updateVariable(variableName, this.strStack.Pop());
      } else if(this.symbolTable.hasBool(variableName)) {
        this.symbolTable.updateVariable(variableName, this.boolStack.Pop());
      }
    }

    private void accessInnerNodes(TypeNode node) {
      foreach(INode innerNode in node.getChildren()) {
        innerNode.accept(this);
      }
    }

    private bool variableAlreadyDeclared(string variableName) {
      return this.symbolTable.hasVariable(variableName);
    }

    public void visitVarAssignment(VarAssignmentNode node) {
      throw new NotImplementedException();
    }

    public void visitForLoop(ForLoopNode forLoopNode) {
      throw new NotImplementedException();
    }

    public void visitPrint(PrintNode printNode) {
      throw new NotImplementedException();
    }

    public void visitAssert(AssertNode assertNode) {
      throw new NotImplementedException();
    }

    public void visitRead(ReadNode readNode) {
      throw new NotImplementedException();
    }

    public void visitLogicalNotOperator(LogicalNotOperationNode logicalNotOperationNode) {
      throw new NotImplementedException();
    }

    public void visitLogicalAndOperator(LogicalAndOperationNode logicalAndOperationNode) {
      throw new NotImplementedException();
    }

  }

}