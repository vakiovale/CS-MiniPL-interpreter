using System;
using MiniPL.scanner;
using MiniPL.tokens;
using System.Collections.Generic;
using MiniPL.exceptions;
using MiniPL.syntax;
using MiniPL.logger;

namespace MiniPL.parser {

  public class MiniPLParser : IParser {

    private Logger logger;

    private TokenReader tokenReader;

    private TokenMatcher tokenMatcher;

    private FirstAndFollow firstAndFollow;

    private bool syntaxOk;

    public MiniPLParser(TokenReader tokenReader, Logger logger) {
      initializeLogger(logger);
      initializeTokenMatcher();
      initializeTokenReader(tokenReader);
      intializeFirstAndFollow();
      initializeSyntaxFlag();
    }

    private void initializeLogger(Logger logger) {
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

    private void exceptionRecovery(MiniPLException exception, MiniPLSymbols symbol, Action procedureMethod) {
      logger.log(exception.getMessage());
      syntaxOk = false;
      do {
        readToken();
        Token<MiniPLTokenType> goodToken = this.tokenReader.token();
        if(goodToken != null) {
          if(this.firstAndFollow.firstContains(symbol, goodToken.getType())) {
            procedureMethod(); 
            return;
          }
          if(this.firstAndFollow.followContains(symbol, goodToken.getType())) {
            return;
          }
        }
      } while(this.tokenReader.hasNextToken());
    }

    private void doProgramProcedure() {
      try {
        doStatementListProcedure();
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbols.PROGRAM, doProgramProcedure);
      }  
    }

    private void doStatementListProcedure() {
      try {
        doStatemenProcedure();
        while(peekType(firstAndFollow.first(MiniPLSymbols.PROGRAM))) {
          readToken();
          doStatemenProcedure();
        }
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbols.STATEMENT_LIST, doStatementListProcedure); 
      }
    }

    private void doStatemenProcedure() {
      if(tokenMatcher.isTokenTypeInCollection(first(MiniPLSymbols.VAR_DECLARATION))) {
        doVarDeclarationProcedure();
      } else if(tokenMatcher.isTokenTypeInCollection(first(MiniPLSymbols.VAR_ASSIGNMENT))) {
        doVarAssignmentProcedure();
      } else if(tokenMatcher.isTokenTypeInCollection(first(MiniPLSymbols.FOR_LOOP))) {
        doForProcedure(); 
      } else if(tokenMatcher.isTokenTypeInCollection(first(MiniPLSymbols.READ_PROCEDURE))) {
        doReadProcedure();
      } else if(tokenMatcher.isTokenTypeInCollection(first(MiniPLSymbols.PRINT_PROCEDURE))) {
        doPrintProcedure();        
      } else if(tokenMatcher.isTokenTypeInCollection(first(MiniPLSymbols.ASSERT_PROCEDURE))) {
        doAssertProcedure();
      } else {
        syntaxError("Illegal start of a statement. " + (tokenReader.token() != null ? "A statement can't begin with '" + tokenReader.token().getLexeme() + "'." : ""));
      }
      readToken();
      tokenMatcher.matchSemicolon();
    }

    private void doReadProcedure() {
      tokenMatcher.matchRead();
      readToken();
      tokenMatcher.matchIdentifier();
    }

    private void doPrintProcedure() {
      tokenMatcher.matchPrint();
      readToken();
      doExpressionProcedure();
    }

    private void doAssertProcedure() {
      tokenMatcher.matchAssert();
      readToken();
      tokenMatcher.matchLeftParenthesis();
      readToken();
      doExpressionProcedure();
      readToken();
      tokenMatcher.matchRightParenthesis();
    }

    private void doForProcedure() {
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
    }

    private void doVarAssignmentProcedure() {
      tokenMatcher.matchIdentifier();
      readToken();
      tokenMatcher.matchAssignment();
      readToken();
      doExpressionProcedure();
    }

    private void doVarDeclarationProcedure() {
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
    }

    private void doTypeProcedure() {
      if(tokenMatcher.isTokenType(MiniPLTokenType.TYPE_IDENTIFIER_INTEGER) || tokenMatcher.isTokenType(MiniPLTokenType.TYPE_IDENTIFIER_STRING) || tokenMatcher.isTokenType(MiniPLTokenType.TYPE_IDENTIFIER_BOOL))
        return;
      syntaxError("Expected a type (int, string, bool).");
    }

    private void doExpressionProcedure() {
      if(tokenMatcher.isTokenType(MiniPLTokenType.LOGICAL_NOT)) {
        readToken();
        doOperandProcedure();
        return;
      } 
      doOperandProcedure();
      if(peekType(first(MiniPLSymbols.OPERATION))) {
        readToken();
        doOperationProcedure();
        readToken();
        doOperandProcedure();
        return;
      }
      return;
    }

    private void doOperationProcedure() {
      if(tokenMatcher.isTokenType(MiniPLTokenType.PLUS) || 
         tokenMatcher.isTokenType(MiniPLTokenType.MINUS) || 
         tokenMatcher.isTokenType(MiniPLTokenType.ASTERISK) || 
         tokenMatcher.isTokenType(MiniPLTokenType.SLASH) || 
         tokenMatcher.isTokenType(MiniPLTokenType.LESS_THAN_COMPARISON) || 
         tokenMatcher.isTokenType(MiniPLTokenType.EQUALITY_COMPARISON) || 
         tokenMatcher.isTokenType(MiniPLTokenType.LOGICAL_AND) || 
         tokenMatcher.isTokenType(MiniPLTokenType.LOGICAL_NOT))
        return;
      throw new SyntaxException("Expected an operation.", tokenReader.token());
    }

    private bool peekType(ICollection<MiniPLTokenType> set) {
      if(tokenReader.hasNextToken()) {
        return set.Contains(tokenReader.getNextTokensType());
      }
      return false;
    }

    private void doOperandProcedure() {
      if(tokenMatcher.isTokenType(MiniPLTokenType.INTEGER_LITERAL) || tokenMatcher.isTokenType(MiniPLTokenType.STRING_LITERAL) || tokenMatcher.isTokenType(MiniPLTokenType.IDENTIFIER)) {
        return;
      } 
      tokenMatcher.matchLeftParenthesis();
      readToken();
      doExpressionProcedure();
      readToken();
      tokenMatcher.matchRightParenthesis();
    }

    private void syntaxError(string message) {
      throw new SyntaxException(message, tokenReader.token());
    }

    private void lexicalError(string message) {
      throw new LexicalException(message, tokenReader.token());
    }

    private ICollection<MiniPLTokenType> first(MiniPLSymbols symbol) {
      return firstAndFollow.first(symbol);
    }
  }
}