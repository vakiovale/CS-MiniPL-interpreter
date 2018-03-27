using System;
using System.Collections.Generic;
using MiniPL.syntax;
using MiniPL.tokens;

namespace MiniPL.parser {

  public class FirstAndFollow {

    private IDictionary<MiniPLSymbols, ICollection<MiniPLTokenType>> firstMap;

    private IDictionary<MiniPLSymbols, ICollection<MiniPLTokenType>> followMap;

    public FirstAndFollow() {
      initializeFirstSet();
      initializeFollowSet();
    }

    public bool followContains(MiniPLSymbols symbol, MiniPLTokenType tokenType) {
      if(followMap.ContainsKey(symbol)) {
        return followMap[symbol].Contains(tokenType);
      } else {
        return false;
      }
    }

    public bool firstContains(MiniPLSymbols symbol, MiniPLTokenType tokenType) {
      if(firstMap.ContainsKey(symbol)) {
        return firstMap[symbol].Contains(tokenType);
      } else {
        return false;
      }
    }

    public ICollection<MiniPLTokenType> first(MiniPLSymbols symbol) {
      if(firstMap.ContainsKey(symbol)) {
        return firstMap[symbol];
      } else {
        return new HashSet<MiniPLTokenType>();
      }
    }

    private void initializeFirstSet() {
      firstMap = new Dictionary<MiniPLSymbols, ICollection<MiniPLTokenType>>();

      ICollection<MiniPLTokenType> type = new HashSet<MiniPLTokenType>();
      type.Add(MiniPLTokenType.TYPE_IDENTIFIER_INTEGER);
      type.Add(MiniPLTokenType.TYPE_IDENTIFIER_STRING);
      type.Add(MiniPLTokenType.TYPE_IDENTIFIER_BOOL);
      firstMap.Add(MiniPLSymbols.TYPE, type);

      ICollection<MiniPLTokenType> operation = new HashSet<MiniPLTokenType>();
      operation.Add(MiniPLTokenType.PLUS);
      operation.Add(MiniPLTokenType.MINUS);
      operation.Add(MiniPLTokenType.ASTERISK);
      operation.Add(MiniPLTokenType.SLASH);
      operation.Add(MiniPLTokenType.LOGICAL_AND);
      operation.Add(MiniPLTokenType.LOGICAL_NOT);
      operation.Add(MiniPLTokenType.EQUALITY_COMPARISON);
      operation.Add(MiniPLTokenType.LESS_THAN_COMPARISON);
      firstMap.Add(MiniPLSymbols.OPERATION, operation);

      ICollection<MiniPLTokenType> operand = new HashSet<MiniPLTokenType>();
      operand.Add(MiniPLTokenType.INTEGER_LITERAL);
      operand.Add(MiniPLTokenType.STRING_LITERAL);
      operand.Add(MiniPLTokenType.IDENTIFIER);
      operand.Add(MiniPLTokenType.LEFT_PARENTHESIS);
      firstMap.Add(MiniPLSymbols.OPERAND, operand);

      ICollection<MiniPLTokenType> expression = new HashSet<MiniPLTokenType>();
      expression = union(firstMap[MiniPLSymbols.OPERAND], expression);
      expression.Add(MiniPLTokenType.LOGICAL_NOT);
      expression.Add(MiniPLTokenType.LOGICAL_NOT);
      firstMap.Add(MiniPLSymbols.EXPRESSION, expression);

      ICollection<MiniPLTokenType> statement = new HashSet<MiniPLTokenType>();
      statement.Add(MiniPLTokenType.KEYWORD_VAR);
      statement.Add(MiniPLTokenType.IDENTIFIER);
      statement.Add(MiniPLTokenType.KEYWORD_FOR);
      statement.Add(MiniPLTokenType.KEYWORD_READ);
      statement.Add(MiniPLTokenType.KEYWORD_PRINT);
      statement.Add(MiniPLTokenType.KEYWORD_ASSERT);
      firstMap.Add(MiniPLSymbols.STATEMENT, statement);

      ICollection<MiniPLTokenType> statementList = new HashSet<MiniPLTokenType>();
      statementList = union(firstMap[MiniPLSymbols.STATEMENT], statementList);
      firstMap.Add(MiniPLSymbols.STATEMENT_LIST, statement);

      ICollection<MiniPLTokenType> program = new HashSet<MiniPLTokenType>();
      program = union(firstMap[MiniPLSymbols.STATEMENT_LIST], program);
      firstMap.Add(MiniPLSymbols.PROGRAM, program);

      ICollection<MiniPLTokenType> var_declaration = new HashSet<MiniPLTokenType>();
      var_declaration.Add(MiniPLTokenType.KEYWORD_VAR);
      firstMap.Add(MiniPLSymbols.VAR_DECLARATION, var_declaration);
      
      ICollection<MiniPLTokenType> var_assignment = new HashSet<MiniPLTokenType>();
      var_assignment.Add(MiniPLTokenType.IDENTIFIER);
      firstMap.Add(MiniPLSymbols.VAR_ASSIGNMENT, var_assignment);

      ICollection<MiniPLTokenType> for_loop = new HashSet<MiniPLTokenType>();
      for_loop.Add(MiniPLTokenType.KEYWORD_FOR);
      firstMap.Add(MiniPLSymbols.FOR_LOOP, for_loop);
      
      ICollection<MiniPLTokenType> read = new HashSet<MiniPLTokenType>();
      read.Add(MiniPLTokenType.KEYWORD_READ);
      firstMap.Add(MiniPLSymbols.READ_PROCEDURE, read);

      ICollection<MiniPLTokenType> print = new HashSet<MiniPLTokenType>();
      print.Add(MiniPLTokenType.KEYWORD_PRINT);
      firstMap.Add(MiniPLSymbols.PRINT_PROCEDURE, print);

      ICollection<MiniPLTokenType> assert = new HashSet<MiniPLTokenType>();
      assert.Add(MiniPLTokenType.KEYWORD_ASSERT);
      firstMap.Add(MiniPLSymbols.ASSERT_PROCEDURE, assert);
    }

