using System;
using System.Collections.Generic;
using MiniPL.parser.AST;
using MiniPL.tokens;

namespace MiniPL.semantics.visitor {

  public class VarDeclarationVisitor : INodeVisitor
  {
    IDictionary<string, int> integerVariables;

    public VarDeclarationVisitor(IDictionary<string, int> integerVariables) {
      this.integerVariables = integerVariables; 
    }

    public void visitVarDeclaration(VarDeclarationNode node) {
      IdentifierNode identifier = (IdentifierNode)node.getChildren()[0];
      string variableName = identifier.getVariableName();
      TypeNode typeNode = (TypeNode)node.getChildren()[1];
      MiniPLTokenType type = (MiniPLTokenType)typeNode.getValue();
      if(type == MiniPLTokenType.TYPE_IDENTIFIER_INTEGER) {
        int value = typeNode.getIntegerValue();
        this.integerVariables.Add("x", value);
      }
      
    }
  }

}