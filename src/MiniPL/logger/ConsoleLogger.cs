using System;

namespace MiniPL.logger {

  public class ConsoleLogger : ILogger {

    public void log(string log) {
      Console.WriteLine(log);
    }

  }

}