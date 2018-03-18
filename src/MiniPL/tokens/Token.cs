using System;

namespace MiniPL.tokens {

  public class Token<T> {
        
    private T type;

    private String lexeme;

    public Token(T type) {
      this.type = type;
      this.lexeme = null;
    }

    public Token(T type, String lexeme) {
      this.type= type;
      this.lexeme = lexeme;
    }

    public T getType() {
      return this.type;
    }

    public String getLexeme() {
      return this.lexeme;
    }
  }
}