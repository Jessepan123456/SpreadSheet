// <copyright file="FormulaSyntaxTests.cs" company="UofU-CS3500">
//   Copyright 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <authors> Jesse Pan </authors>
// <date> 1/8/2026 </date>

namespace FormulaTests;

using Formula; // Change this using statement to use different formula implementations.

/// <summary>
///   <para>
///     The following class shows the basics of how to use the MSTest framework,
///     including:
///   </para>
///   <list type="number">
///     <item> How to catch exceptions. </item>
///     <item> How a test of valid code should look. </item>
///   </list>
/// </summary>
[TestClass]
public class FormulaSyntaxTests
{
    // --- Tests for One Token Rule ---

    /// <summary>
    ///   <para>
    ///     This test makes sure the right kind of exception is thrown
    ///     when trying to create a formula with no tokens.
    ///   </para>
    ///   <remarks>
    ///     <list type="bullet">
    ///       <item>
    ///         We use the _ (discard) notation because the formula object
    ///         is not used after that point in the method.  Note: you can also
    ///         use _ when a method must match an interface but does not use
    ///         some of the required arguments to that method.
    ///       </item>
    ///       <item>
    ///         string.Empty is often considered best practice (rather than using "") because it
    ///         is explicit in intent (e.g., perhaps the coder forgot to but something in "").
    ///       </item>
    ///       <item>
    ///         The name of a test method should follow the MS standard:
    ///         https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices
    ///       </item>
    ///       <item>
    ///         All methods should be documented, but perhaps not to the same extent
    ///         as this one.  The remarks here are for your educational
    ///         purposes (i.e., a developer would assume another developer would know these
    ///         items) and would be superfluous in your code.
    ///       </item>
    ///       <item>
    ///         Notice the use of the attribute tag [ExpectedException] which tells the test
    ///         that the code should throw an exception, and if it doesn't an error has occurred;
    ///         i.e., the correct implementation of the constructor should result
    ///         in this exception being thrown based on the given poorly formed formula.
    ///       </item>
    ///     </list>
    ///   </remarks>
    ///   <example>
    ///     <code>
    ///        // here is how we call the formula constructor with a string representing the formula
    ///        _ = new Formula( "5+5" );
    ///     </code>
    ///   </example>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestNoTokens_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("")
        );
        // note: it is arguable that you should replace "" with string.Empty for readability and clarity of intent (e.g., not a cut-and-paste error or a "I forgot to put something there" error).
    }

    [TestMethod]
    public void FormulaConstructor_TestNumberFirstToken_Valid()
    {
        _ = new Formula("1");
    }

    [TestMethod]
    public void FormulaConstructor_TestNumberFirstFollowByLetterToken_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("1a")
        );
    }

    [TestMethod]
    public void FormulaConstructor_TestFirstTokenSingleOpenParen_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("(")
        );
    }

    [TestMethod]
    public void FormulaConstructor_TestFirstTokenSingleOper_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("+")
        );
    }

    [TestMethod]
    public void FormulaConstructor_TestNonValidTokenPercentSymbol_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("%")
        );
    }

    [TestMethod]
    public void FormulaConstructor_TestVariableFirstToken_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("a")
        );
    }

    [TestMethod]
    public void FormulaConstructor_TestDecimalNumberFirstToken_Valid()
    {
        _ = new Formula("3.5");
    }

    [TestMethod]
    public void FormulaConstructor_TestLeadingDecimalFirstToken_Valid()
    {
        _ = new Formula(".5");
    }

    [TestMethod]
    public void FormulaConstructor_TestNumberInBetweenVarFirstToken_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("a1a")
        );
    }

    // --- Tests for Valid Token Rule ---

    [TestMethod]
    public void FormulaConstructor_TestValidTokenOper_Valid()
    {
        _ = new Formula("1/1");
    }

    [TestMethod]
    public void FormulaConstructor_TestValidLetterToken_Valid()
    {
        _ = new Formula("a1+a2");
    }

    [TestMethod]
    public void FormulaConstructor_TestLetterAndNumberValidToken_Valid()
    {
        _ = new Formula("x1-1");
    }

    [TestMethod]
    public void FormulaConstructor_TestLargeLetterAndNumberValidToken_Valid()
    {
        _ = new Formula("xy1*io90");
    }

    [TestMethod]
    public void FormulaConstructor_TestLongVariableNamesToken_Valid()
    {
        _ = new Formula("var123 + var4567 * x1");
    }

    [TestMethod]
    public void FormulaConstructor_TestNonValidTokenAtSymbol_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("a6@9")
        );
    }


    [TestMethod]
    public void FormulaConstructor_TestNonValidTokenCarrot_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("x2^x9")
        );
    }

    [TestMethod]
    public void FormulaConstructor_TestExponentWithNumbers_Valid()
    {
        _ = new Formula("3.5E-6");
    }

    [TestMethod]
    public void FormulaConstructor_TestExponentWithNumbersAdd_Valid()
    {
        _ = new Formula("3.5E+6");
    }

    [TestMethod]
    public void FormulaConstructor_TestInvaildExponent_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("3.5E")
        );
    }

    [TestMethod]
    public void FormulaConstructor_TestDecimalNumberOper_Valid()
    {
        _ = new Formula("3.5 + 7.32");
    }

    [TestMethod]
    public void FormulaConstructor_TestMultiDecimal_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("1.2.3")
        );
    }

    // --- Tests for Closing Parenthesis Rule ---

    [TestMethod]
    public void FormulaConstructor_TestSingleNonClosingParenToken_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula(")")
        );
    }

    [TestMethod]
    public void FormulaConstructor_TestClosingFollowByOpen_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("()1)")
        );
    }

    [TestMethod]
    public void FormulaConstructor_TestNonClosingParenToken_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("1+)")
        );
    }

    // --- Tests for Balanced Parentheses Rule ---

    [TestMethod]
    public void FormulaConstructor_TestBalancedParen_Valid()
    {
        _ = new Formula("(1+1)");
    }

    [TestMethod]
    public void FormulaConstructor_TestBalancedNestedParen_Valid()
    {
        _ = new Formula("((1+1)*9)");
    }

    [TestMethod]
    public void FormulaConstructor_TestBalancedMultipleParen_Valid()
    {
        _ = new Formula("(a6+b10)*(1+1)");
    }

    [TestMethod]
    public void FormulaConstructor_TestBalancedExtraParen_Valid()
    {
        _ = new Formula("((a1+b2)*(1+1))");
    }

    [TestMethod]
    public void FormulaConstructor_TestComplexNested_Valid()
    {
        _ = new Formula("(a1+(b1*c1-(d1/e2)))");
    }

    [TestMethod]
    public void FormulaConstructor_TestBalanceEmptyParen_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("(a1+123)+()")
        );
    }

    [TestMethod]
    public void FormulaConstructor_TestExtraParen_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("(123+id1))")
        );
    }

    [TestMethod]
    public void FormulaConstructor_TestunBalanceNestedParen_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("((1+1)*3))")
        );
    }

    // --- Tests for First Token Rule ---

    [TestMethod]
    public void FormulaConstructor_TestFirstTokenNumber_Valid()
    {
        _ = new Formula("1+1");
    }

    [TestMethod]
    public void FormulaConstructor_TestFirstTokenVariables_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("1abc")
        );
    }

    [TestMethod]
    public void FormulaConstructor_TestLastTokenWhiteSpaces_Valid()
    {
        _ = new Formula("(z1+1)   ");
    }

    [TestMethod]
    public void FormulaConstructor_TestNonValidFirstToken_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("/j12")
        );
    }

    // --- Tests for  Last Token Rule ---

    [TestMethod]
    public void FormulaConstructor_TestOpenParenthesesLastToken_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("1+b3(")
        );
    }

    [TestMethod]
    public void FormulaConstructor_TestLastTokenOperator_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("a10+10+")
        );
    }

    // --- Tests for Parentheses/Operator Following Rule ---

    [TestMethod]
    public void FormulaConstructor_TestOperFollowByOpenParen_Valid()
    {
        _ = new Formula("a1*(z1+1)");
    }

    [TestMethod]
    public void FormulaConstructor_TestCloseParenFollowByOper_Valid()
    {
        _ = new Formula("(9+9)/2");
    }

    [TestMethod]
    public void FormulaConstructor_TestSlashFollowByZero_Valid()
    {
        _ = new Formula("10/0");
    }

    [TestMethod]
    public void FormulaConstructor_TestCloseParenFollowByNumber_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("(81+67)2")
        );
    }

    [TestMethod]
    public void FormulaConstructor_TestCloseParenFollowByVariable_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("(11+56)a")
        );
    }

    [TestMethod]
    public void FormulaConstructor_TestOperFollowByAnotherOper_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("1++1")
        );
    }

    [TestMethod]
    public void FormulaConstructor_TestMixedOper_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("1+-1")
        );
    }

    [TestMethod]
    public void FormulaConstructor_TestOpenParenFollowByOper_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("(+10)")
        );
    }

    // --- Tests for Extra Following Rule ---

    [TestMethod]
    public void FormulaConstructor_TestVariableFollowBySpace_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("(x1 y1)")
        );
    }

    [TestMethod]
    public void FormulaConstructor_TestNumSpaceFollowByEquation_Invalid()
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("((1 2+3))")
        );
    }

    // --- Tests for ToString Method ---

    [TestMethod]
    public void FormulaToString_TestNumbers_Normalized()
    {
        var formula = new Formula("1+1");
        Assert.AreEqual("1+1", formula.ToString());
    }

    [TestMethod]
    public void FormulaToString_TestVariables_Normalized()
    {
        var formula = new Formula("a1+b1");
        Assert.AreEqual("A1+B1", formula.ToString());
    }

    [TestMethod]
    public void FormulaToString_TestVariablesMixed_Normalized()
    {
        var formula = new Formula("xY1 + 7");
        Assert.AreEqual("XY1+7", formula.ToString());
    }

    [TestMethod]
    public void FormulaToString_TestParenAndOpers_Maintained()
    {
        var formula = new Formula("((23+a2)*3)");
        Assert.AreEqual("((23+A2)*3)", formula.ToString());
    }

    [TestMethod]
    public void FormulaToString_TestDecimals_Normalized()
    {
        var formula = new Formula("3.23+9.000");
        Assert.AreEqual("3.23+9", formula.ToString());
    }

    [TestMethod]
    public void FormulaToString_TestNotations_Normalized()
    {
        var formula = new Formula("3E-2");
        Assert.AreEqual("0.03", formula.ToString());
    }

    // --- Tests GetVariables Method ---

    [TestMethod]
    public void FormulaGetVariables_TestSingleVariable_ReturnSet()
    {
        var formula = new Formula("a1");
        var vars = formula.GetVariables();
        Assert.HasCount(1, vars);
        Assert.Contains("A1", vars);
    }

    [TestMethod]
    public void FormulaGetVariables_TestDupVariables_ReturnUniqueSet()
    {
        var formula = new Formula("x1 + x1 + x1");
        var vars = formula.GetVariables();
        Assert.HasCount(1, vars);
        Assert.Contains("X1", vars);
    }

    [TestMethod]
    public void FormulaGetVariables_TestNoVariables_ReturnNone()
    {
        var formula = new Formula("123.1 + 2");
        var vars = formula.GetVariables();
        Assert.HasCount(0, vars);
    }

    [TestMethod]
    public void FormulaGetVariables_TestDiffKindVariables_ReturnUniqueSet()
    {
        var formula = new Formula("abD12 + o2 + by7");
        var vars = formula.GetVariables();
        Assert.HasCount(3, vars);
        Assert.Contains("ABD12", vars);
    }

    [TestMethod]
    public void FormulaGetVariables_TestNotation_ReturnNone()
    {
        var formula = new Formula("3E+3");
        var vars = formula.GetVariables();
        Assert.HasCount(0, vars);
    }

    // -- Equals Method --

    [TestMethod]
    public void FormulaEquals_TestEquals_True()
    {
        var f1 = new Formula("a1");
        var f2 = new Formula("a1");
        Assert.IsTrue(f1.Equals(f2));
    }

    [TestMethod]
    public void FormulaEquals_TestNotEquals_False()
    {
        var f1 = new Formula("a1");
        var f2 = new Formula("b1");
        Assert.IsFalse(f1.Equals(f2));
    }

    [TestMethod]
    public void FormulaEquals_TestNotEqualsEmpty_False()
    {
        var f1 = new Formula("a1");
        Assert.IsFalse(f1.Equals(null));
    }

    [TestMethod]
    public void FormulaEquals_TestNotEqualsNonFormula_False()
    {
        var f1 = new Formula("a1");
        Assert.IsFalse(f1.Equals("a1"));
    }
    
    [TestMethod]
    public void FormulaEqualOper_TestEquals_True()
    {
        var f1 = new Formula("a1");
        var f2 = new Formula("a1");
        Assert.IsTrue(f1 == f2);
    }

    [TestMethod]
    public void FormulaNotEqualOper_TestNotEquals_True()
    {
        var f1 = new Formula("a1");
        var f2 = new Formula("1");
        Assert.IsTrue(f1 != f2);
    }
    
    [TestMethod]
    public void FormulaNotEqualOper_TestNotEqualsParent_True()
    {
        var f1 = new Formula("a1");
        var f2 = new Formula("(a1)");
        Assert.IsTrue(f1 != f2);
    }

    // -- Evaluate --
    [TestMethod]
    public void FormulaEvaluate_TestBasicMath_Equal()
    {
        var f = new Formula("3*3");
        Assert.AreEqual(9.0, f.Evaluate(s => 0));
    }
    
    [TestMethod]
    public void FormulaEvaluate_TestMultiplyLargerExpression_Equal()
    {
        var f = new Formula("3*3+5-1");
        Assert.AreEqual(13.0, f.Evaluate(s => 0));
    }
    
    [TestMethod]
    public void FormulaEvaluate_TestSub_Equal()
    {
        var f = new Formula("6-3");
        Assert.AreEqual(3.0, f.Evaluate(s => 0));
    }
    
    [TestMethod]
    public void FormulaEvaluate_TestAddDecimal_Equal()
    {
        var f = new Formula("1.32 + 30.1");
        Assert.AreEqual(31.42, f.Evaluate(s => 0));
    }
    
    [TestMethod]
    public void FormulaEvaluate_TestOrderLeftToRight_Equal()
    {
        var f = new Formula("1 + 2 - 3 +1");
        Assert.AreEqual(1.0, f.Evaluate(s => 0));
    }

    [TestMethod]
    public void FormulaEvaluate_TestOrderOfOperation_Equal()
    {
        var f = new Formula("6 - 9 / 3");
        Assert.AreEqual(3.0, f.Evaluate(s => 0));
    }
    
    [TestMethod]
    public void FormulaEvaluate_TestNoOper_Equal()
    {
        var f = new Formula("3");
        Assert.AreEqual(3.0, f.Evaluate(s => 0));
    }
    
    [TestMethod]
    public void FormulaEvaluate_TestSubToZeroDecimal_Equal()
    {
        var f = new Formula("9.230 - 5 - 4.230");
        Assert.AreEqual(0.0, f.Evaluate(s => 0));
    }
    
    [TestMethod]
    public void FormulaEvaluate_TestParenthesesSingle_Equal()
    {
        var f = new Formula("(5)");
        Assert.AreEqual(5.0, f.Evaluate(s => 0));
    }
    
    [TestMethod]
    public void FormulaEvaluate_TestParentheses_Equal()
    {
        var f = new Formula("(5+9)*2");
        Assert.AreEqual(28.0, f.Evaluate(s => 0));
    }
    
    [TestMethod]
    public void FormulaEvaluate_TestDivideByParentheses_Equal()
    {
        var f = new Formula("2/(4)");
        Assert.AreEqual(0.5, f.Evaluate(s => 0));
    }
    
    [TestMethod]
    public void FormulaEvaluate_TestNestedParentheses_Equal()
    {
        var f = new Formula("((5+9)*(2+1))");
        Assert.AreEqual(42.0, f.Evaluate(s => 0));
    }

    [TestMethod]
    public void FormulaEvaluate_TestDivideByZeros_Invalid()
    {
        var f = new Formula("1/0");
        
        var result = f.Evaluate(s => throw new ArgumentException());
        
        Assert.IsInstanceOfType(result, typeof(FormulaError));
    }
    
    [TestMethod]
    public void FormulaEvaluate_TestDivideByZerosWithParen_Invalid()
    {
        var f = new Formula("(5+9/0)");
        
        var result = f.Evaluate(s => throw new ArgumentException());
        
        Assert.IsInstanceOfType(result, typeof(FormulaError));
    }
    
    [TestMethod]
    public void FormulaEvaluate_TestVariables_Equal()
    {
        var f = new Formula("a1");
        Assert.AreEqual(5.0, f.Evaluate(s => 5));
    }
    
    [TestMethod]
    public void FormulaEvaluate_TestMultiVariables_Equal()
    {
        var f = new Formula("a1 + b1");
        Assert.AreEqual(10.0, f.Evaluate(s => 5));
    }
    
    [TestMethod]
    public void FormulaEvaluate_TestVariablesNum_Equal()
    {
        var f = new Formula("a1 / 0");
        var result = f.Evaluate(s => 5);
        
        Assert.IsInstanceOfType(result, typeof(FormulaError));
    }
    
    [TestMethod]
    public void FormulaEvaluate_TestUnVariables_Equal()
    {
        var f = new Formula("a1 / 0");
        var result = f.Evaluate(s => throw new ArgumentException());
        
        Assert.IsInstanceOfType(result, typeof(FormulaError));
    }
    
    [TestMethod]
    public void FormulaEvaluate_TestVariablesParent_Equal()
    {
        var f = new Formula("((a3 * 2) + a2)");
        Assert.AreEqual(15.0, f.Evaluate(s => 5));
    }
    
    [TestMethod]
    public void FormulaEvaluate_TestVariablesParentZero_Equal()
    {
        var f = new Formula("(5/(a3 - 5))");
        var result = f.Evaluate(s => 5);
        Assert.IsInstanceOfType(result, typeof(FormulaError));
    }

    // -- HashCode --
    [TestMethod]
    public void FormulaHashCode_TestEqual_True()
    {
        var f1 = new Formula("a1");
        var f2 = new Formula("a1");
        Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
    }

    [TestMethod]
    public void FormulaHashCode_TestNotEqual_True()
    {
        var f1 = new Formula("a1");
        var f2 = new Formula("ab1");
        Assert.AreNotEqual(f1.GetHashCode(), f2.GetHashCode());
    }

    [TestMethod]
    public void FormulaHashCode_TestEqualCap_True()
    {
        var f1 = new Formula("a1");
        var f2 = new Formula("A1");
        Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
    }

    [TestMethod]
    public void FormulaHashCode_TestEqualDecimal_True()
    {
        var f1 = new Formula("1");
        var f2 = new Formula("1.0000");
        Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
    }
}