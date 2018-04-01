using System;

namespace MiniPL.io {

  public class ConsoleIO : IInputOutput
  {
    public string input() {
      string read = Console.ReadLine();
      return read.IndexOf(" ") > -1 ? read.Substring(0, read.IndexOf(" ")) : read;
    }

    public void output(string value) {
      Console.Write(value);
    }

    public void output(int value) {
      Console.Write(value);
    }
  }

}