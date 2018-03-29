using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;

namespace MiniPL.parser.AST {

  public abstract class Node<T> : INode {

    protected IList<INode> children;

    protected T value;

    public Node(T value) {
      this.children = new List<INode>();
      this.value = value;
    }

    public abstract void accept(INodeVisitor visitor);

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