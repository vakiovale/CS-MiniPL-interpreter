using System;
using System.Collections.Generic;

namespace MiniPL.parser.AST {

  public interface INode {
         
    IList<INode> getChildren();

    void addNode(INode node);

    object getValue();

  }

}