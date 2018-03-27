using System;
using System.Collections.Generic;

namespace MiniPL.scanner {

  public interface IScanner { 

    String getSource();
    void setSource(String source);
    char readNextCharacter();
    char peek();
    bool hasNext();

  }
}