using System;
using MiniPL.parser.AST;

namespace MiniPL.semantics.visitor {

  public interface INodeVisitor {

    void visitVarDeclaration(VarDeclarationNode node);

    void visitVarAssignment(VarAssignmentNode node);

    void visitExpression(ExpressionNode node);

    void visitIdentifier(IdentifierNode node);

    void visitIntegerLiteral(IntegerLiteralNode node);

    void visitStringLiteral(StringLiteralNode node);

    void visitLessThanOperator(LessThanOperationNode node);

    void visitEqualityOperator(EqualityOperationNode node);

    void visitPlus(PlusOperationNode node);

    void visitMinus(MinusOperationNode node);

    void visitDivision(DivisionOperationNode node);

    void visitMultiplication(MultiplicationOperationNode node);
        
  }

}