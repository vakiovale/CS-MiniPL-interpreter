using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.syntax;

namespace MiniPL.parser.AST {

  public class AssertNode : Node<MiniPLSymbol> {

    public AssertNode() : base(MiniPLSymbol.ASSERT_PROCEDURE) {}

    public bool accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }

  }

}