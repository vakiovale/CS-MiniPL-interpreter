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

    private IDictionary<string, bool> boolVariables;

    public VarDeclarationVisitor(IDictionary<string, int> integerVariables, IDictionary<string, string> stringVariables, IDictionary<string, bool> boolVariables) {
      this.integerVariables = integerVariables; 
      this.stringVariables = stringVariables;
      this.boolVariables = boolVariables;
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
      } else if(type == MiniPLTokenType.TYPE_IDENTIFIER_BOOL) {
        bool value = typeNode.getBoolValue();
        this.boolVariables.Add(variableName, value);
      } else {
        throw new Exception("Unknown type usage in semantic analyzer.");
      }
    }

    private bool variableAlreadyDeclared(string variableName) {
      return integerVariables.ContainsKey(variableName) || stringVariables.ContainsKey(variableName) || boolVariables.ContainsKey(variableName); 
    }
  }

}