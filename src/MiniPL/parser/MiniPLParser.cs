using System;
using MiniPL.scanner;
using MiniPL.tokens;
using System.Collections.Generic;
using MiniPL.exceptions;
using MiniPL.syntax;
using MiniPL.logger;
using MiniPL.parser.AST;
using MiniPL.semantics;

namespace MiniPL.parser {

  public class MiniPLParser : IParser {

    private ILogger logger;

    private TokenReader tokenReader;

    private TokenMatcher tokenMatcher;

    private FirstAndFollow firstAndFollow;

    private RecoveryHandler recoveryHandler;

    private NodeCreator nodeCreator;

    private IAST ast;

    private bool syntaxOk;

    public MiniPLParser(TokenReader tokenReader, ILogger logger) {
      initializeLogger(logger);
      initializeTokenMatcher();
      initializeTokenReader(tokenReader);
      intializeFirstAndFollow();
      initializeRecoveryHandler();
      initializeSyntaxFlag();
      initializeNodeCreator();
      initializeAST();
    }

    private void initializeLogger(ILogger logger) {
      this.logger = logger;
    }

    private void initializeSyntaxFlag() {
      this.syntaxOk = true;
    }

    private void initializeTokenMatcher() {
      this.tokenMatcher = new TokenMatcher();
    }
    
    private void initializeTokenReader(TokenReader reader) {
      this.tokenReader = reader;
    }

    private void intializeFirstAndFollow() {
      this.firstAndFollow = new FirstAndFollow();
    }

    private void initializeRecoveryHandler() {
      this.recoveryHandler = new RecoveryHandler(this.tokenReader, readToken, this.firstAndFollow);
    }

    private void initializeNodeCreator() {
      this.nodeCreator = new NodeCreator();
    }

    private void initializeAST() {
      this.ast = new MiniPLAST();
    }

    private void readToken() {
      Token<MiniPLTokenType> token = tokenReader.readToken();
      if(token != null && token.getType() == MiniPLTokenType.INVALID_TOKEN) {
        lexicalError("Invalid token '" + token.getLexeme() + "'.");
      }
      this.tokenMatcher.setToken(token);
    }

    /**
     * Recognizes the program and builds an AST.
     * Returns true if syntax is ok, false otherwise. 
     */
    public bool processAndBuildAST() {
      readToken();
      ast.addProgramNode(doProgramProcedure());
      return syntaxOk;
    }

    /**
     * Returns a valid AST (valid by syntax).
     * Returns null, if program has not been processed or if there
     * are lexical or syntax errors.
     */
    public IAST getAST() {
      if(this.syntaxOk) {
        return this.ast;
      } else {
        return null;
      }
    }

    private void exceptionRecovery(MiniPLException exception, MiniPLSymbol symbol, Func<INode> procedureMethod) {
      logError(exception);
      failSyntax();
      recover(symbol, procedureMethod);
    }

    private void logError(MiniPLException exception) {
      logger.log(exception.getMessage());
    }

    private void failSyntax() {
      syntaxOk = false;
    }

    private void recover(MiniPLSymbol symbol, Func<INode> procedureMethod) {
      recoveryHandler.tryToRecoverFromException(symbol, procedureMethod);
    }

    private INode doProgramProcedure() {
      INode programNode = makeNode(MiniPLSymbol.PROGRAM);
      try {
        programNode.addNode(doStatementListProcedure());
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.PROGRAM, doProgramProcedure);
      }  
      return programNode;
    }

