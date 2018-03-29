using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class IntegerLiteralNode : Node<int> {

    public IntegerLiteralNode(Token<MiniPLTokenType> token) : base(getLexemeAsInteger(token)) {}

    public static int getLexemeAsInteger(Token<MiniPLTokenType> token) {
      return Int32.Parse(token.getLexeme());
    }

    public override void accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }
  }

}