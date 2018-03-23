using System;
using System.Text;
using System.Collections.Generic;
using MiniPL.scanner;

namespace MiniPL.tokens {

  public class MiniPLTokenScanner : TokenScanner<MiniPLTokenType> {

    private IDictionary<String, MiniPLTokenType> keywords;

    private StringBuilder currentTokenContent;

    private Token<MiniPLTokenType> token;

    private char currentCharacter;

    public MiniPLTokenScanner(IScanner characterScanner) : base(characterScanner) {
      initializeToken();
      initializeCurrentCharacter();
      initializeKeywords();
    }

    private void initializeToken() {
      this.token = null;
    }

    private void initializeCurrentCharacter() {
      this.currentCharacter = '\0';
    }

    private void initializeKeywords() {
      keywords = new Dictionary<String, MiniPLTokenType>();
      keywords.Add("var", MiniPLTokenType.KEYWORD_VAR);
      keywords.Add("for", MiniPLTokenType.KEYWORD_FOR);
      keywords.Add("end", MiniPLTokenType.KEYWORD_END);
      keywords.Add("in", MiniPLTokenType.KEYWORD_IN);
      keywords.Add("do", MiniPLTokenType.KEYWORD_DO);
      keywords.Add("read", MiniPLTokenType.KEYWORD_READ);
      keywords.Add("print", MiniPLTokenType.KEYWORD_PRINT);
      keywords.Add("assert", MiniPLTokenType.KEYWORD_ASSERT);
      keywords.Add("int", MiniPLTokenType.TYPE_IDENTIFIER_INTEGER);
      keywords.Add("string", MiniPLTokenType.TYPE_IDENTIFIER_STRING);
      keywords.Add("bool", MiniPLTokenType.TYPE_IDENTIFIER_BOOL);
    }

    public override Token<MiniPLTokenType> readNextToken() { 
      initializeToken();
      removeWhitespaceIfExists();
      processNextToken();
      return this.token;
    }

    private bool hasNext() {
      return this.characterScanner.hasNext();
    }

    private bool hasWhitespace() {
      return Char.IsWhiteSpace(peek());
    }

    private void removeWhitespace() {
      removeNextCharacter();
    }

    private bool isLetter() {
      return Char.IsLetter(currentCharacter);
    }

    private bool isDigit() {
      return Char.IsDigit(currentCharacter);
    }

    private bool nextCharacterIsUnderscoreLetterOrDigit() {
      if(!hasNext()) {
        return false;
      } else {
        char peekedCharacter = peek();
        return Char.IsLetterOrDigit(peekedCharacter) || peekedCharacter == '_'; 
      }
    }

    private bool nextCharacterIsDigit() {
      if(!hasNext()) {
        return false;
      } else {
        char peekedCharacter = peek();
        return Char.IsDigit(peekedCharacter);
      }
    }

    private bool nextCharacterIsLetter() {
      if(!hasNext()) {
        return false;
      } else {
        char peekedCharacter = peek();
        return Char.IsLetter(peekedCharacter);
      }
    }

    private char peek() {
      return this.characterScanner.peek();
    }

    private char readNextCharacter() {
      this.currentCharacter = this.characterScanner.readNextCharacter();
      this.currentTokenContent.Append(this.currentCharacter);
      return this.currentCharacter;
    }

    private char removeNextCharacter() {
      return this.characterScanner.readNextCharacter();
    }

    private Token<MiniPLTokenType> createIdentifier(String lexeme) {
      return new Token<MiniPLTokenType>(MiniPLTokenType.IDENTIFIER, lexeme);
    }

    private Token<MiniPLTokenType> createStringLiteral(String lexeme) {
      return new Token<MiniPLTokenType>(MiniPLTokenType.STRING_LITERAL, lexeme);
    }

    private Token<MiniPLTokenType> createIntegerLiteral(String lexeme) {
      return new Token<MiniPLTokenType>(MiniPLTokenType.INTEGER_LITERAL, lexeme);
    }

