using System;
using System.Text;
using System.Collections.Generic;
using MiniPL.scanner;

namespace MiniPL.tokens {

  public class MiniPLTokenScanner : TokenScanner<MiniPLTokenType> {

    private IDictionary<String, MiniPLTokenType> keywords;

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

    private bool nextCharacterIsUnderscoreLetterOrDigit() {
      if(!hasNext()) {
        return false;
      } else {
        char peekedCharacter = peek();
        return Char.IsLetterOrDigit(peekedCharacter) || peekedCharacter == '_'; 
      }
    }

    private char peek() {
      return this.characterScanner.peek();
    }

    private char readNextCharacter() {
      return this.characterScanner.readNextCharacter();
    }

    private Token<MiniPLTokenType> createStringLiteral(String lexeme) {
      return new Token<MiniPLTokenType>(MiniPLTokenType.STRING_LITERAL, lexeme);
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

    private Token<MiniPLTokenType> readToken() {
      removeWhitespaceIfExists();
      char character = readNextCharacter();

      if(isLetter(character)) {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(character);
        while(hasNext() && nextCharacterIsUnderscoreLetterOrDigit()) {
          character = readNextCharacter(); 
          stringBuilder.Append(character);
        }
        String token = stringBuilder.ToString();
        if(isReservedKeyword(token)) {
          return getKeywordToken(token);
        } else {
          return createStringLiteral(token);
        }
      }

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
        case '\"':
          return createToken(MiniPLTokenType.QUOTE);
        case '\\':
          return createToken(MiniPLTokenType.BACKSLASH);
        default:
          return new Token<MiniPLTokenType>(MiniPLTokenType.INVALID_TOKEN);
      }
    }

    private Token<MiniPLTokenType> createToken(MiniPLTokenType type) {
      return new Token<MiniPLTokenType>(type);
    }
  }

}