using System;
using System.Collections;
using System.Collections.Generic;
using MiniPL.exceptions;
using MiniPL.parser.AST;
using MiniPL.tokens;

namespace MiniPL.semantics.visitor {

  public class VarDeclarationVisitor : INodeVisitor
  {
    private IDictionary<string, int> integerVariables;

    private IDictionary<string, string> stringVariables;

    private IDictionary<string, bool> boolVariables;

    private bool strType = false;

    private bool intType = false;

    private bool boolType = false;

    private Stack<int> intStack;

    private Stack<string> strStack;


    private bool boolValue = false;

    public VarDeclarationVisitor(IDictionary<string, int> integerVariables, IDictionary<string, string> stringVariables, IDictionary<string, bool> boolVariables) {
      this.integerVariables = integerVariables; 
      this.stringVariables = stringVariables;
      this.boolVariables = boolVariables;
      this.intStack = new Stack<int>();
      this.strStack = new Stack<string>();
    }

    public void visitExpression(ExpressionNode node) {
      node.getChildren()[0].accept(this);
    }

    public void visitIdentifier(IdentifierNode node) {
      throw new NotImplementedException();
    }

    public void visitIntegerLiteral(IntegerLiteralNode node) {
      this.intStack.Push(node.getInt());
    }

    public void visitStringLiteral(StringLiteralNode node) {
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

    public void visitVarDeclaration(VarDeclarationNode node) {
      this.intStack.Clear();
      this.intStack.Push(0);
      this.strStack.Clear();
      this.strStack.Push("");
      this.boolValue = false;
      this.intType = false;
      this.strType = false;
      this.boolType = false;
      IdentifierNode identifier = (IdentifierNode)node.getChildren()[0];
      string variableName = identifier.getVariableName();
      if(variableAlreadyDeclared(variableName)) {
        throw new SemanticException("Variable '" + variableName + "' already declared.");
      }
      TypeNode typeNode = (TypeNode)node.getChildren()[1];
      
      MiniPLTokenType type = (MiniPLTokenType)typeNode.getValue();
      if(type == MiniPLTokenType.TYPE_IDENTIFIER_INTEGER) {
        this.intType = true;
        foreach(INode innerNode in typeNode.getChildren()) {
          innerNode.accept(this);
        }
        this.integerVariables.Add(variableName, this.intStack.Pop());
      } else if(type == MiniPLTokenType.TYPE_IDENTIFIER_STRING) {
        this.strType = true;
        foreach(INode innerNode in typeNode.getChildren()) {
          innerNode.accept(this);
        }
        this.stringVariables.Add(variableName, this.strStack.Pop());
      } else if(type == MiniPLTokenType.TYPE_IDENTIFIER_BOOL) {
        this.boolType = true;
        foreach(INode innerNode in typeNode.getChildren()) {
          innerNode.accept(this);
        }
        this.boolVariables.Add(variableName, this.boolValue);
      } else {
        throw new Exception("Unknown type usage in semantic analyzer.");
      }
    }

    private bool variableAlreadyDeclared(string variableName) {
      return integerVariables.ContainsKey(variableName) || stringVariables.ContainsKey(variableName) || boolVariables.ContainsKey(variableName); 
    }

  }

}