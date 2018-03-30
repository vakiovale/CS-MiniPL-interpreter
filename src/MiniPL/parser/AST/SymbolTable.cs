using System;
using System.Collections.Generic;

namespace MiniPL.parser.AST {

  public class SymbolTable : ISymbolTable {

    private IDictionary<string, int> integerVariables;

    private IDictionary<string, string> stringVariables;

    private IDictionary<string, bool> boolVariables;

    public SymbolTable() {
      this.integerVariables = new Dictionary<string, int>();
      this.stringVariables = new Dictionary<string, string>();
      this.boolVariables = new Dictionary<string, bool>();
    }

    public void addVariable(string variableName, int value) {
      this.integerVariables.Add(variableName, value);
    }

    public void addVariable(string variableName, string value) {
      this.stringVariables.Add(variableName, value);
    }

    public void addVariable(string variableName, bool value) {
      this.boolVariables.Add(variableName, value);
    }

    public bool getBool(string variableName) {
      return this.boolVariables[variableName];
    }

    public int getInt(string variableName) {
      return this.integerVariables[variableName];
    }

    public string getString(string variableName) {
      return this.stringVariables[variableName];
    }

    public bool hasBool(string variableName) {
      return this.boolVariables.ContainsKey(variableName);
    }

    public bool hasInteger(string variableName) {
      return this.integerVariables.ContainsKey(variableName);
    }

    public bool hasString(string variableName) {
      return this.stringVariables.ContainsKey(variableName);
    }

    public bool hasVariable(string variableName) {
      return this.integerVariables.ContainsKey(variableName) || this.stringVariables.ContainsKey(variableName) || this.boolVariables.ContainsKey(variableName);
    }
  }

}