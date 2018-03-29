using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class LogicalNotOperationNode : Node<MiniPLTokenType> {

    public LogicalNotOperationNode() : base(MiniPLTokenType.LOGICAL_NOT) {}

    public bool accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }
  }

}
