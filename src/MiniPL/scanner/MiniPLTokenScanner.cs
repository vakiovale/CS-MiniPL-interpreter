using System;
using System.Text;
using System.Collections.Generic;
using MiniPL.scanner;

namespace MiniPL.tokens {

  public class MiniPLTokenScanner : TokenScanner<MiniPLTokenType> {

    private IDictionary<String, MiniPLTokenType> keywords;

    private StringBuilder currentTokenContent;

    public MiniPLTokenScanner(IScanner characterScanner) : base(characterScanner) {
      initializeKeywords();
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
      if(!hasNext()) {
        return null;
      } else {
        return readToken();
      }
    }

    private bool hasNext() {
      return this.characterScanner.hasNext();
    }

    private bool hasWhitespace() {
      return Char.IsWhiteSpace(peek());
    }

    private void removeWhitespace() {
      readNextCharacter();
    }

    private bool isLetter(char character) {
      return Char.IsLetter(character);
    }

    private bool isDigit(char character) {
      return Char.IsDigit(character);
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

    private Token<MiniPLTokenType> readToken() {
      removeWhitespaceIfExists();

      currentTokenContent = new StringBuilder();

      char character = readNextCharacter();
      currentTokenContent.Append(character);

      // Checking identifiers AND keywords
      if(isLetter(character)) {
        while(hasNext() && nextCharacterIsUnderscoreLetterOrDigit()) {
          character = readNextCharacter(); 
          currentTokenContent.Append(character);
        }
        String token = currentTokenContent.ToString();
        if(isReservedKeyword(token)) {
          return getKeywordToken(token);
        } else {
          return createIdentifier(token);
        }
      }

      // Checking integer literals
      if(isDigit(character)) {
        while(hasNext() && nextCharacterIsDigit()) {
          character = readNextCharacter();
          currentTokenContent.Append(character);
        }
        String token = currentTokenContent.ToString();
        if(nextCharacterIsAllowedAfterDigit()) {
          return createIntegerLiteral(token);
        }
      }

      // Checking special characters
      if(currentTokenContent.Length == 1) {
        Token<MiniPLTokenType> specialToken = findValidTokenStartingWithSpecialCharacter();
        if(specialToken != null) {
          return specialToken;
        }
      }

      // Checking invalid tokens
      if(hasNext() && !hasWhitespace() && currentTokenContent.Length > 0) {
        while(hasNext() && !hasWhitespace()) {
          currentTokenContent.Append(readNextCharacter());
        }
      }
      
      return createInvalidToken(currentTokenContent.ToString());
    }

    private Token<MiniPLTokenType> findValidTokenStartingWithSpecialCharacter() {
      char character = currentTokenContent[0];
      switch(character) {
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
        char nextChar = readNextCharacter();
        if(nextChar == '\\') {
          lastCharWasEscapeCharacter = true;
          continue;
        } else {
          lastCharWasEscapeCharacter = false;
        }
        if(!lastCharWasEscapeCharacter && nextChar == '"') {
          return createStringLiteral(stringLiteral.ToString());
        }
        stringLiteral.Append(nextChar);
        currentTokenContent.Append(nextChar);
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