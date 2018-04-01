using System;

namespace MiniPL.io {

  public interface IInputOutput {

    void outputLine(string value);

    void outputLine(int value);

    void output(string value);

    void output(int value);

    string input();

  }

}