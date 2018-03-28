using System;
using MiniPL.syntax;

namespace MiniPL.parser.AST {

  public class MiniPLAST : IAST
  {
    private INode program;

    public MiniPLAST() {
      this.program = null;
    }

    public void addProgramNode(INode program) {
      this.program = program;
    }

    public INode getProgram() {
      return this.program;
    }
  }

}