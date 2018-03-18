using System;
using System.IO;

namespace MiniPL.scanner {

  // Scanner for reading characters one by one from a String.
  // Does not backtrack.
  public class Scanner : IScanner {

    private String source;

    private int length;

    private int readIndex;

    public Scanner() {
      this.source = null;
      this.readIndex = 0;
      this.length = 0;
    }

    public Scanner(String source) {
      setSource(source);
    }

    public void setSource(String source) {
      if(source != null) {
        this.source = source;
      }
      this.readIndex = 0;
      this.length = source != null ? source.Length : 0;
    }
    public String getSource() {
      return this.source;
    }

    public char readNextCharacter() {
      if(readIndex < this.length) {
        return this.source[readIndex++];
      } else {
        throw new IndexOutOfRangeException("Can't read any more characters");
      }
    }

    public bool hasNext() { 
      return this.readIndex < this.length;
    }
  }

}