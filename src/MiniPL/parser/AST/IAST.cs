using System;

namespace MiniPL.parser.AST {

  public interface IAST {

    INode getProgram();

    void addProgramNode(INode program);
        
  }

}