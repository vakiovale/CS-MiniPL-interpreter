using System;
using MiniPL.parser.AST;

namespace MiniPL.semantics.visitor {

  public class TypeCheckingVisitor : INodeVisitor
  {
    public void visitVarDeclaration(VarDeclarationNode node) {
      throw new NotImplementedException();
    }
  }

}