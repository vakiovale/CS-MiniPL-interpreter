using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.syntax;

namespace MiniPL.parser.AST {

  public class VarDeclarationNode : Node<MiniPLSymbol> {

    public VarDeclarationNode() : base(MiniPLSymbol.VAR_DECLARATION) {}

    public bool accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }

  }

}