    private void removeWhitespaceIfExists() {
      while(hasNext() && hasWhitespace()) {
        removeWhitespace();
      }
    }

    private bool isReservedKeyword(String content) {
      return this.keywords.ContainsKey(content);
    }

    private Token<MiniPLTokenType> getKeywordToken(String key) {
      return createToken(this.keywords[key]);
    }

    private Token<MiniPLTokenType> createToken(MiniPLTokenType type) {
      return new Token<MiniPLTokenType>(type);
    }

    private Token<MiniPLTokenType> createInvalidToken(String lexeme) {
      return new Token<MiniPLTokenType>(MiniPLTokenType.INVALID_TOKEN, lexeme);
    }

    private bool nextCharacterIsAllowedAfterDigit() {
      if(!hasNext() || hasWhitespace()) {
        return true;
      } else {
        char character = peek();
        switch(character) {
          case '.':
          case '-':
          case '+':
          case '*':
          case '/':
          case ';':
          case '=':
          case '<':
            return true;
          default:
            return false;
        }
      }
    }

    private void processNextToken() {
      if(hasNext()) {
        if(!findValidOrNullToken()) {
          handleInvalidToken();
        }
      } 
    }

    private void handleInvalidToken() {
      if(hasNext() && !hasWhitespace() && currentTokenContent.Length > 0) {
        while(hasNext() && !hasWhitespace()) {
          readNextCharacter();
        }
      }
      this.token = createInvalidToken(currentTokenContent.ToString());
    }

    private bool findValidOrNullToken() {
      currentTokenContent = new StringBuilder();
      readNextCharacter();

      if(clearPossibleOneLineComment()) {
        if(hasNext()) {
          readNextCharacter();
        } else {
          return true;
        }
      }

      if(clearPossibleMultiLineComment()) {
        if(this.token != null) {
          return true;
        }
        if(hasNext()) {
          readNextCharacter();
        } else {
          return true;
        }
      }

      if(checkKeywordsAndIdentifiers()) {
        return true;
      }

      if(checkIntegerLiteral()) {
        return true;
      }

      if(checkOneAndTwoCharacterTokens()) {
        return true;
      }

      return false;
    }

    private bool checkKeywordsAndIdentifiers() {
      if(isLetter()) {
        while(hasNext() && nextCharacterIsUnderscoreLetterOrDigit()) {
          readNextCharacter(); 
        }
        String token = currentTokenContent.ToString();
        if(isReservedKeyword(token)) {
          this.token = getKeywordToken(token);
          return true;
        } else {
          this.token = createIdentifier(token);
          return true;
        }
      }
      return false;
    }

    private bool checkIntegerLiteral() {
      if(isDigit()) {
        while(hasNext() && nextCharacterIsDigit()) {
          readNextCharacter();
        }
        String token = currentTokenContent.ToString();
        if(nextCharacterIsAllowedAfterDigit()) {
          this.token = createIntegerLiteral(token);
          return true;
        }
      }
      return false;
    }

    private bool checkOneAndTwoCharacterTokens() {
      if(currentTokenContent.Length == 1) {
        Token<MiniPLTokenType> specialToken = findValidTokenStartingWithSpecialCharacter();
        if(specialToken != null) {
          this.token = specialToken;
          return true;
        }
      }
      return false;
    }

    private bool clearPossibleOneLineComment() {
      if(isStartOfOneLineComment()) {
        this.currentTokenContent.Length--;
        skipToTheEndOfLine(); 
        removeWhitespaceIfExists();
        return true;
      }
      return false;
    }

    private bool clearPossibleMultiLineComment() {
      if(isStartOfMultiLineComment()) {
        if(!skipToTheEndOfMultiLineComment()) {
          this.token = createInvalidToken(currentTokenContent.ToString());
          return true;
        } else {
          currentTokenContent = new StringBuilder();
        }
        removeWhitespaceIfExists();
        if(hasNext()) {
          readNextCharacter();
        } else {
          return true;
        }
      }
      return false;
    }

