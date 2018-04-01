using System;

namespace MiniPL.parser.AST {

  public interface ISymbolTable {

    bool hasVariable(string variableName);

    void addVariable(string variableName, int value);

    void addVariable(string variableName, string value);

    void addVariable(string variableName, bool value);

    void updateVariable(string variableName, int value);

    void updateVariable(string variableName, string value);

    void updateVariable(string variableName, bool value);

    int getInt(string variableName);

    string getString(string variableName);

    bool getBool(string variableName);

    bool hasInteger(string variableName);

    bool hasString(string variableName);

    bool hasBool(string variableName);

  }
}