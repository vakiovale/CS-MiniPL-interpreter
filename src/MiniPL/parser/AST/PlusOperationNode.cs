using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class PlusOperationNode : Node<MiniPLTokenType> {

    public PlusOperationNode() : base(MiniPLTokenType.PLUS) {}

    public override void accept(INodeVisitor visitor) {
      visitor.visitPlus(this);
    }

  }

}