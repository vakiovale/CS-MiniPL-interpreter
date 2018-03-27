using System;
using System.Runtime.Serialization;
using MiniPL.tokens;

namespace MiniPL.exceptions {

  public class SyntaxException : Exception
  {

    private string message;

    public SyntaxException(string message, Token<MiniPLTokenType> token) : base(constructMessage(message, token)) {
      this.message = "SYNTAX ERROR: " + constructMessage(message, token);
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