using System;
using MiniPL.exceptions;
using MiniPL.logger;
using MiniPL.parser;
using MiniPL.parser.AST;
using MiniPL.scanner;
using MiniPL.semantics;
using MiniPL.semantics.visitor;

namespace MiniPL.interpreter
{

  public class MiniPLInterpreter : IInterpreter {

    private string sampleProgram;

    private ILogger logger;
    
    private IAST ast;

    private ISymbolTable symbolTable;

    public MiniPLInterpreter(string sampleProgram, ISymbolTable symbolTable, ILogger logger) {
      this.sampleProgram = sampleProgram;
      this.logger = logger;
      this.ast = null;
      this.symbolTable = symbolTable;
    }

    public void interpret() {
      buildProgram();
    }

    private void buildProgram() {
      IParser parser = new MiniPLParser(new TokenReader(ScannerFactory.createMiniPLScanner(this.sampleProgram)), this.logger);
      try {
        parser.processAndBuildAST();
        this.ast = parser.getAST();
        ISemanticAnalyzer analyzer = new MiniPLSemanticAnalyzer();
        if(analyzer.analyze(this.ast, this.symbolTable)) {
          ProgramNode program = (ProgramNode)this.ast.getProgram();
          INodeVisitor interpreterVisitor = new InterpreterVisitor(this.symbolTable, this.logger);
          program.accept(interpreterVisitor);
        }
      } catch(MiniPLException exception) {
        this.logger.log(exception.getMessage());
      } catch(SemanticException exception) {
        this.logger.log(exception.ToString());
      }
    }
  }

}