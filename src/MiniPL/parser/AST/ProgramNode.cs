using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.syntax;

namespace MiniPL.parser.AST {

  public class ProgramNode : Node<MiniPLSymbol> {

    public ProgramNode() : base(MiniPLSymbol.PROGRAM) {}

    public override void accept(INodeVisitor visitor) {
      foreach(INode node in this.children) {
        node.accept(visitor);
      }
    }

    public StatementListNode getStatements() {
      return (StatementListNode)this.children[0];
    }
  }

}