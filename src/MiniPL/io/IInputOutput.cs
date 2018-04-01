using System;

namespace MiniPL.io {

  public interface IInputOutput {

    void output(string value);

    void output(int value);

    string input();

  }

}