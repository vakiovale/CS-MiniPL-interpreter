using System;
using MiniPL.scanner;
using MiniPL.tokens;
using System.Collections.Generic;

namespace MiniPL.parser {

  public class MiniPLParser : IParser {

    private ITokenScanner<MiniPLTokenType> scanner;

    private Token<MiniPLTokenType> currentToken;

    private Token<MiniPLTokenType> nextToken;

    public MiniPLParser(ITokenScanner<MiniPLTokenType> scanner) {
      this.scanner = scanner;
      this.currentToken = null;
      this.nextToken = null;
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
      return matchProgram();
    }

    private bool matchProgram() {
      readToken();
      do {
        if(matchStatement()) {
          readToken();
          if(!matchSemicolon()) {
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

    private bool match(MiniPLTokenType type) {
      if(currentToken == null) {
        return false;
      } else {
        return currentToken.getType().Equals(type);
      }
    }

    private ICollection<MiniPLTokenType> first(string rule) {
      ICollection<MiniPLTokenType> followSet = new HashSet<MiniPLTokenType>();
      switch(rule) {
        case "operation":
          followSet.Add(MiniPLTokenType.PLUS);
          followSet.Add(MiniPLTokenType.MINUS);
          followSet.Add(MiniPLTokenType.ASTERISK);
          followSet.Add(MiniPLTokenType.SLASH);
          followSet.Add(MiniPLTokenType.LOGICAL_AND);
          followSet.Add(MiniPLTokenType.LOGICAL_NOT);
          followSet.Add(MiniPLTokenType.EQUALITY_COMPARISON);
          followSet.Add(MiniPLTokenType.LESS_THAN_COMPARISON);
          break;
      }
      return followSet;
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