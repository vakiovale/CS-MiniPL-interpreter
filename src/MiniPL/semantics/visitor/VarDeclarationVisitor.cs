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

    private Stack<bool> boolStack;

    public VarDeclarationVisitor(IDictionary<string, int> integerVariables, IDictionary<string, string> stringVariables, IDictionary<string, bool> boolVariables) {
      this.integerVariables = integerVariables; 
      this.stringVariables = stringVariables;
      this.boolVariables = boolVariables;
      this.intStack = new Stack<int>();
      this.strStack = new Stack<string>();
      this.boolStack = new Stack<bool>();
    }

    public void visitExpression(ExpressionNode node) {
      node.getChildren()[0].accept(this);
    }

    public void visitIdentifier(IdentifierNode node) {
      string variableName = node.getVariableName();
      if(integerVariables.ContainsKey(variableName)) {
        this.intStack.Push(integerVariables[variableName]);
      } else if(stringVariables.ContainsKey(variableName)) {
        this.strStack.Push(stringVariables[variableName]);
      } else if(boolVariables.ContainsKey(variableName)) {
        this.boolStack.Push(boolVariables[variableName]);
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
      } else if(boolType) {
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
      } else if(boolType) {
        bool lhs = this.boolStack.Pop();
        bool rhs = this.boolStack.Pop();
        bool value = lhs == rhs; 
        this.boolStack.Push(value);
      }
    }

    public void visitVarDeclaration(VarDeclarationNode node) {
      this.intStack.Clear();
      this.intStack.Push(0);
      this.strStack.Clear();
      this.strStack.Push("");
      this.boolStack.Clear();
      this.boolStack.Push(false);
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
        foreach(INode innerNode in typeNode.getChildren()) {
          innerNode.accept(this);
        }
        this.integerVariables.Add(variableName, this.intStack.Pop());
      } else if(type == MiniPLTokenType.TYPE_IDENTIFIER_STRING) {
        foreach(INode innerNode in typeNode.getChildren()) {
          innerNode.accept(this);
        }
        this.stringVariables.Add(variableName, this.strStack.Pop());
      } else if(type == MiniPLTokenType.TYPE_IDENTIFIER_BOOL) {
        this.boolType = true;
        foreach(INode innerNode in typeNode.getChildren()) {
          innerNode.accept(this);
        }
        this.boolVariables.Add(variableName, this.boolStack.Pop());
      } else {
        throw new Exception("Unknown type usage in semantic analyzer.");
      }
    }

    private bool variableAlreadyDeclared(string variableName) {
      return integerVariables.ContainsKey(variableName) || stringVariables.ContainsKey(variableName) || boolVariables.ContainsKey(variableName); 
    }

  }

}