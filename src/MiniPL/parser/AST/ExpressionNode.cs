using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.syntax;

namespace MiniPL.parser.AST {

  public class ExpressionNode : Node<MiniPLSymbol> {

    public ExpressionNode() : base(MiniPLSymbol.EXPRESSION) {}

    public override void accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }

    public int getIntegerValue() {
      INode node = this.children[0];
      if(node.GetType() == typeof(IntegerLiteralNode)) {
        return ((IntegerLiteralNode)node).getInt();
      } else if(node.GetType() == typeof(PlusOperationNode)) {
        return ((PlusOperationNode)node).getSum();
      } else if(node.GetType() == typeof(MinusOperationNode)) {
        return ((MinusOperationNode)node).getSubtraction();
      } else if(node.GetType() == typeof(DivisionOperationNode)) {
        return ((DivisionOperationNode)node).getDivision();
      } else if(node.GetType() == typeof(MultiplicationOperationNode)) {
        return ((MultiplicationOperationNode)node).getMultiplication();
      }
      throw new NotImplementedException();
    }

    public string getStringValue() {
      INode node = this.children[0];
      if(node.GetType() == typeof(StringLiteralNode)) {
        return ((StringLiteralNode)node).getString();
      } else if(node.GetType() == typeof(PlusOperationNode)) {
        return ((PlusOperationNode)node).getStringSum();
      }
      throw new NotImplementedException();
    }

    public bool getBoolValue() {
      throw new NotImplementedException();
    }
  }

}