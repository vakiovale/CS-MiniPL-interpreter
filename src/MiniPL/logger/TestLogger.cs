using System;
using System.Collections.Generic;

namespace MiniPL.logger {

  public class TestLogger : ILogger {

    private IList<string> logs;

    public TestLogger() {
      this.logs = new List<string>();
    }

    public void log(string log) {
      this.logs.Add(log);
    }

    public IList<string> getLogs() {
      return this.logs;
    }
        
  }
}