using System;
using System.Runtime.Serialization;
using MiniPL.tokens;

namespace MiniPL.exceptions {

  public class LexicalException : MiniPLException {
    public LexicalException(string message, Token<MiniPLTokenType> token) : base("LEXICAL ERROR: " + message, token) { }
  }

}