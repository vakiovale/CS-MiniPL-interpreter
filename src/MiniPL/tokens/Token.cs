using System;

namespace MiniPL.tokens {

  public class Token<T> {
        
    private T type;

    private String lexeme;

    private int rowNumber;

    private int columnNumber;

    public Token(T type) {
      this.type = type;
      this.lexeme = null;
    }

    public Token(T type, String lexeme) {
      this.type= type;
      this.lexeme = lexeme;
    }

    public Token(T type, String lexeme, int rowNumber, int columnNumber) {
      this.type= type;
      this.lexeme = lexeme;
      this.rowNumber = rowNumber;
      this.columnNumber = columnNumber;
    }

    public T getType() {
      return this.type;
    }

    public String getLexeme() {
      return this.lexeme;
    }

    public int getRowNumber() {
      return this.rowNumber;
    }

    public int getColumnNumber() {
      return this.columnNumber;
    }

    public void setRowNumber(int rowNumber) {
      this.rowNumber = rowNumber;
    }

    public void setColumnNumber(int columnNumber) {
      this.columnNumber = columnNumber;
    }
  }
}