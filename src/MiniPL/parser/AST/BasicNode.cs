using System;
using System.Collections.Generic;

namespace MiniPL.parser.AST {

  public class BasicNode<T> : INode {

    private IList<INode> children;

    private T value;

    public BasicNode(T value) {
      this.children = new List<INode>();
      this.value = value;
    }

    public void addNode(INode node) {
      children.Add(node);
    }

    public IList<INode> getChildren() {
      return this.children;
    }

    public object getValue() {
      return this.value;
    }
  }
}