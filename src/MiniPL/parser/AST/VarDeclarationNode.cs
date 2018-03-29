using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.syntax;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class VarDeclarationNode : Node<MiniPLSymbol> {

    public VarDeclarationNode() : base(MiniPLSymbol.VAR_DECLARATION) {}

    public override void accept(INodeVisitor visitor) {
      visitor.visitVarDeclaration(this);
    }

    public MiniPLTokenType getType() {
      INode type = this.getChildren()[0];
      return (MiniPLTokenType)type.getValue();
    }

    public int getIntegerValue() {
      return 0;
    }
  }

}