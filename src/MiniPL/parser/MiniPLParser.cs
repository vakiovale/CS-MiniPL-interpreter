using System;
using MiniPL.scanner;
using MiniPL.tokens;
using System.Collections.Generic;
using MiniPL.exceptions;
using MiniPL.syntax;
using MiniPL.logger;
using MiniPL.parser.AST;

namespace MiniPL.parser {

  public class MiniPLParser : IParser {

    private ILogger logger;

    private TokenReader tokenReader;

    private TokenMatcher tokenMatcher;

    private FirstAndFollow firstAndFollow;

    private RecoveryHandler recoveryHandler;

    private bool syntaxOk;

    private IAST ast;

    public MiniPLParser(TokenReader tokenReader, ILogger logger) {
      initializeLogger(logger);
      initializeTokenMatcher();
      initializeTokenReader(tokenReader);
      intializeFirstAndFollow();
      initializeRecoveryHandler();
      initializeSyntaxFlag();
      initializeAST();
    }

    private void initializeLogger(ILogger logger) {
      this.logger = logger;
    }

    private void initializeSyntaxFlag() {
      this.syntaxOk = true;
    }

    private void initializeTokenMatcher() {
      this.tokenMatcher = new TokenMatcher();
    }
    
    private void initializeTokenReader(TokenReader reader) {
      this.tokenReader = reader;
    }

    private void intializeFirstAndFollow() {
      this.firstAndFollow = new FirstAndFollow();
    }

    private void initializeRecoveryHandler() {
      this.recoveryHandler = new RecoveryHandler(this.tokenReader, readToken, this.firstAndFollow);
    }

    private void initializeAST() {
      this.ast = new MiniPLAST();
    }

    private void readToken() {
      Token<MiniPLTokenType> token = tokenReader.readToken();
      if(token != null && token.getType() == MiniPLTokenType.INVALID_TOKEN) {
        lexicalError("Invalid token '" + token.getLexeme() + "'.");
      }
      this.tokenMatcher.setToken(token);
    }

    public bool checkSyntax() {
      readToken();
      ast.addProgramNode(doProgramProcedure());
      return syntaxOk;
    }

    public IAST getAST() {
      if(this.syntaxOk) {
        return this.ast;
      } else {
        return null;
      }
    }

    private void exceptionRecovery(MiniPLException exception, MiniPLSymbol symbol, Func<INode> procedureMethod) {
      logError(exception);
      failSyntax();
      recover(symbol, procedureMethod);
    }

    private void logError(MiniPLException exception) {
      logger.log(exception.getMessage());
    }

    private void failSyntax() {
      syntaxOk = false;
    }

    private void recover(MiniPLSymbol symbol, Func<INode> procedureMethod) {
      recoveryHandler.tryToRecoverFromException(symbol, procedureMethod);
    }

    private INode doProgramProcedure() {
      INode programNode = new BasicNode<MiniPLSymbol>(MiniPLSymbol.PROGRAM);
      try {
        programNode.addNode(doStatementListProcedure());
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.PROGRAM, doProgramProcedure);
      }  
      return programNode;
    }

