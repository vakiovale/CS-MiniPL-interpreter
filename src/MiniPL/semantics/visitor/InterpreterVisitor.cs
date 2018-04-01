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
      initializeStacks();
      this.intType = false;
      this.strType = false;
    }

    private void initializeStacks() {
      this.intStack = new Stack<int>();
      this.strStack = new Stack<string>();
      this.boolStack = new Stack<bool>();
      reviveIntegerStackIfNeeded();
      reviveStringStackIfNeeded();
      reviveBoolStackIfNeeded();
    }

    public void visitExpression(ExpressionNode node) {
      foreach(INode child in node.getChildren()) {
        child.accept(this);
      }
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

    public void visitVarAssignment(VarAssignmentNode node) {
      updateValue(node);
    }

    public void visitVarDeclaration(VarDeclarationNode node) {
      updateValue(node);
    }

    private void updateValue(INode node) {
      IdentifierNode identifier = (IdentifierNode)node.getChildren()[0];
      string variableName = identifier.getVariableName();
      INode possibleNode = node.getChildren()[1];
      foreach(INode child in possibleNode.getChildren()) {
        child.accept(this);
      }
      if(this.symbolTable.hasInteger(variableName)) {
        this.symbolTable.updateVariable(variableName, this.intStack.Pop());
        reviveIntegerStackIfNeeded();
      } else if(this.symbolTable.hasString(variableName)) {
        this.symbolTable.updateVariable(variableName, this.strStack.Pop());
        reviveStringStackIfNeeded();
      } else if(this.symbolTable.hasBool(variableName)) {
        this.symbolTable.updateVariable(variableName, this.boolStack.Pop());
        reviveBoolStackIfNeeded();
      }
    }

    private void reviveBoolStackIfNeeded() {
      if(this.boolStack.Count == 0) {
        this.boolStack.Push(false);
      }
    }

    private void reviveStringStackIfNeeded() {
      if(this.strStack.Count == 0) {
        this.strStack.Push("");
      }
    }

    private void reviveIntegerStackIfNeeded() {
      if(this.intStack.Count == 0) {
        this.intStack.Push(0);
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

    public void visitForLoop(ForLoopNode forLoopNode) {
      IdentifierNode identifier = (IdentifierNode)forLoopNode.getChildren()[0];
      string controlVariable = identifier.getVariableName();
      RangeOperatorNode rangeNode = (RangeOperatorNode)forLoopNode.getChildren()[1];
      StatementListNode forStatements = (StatementListNode)forLoopNode.getChildren()[2];
      rangeNode.getChildren()[0].accept(this);
      int begin = this.intStack.Pop();
      rangeNode.getChildren()[1].accept(this);
      int end = this.intStack.Pop();
      for(int i = begin; i <= end; i++) {
        this.symbolTable.updateVariable(controlVariable, i);
        foreach(INode child in forStatements.getChildren()) {
          child.accept(this);
        }
      }
    }

    public void visitPrint(PrintNode printNode) {
      INode expression = printNode.getChildren()[0];
      expression.accept(this);
      if(this.intType) {
        this.logger.log(this.intStack.Pop().ToString());
        this.intType = false;
      } else if(this.strType) {
        this.logger.log(this.strStack.Pop());
        this.strType = false;
      }
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