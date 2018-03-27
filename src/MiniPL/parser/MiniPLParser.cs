using System;
using MiniPL.scanner;
using MiniPL.tokens;
using System.Collections.Generic;
using MiniPL.exceptions;
using MiniPL.syntax;

namespace MiniPL.parser {

  public class MiniPLParser : IParser {

    private TokenReader tokenReader;

    private TokenMatcher tokenMatcher;

    private FirstAndFollow firstAndFollow;

    private bool syntaxOk;

    public MiniPLParser(TokenReader tokenReader) {
      initializeTokenMatcher();
      initializeTokenReader(tokenReader);
      intializeFirstAndFollow();
      initializeSyntaxFlag();
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

    private void doProgramProcedure() {
      try {
        doStatementListProcedure();
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbols.PROGRAM, doProgramProcedure);
      }  
    }

    private void exceptionRecovery(MiniPLException exception, MiniPLSymbols symbol, Action procedureMethod) {
      Console.WriteLine(exception.getMessage());
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
      if(tokenMatcher.isSymbol(first(MiniPLSymbols.VAR_DECLARATION))) {
        doVarDeclarationProcedure();
      } else if(tokenMatcher.isSymbol(first(MiniPLSymbols.VAR_ASSIGNMENT))) {
        doVarAssignmentProcedure();
      } else if(tokenMatcher.isSymbol(first(MiniPLSymbols.FOR_LOOP))) {
        doForProcedure(); 
      } else if(tokenMatcher.isSymbol(first(MiniPLSymbols.READ_PROCEDURE))) {
        doReadProcedure();
      } else if(tokenMatcher.isSymbol(first(MiniPLSymbols.PRINT_PROCEDURE))) {
        doPrintProcedure();        
      } else if(tokenMatcher.isSymbol(first(MiniPLSymbols.ASSERT_PROCEDURE))) {
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
      if(tokenMatcher.isSymbol(MiniPLTokenType.TYPE_IDENTIFIER_INTEGER) || tokenMatcher.isSymbol(MiniPLTokenType.TYPE_IDENTIFIER_STRING) || tokenMatcher.isSymbol(MiniPLTokenType.TYPE_IDENTIFIER_BOOL))
        return;
      syntaxError("Expected a type (int, string, bool).");
    }

    private void doExpressionProcedure() {
      if(tokenMatcher.isSymbol(MiniPLTokenType.LOGICAL_NOT)) {
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
      if(tokenMatcher.isSymbol(MiniPLTokenType.PLUS) || 
         tokenMatcher.isSymbol(MiniPLTokenType.MINUS) || 
         tokenMatcher.isSymbol(MiniPLTokenType.ASTERISK) || 
         tokenMatcher.isSymbol(MiniPLTokenType.SLASH) || 
         tokenMatcher.isSymbol(MiniPLTokenType.LESS_THAN_COMPARISON) || 
         tokenMatcher.isSymbol(MiniPLTokenType.EQUALITY_COMPARISON) || 
         tokenMatcher.isSymbol(MiniPLTokenType.LOGICAL_AND) || 
         tokenMatcher.isSymbol(MiniPLTokenType.LOGICAL_NOT))
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
      if(tokenMatcher.isSymbol(MiniPLTokenType.INTEGER_LITERAL) || tokenMatcher.isSymbol(MiniPLTokenType.STRING_LITERAL) || tokenMatcher.isSymbol(MiniPLTokenType.IDENTIFIER)) {
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