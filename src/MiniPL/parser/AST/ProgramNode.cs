using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.syntax;

namespace MiniPL.parser.AST {

  public class ProgramNode : Node<MiniPLSymbol> {

    public ProgramNode() : base(MiniPLSymbol.PROGRAM) {}

    public bool accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }

  }

}