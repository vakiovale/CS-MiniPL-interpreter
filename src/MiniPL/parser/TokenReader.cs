using System;
using MiniPL.scanner;
using MiniPL.tokens;

namespace MiniPL.parser {

  public class TokenReader {

    private Token<MiniPLTokenType> currentToken;

    private Token<MiniPLTokenType> nextToken;
    
    private ITokenScanner<MiniPLTokenType> scanner;
    
    public TokenReader(ITokenScanner<MiniPLTokenType> scanner) {
      this.currentToken = null;
      this.nextToken = null;
      this.scanner = scanner;
    }
    
    public Token<MiniPLTokenType> readToken() {
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

    public Token<MiniPLTokenType> token() {
      return this.currentToken;
    }

    public Token<MiniPLTokenType> getNextToken() {
      return this.nextToken;
    }

    public bool hasNextToken() {
      return this.nextToken != null;
    }

    public MiniPLTokenType getCurrentType() {
      return this.currentToken.getType();
    }

    public bool isNextTokensType(MiniPLTokenType type) {
      return this.nextToken != null && this.nextToken.getType().Equals(type);
    }

    public MiniPLTokenType getNextTokensType() {
      return this.nextToken.getType();
    }

  }

}