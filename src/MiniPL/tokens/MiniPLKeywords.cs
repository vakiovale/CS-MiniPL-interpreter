using System;
using System.Text;
using System.Collections.Generic;

namespace MiniPL.tokens {

  public class MiniPLKeywords {

    private IDictionary<String, MiniPLTokenType> keywords;

    public MiniPLKeywords() {
      initializeKeywords();
    }

    public bool containsKey(String key) {
      return this.keywords.ContainsKey(key);
    }

    public MiniPLTokenType get(String keyword) {
      return this.keywords[keyword];
    }

    private void initializeKeywords() {
      keywords = new Dictionary<String, MiniPLTokenType>();
      keywords.Add("var", MiniPLTokenType.KEYWORD_VAR);
      keywords.Add("for", MiniPLTokenType.KEYWORD_FOR);
      keywords.Add("end", MiniPLTokenType.KEYWORD_END);
      keywords.Add("in", MiniPLTokenType.KEYWORD_IN);
      keywords.Add("do", MiniPLTokenType.KEYWORD_DO);
      keywords.Add("read", MiniPLTokenType.KEYWORD_READ);
      keywords.Add("print", MiniPLTokenType.KEYWORD_PRINT);
      keywords.Add("assert", MiniPLTokenType.KEYWORD_ASSERT);
      keywords.Add("int", MiniPLTokenType.TYPE_IDENTIFIER_INTEGER);
      keywords.Add("string", MiniPLTokenType.TYPE_IDENTIFIER_STRING);
      keywords.Add("bool", MiniPLTokenType.TYPE_IDENTIFIER_BOOL);
    }
        
  }
}