    private bool skipToTheEndOfMultiLineComment() {
      int multiLineCommentNestLevel = 1;
      bool endOfMultiLineComment = false;
      while(!endOfMultiLineComment && hasNext()) {
        if(hasNext()) {
          readNextCharacter();
          if(currentCharacter == '/' && hasNext() && peek() == '*') {
            readNextCharacter();
            multiLineCommentNestLevel++;
          } else if (currentCharacter == '*' && hasNext() && peek() == '/') {
            readNextCharacter();
            multiLineCommentNestLevel--;
          }
          if(multiLineCommentNestLevel == 0) {
            endOfMultiLineComment = true;
          }
        }
      }
      return endOfMultiLineComment;
    }

    private bool isStartOfMultiLineComment() {
      if(currentCharacter == '/' && hasNext() && peek() == '*') {
        readNextCharacter();
        return true;
      } else {
        return false;
      }
    }

    private void skipToTheEndOfLine() {
      while(hasNext() && peek() != '\n') {
        removeNextCharacter(); 
      }
    }

    private bool isStartOfOneLineComment() {
      return currentCharacter == '/' && hasNext() && peek() == '/';
    }

    private Token<MiniPLTokenType> findValidTokenStartingWithSpecialCharacter() {
      switch(currentCharacter) {
        case ';':
          return createToken(MiniPLTokenType.SEMICOLON);
        case '=':
          return createToken(MiniPLTokenType.EQUALITY_COMPARISON);
        case '<':
          return createToken(MiniPLTokenType.LESS_THAN_COMPARISON);
        case '+':
          return createToken(MiniPLTokenType.PLUS);
        case '-':
          return createToken(MiniPLTokenType.MINUS);
        case '*':
          return createToken(MiniPLTokenType.ASTERISK);
        case '/':
          return createToken(MiniPLTokenType.SLASH);
        case '&':
          return createToken(MiniPLTokenType.LOGICAL_AND);
        case '!':
          return createToken(MiniPLTokenType.LOGICAL_NOT);
        case '(':
          return createToken(MiniPLTokenType.LEFT_PARENTHESIS);
        case ')':
          return createToken(MiniPLTokenType.RIGHT_PARENTHESIS);
        case '\\':
          return createToken(MiniPLTokenType.BACKSLASH);
        case ':':
          if(hasNext() && peek() == '=') {
            readNextCharacter();
            return createToken(MiniPLTokenType.ASSIGNMENT_OPERATOR);
          } else {
            return createToken(MiniPLTokenType.COLON);
          }
        case '.':
          if(hasNext() && peek() == '.') {
            readNextCharacter();
            return createToken(MiniPLTokenType.RANGE_OPERATOR);
          } else {
            return null;
          }
        case '\"':
          return tryToReadStringLiteral();
        default:
          return null;
      }
    }

    private Token<MiniPLTokenType> tryToReadStringLiteral() {
      StringBuilder stringLiteral = new StringBuilder();
      bool lastCharWasEscapeCharacter = false;
      while(hasNext() && !nextCharIsLineBreak()) {
        readNextCharacter();
        if(!lastCharWasEscapeCharacter && currentCharacter == '\\') {
          lastCharWasEscapeCharacter = true;
        } else if(!lastCharWasEscapeCharacter && currentCharacter == '"') {
          return createStringLiteral(stringLiteral.ToString());
        }
        stringLiteral.Append(currentCharacter);
        lastCharWasEscapeCharacter = false;
      }
      return createInvalidToken(currentTokenContent.ToString());
    }

    private bool nextCharIsLineBreak() {
      if(!hasNext()) {
        return false;
      } else {
        char peekedCharacter = peek();
        return peekedCharacter == '\n';
      }
    }
  }

}