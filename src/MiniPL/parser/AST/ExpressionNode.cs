using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.syntax;

namespace MiniPL.parser.AST {

  public class ExpressionNode : Node<MiniPLSymbol> {

    public ExpressionNode() : base(MiniPLSymbol.EXPRESSION) {}

    public override void accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }

    public int getIntegerValue() {
      throw new NotImplementedException();
    }

    public string getStringValue() {
      throw new NotImplementedException();
    }

    public bool getBoolValue() {
      throw new NotImplementedException();
    }
  }

}