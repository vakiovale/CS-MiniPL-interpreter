using System;
using System.Runtime.Serialization;
using MiniPL.tokens;

namespace MiniPL.exceptions {

  public class SyntaxException : MiniPLException {
    public SyntaxException(string message, Token<MiniPLTokenType> token) : base("SYNTAX ERROR: " + message, token) { }
  }

}