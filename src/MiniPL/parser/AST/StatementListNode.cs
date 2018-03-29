using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.syntax;

namespace MiniPL.parser.AST {

  public class StatementListNode : Node<MiniPLSymbol> {

    public StatementListNode() : base(MiniPLSymbol.STATEMENT_LIST) {}

    public bool accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }

  }

}