    private INode doStatementListProcedure() {
      INode statementList = makeNode(MiniPLSymbol.STATEMENT_LIST); 
      try {
        statementList.addNode(doStatementProcedure());
        while(peekType(firstAndFollow.first(MiniPLSymbol.STATEMENT))) {
          readToken();
          statementList.addNode(doStatementProcedure());
        }
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.STATEMENT_LIST, doStatementListProcedure); 
      }
      return statementList;
    }

    private INode doStatementProcedure() {
      INode statement = null;
      try {
        if(isTokenInFirst(MiniPLSymbol.VAR_DECLARATION)) {
          statement = doVarDeclarationProcedure();
        } else if(isTokenInFirst(MiniPLSymbol.VAR_ASSIGNMENT)) {
          statement = doVarAssignmentProcedure();
        } else if(isTokenInFirst(MiniPLSymbol.FOR_LOOP)) {
          statement = doForProcedure(); 
        } else if(isTokenInFirst(MiniPLSymbol.READ_PROCEDURE)) {
          statement = doReadProcedure();
        } else if(isTokenInFirst(MiniPLSymbol.PRINT_PROCEDURE)) {
          statement = doPrintProcedure();        
        } else if(isTokenInFirst(MiniPLSymbol.ASSERT_PROCEDURE)) {
          statement = doAssertProcedure();
        } else {
          syntaxError("Illegal start of a statement. " + (tokenReader.token() != null ? "A statement can't begin with '" + tokenReader.token().getLexeme() + "'." : ""));
        }
        readToken();
        tokenMatcher.matchSemicolon();
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.STATEMENT, doStatementProcedure);
      }
      return statement;
    }

    private INode doReadProcedure() {
      INode read = makeNode(MiniPLSymbol.READ_PROCEDURE);
      try {
        tokenMatcher.matchRead();
        readToken();
        tokenMatcher.matchIdentifier();
        read.addNode(makeNode(this.tokenReader.token()));
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.READ_PROCEDURE, doReadProcedure);
      }
      return read;
    }

    private INode doPrintProcedure() {
      INode print = makeNode(MiniPLSymbol.PRINT_PROCEDURE);
      try {
        tokenMatcher.matchPrint();
        readToken();
        print.addNode(doExpressionProcedure());
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.PRINT_PROCEDURE, doPrintProcedure);
      }
      return print;
    }

    private INode doAssertProcedure() {
      INode assert = makeNode(MiniPLSymbol.ASSERT_PROCEDURE);
      try {
        tokenMatcher.matchAssert();
        readToken();
        tokenMatcher.matchLeftParenthesis();
        readToken();
        assert.addNode(doExpressionProcedure());
        readToken();
        tokenMatcher.matchRightParenthesis();
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.ASSERT_PROCEDURE, doAssertProcedure);
      }
      return assert;
    }

    private INode doForProcedure() {
      INode forLoop = makeNode(MiniPLSymbol.FOR_LOOP);
      try {
        tokenMatcher.matchFor();
        readToken();
        tokenMatcher.matchIdentifier();
        INode identifier = makeNode(this.tokenReader.token());
        readToken();
        tokenMatcher.matchIn();
        readToken();
        INode leftHandSide = doExpressionProcedure();
        readToken();
        tokenMatcher.matchRange();
        INode range = makeNode(this.tokenReader.token());
        readToken();
        INode rightHandSide = doExpressionProcedure();
        readToken();
        tokenMatcher.matchDo();
        readToken();
        INode statementList = doStatementListProcedure();
        readToken();
        tokenMatcher.matchEnd();
        readToken();
        tokenMatcher.matchFor();
        addToNode(range, leftHandSide, rightHandSide);
        forLoop.addNode(identifier);
        forLoop.addNode(range);
        forLoop.addNode(statementList);
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.FOR_LOOP, doForProcedure);
      }
      return forLoop;
    }

    private INode doVarAssignmentProcedure() {
      INode varAssignment = makeNode(MiniPLSymbol.VAR_ASSIGNMENT);
      try {
        tokenMatcher.matchIdentifier();
        INode leftHandSide = makeNode(this.tokenReader.token());
        readToken();
        tokenMatcher.matchAssignment();
        readToken();
        INode rightHandSide = doExpressionProcedure();
        addToNode(varAssignment, leftHandSide, rightHandSide);
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.VAR_ASSIGNMENT, doVarAssignmentProcedure);
      }
      return varAssignment;
    }

    private INode doVarDeclarationProcedure() {
      INode varDeclaration = makeNode(MiniPLSymbol.VAR_DECLARATION);
      try {
        tokenMatcher.matchVar();
        readToken();
        tokenMatcher.matchIdentifier();
        varDeclaration.addNode(makeNode(this.tokenReader.token()));
        readToken();
        tokenMatcher.matchColon();
        readToken();
        INode type = doTypeProcedure();
        if(tokenReader.isNextTokensType(MiniPLTokenType.ASSIGNMENT_OPERATOR)) {
          readToken();
          tokenMatcher.matchAssignment();
          readToken();
          type.addNode(doExpressionProcedure());
        }
        varDeclaration.addNode(type);
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.VAR_DECLARATION, doVarDeclarationProcedure);
      }
      return varDeclaration;
    }

    private INode doTypeProcedure() {
      try {
        if(isTokenInFirst(MiniPLSymbol.TYPE)) {
          return makeNode(this.tokenReader.token());
        }
        syntaxError("Expected a type (int, string, bool).");
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.TYPE, doTypeProcedure);
      }
      return null;
    }

    private INode doExpressionProcedure() {
      INode expression = makeNode(MiniPLSymbol.EXPRESSION);
      try {
        if(tokenMatcher.isTokenType(MiniPLTokenType.LOGICAL_NOT)) {
          readToken();
          INode notNode = makeNode(this.tokenReader.token());
          notNode.addNode(doOperandProcedure());
          expression.addNode(notNode);
          return expression;
        } 
        INode leftHandSide = doOperandProcedure();
        if(peekType(first(MiniPLSymbol.OPERATION))) {
          readToken();
          INode operation = doOperationProcedure();
          readToken();
          INode rightHandSide = doOperandProcedure();
          addToNode(operation, leftHandSide, rightHandSide);
          expression.addNode(operation);
          return expression;
        } else {
          expression.addNode(leftHandSide);
        }
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.EXPRESSION, doExpressionProcedure);
      }
      return expression;
    }

    private INode doOperandProcedure() {
      try {
        if(isTokenInFirst(MiniPLSymbol.OPERAND)) {
          if(tokenMatcher.isTokenType(MiniPLTokenType.INTEGER_LITERAL) || 
             tokenMatcher.isTokenType(MiniPLTokenType.STRING_LITERAL) || 
             tokenMatcher.isTokenType(MiniPLTokenType.IDENTIFIER)) {
            return makeNode(tokenReader.token());
          } 
          tokenMatcher.matchLeftParenthesis();
          readToken();
          INode expression = doExpressionProcedure();
          readToken();
          tokenMatcher.matchRightParenthesis();
          return expression;
        }
        syntaxError("Illegal start of an expression. Expected a valid operand.");
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.OPERAND, doOperandProcedure);
      }
      return null;
    }

    private INode doOperationProcedure() {
      try {
        if(isTokenInFirst(MiniPLSymbol.OPERATION)) {
          return makeNode(this.tokenReader.token());
        }
        throw new SyntaxException("Expected an operation.", tokenReader.token());
      } catch(MiniPLException exception) {
        exceptionRecovery(exception, MiniPLSymbol.OPERATION, doOperationProcedure);
      }
      return null;
    }

    private bool isTokenInFirst(MiniPLSymbol symbol) {
      if(tokenReader.token() != null) {
        return firstAndFollow.firstContains(symbol, tokenReader.token().getType()); 
      } else {
        return false;
      }
    }

    private bool peekType(ICollection<MiniPLTokenType> set) {
      if(tokenReader.hasNextToken()) {
        if(tokenReader.getNextTokensType().Equals(MiniPLTokenType.INVALID_TOKEN)) {
          readToken();
          lexicalError("Invalid token '" + this.tokenReader.token().getLexeme() + "'.");
        } else {
          return set.Contains(tokenReader.getNextTokensType());
        }
      }
      return false;
    }

    private INode addToNode(INode root, INode leftHandSide, INode rightHandSide) {
      root.addNode(leftHandSide);
      root.addNode(rightHandSide);
      return root;
    }

    private INode makeNode(MiniPLSymbol symbol) {
      return this.nodeCreator.makeNode(symbol);
    }

    private INode makeNode(Token<MiniPLTokenType> token) {
      return this.nodeCreator.makeNode(token);
    }

    private void syntaxError(string message) {
      throw new SyntaxException(message, tokenReader.token());
    }

    private void lexicalError(string message) {
      throw new LexicalException(message, tokenReader.token());
    }

    private ICollection<MiniPLTokenType> first(MiniPLSymbol symbol) {
      return firstAndFollow.first(symbol);
    }
  }
}