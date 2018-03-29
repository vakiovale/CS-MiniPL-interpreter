using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class IntegerLiteralNode : Node<int> {

    private int intValue;

    public IntegerLiteralNode(Token<MiniPLTokenType> token) : base(getLexemeAsInteger(token)) {
      this.intValue = getLexemeAsInteger(token);
    }

    public override void accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }

    public static int getLexemeAsInteger(Token<MiniPLTokenType> token) {
      return Int32.Parse(token.getLexeme());
    }

    public int getInt() {
      return intValue;
    }
  }

}