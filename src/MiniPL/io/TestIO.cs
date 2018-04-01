using System;
using System.Collections.Generic;

namespace MiniPL.io {

  public class TestIO : IInputOutput {

    private IList<string> outputList;

    private IList<string> inputList;

    private int readIndex;

    public TestIO() {
      this.inputList = new List<string>();
      this.outputList = new List<string>();
      this.readIndex = 0;
    }

    public TestIO(IList<string> input) {
      this.inputList = input;
      this.outputList = new List<string>();
      this.readIndex = 0;
    }

    public IList<string> getOutput() {
      return this.outputList;
    }

    public void output(string value) {
      this.outputList.Add(value);
    }

    public void output(int value) {
      this.outputList.Add(value.ToString());
    }

    string IInputOutput.input() {
      string read = this.inputList[readIndex];
      this.readIndex++;
      return read.IndexOf(" ") > -1 ? read.Substring(0, read.IndexOf(" ")) : read;
    }
  }

}