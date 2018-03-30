using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.syntax;

namespace MiniPL.parser.AST {

  public class ExpressionNode : Node<MiniPLSymbol> {

    public ExpressionNode() : base(MiniPLSymbol.EXPRESSION) {}

    public override void accept(INodeVisitor visitor) {
      visitor.visitExpression(this);
    }
  }

}