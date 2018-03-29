using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class DivisionOperationNode : Node<MiniPLTokenType> {

    public DivisionOperationNode() : base(MiniPLTokenType.SLASH) {}

    public bool accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }
  }

}