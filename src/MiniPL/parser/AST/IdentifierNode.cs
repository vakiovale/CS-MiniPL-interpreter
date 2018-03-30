using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class IdentifierNode : Node<string> {

    public IdentifierNode(Token<MiniPLTokenType> token) : base(token.getLexeme()) {}

    public override void accept(INodeVisitor visitor) {
      visitor.visitIdentifier(this);
    }

    public string getVariableName() {
      return (string)this.value;
    }
  }

}