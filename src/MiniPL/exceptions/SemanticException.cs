using System;
using System.Runtime.Serialization;
using MiniPL.tokens;

namespace MiniPL.exceptions {

  public class SemanticException : Exception {

    public SemanticException(string message) : base("SEMANTIC EXCEPTION: " + message) { }

  }

}