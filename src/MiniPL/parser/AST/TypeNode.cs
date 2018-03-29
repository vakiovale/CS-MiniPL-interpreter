using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.syntax;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class TypeNode : Node<MiniPLTokenType> {

    public TypeNode(MiniPLTokenType type) : base(type) {}

    public override void accept(INodeVisitor visitor)
    {
      throw new NotImplementedException();
    }

    internal int getIntegerValue() {
      if(this.children.Count > 0) {
        return ((ExpressionNode)this.children[0]).getIntegerValue();
      } else {
        return 0;
      }
    }
  }

}