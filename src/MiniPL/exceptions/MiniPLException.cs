using System;
using MiniPL.tokens;

namespace MiniPL.exceptions {

  public class MiniPLException : Exception {
        
    protected string message;

    public MiniPLException(string message, Token<MiniPLTokenType> token) : base(constructMessage(message, token)) {
      this.message = constructMessage(message, token);
    }

    public string getMessage() {
      return this.message;
    }

    private static string constructMessage(string message, Token<MiniPLTokenType> token)
    {
      if(token != null) {
        return "[Row: " + token.getRowNumber() + ", Column: " + token.getColumnNumber() + "] " + message;
      } else {
        return message;
      }
    }
  }

}