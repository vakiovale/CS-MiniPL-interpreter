using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class PlusOperationNode : Node<MiniPLTokenType> {

    public PlusOperationNode() : base(MiniPLTokenType.PLUS) {}

    public bool accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }
  }

}