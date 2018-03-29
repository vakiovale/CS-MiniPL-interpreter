using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.syntax;

namespace MiniPL.parser.AST {

  public class VarAssignmentNode : Node<MiniPLSymbol> {

    public VarAssignmentNode() : base(MiniPLSymbol.VAR_ASSIGNMENT) {}

    public override void accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }

  }

}