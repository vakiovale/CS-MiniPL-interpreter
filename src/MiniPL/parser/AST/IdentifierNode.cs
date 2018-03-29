using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class IdentifierNode : Node<string> {

    public IdentifierNode(Token<MiniPLTokenType> token) : base(token.getLexeme()) {}

    public bool accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }

  }

}