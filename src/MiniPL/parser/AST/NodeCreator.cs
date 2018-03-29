using System;
using MiniPL.syntax;
using MiniPL.tokens;

namespace MiniPL.parser.AST {

  public class NodeCreator {

    public INode makeNode(MiniPLSymbol symbol) {
      switch(symbol) {
        case MiniPLSymbol.EXPRESSION:
          return new ExpressionNode();
        case MiniPLSymbol.VAR_DECLARATION:
          return new VarDeclarationNode();
        case MiniPLSymbol.VAR_ASSIGNMENT:
          return new VarAssignmentNode();
        case MiniPLSymbol.FOR_LOOP:
          return new ForLoopNode();
        case MiniPLSymbol.ASSERT_PROCEDURE:
          return new AssertNode();
        case MiniPLSymbol.PRINT_PROCEDURE:
          return new PrintNode();
        case MiniPLSymbol.READ_PROCEDURE:
          return new ReadNode();
        case MiniPLSymbol.PROGRAM:
          return new ProgramNode();
        case MiniPLSymbol.STATEMENT_LIST:
          return new StatementListNode();
        default:
          return new Node<MiniPLSymbol>(symbol);
      }
    }

    public INode makeNode(MiniPLTokenType type) {
      switch(type) {
        case MiniPLTokenType.PLUS:
          return new PlusOperationNode();
        case MiniPLTokenType.MINUS:
          return new MinusOperationNode();
        case MiniPLTokenType.ASTERISK:
          return new MultiplicationOperationNode();
        case MiniPLTokenType.SLASH:
          return new DivisionOperationNode();
        case MiniPLTokenType.LOGICAL_NOT:
          return new LogicalNotOperationNode();
        case MiniPLTokenType.LOGICAL_AND:
          return new LogicalAndOperationNode();
        case MiniPLTokenType.EQUALITY_COMPARISON:
          return new EqualityOperationNode();
        case MiniPLTokenType.LESS_THAN_COMPARISON:
          return new LessThanOperationNode();
        case MiniPLTokenType.TYPE_IDENTIFIER_INTEGER:
        case MiniPLTokenType.TYPE_IDENTIFIER_STRING:
        case MiniPLTokenType.TYPE_IDENTIFIER_BOOL:
          return new TypeNode(type);
        case MiniPLTokenType.RANGE_OPERATOR:
          return new RangeOperatorNode();
        default:
          return new Node<MiniPLTokenType>(type);
      }
    }
    
    public INode makeNode(Token<MiniPLTokenType> token) {
      switch(token.getType()) {
        case MiniPLTokenType.INTEGER_LITERAL:
          return new IntegerLiteralNode(token);
        case MiniPLTokenType.STRING_LITERAL:
          return new StringLiteralNode(token);
        case MiniPLTokenType.IDENTIFIER:
          return new IdentifierNode(token);
        default:
          return makeNode(token.getType());
      }
    }

  }

}