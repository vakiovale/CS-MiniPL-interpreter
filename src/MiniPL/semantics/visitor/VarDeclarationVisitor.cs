using System;
using System.Collections.Generic;
using MiniPL.exceptions;
using MiniPL.parser.AST;
using MiniPL.tokens;

namespace MiniPL.semantics.visitor {

  public class VarDeclarationVisitor : INodeVisitor
  {
    private IDictionary<string, int> integerVariables;

    private IDictionary<string, string> stringVariables;

    public VarDeclarationVisitor(IDictionary<string, int> integerVariables, IDictionary<string, string> stringVariables) {
      this.integerVariables = integerVariables; 
      this.stringVariables = stringVariables;
    }

    public void visitVarDeclaration(VarDeclarationNode node) {
      IdentifierNode identifier = (IdentifierNode)node.getChildren()[0];
      string variableName = identifier.getVariableName();
      if(variableAlreadyDeclared(variableName)) {
        throw new SemanticException("Variable '" + variableName + "' already declared.");
      }
      TypeNode typeNode = (TypeNode)node.getChildren()[1];
      MiniPLTokenType type = (MiniPLTokenType)typeNode.getValue();
      if(type == MiniPLTokenType.TYPE_IDENTIFIER_INTEGER) {
        int value = typeNode.getIntegerValue();
        this.integerVariables.Add(variableName, value);
      } else if(type == MiniPLTokenType.TYPE_IDENTIFIER_STRING) {
        string value = typeNode.getStringValue();
        this.stringVariables.Add(variableName, value);
      }
      
    }

    private bool variableAlreadyDeclared(string variableName) {
      return integerVariables.ContainsKey(variableName) || stringVariables.ContainsKey(variableName); 
    }
  }

}