using System;

namespace MiniPL.scanner {

  public interface IScanner { 

    String getSource();

    void setSource(String source);

    char readNextCharacter();

    bool hasNext();

  }
}