using System;

namespace MiniPL.scanner {
  public interface ITokenScanner {
      
      String getSource();

      void setSource(String source);
  }
}