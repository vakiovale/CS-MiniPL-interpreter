using System;
using MiniPL.parser.AST;

namespace MiniPL.parser {

  public interface IParser {

    bool processAndBuildAST();
    IAST getAST();

  }
}