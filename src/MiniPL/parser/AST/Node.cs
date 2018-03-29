using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;

namespace MiniPL.parser.AST {

  public class Node<T> : INode {

    private IList<INode> children;

    private T value;

    public Node(T value) {
      this.children = new List<INode>();
      this.value = value;
    }

    public bool accept(INodeVisitor visitor) {
      throw new NotImplementedException();
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