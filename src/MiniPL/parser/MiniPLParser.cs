using System;
using MiniPL.scanner;
using MiniPL.tokens;
using System.Collections.Generic;
using MiniPL.exceptions;
using MiniPL.syntax;
using MiniPL.logger;

namespace MiniPL.parser {

  public class MiniPLParser : IParser {

    private ILogger logger;

    private TokenReader tokenReader;

    private TokenMatcher tokenMatcher;

    private FirstAndFollow firstAndFollow;

    private RecoveryHandler recoveryHandler;

    private bool syntaxOk;

    public MiniPLParser(TokenReader tokenReader, ILogger logger) {
      initializeLogger(logger);
      initializeTokenMatcher();
      initializeTokenReader(tokenReader);
      intializeFirstAndFollow();
      initializeRecoveryHandler();
      initializeSyntaxFlag();
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

    private void readToken() {
      Token<MiniPLTokenType> token = tokenReader.readToken();
      if(token != null && token.getType() == MiniPLTokenType.INVALID_TOKEN) {
        lexicalError("Invalid token '" + token.getLexeme() + "'.");
      }
      this.tokenMatcher.setToken(token);
    }

    public bool checkSyntax() {
      readToken();
      doProgramProcedure();
      return syntaxOk;
    }

    private void exceptionRecovery(MiniPLException exception, MiniPLSymbol symbol, Action procedureMethod) {
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

    private void recover(MiniPLSymbol symbol, Action procedureMethod) {
      recoveryHandler.tryToRecoverFromException(symbol, procedureMethod);
    }

    private void doProgramProcedure() {
      try {
        doStatementListProcedure();
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.PROGRAM, doProgramProcedure);
      }  
    }

    private void doStatementListProcedure() {
      try {
        doStatemenProcedure();
        while(peekType(firstAndFollow.first(MiniPLSymbol.PROGRAM))) {
          readToken();
          doStatemenProcedure();
        }
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.STATEMENT_LIST, doStatementListProcedure); 
      }
    }

    private void doStatemenProcedure() {
      try {
        if(isTokenInFirst(MiniPLSymbol.VAR_DECLARATION)) {
          doVarDeclarationProcedure();
        } else if(isTokenInFirst(MiniPLSymbol.VAR_ASSIGNMENT)) {
          doVarAssignmentProcedure();
        } else if(isTokenInFirst(MiniPLSymbol.FOR_LOOP)) {
          doForProcedure(); 
        } else if(isTokenInFirst(MiniPLSymbol.READ_PROCEDURE)) {
          doReadProcedure();
        } else if(isTokenInFirst(MiniPLSymbol.PRINT_PROCEDURE)) {
          doPrintProcedure();        
        } else if(isTokenInFirst(MiniPLSymbol.ASSERT_PROCEDURE)) {
          doAssertProcedure();
        } else {
          syntaxError("Illegal start of a statement. " + (tokenReader.token() != null ? "A statement can't begin with '" + tokenReader.token().getLexeme() + "'." : ""));
        }
        readToken();
        tokenMatcher.matchSemicolon();
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.STATEMENT, doStatemenProcedure);
      }
    }

    private void doReadProcedure() {
      try {
        tokenMatcher.matchRead();
        readToken();
        tokenMatcher.matchIdentifier();
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.READ_PROCEDURE, doReadProcedure);
      }
    }

    private void doPrintProcedure() {
      try {
        tokenMatcher.matchPrint();
        readToken();
        doExpressionProcedure();
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.PRINT_PROCEDURE, doPrintProcedure);
      }
    }

    private void doAssertProcedure() {
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
    }

    private void doForProcedure() {
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
    }

    private void doVarAssignmentProcedure() {
      try {
        tokenMatcher.matchIdentifier();
        readToken();
        tokenMatcher.matchAssignment();
        readToken();
        doExpressionProcedure();
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.VAR_ASSIGNMENT, doVarAssignmentProcedure);
      }
    }

    private void doVarDeclarationProcedure() {
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
    }

    private void doTypeProcedure() {
      try {
        if(isTokenInFirst(MiniPLSymbol.TYPE)) {
          return;
        }
        syntaxError("Expected a type (int, string, bool).");
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.TYPE, doTypeProcedure);
      }
    }

    private void doExpressionProcedure() {
      if(tokenMatcher.isTokenType(MiniPLTokenType.LOGICAL_NOT)) {
        readToken();
        doOperandProcedure();
        return;
      } 
      doOperandProcedure();
      if(peekType(first(MiniPLSymbol.OPERATION))) {
        readToken();
        doOperationProcedure();
        readToken();
        doOperandProcedure();
        return;
      }
      return;
    }

    private void doOperandProcedure() {
      try {
        if(tokenMatcher.isTokenType(MiniPLTokenType.INTEGER_LITERAL) || 
          tokenMatcher.isTokenType(MiniPLTokenType.STRING_LITERAL) || 
          tokenMatcher.isTokenType(MiniPLTokenType.IDENTIFIER)) {
          return;
        } 
        tokenMatcher.matchLeftParenthesis();
        readToken();
        doExpressionProcedure();
        readToken();
        tokenMatcher.matchRightParenthesis();
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.OPERAND, doOperandProcedure);
      }
    }

    private void doOperationProcedure() {
      try {
        if(isTokenInFirst(MiniPLSymbol.OPERATION)) {
          return;
        }
        throw new SyntaxException("Expected an operation.", tokenReader.token());
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.OPERATION, doOperationProcedure);
      }
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
        return set.Contains(tokenReader.getNextTokensType());
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