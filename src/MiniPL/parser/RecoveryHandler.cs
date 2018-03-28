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
          if(this.firstAndFollow.firstContains(symbol, goodToken.getType())) {
            procedureMethod(); 
            return;
          }
          if(this.firstAndFollow.followContains(symbol, goodToken.getType())) {
            return;
          }
          if(nextTokenShouldEndRecovery(symbol)) {
            return;
          }
        }
      } while(this.tokenReader.hasNextToken());
    }

    private bool nextTokenShouldEndRecovery(MiniPLSymbol symbol) {
      return this.tokenReader.hasNextToken() && this.firstAndFollow.followContains(symbol, this.tokenReader.getNextTokensType());
    }

  }

}