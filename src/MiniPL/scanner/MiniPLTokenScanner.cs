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

    private TokenCreator tokenCreator;

    private char currentCharacter;

    public MiniPLTokenScanner(IScanner characterScanner) : base(characterScanner) {
      initializeToken();
      initializeCurrentCharacter();
      initializeKeywords();
      initializeTokenCreator();
    }

    public override Token<MiniPLTokenType> readNextToken() { 
      initializeToken();
      initializeCurrentCharacter();
      processNextToken();
      return this.token;
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

    private void initializeTokenCreator() {
      this.tokenCreator = new TokenCreator();
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
      this.tokenCreator.update(this.currentCharacter);
      return this.currentCharacter;
    }

    private void removeNextCharacter() {
      this.currentCharacter = this.characterScanner.readNextCharacter();
      this.tokenCreator.update(this.currentCharacter);
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
      return tokenCreator.createToken(this.keywords.get(key), key);
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
      this.token = tokenCreator.createInvalidToken(currentTokenContent.ToString());
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
          this.token = tokenCreator.createIdentifier(token);
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
        this.token = tokenCreator.createIntegerLiteral(token);
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
          this.token = tokenCreator.createInvalidToken(currentTokenContent.ToString());
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
          return tokenCreator.createToken(MiniPLTokenType.SEMICOLON, ";");
        case '=':
          return tokenCreator.createToken(MiniPLTokenType.EQUALITY_COMPARISON, "=");
        case '<':
          return tokenCreator.createToken(MiniPLTokenType.LESS_THAN_COMPARISON, "<");
        case '+':
          return tokenCreator.createToken(MiniPLTokenType.PLUS, "+");
        case '-':
          return tokenCreator.createToken(MiniPLTokenType.MINUS, "-");
        case '*':
          return tokenCreator.createToken(MiniPLTokenType.ASTERISK, "*");
        case '/':
          return tokenCreator.createToken(MiniPLTokenType.SLASH, "/");
        case '&':
          return tokenCreator.createToken(MiniPLTokenType.LOGICAL_AND, "&");
        case '!':
          return tokenCreator.createToken(MiniPLTokenType.LOGICAL_NOT, "!");
        case '(':
          return tokenCreator.createToken(MiniPLTokenType.LEFT_PARENTHESIS, "(");
        case ')':
          return tokenCreator.createToken(MiniPLTokenType.RIGHT_PARENTHESIS, ")");
        case '\\':
          return tokenCreator.createToken(MiniPLTokenType.BACKSLASH, "\\");
        case ':':
          if(hasNext() && peek() == '=') {
            readNextCharacter();
            return tokenCreator.createToken(MiniPLTokenType.ASSIGNMENT_OPERATOR, ":=");
          } else {
            return tokenCreator.createToken(MiniPLTokenType.COLON, ":");
          }
        case '.':
          if(hasNext() && peek() == '.') {
            readNextCharacter();
            return tokenCreator.createToken(MiniPLTokenType.RANGE_OPERATOR, "..");
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
          return tokenCreator.createStringLiteral(stringLiteral.ToString());
        }
        stringLiteral.Append(currentCharacter);
        lastCharWasEscapeCharacter = false;
      }
      return tokenCreator.createInvalidToken(currentTokenContent.ToString());
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