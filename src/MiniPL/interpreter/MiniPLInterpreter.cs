using System;
using MiniPL.exceptions;
using MiniPL.io;
using MiniPL.parser;
using MiniPL.parser.AST;
using MiniPL.scanner;
using MiniPL.semantics;
using MiniPL.semantics.visitor;

namespace MiniPL.interpreter
{

  public class MiniPLInterpreter : IInterpreter {

    private string sampleProgram;

    private IInputOutput io;
    
    private IAST ast;

    private ISymbolTable symbolTable;

    public MiniPLInterpreter(string sampleProgram, ISymbolTable symbolTable, IInputOutput io) {
      this.sampleProgram = sampleProgram;
      this.io = io;
      this.ast = null;
      this.symbolTable = symbolTable;
    }

    public void interpret() {
      try {
        if(buildProgram()) {
          ProgramNode program = (ProgramNode)this.ast.getProgram();
          INodeVisitor interpreterVisitor = new InterpreterVisitor(this.symbolTable, this.io);
          program.accept(interpreterVisitor);
        }
      } catch(MiniPLException exception) {
        this.io.output(exception.getMessage());
      } catch(SemanticException exception) {
        this.io.output(exception.ToString());
      }
    }

    private bool buildProgram() {
      IParser parser = new MiniPLParser(new TokenReader(ScannerFactory.createMiniPLScanner(this.sampleProgram)), this.io);
      if(parser.processAndBuildAST()) {
        this.ast = parser.getAST();
        ISemanticAnalyzer analyzer = new MiniPLSemanticAnalyzer();
        if(analyzer.analyze(this.ast, this.symbolTable)) {
          return true;
        } else {
          return false;
        }
      } else {
        return false;
      }
    }
  }

}