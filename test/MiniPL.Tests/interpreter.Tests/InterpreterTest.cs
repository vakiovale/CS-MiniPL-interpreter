
    /* 

    [Fact]
    public void integerShouldHaveValue10() {
      this.parser = TestHelpers.getParser("var x : int := 10;");
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.Equal(10, this.analyzer.getInt("x"));
    }

    [Fact]
    public void integerShouldHaveValue3() {
      this.parser = TestHelpers.getParser("var x : int := 1 + 2;");
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.Equal(3, this.analyzer.getInt("x"));
    }

    [Fact]
    public void stringShouldHaveValueHelloWorld() {
      this.parser = TestHelpers.getParser("var x : string := \"Hello World\";");
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.Equal("Hello World", this.analyzer.getString("x"));
    }

    [Theory]
    [InlineData("var x : int := 10;", 10)]
    [InlineData("var x : int := 1 + 3;", 4)]
    [InlineData("var x : int := 4 + 3;", 7)]
    [InlineData("var x : int := (4 + 3) + (10);", 17)]
    [InlineData("var x : int := (4 + 3) + (10 + 4);", 21)]
    [InlineData("var x : int := ((4 + 3) + 12) + (10 + 4);", 33)]
    [InlineData("var x : int := 1 - 3;", -2)]
    [InlineData("var x : int := (4 + 3) - (10);", -3)]
    [InlineData("var x : int := (4 + 3) + (10 - 4);", 13)]
    public void integerShouldHaveValueThatIsCalculatedFromExpression(string source, int value) {
      this.parser = TestHelpers.getParser(source);
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.Equal(value, this.analyzer.getInt("x"));
    }

    [Theory]
    [InlineData("var x : string := \"Hello \" + \"World\";", "Hello World")]
    [InlineData("var x : string := (\"Hello \") + \"World\";", "Hello World")]
    [InlineData("var x : string := (\"Hello \") + (\"World\" + \"!\");", "Hello World!")]
    public void stringShouldHaveValueThatIsCalculatedFromExpression(string source, string value) {
      this.parser = TestHelpers.getParser(source);
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.Equal(value, this.analyzer.getString("x"));
    }
        
    [Theory]
    [InlineData("var x : int := 10;", 10)]
    [InlineData("var x : int := 1 + 3;", 4)]
    [InlineData("var x : int := 2 * 3;", 6)]
    [InlineData("var x : int := 3 / 2;", 1)]
    [InlineData("var x : int := ((2 + 1) * ((4 / 2) + 1)) - 1;", 8)]
    [InlineData("var x : int := ((2 * 3) + (16 / 4));", 10)]
    public void integerShouldHaveValueThatIsCalculatedFromDifferentExpressions(string source, int value) {
      this.parser = TestHelpers.getParser(source);
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.Equal(value, this.analyzer.getInt("x"));
    }

    [Theory]
    [InlineData("var x : bool := 1 = 2;", false)]
    [InlineData("var x : bool := 2 = 2;", true)]
    [InlineData("var x : bool := (1 + 2) = (4 - 1);", true)]
    [InlineData("var x : bool := (1 + 2) = (4 * 1);", false)]
    [InlineData("var x : bool := 1 < 2;", true)]
    [InlineData("var x : bool := 1 < 0;", false)]
    [InlineData("var x : bool := 1 < 1;", false)]
    [InlineData("var x : bool := \"abba\" = \"dagga\";", false)]
    [InlineData("var x : bool := \"abba\" = \"abba\";", true)]
    [InlineData("var x : bool := (\"abba\" = \"abba\") = (2 < 100);", true)]
    [InlineData("var x : bool := \"abba\" < \"dagga\";", true)]
    [InlineData("var x : bool := \"gagga\" < \"dagga\";", false)]
    [InlineData("var x : bool := \"abba\" < \"abba\";", false)]
    [InlineData("var x : bool := (2 < 1) < (1 < 2);", true)]
    [InlineData("var x : bool := (1 < 2) < (1 < 2);", false)]
    [InlineData("var x : bool := (1 < 2) < (2 < 1);", false)]
    [InlineData("var x : bool := (2 < 1) < (2 < 1);", false)]
    [InlineData("var x : bool := (\"abba\" < \"abba\") < (1 < 2);", true)]
    [InlineData("var x : bool := (2 < 1) < (\"abba\" < \"abba\");", false)]
    public void checkBoolExpressions(string source, bool value) {
      this.parser = TestHelpers.getParser(source);
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.Equal(value, this.analyzer.getBool("x"));
    }

    [Fact]
    public void shouldHaveIntValue100WhenReadFromVariable() {
      this.parser = TestHelpers.getParser("var x : int := 30;\nvar z : int := x + 70;");
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.Equal(100, this.analyzer.getInt("z"));
    }

    [Theory]
    [InlineData("var x : string := \"Hello\"; var y : string := \"World\"; var z : string := (x + \" \") + y;", "Hello World")]
    [InlineData("var x : string := \"Hello\"; var y : string := \"World\"; var z : string := (x + \" \") + (y + \"!\");", "Hello World!")]
    public void shouldHaveCorrectValueFromStringVariable(string source, string value) {
      this.parser = TestHelpers.getParser(source);
      Assert.True(this.parser.processAndBuildAST());
      IAST ast = this.parser.getAST();
      this.analyzer.analyze(ast);
      Assert.Equal(value, this.analyzer.getString("z"));
    }
    */