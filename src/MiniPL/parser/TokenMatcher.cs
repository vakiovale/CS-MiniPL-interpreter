using System;
using System.Collections.Generic;
using MiniPL.exceptions;
using MiniPL.tokens;

namespace MiniPL.parser {

  public class TokenMatcher {

    private Token<MiniPLTokenType> token;

    public TokenMatcher() {
      this.token = null;
    }

    public void setToken(Token<MiniPLTokenType> token) {
      this.token = token;
    }

    public bool matchDo() {
      return match(MiniPLTokenType.KEYWORD_DO, "keyword 'do'");
    }

    public bool matchRange() {
      return match(MiniPLTokenType.RANGE_OPERATOR, "a range operator '..'"); 
    }

    public bool matchFor() {
      return match(MiniPLTokenType.KEYWORD_FOR, "keyword 'for'");
    }

    public bool matchIn() {
      return match(MiniPLTokenType.KEYWORD_IN, "keyword 'in'");
    }

    public bool matchEnd() {
      return match(MiniPLTokenType.KEYWORD_END, "keyword 'end'");
    }

    public bool matchIdentifier() {
      return match(MiniPLTokenType.IDENTIFIER, "an identifier");
    }

    public bool matchIntegerLiteral() {
      return match(MiniPLTokenType.INTEGER_LITERAL, "an integer");
    }

    public bool matchStringLiteral() {
      return match(MiniPLTokenType.STRING_LITERAL, "a string");
    }

    public bool matchLeftParenthesis() {
      return match(MiniPLTokenType.LEFT_PARENTHESIS, "left parenthesis '('");
    }

    public bool matchRightParenthesis() {
      return match(MiniPLTokenType.RIGHT_PARENTHESIS, "right parenthesis ')'");
    }

    public bool matchTypeInt() {
      return match(MiniPLTokenType.TYPE_IDENTIFIER_INTEGER, "type int");
    }

    public bool matchTypeString() {
      return match(MiniPLTokenType.TYPE_IDENTIFIER_STRING, "type string");
    }

    public bool matchTypeBool() {
      return match(MiniPLTokenType.TYPE_IDENTIFIER_BOOL, "type bool");
    }

    public bool matchAssignment() {
      return match(MiniPLTokenType.ASSIGNMENT_OPERATOR, "an assignment operator ':='");
    }

    public bool matchPlus() {
      return match(MiniPLTokenType.PLUS, "operator '+'");
    }

    public bool matchMinus() {
      return match(MiniPLTokenType.MINUS, "operator '-'");
    }

    public bool matchAsterisk() {
      return match(MiniPLTokenType.ASTERISK, "operator '*'");
    }

    public bool matchSlash() {
      return match(MiniPLTokenType.SLASH, "operator '/'");
    }

    public bool matchLessThan() {
      return match(MiniPLTokenType.LESS_THAN_COMPARISON, "operator '<'");
    }

    public bool matchEqual() {
      return match(MiniPLTokenType.EQUALITY_COMPARISON, "operator '='");
    }

    public bool matchAnd() {
      return match(MiniPLTokenType.LOGICAL_AND, "logicalt not operator '!'");
    }

    public bool matchNot() {
      return match(MiniPLTokenType.LOGICAL_NOT, "logical and operator '&'");
    }

    public bool matchSemicolon() {
      return match(MiniPLTokenType.SEMICOLON, "a semicolon ';'");
    }

    public bool matchColon() {
      return match(MiniPLTokenType.COLON, "a colon ':'");
    }

    public bool matchRead() {
      return match(MiniPLTokenType.KEYWORD_READ, "keyword read");
    }

    public bool matchVar() {
      return match(MiniPLTokenType.KEYWORD_VAR, "keyword var");
    }

    public bool matchPrint() {
      return match(MiniPLTokenType.KEYWORD_PRINT, "keyword print");
    }

    public bool matchAssert() {
      return match(MiniPLTokenType.KEYWORD_ASSERT, "keyword assert");
    }

    public bool match(MiniPLTokenType type, string errorToken) {
      if(this.token == null) {
        throw new SyntaxException("Expected " + errorToken + ". Could not find any token.", token);
      } else {
        if(this.token.getType().Equals(type)) {
          return true;
        } else {
          throw new SyntaxException("Expected " + errorToken + ". Found '" + token.getLexeme() + "' instead.", token);
        }
      }
    }

    public bool isTokenType(MiniPLTokenType type) {
      if(this.token == null) {
        return false;
      } else {
        return this.token.getType().Equals(type);
      }
    }

    public bool isTokenTypeInCollection(ICollection<MiniPLTokenType> set) {
      if(this.token == null) {
        return false;
      } else {
        return set.Contains(this.token.getType());
      } 
    }
  }

}