    private INode doStatementListProcedure() {
      INode statementList = new BasicNode<MiniPLSymbol>(MiniPLSymbol.STATEMENT_LIST); 
      try {
        statementList.addNode(doStatemenProcedure());
        while(peekType(firstAndFollow.first(MiniPLSymbol.STATEMENT))) {
          readToken();
          statementList.addNode(doStatemenProcedure());
        }
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.STATEMENT_LIST, doStatementListProcedure); 
      }
      return statementList;
    }

    private INode doStatemenProcedure() {
      INode statement = new BasicNode<MiniPLSymbol>(MiniPLSymbol.STATEMENT);
      try {
        if(isTokenInFirst(MiniPLSymbol.VAR_DECLARATION)) {
          statement.addNode(doVarDeclarationProcedure());
        } else if(isTokenInFirst(MiniPLSymbol.VAR_ASSIGNMENT)) {
          statement.addNode(doVarAssignmentProcedure());
        } else if(isTokenInFirst(MiniPLSymbol.FOR_LOOP)) {
          statement.addNode(doForProcedure()); 
        } else if(isTokenInFirst(MiniPLSymbol.READ_PROCEDURE)) {
          statement.addNode(doReadProcedure());
        } else if(isTokenInFirst(MiniPLSymbol.PRINT_PROCEDURE)) {
          statement.addNode(doPrintProcedure());        
        } else if(isTokenInFirst(MiniPLSymbol.ASSERT_PROCEDURE)) {
          statement.addNode(doAssertProcedure());
        } else {
          syntaxError("Illegal start of a statement. " + (tokenReader.token() != null ? "A statement can't begin with '" + tokenReader.token().getLexeme() + "'." : ""));
        }
        readToken();
        tokenMatcher.matchSemicolon();
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.STATEMENT, doStatemenProcedure);
      }
      return statement;
    }

    private INode doReadProcedure() {
      try {
        tokenMatcher.matchRead();
        readToken();
        tokenMatcher.matchIdentifier();
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.READ_PROCEDURE, doReadProcedure);
      }
      return null;
    }

    private INode doPrintProcedure() {
      INode print = new BasicNode<MiniPLSymbol>(MiniPLSymbol.PRINT_PROCEDURE);
      try {
        tokenMatcher.matchPrint();
        readToken();
        print.addNode(doExpressionProcedure());
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.PRINT_PROCEDURE, doPrintProcedure);
      }
      return print;
    }

    private INode doAssertProcedure() {
      try {
        tokenMatcher.matchAssert();
        readToken();
        tokenMatcher.matchLeftParenthesis();
        readToken();
        doExpressionProcedure();
        readToken();
        tokenMatcher.matchRightParenthesis();
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.ASSERT_PROCEDURE, doAssertProcedure);
      }
      return null;
    }

    private INode doForProcedure() {
      try {
        tokenMatcher.matchFor();
        readToken();
        tokenMatcher.matchIdentifier();
        readToken();
        tokenMatcher.matchIn();
        readToken();
        doExpressionProcedure();
        readToken();
        tokenMatcher.matchRange();
        readToken();
        doExpressionProcedure();
        readToken();
        tokenMatcher.matchDo();
        readToken();
        doStatementListProcedure();
        readToken();
        tokenMatcher.matchEnd();
        readToken();
        tokenMatcher.matchFor();
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.FOR_LOOP, doForProcedure);
      }
      return null;
    }

    private INode doVarAssignmentProcedure() {
      try {
        tokenMatcher.matchIdentifier();
        readToken();
        tokenMatcher.matchAssignment();
        readToken();
        doExpressionProcedure();
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.VAR_ASSIGNMENT, doVarAssignmentProcedure);
      }
      return null;
    }

    private INode doVarDeclarationProcedure() {
      try {
        tokenMatcher.matchVar();
        readToken();
        tokenMatcher.matchIdentifier();
        readToken();
        tokenMatcher.matchColon();
        readToken();
        doTypeProcedure();
        if(tokenReader.isNextTokensType(MiniPLTokenType.ASSIGNMENT_OPERATOR)) {
          readToken();
          tokenMatcher.matchAssignment();
          readToken();
          doExpressionProcedure();
        }
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.VAR_DECLARATION, doVarDeclarationProcedure);
      }
      return null;
    }

    private INode doTypeProcedure() {
      try {
        if(isTokenInFirst(MiniPLSymbol.TYPE)) {
          return null;
        }
        syntaxError("Expected a type (int, string, bool).");
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.TYPE, doTypeProcedure);
      }
      return null;
    }

    private INode doExpressionProcedure() {
      INode expression = new BasicNode<MiniPLSymbol>(MiniPLSymbol.EXPRESSION);
      try {
        if(tokenMatcher.isTokenType(MiniPLTokenType.LOGICAL_NOT)) {
          readToken();
          INode notNode = new BasicNode<MiniPLTokenType>(MiniPLTokenType.LOGICAL_NOT);
          notNode.addNode(doOperandProcedure());
          expression.addNode(notNode);
          return expression;
        } 
        INode leftHandSide = doOperandProcedure();
        if(peekType(first(MiniPLSymbol.OPERATION))) {
          readToken();
          INode operation = doOperationProcedure();
          readToken();
          INode rightHandSide = doOperandProcedure();
          operation.addNode(leftHandSide);
          operation.addNode(rightHandSide);
          expression.addNode(operation);
          return expression;
        } else {
          expression.addNode(leftHandSide);
        }
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.EXPRESSION, doExpressionProcedure);
      }
      return expression;
    }

    private INode doOperandProcedure() {
      try {
        if(isTokenInFirst(MiniPLSymbol.OPERAND)) {
          if(tokenMatcher.isTokenType(MiniPLTokenType.INTEGER_LITERAL)) {
            return new BasicNode<int>(Int32.Parse(tokenReader.token().getLexeme()));
          } else if(tokenMatcher.isTokenType(MiniPLTokenType.STRING_LITERAL)) {
            return new BasicNode<string>(tokenReader.token().getLexeme());
          } else if(tokenMatcher.isTokenType(MiniPLTokenType.IDENTIFIER)) {
            return new BasicNode<string>(tokenReader.token().getLexeme());
          } 
          tokenMatcher.matchLeftParenthesis();
          readToken();
          INode expression = doExpressionProcedure();
          readToken();
          tokenMatcher.matchRightParenthesis();
          return expression;
        }
        syntaxError("Illegal start of an expression. Expected a valid operand.");
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.OPERAND, doOperandProcedure);
      }
      return null;
    }

    private INode doOperationProcedure() {
      try {
        if(isTokenInFirst(MiniPLSymbol.OPERATION)) {
          return new BasicNode<string>(tokenReader.token().getLexeme());
        }
        throw new SyntaxException("Expected an operation.", tokenReader.token());
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.OPERATION, doOperationProcedure);
      }
      return null;
    }

    private bool isTokenInFirst(MiniPLSymbol symbol) {
      if(tokenReader.token() != null) {
        return firstAndFollow.firstContains(symbol, tokenReader.token().getType()); 
      } else {
        return false;
      }
    }

    private bool peekType(ICollection<MiniPLTokenType> set) {
      if(tokenReader.hasNextToken()) {
        if(tokenReader.getNextTokensType().Equals(MiniPLTokenType.INVALID_TOKEN)) {
          readToken();
          lexicalError("Invalid token '" + this.tokenReader.token().getLexeme() + "'.");
        } else {
          return set.Contains(tokenReader.getNextTokensType());
        }
      }
      return false;
    }

    private void syntaxError(string message) {
      throw new SyntaxException(message, tokenReader.token());
    }

    private void lexicalError(string message) {
      throw new LexicalException(message, tokenReader.token());
    }

    private ICollection<MiniPLTokenType> first(MiniPLSymbol symbol) {
      return firstAndFollow.first(symbol);
    }
  }
}