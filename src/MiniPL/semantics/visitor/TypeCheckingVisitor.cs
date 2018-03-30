using System;
using MiniPL.parser.AST;

namespace MiniPL.semantics.visitor {

  public class TypeCheckingVisitor : INodeVisitor {
    public void visitDivision(DivisionOperationNode node)
    {
      throw new NotImplementedException();
    }

    public void visitExpression(ExpressionNode node) {
      throw new NotImplementedException();
    }

    public void visitIdentifier(IdentifierNode node) {
      throw new NotImplementedException();
    }

    public void visitIntegerLiteral(IntegerLiteralNode node) {
      throw new NotImplementedException();
    }

    public void visitMinus(MinusOperationNode node)
    {
      throw new NotImplementedException();
    }

    public void visitMultiplication(MultiplicationOperationNode node)
    {
      throw new NotImplementedException();
    }

    public void visitPlus(PlusOperationNode node) {
      throw new NotImplementedException();
    }

    public void visitStringLiteral(StringLiteralNode node)
    {
      throw new NotImplementedException();
    }

    public void visitVarDeclaration(VarDeclarationNode node) {
      throw new NotImplementedException();
    }
  }

}