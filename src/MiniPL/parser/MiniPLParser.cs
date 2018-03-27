using System;
using MiniPL.scanner;
using MiniPL.tokens;
using System.Collections.Generic;
using MiniPL.exceptions;

namespace MiniPL.parser {

  public class MiniPLParser : IParser {

    private TokenReader tokenReader;

    private TokenMatcher tokenMatcher;

    private FirstAndFollow firstAndFollow;

    public MiniPLParser(TokenReader tokenReader) {
      initializeTokenMatcher();
      initializeTokenReader(tokenReader);
      intializeFirstAndFollow();
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
      bool syntaxOk = false;
      try {
        syntaxOk = matchProgram();
      } catch(MiniPLException exception) {
        Console.WriteLine(exception.getMessage());
        syntaxOk = false;
      }      
      return syntaxOk;
    }

    private bool matchProgram() {
      readToken();
      doStatementListProcedure();
      return true;
    }

    private void doStatementListProcedure() {
      doStatemenProcedure();
      while(peekType(firstAndFollow.first("statement"))) {
        readToken();
        doStatemenProcedure();
      }
    }

    private void doStatemenProcedure() {
      matchStatement();
      readToken();
      tokenMatcher.matchSemicolon();
    }

    private void matchStatement() {
      if(tokenMatcher.isSymbol(first("var_declaration"))) {
        doVarDeclarationProcedure();
      } else if(tokenMatcher.isSymbol(first("var_assignment"))) {
        doVarAssignmentProcedure();
      } else if(tokenMatcher.isSymbol(first("for"))) {
        doForProcedure(); 
      } else if(tokenMatcher.isSymbol(first("read"))) {
        doReadProcedure();
      } else if(tokenMatcher.isSymbol(first("print"))) {
        doPrintProcedure();        
      } else if(tokenMatcher.isSymbol(first("assert"))) {
        doAssertProcedure();
      } else {
        syntaxError("Illegal start of a statement. " + (tokenReader.token() != null ? "A statement can't begin with '" + tokenReader.token().getLexeme() + "'." : ""));
      }
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
      if(peekType(first("operation"))) {
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

    private ICollection<MiniPLTokenType> first(string rule) {
      return firstAndFollow.first(rule);
    }
  }
}