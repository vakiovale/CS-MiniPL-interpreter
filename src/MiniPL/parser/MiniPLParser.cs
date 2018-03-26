using System;
using MiniPL.scanner;
using MiniPL.tokens;
using System.Collections.Generic;

namespace MiniPL.parser {

  public class MiniPLParser : IParser {

    private ITokenScanner<MiniPLTokenType> scanner;

    private Token<MiniPLTokenType> currentToken;

    private Token<MiniPLTokenType> nextToken;

    private IList<string> errorMessages;

    public MiniPLParser(ITokenScanner<MiniPLTokenType> scanner) {
      this.scanner = scanner;
      this.currentToken = null;
      this.nextToken = null;
      this.errorMessages = new List<string>();
    }
    
    private Token<MiniPLTokenType> readToken() {
      if(this.currentToken == null) {
        this.currentToken = this.scanner.readNextToken();
        this.nextToken = this.scanner.readNextToken();
      } else if(this.nextToken != null) {
        this.currentToken = this.nextToken;
        this.nextToken = this.scanner.readNextToken();
      } else {
        this.currentToken = null;
      }
      if(this.currentToken != null && this.currentToken.getType() == MiniPLTokenType.INVALID_TOKEN) {
        addError("Invalid token found: " + this.currentToken.getLexeme());
      }
      return this.currentToken;
    }

    private void initializeFirstTokens() {
      this.currentToken = this.scanner.readNextToken();
      this.nextToken = this.scanner.readNextToken();
    }

    private Token<MiniPLTokenType> peek() {
      return this.nextToken;
    }

    private bool hasMoreTokens() {
      return this.currentToken != null;
    }

    public bool checkSyntax() {
      bool syntaxOk = matchProgram();
      foreach(string errorMessage in errorMessages) {
        Console.WriteLine(errorMessage);
      }
      return syntaxOk;
    }

    private void addError(string message) {
      if(this.currentToken != null) {
        message = "[Row: " + this.currentToken.getRowNumber() + ", Column: " + this.currentToken.getColumnNumber() + "] " + message;
      }
      errorMessages.Add(message);
    }

    private bool matchProgram() {
      readToken();
      do {
        if(matchStatement()) {
          readToken();
          if(!matchSemicolon()) {
            addError("Expected a semicolon after a statement.");
            return false;
          }
        } else {
          return false;
        }
        readToken();
      } while(hasMoreTokens());
      return true;
    }

    private bool matchStatement() {
      if(matchRead()) {
        readToken();
        return matchIdentifier();
      } else if(matchPrint()) {
        readToken();
        return matchExpression();
      } else if(matchAssert()) {
        readToken();
        if(matchLeftParenthesis()) {
          readToken();
          if(matchExpression()) {
            readToken();
            if(matchRightParenthesis()) {
              return true;
            } else {
              addError("Expected a closing right parenthesis after expression.");
            }
          } else {
            addError("Expected an expression.");
          }
        } else {
          addError("Expected a left parenthesis after assert.");
        }
      } else {
        addError("Illegal start of a statement. A statement can't begin with '" + this.currentToken.getLexeme() + "'.");
      }
      return false;
    }

    private bool matchExpression() {
      if(matchNot()) {
        readToken();
        return matchOperand();
      } else if(matchOperand()) {
        if(peekType(first("operation"))) {
          readToken();
          matchOperation();
          readToken();
          return matchOperand(); 
        }
        return true;
      }
      return false;
    }

    private bool peekType(ICollection<MiniPLTokenType> set) {
      if(this.nextToken != null) {
        return set.Contains(this.nextToken.getType());
      }
      return false;
    }

    private bool matchOperand() {
      if(matchIntegerLiteral() || matchStringLiteral() || matchIdentifier()) {
        return true;
      } else if(matchLeftParenthesis()) {
        readToken();
        if(matchExpression()) {
          readToken();
          return matchRightParenthesis();
        }
      }
      return false;
    }

    private bool peekType(MiniPLTokenType type) {
      return this.nextToken != null && this.nextToken.getType().Equals(type);
    }

    private bool matchIdentifier() {
      return match(MiniPLTokenType.IDENTIFIER);
    }

    private bool matchIntegerLiteral() {
      return match(MiniPLTokenType.INTEGER_LITERAL);
    }

    private bool matchStringLiteral() {
      return match(MiniPLTokenType.STRING_LITERAL);
    }

    private bool matchLeftParenthesis() {
      return match(MiniPLTokenType.LEFT_PARENTHESIS);
    }

    private bool matchRightParenthesis() {
      return match(MiniPLTokenType.RIGHT_PARENTHESIS);
    }

    private bool matchOperation() {
      if(matchPlus() || matchMinus() || matchAsterisk() || matchSlash() || matchLessThan() || matchEqual() || matchAnd() || matchNot())
        return true;
      else
        return false;
    }

    private bool matchPlus() {
      return match(MiniPLTokenType.PLUS);
    }

    private bool matchMinus() {
      return match(MiniPLTokenType.MINUS);
    }

    private bool matchAsterisk() {
      return match(MiniPLTokenType.ASTERISK);
    }

    private bool matchSlash() {
      return match(MiniPLTokenType.SLASH);
    }

    private bool matchLessThan() {
      return match(MiniPLTokenType.LESS_THAN_COMPARISON);
    }

    private bool matchEqual() {
      return match(MiniPLTokenType.EQUALITY_COMPARISON);
    }

    private bool matchAnd() {
      return match(MiniPLTokenType.LOGICAL_AND);
    }

    private bool matchNot() {
      return match(MiniPLTokenType.LOGICAL_NOT);
    }

    private bool matchSemicolon() {
      return match(MiniPLTokenType.SEMICOLON);
    }

    private bool matchRead() {
      return match(MiniPLTokenType.KEYWORD_READ);
    }

    private bool matchPrint() {
      return match(MiniPLTokenType.KEYWORD_PRINT);
    }

    private bool matchAssert() {
      return match(MiniPLTokenType.KEYWORD_ASSERT);
    }

    private bool match(MiniPLTokenType type) {
      if(currentToken == null) {
        return false;
      } else {
        return currentToken.getType().Equals(type);
      }
    }

    private ICollection<MiniPLTokenType> first(string rule) {
      ICollection<MiniPLTokenType> firstSet = new HashSet<MiniPLTokenType>();
      switch(rule) {
        case "operation":
          firstSet.Add(MiniPLTokenType.PLUS);
          firstSet.Add(MiniPLTokenType.MINUS);
          firstSet.Add(MiniPLTokenType.ASTERISK);
          firstSet.Add(MiniPLTokenType.SLASH);
          firstSet.Add(MiniPLTokenType.LOGICAL_AND);
          firstSet.Add(MiniPLTokenType.LOGICAL_NOT);
          firstSet.Add(MiniPLTokenType.EQUALITY_COMPARISON);
          firstSet.Add(MiniPLTokenType.LESS_THAN_COMPARISON);
          break;
      }
      return firstSet;
    }

    private ICollection<MiniPLTokenType> follow(string rule) {
      ICollection<MiniPLTokenType> followSet = new HashSet<MiniPLTokenType>();
      switch(rule) {
        case "opnd":
          followSet.Add(MiniPLTokenType.INTEGER_LITERAL);
          followSet.Add(MiniPLTokenType.STRING_LITERAL);
          followSet.Add(MiniPLTokenType.IDENTIFIER);
          followSet.Add(MiniPLTokenType.LEFT_PARENTHESIS);
          break;
      }
      return followSet;
    }
  }

}