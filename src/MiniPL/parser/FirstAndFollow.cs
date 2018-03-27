using System;
using System.Collections.Generic;
using MiniPL.tokens;

namespace MiniPL.parser {

  public class FirstAndFollow {

    private IDictionary<String, ICollection<MiniPLTokenType>> firstMap;

    //private IDictionary<String, ICollection<MiniPLTokenType>> followMap;

    public FirstAndFollow() {
      initializeFirstSet();
      // initializeFollowSet();
    }

    private void initializeFirstSet() {
      firstMap = new Dictionary<String, ICollection<MiniPLTokenType>>();

      ICollection<MiniPLTokenType> statement = new HashSet<MiniPLTokenType>();
      statement.Add(MiniPLTokenType.KEYWORD_VAR);
      statement.Add(MiniPLTokenType.IDENTIFIER);
      statement.Add(MiniPLTokenType.KEYWORD_FOR);
      statement.Add(MiniPLTokenType.KEYWORD_READ);
      statement.Add(MiniPLTokenType.KEYWORD_PRINT);
      statement.Add(MiniPLTokenType.KEYWORD_ASSERT);
      firstMap.Add("statement", statement);

      ICollection<MiniPLTokenType> operation = new HashSet<MiniPLTokenType>();
      operation.Add(MiniPLTokenType.PLUS);
      operation.Add(MiniPLTokenType.MINUS);
      operation.Add(MiniPLTokenType.ASTERISK);
      operation.Add(MiniPLTokenType.SLASH);
      operation.Add(MiniPLTokenType.LOGICAL_AND);
      operation.Add(MiniPLTokenType.LOGICAL_NOT);
      operation.Add(MiniPLTokenType.EQUALITY_COMPARISON);
      operation.Add(MiniPLTokenType.LESS_THAN_COMPARISON);
      firstMap.Add("operation", operation);

      ICollection<MiniPLTokenType> var_declaration = new HashSet<MiniPLTokenType>();
      var_declaration.Add(MiniPLTokenType.KEYWORD_VAR);
      firstMap.Add("var_declaration", var_declaration);
      
      ICollection<MiniPLTokenType> var_assignment = new HashSet<MiniPLTokenType>();
      var_assignment.Add(MiniPLTokenType.IDENTIFIER);
      firstMap.Add("var_assignment", var_assignment);

      ICollection<MiniPLTokenType> for_loop = new HashSet<MiniPLTokenType>();
      for_loop.Add(MiniPLTokenType.KEYWORD_FOR);
      firstMap.Add("for", for_loop);
      
      ICollection<MiniPLTokenType> read = new HashSet<MiniPLTokenType>();
      read.Add(MiniPLTokenType.KEYWORD_READ);
      firstMap.Add("read", read);

      ICollection<MiniPLTokenType> print = new HashSet<MiniPLTokenType>();
      print.Add(MiniPLTokenType.KEYWORD_PRINT);
      firstMap.Add("print", print);

      ICollection<MiniPLTokenType> assert = new HashSet<MiniPLTokenType>();
      assert.Add(MiniPLTokenType.KEYWORD_ASSERT);
      firstMap.Add("assert", assert);
    }

    public ICollection<MiniPLTokenType> first(string rule) {
      ICollection<MiniPLTokenType> firstSet = new HashSet<MiniPLTokenType>();
      if(firstMap.ContainsKey(rule)) {
        return firstMap[rule];
      } else {
        return new HashSet<MiniPLTokenType>();
      }
    }

  }

}