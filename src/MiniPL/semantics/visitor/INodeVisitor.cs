using System;
using MiniPL.parser.AST;

namespace MiniPL.semantics.visitor {

  public interface INodeVisitor {

    void visitVarDeclaration(VarDeclarationNode node);
        
  }

}