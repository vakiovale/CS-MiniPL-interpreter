using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class StringLiteralNode : Node<string> {

    private string strValue;

    public StringLiteralNode(Token<MiniPLTokenType> token) : base(token.getLexeme()) {
      this.strValue = token.getLexeme();
    }

    public override void accept(INodeVisitor visitor) {
      visitor.visitStringLiteral(this);
    }

    public string getString() {
      return this.strValue;
    }
  }

}