using System;

namespace MiniPL.logger {

  public class ConsoleLogger : Logger {

    public void log(string log) {
      Console.WriteLine(log);
    }

  }

}