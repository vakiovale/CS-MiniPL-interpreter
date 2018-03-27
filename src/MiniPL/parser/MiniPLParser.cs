using System;
using MiniPL.scanner;
using MiniPL.tokens;
using System.Collections.Generic;
using MiniPL.exceptions;

namespace MiniPL.parser {

  public class MiniPLParser : IParser {

    private ITokenScanner<MiniPLTokenType> scanner;

    private Token<MiniPLTokenType> currentToken;

    private Token<MiniPLTokenType> nextToken;
    
    private TokenMatcher tm;

    private FirstAndFollow firstAndFollow;

    public MiniPLParser(ITokenScanner<MiniPLTokenType> scanner) {
      this.scanner = scanner;
      this.currentToken = null;
      this.nextToken = null;
      this.tm = new TokenMatcher();
      this.firstAndFollow = new FirstAndFollow();
    }
    
    private Token<MiniPLTokenType> readToken() {
      if(this.currentToken == null) {
        this.currentToken = this.scanner.readNextToken();
        this.nextToken = this.scanner.readNextToken();
        this.tm.setToken(this.currentToken);
      } else if(this.nextToken != null) {
        this.currentToken = this.nextToken;
        this.nextToken = this.scanner.readNextToken();
        this.tm.setToken(this.currentToken);
      } else {
        this.currentToken = null;
        this.tm.setToken(this.currentToken);
      }
      if(this.currentToken != null && this.currentToken.getType() == MiniPLTokenType.INVALID_TOKEN) {
        lexicalError("Invalid token '" + this.currentToken.getLexeme() + "'.");
      }
      return this.currentToken;
    }

    private void initializeFirstTokens() {
      this.currentToken = this.scanner.readNextToken();
      this.nextToken = this.scanner.readNextToken();
      this.tm.setToken(this.currentToken);
    }

    private bool hasMoreTokens() {
      return this.currentToken != null;
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
      tm.matchSemicolon();
    }

    private void matchStatement() {
      if(tm.isSymbol(first("var_declaration"))) {
        doVarDeclarationProcedure();
      } else if(tm.isSymbol(first("var_assignment"))) {
        doVarAssignmentProcedure();
      } else if(tm.isSymbol(first("for"))) {
        doForProcedure(); 
      } else if(tm.isSymbol(first("read"))) {
        doReadProcedure();
      } else if(tm.isSymbol(first("print"))) {
        doPrintProcedure();        
      } else if(tm.isSymbol(first("assert"))) {
        doAssertProcedure();
      } else {
        syntaxError("Illegal start of a statement. " + (this.currentToken != null ? "A statement can't begin with '" + this.currentToken.getLexeme() + "'." : ""));
      }
    }

    private void doReadProcedure() {
      tm.matchRead();
      readToken();
      tm.matchIdentifier();
    }

    private void doPrintProcedure() {
      tm.matchPrint();
      readToken();
      doExpressionProcedure();
    }

    private void doAssertProcedure() {
      tm.matchAssert();
      readToken();
      tm.matchLeftParenthesis();
      readToken();
      doExpressionProcedure();
      readToken();
      tm.matchRightParenthesis();
    }

    private void doForProcedure() {
      tm.matchFor();
      readToken();
      tm.matchIdentifier();
      readToken();
      tm.matchIn();
      readToken();
      doExpressionProcedure();
      readToken();
      tm.matchRange();
      readToken();
      doExpressionProcedure();
      readToken();
      tm.matchDo();
      readToken();
      doStatementListProcedure();
      readToken();
      tm.matchEnd();
      readToken();
      tm.matchFor();
    }

    private void doVarAssignmentProcedure() {
      tm.matchIdentifier();
      readToken();
      tm.matchAssignment();
      readToken();
      doExpressionProcedure();
    }

    private void doVarDeclarationProcedure() {
      tm.matchVar();
      readToken();
      tm.matchIdentifier();
      readToken();
      tm.matchColon();
      readToken();
      doTypeProcedure();
      if(peekType(MiniPLTokenType.ASSIGNMENT_OPERATOR)) {
        readToken();
        tm.matchAssignment();
        readToken();
        doExpressionProcedure();
      }
    }

    private void doTypeProcedure() {
      if(tm.isSymbol(MiniPLTokenType.TYPE_IDENTIFIER_INTEGER) || tm.isSymbol(MiniPLTokenType.TYPE_IDENTIFIER_STRING) || tm.isSymbol(MiniPLTokenType.TYPE_IDENTIFIER_BOOL))
        return;
      syntaxError("Expected a type (int, string, bool).");
    }

    private void doExpressionProcedure() {
      if(tm.isSymbol(MiniPLTokenType.LOGICAL_NOT)) {
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
      if(tm.isSymbol(MiniPLTokenType.PLUS) || 
         tm.isSymbol(MiniPLTokenType.MINUS) || 
         tm.isSymbol(MiniPLTokenType.ASTERISK) || 
         tm.isSymbol(MiniPLTokenType.SLASH) || 
         tm.isSymbol(MiniPLTokenType.LESS_THAN_COMPARISON) || 
         tm.isSymbol(MiniPLTokenType.EQUALITY_COMPARISON) || 
         tm.isSymbol(MiniPLTokenType.LOGICAL_AND) || 
         tm.isSymbol(MiniPLTokenType.LOGICAL_NOT))
        return;
      throw new SyntaxException("Expected an operation.", this.currentToken);
    }

    private bool peekType(ICollection<MiniPLTokenType> set) {
      if(this.nextToken != null) {
        return set.Contains(this.nextToken.getType());
      }
      return false;
    }

    private void doOperandProcedure() {
      if(tm.isSymbol(MiniPLTokenType.INTEGER_LITERAL) || tm.isSymbol(MiniPLTokenType.STRING_LITERAL) || tm.isSymbol(MiniPLTokenType.IDENTIFIER)) {
        return;
      } 
      tm.matchLeftParenthesis();
      readToken();
      doExpressionProcedure();
      readToken();
      tm.matchRightParenthesis();
    }

    private void syntaxError(string message) {
      throw new SyntaxException(message, this.currentToken);
    }

    private void lexicalError(string message) {
      throw new LexicalException(message, this.currentToken);
    }

    private bool peekType(MiniPLTokenType type) {
      return this.nextToken != null && this.nextToken.getType().Equals(type);
    }

    private ICollection<MiniPLTokenType> first(string rule) {
      return firstAndFollow.first(rule);
    }
  }
}