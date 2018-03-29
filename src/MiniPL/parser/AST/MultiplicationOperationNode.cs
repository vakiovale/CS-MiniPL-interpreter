using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class MultiplicationOperationNode : Node<MiniPLTokenType> {

    public MultiplicationOperationNode() : base(MiniPLTokenType.ASTERISK) {}

    public bool accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }
  }

}