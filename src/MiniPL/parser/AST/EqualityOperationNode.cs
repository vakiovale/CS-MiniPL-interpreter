using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class EqualityOperationNode : Node<MiniPLTokenType> {

    public EqualityOperationNode() : base(MiniPLTokenType.EQUALITY_COMPARISON) {}

    public override void accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }
  }

}