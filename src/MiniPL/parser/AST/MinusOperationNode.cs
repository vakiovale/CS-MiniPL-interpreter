using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class MinusOperationNode : Node<MiniPLTokenType> {

    public MinusOperationNode() : base(MiniPLTokenType.MINUS) {}

    public override void accept(INodeVisitor visitor) {
      visitor.visitMinus(this);
    }

  }

}