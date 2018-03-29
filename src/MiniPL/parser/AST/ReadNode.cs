using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.syntax;

namespace MiniPL.parser.AST {

  public class ReadNode : Node<MiniPLSymbol> {

    public ReadNode() : base(MiniPLSymbol.READ_PROCEDURE) {}

    public bool accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }

  }

}