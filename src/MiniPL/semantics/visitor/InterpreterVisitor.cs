using System;
using System.Collections.Generic;
using MiniPL.io;
using MiniPL.parser.AST;
using MiniPL.tokens;

namespace MiniPL.semantics.visitor {

  public class InterpreterVisitor : INodeVisitor
  {
    private ISymbolTable symbolTable;

    IInputOutput inputOutput;

    private bool strType = false;

    private bool intType = false;

    private Stack<int> intStack;

    private Stack<string> strStack;

    private Stack<bool> boolStack;

    public InterpreterVisitor(ISymbolTable symbolTable, IInputOutput inputOutput) {
      this.symbolTable = symbolTable;
      this.inputOutput = inputOutput;
      initializeStacks();
      this.intType = false;
      this.strType = false;
    }

    private void initializeStacks() {
      this.intStack = new Stack<int>();
      this.strStack = new Stack<string>();
      this.boolStack = new Stack<bool>();
    }

    private string readString() {
      return this.inputOutput.input();
    }

    private int readInteger() {
      return Int32.Parse(this.inputOutput.input());
    }

    public void visitExpression(ExpressionNode node) {
      foreach(INode child in node.getChildren()) {
        child.accept(this);
      }
    }

    public void visitIdentifier(IdentifierNode node) {
      string variableName = node.getVariableName();
      if(this.symbolTable.hasInteger(variableName)) {
        this.intType = true;
        this.intStack.Push(this.symbolTable.getInt(variableName));
      } else if(this.symbolTable.hasString(variableName)) {
        this.strType = true;
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
        int value = popInt() + popInt();
        this.intStack.Push(value);
      } else if(strType) {
        string value = popString() + popString();
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
      int value = popInt() - popInt();
      this.intStack.Push(value);
    }

    public void visitDivision(DivisionOperationNode node) {
      readValues(node);
      int value = popInt() / popInt();
      this.intStack.Push(value);
    }

    public void visitMultiplication(MultiplicationOperationNode node) {
      readValues(node);
      int value = popInt() * popInt();
      this.intStack.Push(value);
    }

    public void visitLogicalNotOperator(LogicalNotOperationNode node) {
      INode rhs = node.getChildren()[0];
      rhs.accept(this);
      bool value = !popBool();
      this.boolStack.Push(value);
    }

    public void visitLogicalAndOperator(LogicalAndOperationNode node) {
      readValues(node);
      bool lhs = popBool();
      bool rhs = popBool();
      bool value = lhs && rhs;
      this.boolStack.Push(value);
    }

    public void visitLessThanOperator(LessThanOperationNode node) {
      readValues(node);
      if(intType) {
        bool value = popInt() < popInt();
        this.boolStack.Push(value);
        this.intType = false;
      } else if(strType) {
        bool value = String.Compare(popString(), popString()) < 0;
        this.boolStack.Push(value);
        this.strType = false;
      } else {
        bool lhs = popBool();
        bool rhs = popBool();
        bool value = (!lhs && rhs) ? true : false; 
        this.boolStack.Push(value);
      }
    }

    public void visitEqualityOperator(EqualityOperationNode node) {
      readValues(node);
      if(intType) {
        bool value = popInt() == popInt();
        this.boolStack.Push(value);
        this.intType = false;
      } else if(strType) {
        bool value = String.Equals(popString(), popString());
        this.boolStack.Push(value);
        this.strType = false;
      } else {
        bool lhs = popBool();
        bool rhs = popBool();
        bool value = lhs == rhs; 
        this.boolStack.Push(value);
      }
    }

    public void visitAssert(AssertNode node) {
      node.getChildren()[0].accept(this);
      if(!popBool()) {
        this.inputOutput.outputLine("Assertion failed.");
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
        this.symbolTable.updateVariable(variableName, popInt());
        this.intType = false;
      } else if(this.symbolTable.hasString(variableName)) {
        this.symbolTable.updateVariable(variableName, popString());
        this.strType = false;
      } else if(this.symbolTable.hasBool(variableName)) {
        this.symbolTable.updateVariable(variableName, popBool());
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
      int begin = popInt();
      rangeNode.getChildren()[1].accept(this);
      int end = popInt();
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
        this.inputOutput.output(popInt());
        this.intType = false;
      } else if(this.strType) {
        this.inputOutput.output(popString());
        this.strType = false;
      }
    }

    private int popInt() {
      if(this.intStack.Count == 0) {
        return 0;
      } else {
        return this.intStack.Pop();
      }
    }

    private string popString() {
      if(this.strStack.Count == 0) {
        return "";
      } else {
        return this.strStack.Pop();
      }
    }

    private bool popBool() {
      if(this.boolStack.Count == 0) {
        return false;
      } else {
        return this.boolStack.Pop();
      }
    }

    public void visitRead(ReadNode node) {
      IdentifierNode identifier = (IdentifierNode)node.getChildren()[0];
      string variableName = identifier.getVariableName();
      if(this.symbolTable.hasInteger(variableName)) {
        this.symbolTable.updateVariable(variableName, readInteger());
      } else if(this.symbolTable.hasString(variableName)) {
        this.symbolTable.updateVariable(variableName, readString());
      }
    }
  }

}