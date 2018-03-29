using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;

namespace MiniPL.parser.AST {

  public interface INode {
         
    IList<INode> getChildren();

    void addNode(INode node);

    bool accept(INodeVisitor visitor); 

    object getValue();

  }

}