    private void initializeFollowSet() {
      followMap = new Dictionary<MiniPLSymbols, ICollection<MiniPLTokenType>>();

      ICollection<MiniPLTokenType> type = new HashSet<MiniPLTokenType>();
      type.Add(MiniPLTokenType.SEMICOLON);
      type.Add(MiniPLTokenType.ASSIGNMENT_OPERATOR);
      followMap.Add(MiniPLSymbols.TYPE, type);

      ICollection<MiniPLTokenType> operation = new HashSet<MiniPLTokenType>();
      operation = union(firstMap[MiniPLSymbols.OPERAND], operation);
      followMap.Add(MiniPLSymbols.OPERATION, operation);

      ICollection<MiniPLTokenType> operand = new HashSet<MiniPLTokenType>();
      operand.Add(MiniPLTokenType.SEMICOLON);
      operand = union(firstMap[MiniPLSymbols.OPERATION], operand);
      followMap.Add(MiniPLSymbols.OPERAND, operand);

      ICollection<MiniPLTokenType> expression = new HashSet<MiniPLTokenType>();
      expression.Add(MiniPLTokenType.SEMICOLON);
      expression.Add(MiniPLTokenType.RANGE_OPERATOR);
      expression.Add(MiniPLTokenType.KEYWORD_DO);
      expression.Add(MiniPLTokenType.RIGHT_PARENTHESIS);
      followMap.Add(MiniPLSymbols.EXPRESSION, expression);

      ICollection<MiniPLTokenType> statement = new HashSet<MiniPLTokenType>();
      statement.Add(MiniPLTokenType.SEMICOLON);
      followMap.Add(MiniPLSymbols.STATEMENT, statement);

      ICollection<MiniPLTokenType> var_declaration = new HashSet<MiniPLTokenType>();
      var_declaration.Add(MiniPLTokenType.SEMICOLON);
      followMap.Add(MiniPLSymbols.VAR_DECLARATION, var_declaration);
      
      ICollection<MiniPLTokenType> var_assignment = new HashSet<MiniPLTokenType>();
      var_assignment.Add(MiniPLTokenType.SEMICOLON);
      followMap.Add(MiniPLSymbols.VAR_ASSIGNMENT, var_assignment);

      ICollection<MiniPLTokenType> for_loop = new HashSet<MiniPLTokenType>();
      for_loop.Add(MiniPLTokenType.SEMICOLON);
      followMap.Add(MiniPLSymbols.FOR_LOOP, for_loop);
      
      ICollection<MiniPLTokenType> read = new HashSet<MiniPLTokenType>();
      read.Add(MiniPLTokenType.SEMICOLON);
      followMap.Add(MiniPLSymbols.READ_PROCEDURE, read);

      ICollection<MiniPLTokenType> print = new HashSet<MiniPLTokenType>();
      print.Add(MiniPLTokenType.SEMICOLON);
      followMap.Add(MiniPLSymbols.PRINT_PROCEDURE, print);

      ICollection<MiniPLTokenType> assert = new HashSet<MiniPLTokenType>();
      assert.Add(MiniPLTokenType.SEMICOLON);
      followMap.Add(MiniPLSymbols.ASSERT_PROCEDURE, assert);

      ICollection<MiniPLTokenType> statementList = new HashSet<MiniPLTokenType>();
      statementList.Add(MiniPLTokenType.KEYWORD_END);
      followMap.Add(MiniPLSymbols.STATEMENT_LIST, statementList);

      ICollection<MiniPLTokenType> program = new HashSet<MiniPLTokenType>();
      followMap.Add(MiniPLSymbols.PROGRAM, program);
    }

    private ICollection<MiniPLTokenType> union(ICollection<MiniPLTokenType> collection, ICollection<MiniPLTokenType> otherCollection) {
      ICollection<MiniPLTokenType> newCollection = new HashSet<MiniPLTokenType>();
      if(collection != null) {
        addTo(collection, newCollection);
      }
      if(newCollection != null) {
        addTo(otherCollection, newCollection);
      }
      return newCollection;
    }

    private void addTo(ICollection<MiniPLTokenType> collection, ICollection<MiniPLTokenType> newCollection) {
      foreach(MiniPLTokenType element in collection) {
        if(!newCollection.Contains(element)) {
          newCollection.Add(element);
        }
      }
    }

  }

}