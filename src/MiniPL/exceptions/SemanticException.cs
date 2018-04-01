using System;
using System.Runtime.Serialization;
using MiniPL.tokens;

namespace MiniPL.exceptions {

  public class SemanticException : Exception {

    private string message;

    public SemanticException(string message) : base(constructMessage(message)) { 
      this.message = constructMessage(message); 
    }

    private static string constructMessage(string message) {
      return "SEMANTIC EXCEPTION: " + message;
    }

    public string getMessage() {
      return this.message;
    }
  }

}