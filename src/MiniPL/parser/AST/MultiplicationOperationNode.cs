using System;
using System.Collections.Generic;
using MiniPL.semantics.visitor;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class MultiplicationOperationNode : Node<MiniPLTokenType> {

    public MultiplicationOperationNode() : base(MiniPLTokenType.ASTERISK) {}

    public override void accept(INodeVisitor visitor) {
      throw new NotImplementedException();
    }

    public int getMultiplication() {
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

      return leftValue * rightValue;
    }
  }

}