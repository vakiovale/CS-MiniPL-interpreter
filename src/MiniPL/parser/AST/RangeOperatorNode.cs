using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class RangeOperatorNode : Node<MiniPLTokenType> {

    public RangeOperatorNode() : base(MiniPLTokenType.RANGE_OPERATOR) {}

    public override void accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }
  }

}