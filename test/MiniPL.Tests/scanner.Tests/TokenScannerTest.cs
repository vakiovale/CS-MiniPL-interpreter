using System;
using Xunit;
using MiniPL.scanner;
using MiniPL.tokens;

namespace MiniPL.Tests.scanner.Tests {
  
  public class TokenScannerTest { 

    private ITokenScanner<MiniPLTokenType> tokenScanner;

    private IScanner scanner;

    public TokenScannerTest() {
      String sampleProgram = "var nTimes : int := 0;\n"
                           + "print \"How many times?\";\n"
                           + "read nTimes;\n"
                           + "var x : int;\n"
                           + "for x in 0..nTimes-1 do\n"
                           + "\tprint x;\n"
                           + "\tprint \" : Hello, World!\\n\";\n"
                           + "end for;\n"
                           + "assert (x = nTimes);";
      this.scanner = new Scanner(sampleProgram);
      this.tokenScanner = new MiniPLTokenScanner(this.scanner);
    }

    [Fact]
    public void tokenScannerExists() {
      Assert.True(this.tokenScanner != null);
    }

    [Fact]
    public void readingEmptySourceShouldReturnNullToken() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner(""));
      Assert.True(this.tokenScanner.isEndOfSource());
      Assert.True(this.tokenScanner.readNextToken() == null);
    }

    [Fact]
    public void ifAllCharactersHaveBeenReadScannerShouldInform() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner(";"));
      Assert.True(!this.tokenScanner.isEndOfSource());
      this.tokenScanner.readNextToken();
      Assert.True(this.tokenScanner.isEndOfSource());
    }

    [Theory]
    [InlineData("\n")]
    [InlineData(" \n")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\t \t")]
    [InlineData("\n \n")]
    public void readingSourceWithOnlyWhitespaceShouldReturnNullToken(String source) {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner(source));
      Assert.True(this.tokenScanner.readNextToken() == null);
    }

    [Theory]
    [InlineData(";", MiniPLTokenType.SEMICOLON)]
    [InlineData("=", MiniPLTokenType.EQUALITY_COMPARISON)]
    [InlineData("+", MiniPLTokenType.PLUS)]
    [InlineData("-", MiniPLTokenType.MINUS)]
    [InlineData("*", MiniPLTokenType.ASTERISK)]
    [InlineData("/", MiniPLTokenType.SLASH)]
    [InlineData("&", MiniPLTokenType.LOGICAL_AND)]
    [InlineData("!", MiniPLTokenType.LOGICAL_NOT)]
    [InlineData("<", MiniPLTokenType.LESS_THAN_COMPARISON)]
    [InlineData("(", MiniPLTokenType.LEFT_PARENTHESIS)]
    [InlineData(")", MiniPLTokenType.RIGHT_PARENTHESIS)]
    [InlineData("\\", MiniPLTokenType.BACKSLASH)]
    [InlineData(":", MiniPLTokenType.COLON)]
    public void readsSingleCharacterTokenFromSource(String source, MiniPLTokenType type) {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner(source));
      dynamic token = this.tokenScanner.readNextToken();
      Assert.Equal(type, token.getType());
    }
    
    [Fact]
    public void readMultipleSingleCharTokens() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner(";;;"));
      Assert.Equal(MiniPLTokenType.SEMICOLON, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.SEMICOLON, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.SEMICOLON, this.tokenScanner.readNextToken().getType());
    }

    [Fact]
    public void ifNoMoreTokensThenReturnNullToken() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("!"));
      this.tokenScanner.readNextToken();
      dynamic nullToken = this.tokenScanner.readNextToken();
      Assert.True(nullToken == null);
    }

    [Fact]
    public void readMultipleDifferentSingleCharTokensSeparatedWithWhiteSpace() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("\\ !\t+\n\n&"));
      Assert.Equal(MiniPLTokenType.BACKSLASH, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.LOGICAL_NOT, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.PLUS, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.LOGICAL_AND, this.tokenScanner.readNextToken().getType());
    }

    [Fact]
    public void readMultipleDifferentSingleCharTokens() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("(!&<=)"));
      Assert.Equal(MiniPLTokenType.LEFT_PARENTHESIS, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.LOGICAL_NOT, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.LOGICAL_AND, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.LESS_THAN_COMPARISON, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.EQUALITY_COMPARISON, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.RIGHT_PARENTHESIS, this.tokenScanner.readNextToken().getType());
    }

    [Fact]
    public void readIdentifierMiddleOfSpecialCharacters() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("(!goodVariable_1)"));
      dynamic leftParenthesis = this.tokenScanner.readNextToken();
      dynamic logicalNot = this.tokenScanner.readNextToken();
      dynamic identifier = this.tokenScanner.readNextToken();
      dynamic rightParenthesis = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.LEFT_PARENTHESIS, leftParenthesis.getType());
      Assert.Equal(MiniPLTokenType.LOGICAL_NOT, logicalNot.getType());
      Assert.Equal(MiniPLTokenType.RIGHT_PARENTHESIS, rightParenthesis.getType());
      Assert.Equal(MiniPLTokenType.IDENTIFIER, identifier.getType());
      Assert.Equal("goodVariable_1", identifier.getLexeme());
    }

    [Theory]
    [InlineData("variable_1")]
    [InlineData("var2")]
    [InlineData("MysticalVariable")]
    [InlineData("A_b_123_assert")]
    [InlineData("variable9_")]
    public void readIdentifierTokenTest(String source) {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner(source));
      dynamic token = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.IDENTIFIER, token.getType());
      Assert.Equal(source, token.getLexeme());
    }

    [Fact]
    public void readIdentifiers() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("token1 token2 token_3"));
      dynamic firstToken = this.tokenScanner.readNextToken();
      dynamic secondToken = this.tokenScanner.readNextToken();
      dynamic thirdToken = this.tokenScanner.readNextToken();
      Assert.Equal("token1", firstToken.getLexeme());
      Assert.Equal("token2", secondToken.getLexeme());
      Assert.Equal("token_3", thirdToken.getLexeme());
    }

    [Theory]
    [InlineData("print \"\"", MiniPLTokenType.KEYWORD_PRINT)]
    [InlineData("for i in 1..n do\n", MiniPLTokenType.KEYWORD_FOR)]
    [InlineData("do\n", MiniPLTokenType.KEYWORD_DO)]
    [InlineData("end\tfor\t;", MiniPLTokenType.KEYWORD_END)]
    public void keywordsCanBeReadWhenThereIsWhitespaceAfterwards(String source, MiniPLTokenType type) {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner(source));
      dynamic token = this.tokenScanner.readNextToken();
      Assert.Equal(type, token.getType());
    }

    [Fact]
    public void readVarKeyword() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("var"));
      dynamic token = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.KEYWORD_VAR, token.getType());
    }

    [Fact]
    public void readForKeyword() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("for"));
      dynamic token = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.KEYWORD_FOR, token.getType());
    }

    [Fact]
    public void readEndKeyword() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("end"));
      dynamic token = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.KEYWORD_END, token.getType());
    }

    [Fact]
    public void readInKeyword() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("in"));
      dynamic token = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.KEYWORD_IN, token.getType());
    }

    [Fact]
    public void readDoKeyword() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("do"));
      dynamic token = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.KEYWORD_DO, token.getType());
    }

    [Fact]
    public void readReadKeyword() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("read"));
      dynamic token = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.KEYWORD_READ, token.getType());
    }

    [Fact]
    public void readPrintKeyword() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("print"));
      dynamic token = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.KEYWORD_PRINT, token.getType());
    }

    [Fact]
    public void readIntKeyword() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("int"));
      dynamic token = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.TYPE_IDENTIFIER_INTEGER, token.getType());
    }

    [Fact]
    public void readStringKeyword() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("string"));
      dynamic token = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.TYPE_IDENTIFIER_STRING, token.getType());
    }

    [Fact]
    public void readBoolKeyword() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("bool"));
      dynamic token = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.TYPE_IDENTIFIER_BOOL, token.getType());
    }

    [Fact]
    public void readAssertKeyword() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("assert"));
      dynamic token = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.KEYWORD_ASSERT, token.getType());
    }

    [Fact]
    public void readKeywordPrintBeforeQuote() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("print\"Hello World!\";"));
      dynamic printToken = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.KEYWORD_PRINT, printToken.getType());
    }

    [Fact]
    public void readKeywordAssertBeforeParenthesis() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("assert(x = 1);"));
      dynamic assertToken = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.KEYWORD_ASSERT, assertToken.getType()); 
    } 

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("100")]
    [InlineData("98")]
    [InlineData("77777")]
    public void readIntegerLiterals(String source) {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner(source));
      dynamic token = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.INTEGER_LITERAL, token.getType());
      Assert.Equal(Convert.ToInt32(source), Convert.ToInt32(token.getLexeme()));
    }

    [Fact]
    public void readTwoIntegerLiteralsSeparatedByWhitespace() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("1 99"));
      dynamic firstToken = this.tokenScanner.readNextToken();
      dynamic secondToken = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.INTEGER_LITERAL, firstToken.getType());
      Assert.Equal(1, Convert.ToInt32(firstToken.getLexeme())); 
      Assert.Equal(MiniPLTokenType.INTEGER_LITERAL, secondToken.getType());
      Assert.Equal(99, Convert.ToInt32(secondToken.getLexeme())); 
    }

    [Theory]
    [InlineData("\"Word\"")]
    [InlineData("\"Hello World!\"")]
    [InlineData("\"how are you?\"")]
    public void readSimpleStringLiterals(String source) {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner(source));
      dynamic stringToken = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.STRING_LITERAL, stringToken.getType());
      Assert.Equal(source.Substring(1, source.Length-2), stringToken.getLexeme());
    }

    [Fact]
    public void readStringLiterals() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("\"Ensimmäinen lause.\"\"print\"\" m e r k k ! \""));
      dynamic firstStringLiteral = this.tokenScanner.readNextToken();
      dynamic secondStringLiteral = this.tokenScanner.readNextToken();
      dynamic thirdStringLiteral = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.STRING_LITERAL, firstStringLiteral.getType());
      Assert.Equal("Ensimmäinen lause.", firstStringLiteral.getLexeme());
      Assert.Equal(MiniPLTokenType.STRING_LITERAL, secondStringLiteral.getType());
      Assert.Equal("print", secondStringLiteral.getLexeme());
      Assert.Equal(MiniPLTokenType.STRING_LITERAL, thirdStringLiteral.getType());
      Assert.Equal(" m e r k k ! ", thirdStringLiteral.getLexeme());
    }

    [Theory]
    [InlineData("_var")]
    [InlineData("12variable")]
    [InlineData("_ThisIs123var&!Illegal!!")]
    [InlineData("^notgood_1")]
    [InlineData("1^Änotgood_1")]
    [InlineData("§½")]
    [InlineData(".")]
    [InlineData(".variable")]
    public void unrecognizedSourceShouldReturnInvalidTokenWithTextAsToken(String source) {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner(source));
      dynamic invalidToken = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.INVALID_TOKEN, invalidToken.getType());
      Assert.Equal(source, invalidToken.getLexeme());
    }

    [Fact]
    public void recognizeCorrectTokensAroundInvalidToken() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("var _ThisIs123var&!Illegal!! ="));
      dynamic varToken = this.tokenScanner.readNextToken();
      dynamic invalidToken = this.tokenScanner.readNextToken();
      dynamic equalityToken = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.KEYWORD_VAR, varToken.getType());
      Assert.Equal(MiniPLTokenType.INVALID_TOKEN, invalidToken.getType());
      Assert.Equal("_ThisIs123var&!Illegal!!", invalidToken.getLexeme());
      Assert.Equal(MiniPLTokenType.EQUALITY_COMPARISON, equalityToken.getType());
    }

    [Fact]
    public void handleEscapeCharacterInsideStringLiteral() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("\"Word\\n\""));
      dynamic stringToken = this.tokenScanner.readNextToken();
      Assert.Equal("Word\\n", stringToken.getLexeme());
    }

    [Fact]
    public void checkCorrectNumberOfEscapeCharactersInStringLiteral() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("\"\\\\\\\""));
      dynamic stringToken = this.tokenScanner.readNextToken();
      Assert.True(stringToken.getLexeme().Length == 3);
      Assert.Equal("\\\\\\", stringToken.getLexeme());
    }

    [Fact]
    public void assignmentOperatorShouldBeRecognized() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner(":="));
      dynamic assignmentToken = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.ASSIGNMENT_OPERATOR, assignmentToken.getType());
    }

    [Fact]
    public void rangeOperatorShouldBeRecognized() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner(".."));
      dynamic rangeOperatorToken = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.RANGE_OPERATOR, rangeOperatorToken.getType());
    }

    [Fact]
    public void rangeOperatorShouldBeRecognizedBetweenStringLiterals() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("\"left\"..\"right\""));
      dynamic leftLiteral = this.tokenScanner.readNextToken();
      dynamic rangeOperator = this.tokenScanner.readNextToken();
      dynamic rightLiteral = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.RANGE_OPERATOR, rangeOperator.getType());
      Assert.Equal(MiniPLTokenType.STRING_LITERAL, leftLiteral.getType());
      Assert.Equal(MiniPLTokenType.STRING_LITERAL, rightLiteral.getType());
    }

    [Fact]
    public void rangeOperatorShouldBeRecognizedBetweenIntegers() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("0..10"));
      dynamic leftNumber = this.tokenScanner.readNextToken();
      dynamic rangeOperator = this.tokenScanner.readNextToken();
      dynamic rightNumber = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.RANGE_OPERATOR, rangeOperator.getType());
      Assert.Equal(MiniPLTokenType.INTEGER_LITERAL, leftNumber.getType());
      Assert.Equal(MiniPLTokenType.INTEGER_LITERAL, rightNumber.getType());
    }

    [Fact]
    public void rangeOperatorShouldBeRecognizedBetweenVariables() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("start_..end_"));
      dynamic left = this.tokenScanner.readNextToken();
      dynamic rangeOperator = this.tokenScanner.readNextToken();
      dynamic right = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.RANGE_OPERATOR, rangeOperator.getType());
      Assert.Equal(MiniPLTokenType.IDENTIFIER, left.getType());
      Assert.Equal("start_", left.getLexeme());
      Assert.Equal(MiniPLTokenType.IDENTIFIER, right.getType());
      Assert.Equal("end_", right.getLexeme());
    }

    [Fact]
    public void commentSectionShouldBeSkippedToTheEndOfTheLine() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner("var_;//This comment should be ignored var := \\n ** . ..\nvar x;"));
      dynamic firstIdentifier = this.tokenScanner.readNextToken();
      dynamic firstSemicolon = this.tokenScanner.readNextToken();
      dynamic varKeyword = this.tokenScanner.readNextToken();
      dynamic secondIdentifier = this.tokenScanner.readNextToken();
      dynamic secondSemicolon = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.IDENTIFIER, firstIdentifier.getType());
      Assert.Equal(MiniPLTokenType.SEMICOLON, firstSemicolon.getType());
      Assert.Equal(MiniPLTokenType.KEYWORD_VAR, varKeyword.getType());
      Assert.Equal(MiniPLTokenType.IDENTIFIER, secondIdentifier.getType());
      Assert.Equal(MiniPLTokenType.SEMICOLON, secondSemicolon.getType());
    }

    [Fact]
    public void complexSingleLineCommentTest() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner(";\n // This is just a comment\n;"));
      dynamic firstSemicolon = this.tokenScanner.readNextToken();
      dynamic lastSemicolon = this.tokenScanner.readNextToken();
      Assert.True(this.tokenScanner.isEndOfSource());
      Assert.Equal(MiniPLTokenType.SEMICOLON, firstSemicolon.getType());
      Assert.Equal(MiniPLTokenType.SEMICOLON, lastSemicolon.getType());
    }

    [Fact]
    public void sourceCanEndInComment() {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner(";\n // This is just a comment"));
      Assert.False(this.tokenScanner.isEndOfSource());
      dynamic lastToken = this.tokenScanner.readNextToken();
      dynamic shouldBeNull = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.SEMICOLON, lastToken.getType());
      Assert.True(shouldBeNull == null);
      Assert.True(this.tokenScanner.isEndOfSource());
    }

    [Theory]
    [InlineData("a/* Multi line comment */b")]
    [InlineData("a /* Multi line comment */ b")]
    [InlineData("a/*/* Multi line comment */*/b")]
    [InlineData("a/** Multi line comment *\\ */b")]
    [InlineData("a/*\n\n/*\n Multi line commen\nt\n */\n*/\n\tb")]
    [InlineData("a\n/*/*\n Multi /*l*/in\n/*e co\nm***/men\nt\n */\n*/\n\tb")]
    public void multiLineCommentsShouldBeReadCorrectly(String source) {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner(source));
      dynamic firstIdentifier = this.tokenScanner.readNextToken();
      dynamic secondIdentifier = this.tokenScanner.readNextToken();
      Assert.Equal("a", firstIdentifier.getLexeme());
      Assert.Equal(MiniPLTokenType.IDENTIFIER, firstIdentifier.getType());
      Assert.Equal("b", secondIdentifier.getLexeme());
      Assert.Equal(MiniPLTokenType.IDENTIFIER, secondIdentifier.getType());
    }

    [Theory]
    [InlineData("v := (j + v) * x * i;")]
    [InlineData("v:=(j+v)*x*i;")]
    [InlineData("v := ( j + v ) * x * i;")]
    [InlineData("v:=( j +v) *x* i;")]
    [InlineData("v:=( j+ v)* x *i;")]
    [InlineData("v \t\n\n:= (\nj +\t v)\n\t\n *\n x *\t i;")]
    public void shouldBeAbleToReadComplexExpressionWithOrWithoutWhitespace(String source) {
      this.tokenScanner = new MiniPLTokenScanner(new Scanner(source));
      dynamic firstVIdentifier = this.tokenScanner.readNextToken();
      dynamic assignmentOperator = this.tokenScanner.readNextToken();
      dynamic leftParenthesis = this.tokenScanner.readNextToken();
      dynamic jIdentifier = this.tokenScanner.readNextToken();
      dynamic plusOperator = this.tokenScanner.readNextToken();
      dynamic lastVIdentifier = this.tokenScanner.readNextToken();
      dynamic rightParenthesis = this.tokenScanner.readNextToken();
      dynamic firstMultiplication = this.tokenScanner.readNextToken();
      dynamic xIdentifier = this.tokenScanner.readNextToken();
      dynamic secondMultiplication = this.tokenScanner.readNextToken();
      dynamic iIdentifier = this.tokenScanner.readNextToken();
      dynamic semicolon = this.tokenScanner.readNextToken();
      Assert.Equal(MiniPLTokenType.IDENTIFIER, firstVIdentifier.getType());
      Assert.Equal(MiniPLTokenType.ASSIGNMENT_OPERATOR, assignmentOperator.getType());
      Assert.Equal(MiniPLTokenType.LEFT_PARENTHESIS, leftParenthesis.getType());
      Assert.Equal(MiniPLTokenType.IDENTIFIER, jIdentifier.getType());
      Assert.Equal(MiniPLTokenType.PLUS, plusOperator.getType());
      Assert.Equal(MiniPLTokenType.IDENTIFIER, lastVIdentifier.getType());
      Assert.Equal(MiniPLTokenType.RIGHT_PARENTHESIS, rightParenthesis.getType());
      Assert.Equal(MiniPLTokenType.ASTERISK, firstMultiplication.getType());
      Assert.Equal(MiniPLTokenType.IDENTIFIER, xIdentifier.getType());
      Assert.Equal(MiniPLTokenType.ASTERISK, secondMultiplication.getType());
      Assert.Equal(MiniPLTokenType.IDENTIFIER, iIdentifier.getType());
      Assert.Equal(MiniPLTokenType.SEMICOLON, semicolon.getType());
    }

    [Fact]
    public void shouldReadCorrectTokensFromSimpleTestProgramSource() {
      Assert.Equal(MiniPLTokenType.KEYWORD_VAR, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.IDENTIFIER, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.COLON, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.TYPE_IDENTIFIER_INTEGER, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.ASSIGNMENT_OPERATOR, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.INTEGER_LITERAL, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.SEMICOLON, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.KEYWORD_PRINT, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.STRING_LITERAL, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.SEMICOLON, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.KEYWORD_READ, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.IDENTIFIER, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.SEMICOLON, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.KEYWORD_VAR, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.IDENTIFIER, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.COLON, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.TYPE_IDENTIFIER_INTEGER, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.SEMICOLON, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.KEYWORD_FOR, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.IDENTIFIER, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.KEYWORD_IN, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.INTEGER_LITERAL, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.RANGE_OPERATOR, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.IDENTIFIER, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.MINUS, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.INTEGER_LITERAL, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.KEYWORD_DO, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.KEYWORD_PRINT, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.IDENTIFIER, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.SEMICOLON, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.KEYWORD_PRINT, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.STRING_LITERAL, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.SEMICOLON, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.KEYWORD_END, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.KEYWORD_FOR, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.SEMICOLON, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.KEYWORD_ASSERT, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.LEFT_PARENTHESIS, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.IDENTIFIER, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.EQUALITY_COMPARISON, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.IDENTIFIER, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.RIGHT_PARENTHESIS, this.tokenScanner.readNextToken().getType());
      Assert.Equal(MiniPLTokenType.SEMICOLON, this.tokenScanner.readNextToken().getType());
    }
  }
 
}