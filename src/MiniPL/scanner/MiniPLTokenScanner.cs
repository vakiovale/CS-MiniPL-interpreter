using System;
using System.Text;
using System.Collections.Generic;
using MiniPL.scanner;
using MiniPL.tokens;

namespace MiniPL.scanner {

  public class MiniPLTokenScanner : TokenScanner<MiniPLTokenType> {

    private MiniPLKeywords keywords;

    private StringBuilder currentTokenContent;

    private Token<MiniPLTokenType> token;

    private int columnNumber;

    private int rowNumber;

    private char currentCharacter;

    public MiniPLTokenScanner(IScanner characterScanner) : base(characterScanner) {
      initializeToken();
      initializeCurrentCharacter();
      initializeKeywords();
      initializeRowAndColumnNumbers();
    }

    private void initializeToken() {
      this.token = null;
    }

    private void initializeCurrentCharacter() {
      this.currentCharacter = '\0';
    }

    private void initializeKeywords() {
      this.keywords = new MiniPLKeywords();
    }

    private void initializeRowAndColumnNumbers() {
      this.rowNumber = 1;
      this.columnNumber = 1;
    }

    public override Token<MiniPLTokenType> readNextToken() { 
      initializeToken();
      initializeCurrentCharacter();
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
      handleRowAndColumnNumbering();
      return this.currentCharacter;
    }

    private void removeNextCharacter() {
      this.characterScanner.readNextCharacter();
      handleRowAndColumnNumbering();
    }

    private void handleRowAndColumnNumbering() {
      if(this.currentCharacter == '\n') {
        rowNumber++;
        columnNumber = 1;
      } else {
        columnNumber++;
      }
    }

    private void removeWhitespaceIfExists() {
      while(hasNext() && hasWhitespace()) {
        removeWhitespace();
      }
    }

    private bool isReservedKeyword(String content) {
      return this.keywords.containsKey(content);
    }

    private Token<MiniPLTokenType> getKeywordToken(String key) {
      return TokenFactory.createToken(this.keywords.get(key), key);
    }

    private void processNextToken() {
      removeWhitespaceIfExists();
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
      this.token = TokenFactory.createInvalidToken(currentTokenContent.ToString());
    }

    private bool findValidOrNullToken() {
      currentTokenContent = new StringBuilder();
      readNextCharacter();

      if(!removePossibleComments()) {
        return true;
      }

      if(checkKeywordsAndIdentifiers() || 
         checkIntegerLiteral() || 
         checkOneAndTwoCharacterTokens())
        return true;

      return false;
    }

    private bool removePossibleComments() {
      while(clearPossibleOneLineComment() || clearPossibleMultiLineComment()) {
        if(this.token != null) {
          return false;
        }
        if(hasNext()) {
          readNextCharacter();
        } else {
          return false;
        }
      }
      return true;
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
          this.token = TokenFactory.createIdentifier(token);
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
        this.token = TokenFactory.createIntegerLiteral(token);
        return true;
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
          this.token = TokenFactory.createInvalidToken(currentTokenContent.ToString());
          return true;
        } else {
          currentTokenContent = new StringBuilder();
        }
        removeWhitespaceIfExists();
        return true;
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
          return TokenFactory.createToken(MiniPLTokenType.SEMICOLON, ";");
        case '=':
          return TokenFactory.createToken(MiniPLTokenType.EQUALITY_COMPARISON, "=");
        case '<':
          return TokenFactory.createToken(MiniPLTokenType.LESS_THAN_COMPARISON, "<");
        case '+':
          return TokenFactory.createToken(MiniPLTokenType.PLUS, "+");
        case '-':
          return TokenFactory.createToken(MiniPLTokenType.MINUS, "-");
        case '*':
          return TokenFactory.createToken(MiniPLTokenType.ASTERISK, "*");
        case '/':
          return TokenFactory.createToken(MiniPLTokenType.SLASH, "/");
        case '&':
          return TokenFactory.createToken(MiniPLTokenType.LOGICAL_AND, "&");
        case '!':
          return TokenFactory.createToken(MiniPLTokenType.LOGICAL_NOT, "!");
        case '(':
          return TokenFactory.createToken(MiniPLTokenType.LEFT_PARENTHESIS, "(");
        case ')':
          return TokenFactory.createToken(MiniPLTokenType.RIGHT_PARENTHESIS, ")");
        case '\\':
          return TokenFactory.createToken(MiniPLTokenType.BACKSLASH, "\\");
        case ':':
          if(hasNext() && peek() == '=') {
            readNextCharacter();
            return TokenFactory.createToken(MiniPLTokenType.ASSIGNMENT_OPERATOR, ":=");
          } else {
            return TokenFactory.createToken(MiniPLTokenType.COLON, ":");
          }
        case '.':
          if(hasNext() && peek() == '.') {
            readNextCharacter();
            return TokenFactory.createToken(MiniPLTokenType.RANGE_OPERATOR, "..");
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
          return TokenFactory.createStringLiteral(stringLiteral.ToString());
        }
        stringLiteral.Append(currentCharacter);
        lastCharWasEscapeCharacter = false;
      }
      return TokenFactory.createInvalidToken(currentTokenContent.ToString());
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