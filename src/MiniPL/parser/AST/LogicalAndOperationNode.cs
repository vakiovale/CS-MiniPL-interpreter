using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class LogicalAndOperationNode : Node<MiniPLTokenType> {

    public LogicalAndOperationNode() : base(MiniPLTokenType.LOGICAL_AND) {}

    public override void accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }
  }

}