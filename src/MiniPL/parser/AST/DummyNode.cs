using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class DummyNode<T> : Node<T> {

    public DummyNode(T t) : base(t) {}

    public override void accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }
  }

}