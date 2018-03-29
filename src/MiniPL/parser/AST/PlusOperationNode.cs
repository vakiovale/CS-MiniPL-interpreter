using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class PlusOperationNode : Node<MiniPLTokenType> {

    public PlusOperationNode() : base(MiniPLTokenType.PLUS) {}

    public override void accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }

    public int getSum() {
      INode leftHandSide = this.children[0];
      INode rightHandSide = this.children[1];

      int leftValue = 0;
      int rightValue = 0;

      if(leftHandSide.GetType() == typeof(IntegerLiteralNode)) {
        leftValue = ((IntegerLiteralNode)leftHandSide).getInt();
      } else if(leftHandSide.GetType() == typeof(ExpressionNode)) {
        leftValue = ((ExpressionNode)leftHandSide).getIntegerValue();
      }

      if(rightHandSide.GetType() == typeof(IntegerLiteralNode)) {
        rightValue = ((IntegerLiteralNode)rightHandSide).getInt();
      } else if(rightHandSide.GetType() == typeof(ExpressionNode)) {
        rightValue = ((ExpressionNode)rightHandSide).getIntegerValue();
      }

      return leftValue + rightValue;
    }

    public string getStringSum() {
      INode leftHandSide = this.children[0];
      INode rightHandSide = this.children[1];

      string leftValue = "";
      string rightValue = "";

      if(leftHandSide.GetType() == typeof(StringLiteralNode)) {
        leftValue = ((StringLiteralNode)leftHandSide).getString();
      } else if(leftHandSide.GetType() == typeof(ExpressionNode)) {
        leftValue = ((ExpressionNode)leftHandSide).getStringValue();
      }

      if(rightHandSide.GetType() == typeof(StringLiteralNode)) {
        rightValue = ((StringLiteralNode)rightHandSide).getString();
      } else if(rightHandSide.GetType() == typeof(ExpressionNode)) {
        rightValue = ((ExpressionNode)rightHandSide).getStringValue();
      }

      return leftValue + rightValue;
    }
  }

}