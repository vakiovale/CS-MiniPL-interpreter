using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.syntax;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class TypeNode : Node<MiniPLTokenType> {

    public TypeNode(MiniPLTokenType type) : base(type) {}

    public bool accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }

  }

}