using System;
using MiniPL.syntax;
using MiniPL.tokens;

namespace MiniPL.parser {

  public class RecoveryHandler {

    private TokenReader tokenReader;

    private Action readToken;

    private FirstAndFollow firstAndFollow;

    public RecoveryHandler(TokenReader tokenReader, Action readTokenMethod, FirstAndFollow firstAndFollow) {
      this.tokenReader = tokenReader;
      this.readToken = readTokenMethod;
      this.firstAndFollow = firstAndFollow;
    }

    public void tryToRecoverFromException(MiniPLSymbol symbol, Action procedureMethod) {
      if(nextTokenShouldEndRecovery(symbol)) {
        return;
      }
      do {
        readToken();
        Token<MiniPLTokenType> goodToken = this.tokenReader.token();
        if(goodToken != null) {
          if(symbolInFirst(symbol, goodToken.getType())) {
            procedureMethod(); 
            return;
          }
          if(symbolInFollow(symbol, goodToken.getType())) {
            return;
          }
          if(nextTokenShouldEndRecovery(symbol)) {
            return;
          }
        }
      } while(this.tokenReader.hasNextToken());
    }

    private bool symbolInFollow(MiniPLSymbol symbol, MiniPLTokenType miniPLTokenType) {
      return this.firstAndFollow.followContains(symbol, miniPLTokenType);
    }

    private bool symbolInFirst(MiniPLSymbol symbol, MiniPLTokenType miniPLTokenType) {
      return this.firstAndFollow.firstContains(symbol, miniPLTokenType);
    }

    private bool nextTokenShouldEndRecovery(MiniPLSymbol symbol) {
      return this.tokenReader.hasNextToken() && this.firstAndFollow.followContains(symbol, this.tokenReader.getNextTokensType());
    }

  }

}