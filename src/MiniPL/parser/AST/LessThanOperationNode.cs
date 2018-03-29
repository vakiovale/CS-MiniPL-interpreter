using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class LessThanOperationNode : Node<MiniPLTokenType> {

    public LessThanOperationNode() : base(MiniPLTokenType.LESS_THAN_COMPARISON) {}

    public override void accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }
  }

}