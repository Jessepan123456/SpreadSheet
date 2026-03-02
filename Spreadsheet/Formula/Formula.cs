// <summary>
//   <para>
//     This code is provided to start your assignment. It was written
//     by Profs Joe, Danny, Jim, and Travis. You should keep this attribution
//     at the top of your code where you have your header comment, along
//     with any other required information.
// </para>
//   <para>
//     You should remove/add/adjust comments in your file as appropriate
//     to represent your work and any changes you make.
//   </para>
// </summary>

// <authors> Jesse Pan </authors>
// <date> 1/8/2026 </date>

namespace Formula;

using System.Text.RegularExpressions;

/// <summary>
///   <para>
///     This class represents formulas written in standard infix notation using standard precedence
///     rules. The allowed symbols are non-negative numbers written using double-precision
///     floating-point syntax; variables that consist of one or more letters followed by
///     one or more numbers; parentheses; and the four operator symbols +, -, *, and /.
///   </para>
///   <para>
///     Spaces are significant only insofar that they delimit tokens. For example, "xy" is
///     a single variable, "x y" consists of two variables "x" and y; "x23" is a  single variable;
///     and "x 23" consists of a variable "x" and a number "23". Otherwise, spaces are to be removed.
///   </para>
///   <para>
///     For Assignment Two, you are to implement the following functionality:
///   </para>
///   <list type="bullet">
///     <item>
///        Formula Constructor which checks the syntax of a formula.
///     </item>
///     <item>
///        Get Variables
///     </item>
///     <item>
///        ToString
///     </item>
///   </list>
/// </summary>
public class Formula
{
    /// <summary>
    ///   All variables are letters followed by numbers. This pattern
    ///   represents valid variable name strings.
    /// </summary>
    private const string VariableRegExPattern = @"[a-zA-Z]+\d+";

    /// <summary>
    ///   Allow the formula to be use the main constructor
    /// </summary>
    private string _formula;

    /// <summary>
    ///   The canonical version of the formula when the expression are all valid and finish
    /// </summary>
    private readonly string _canonicalFormula;

    /// <summary>
    ///   Initializes a new instance of the <see cref="Formula"/> class.
    ///   <para>
    ///     Creates a Formula from a string that consists of an infix expression written as
    ///     described in the class comment. If the expression is syntactically incorrect,
    ///     throws a FormulaFormatException with an explanatory Message. See the assignment
    ///     specifications for the syntax rules you are to implement.
    ///   </para>
    ///   <para>
    ///     Non-Exhaustive Example Errors:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///        Invalid variable name, e.g., x, x1x (Note: x1 is valid, but would be normalized to X1)
    ///     </item>
    ///     <item>
    ///        Empty formula, e.g., string.Empty
    ///     </item>
    ///     <item>
    ///        Mismatched Parentheses, e.g., "(("
    ///     </item>
    ///     <item>
    ///        Invalid Following Rule, e.g., "2x+5"
    ///     </item>
    ///   </list>
    /// </summary>
    /// <param name="formula"> The string representation of the formula to b created.</param>
    public Formula(string formula)
    {
        this._formula = formula;
        string lastToken = "";
        _canonicalFormula = "";
        int balance = 0;
        bool isValue = false;

        List<string> formulaTokens = GetTokens(formula);
        foreach (string formulaToken in formulaTokens)
        {
            if (IsVar(formulaToken))
            {
                if (!isValue)
                {
                    isValue = true;
                    lastToken = formulaToken;
                    _canonicalFormula += formulaToken.ToUpper();
                }
                else
                {
                    {
                        throw new FormulaFormatException(formulaToken);
                    }
                }
            }
            else if (double.TryParse(formulaToken, out double doubleValue))
            {
                if (!isValue && lastToken != ")")
                {
                    isValue = true;
                    lastToken = formulaToken;
                    _canonicalFormula += doubleValue;
                }
                else
                {
                    throw new FormulaFormatException(formulaToken);
                }
            }
            else if (Regex.IsMatch(formulaToken, @"^[\+\-*/]$"))
            {
                if (lastToken == "" || Regex.IsMatch(lastToken, @"^[\+\-*/(]$"))
                {
                    throw new FormulaFormatException(formulaToken);
                }

                isValue = false;
                lastToken = formulaToken;
                _canonicalFormula += formulaToken;
            }
            else if (formulaToken == "(")
            {
                balance++;
                lastToken = formulaToken;
                _canonicalFormula += formulaToken;
            }
            else if (formulaToken == ")")
            {
                isValue = true;
                balance--;
                if (lastToken == "" || Regex.IsMatch(lastToken, @"^[\+\-*/(]$") || balance < 0)
                {
                    throw new FormulaFormatException(formulaToken);
                }

                lastToken = formulaToken;
                _canonicalFormula += formulaToken;
            }
            else
            {
                throw new FormulaFormatException(formulaToken);
            }
        }

        if (balance != 0 || lastToken == "" || isValue == false)
        {
            throw new FormulaFormatException(formula);
        }
    }

    /// <summary>
    ///   <para>
    ///     Returns a set of all the variables in the formula.
    ///   </para>
    ///   <remarks>
    ///     Important: no variable may appear more than once in the returned set, even
    ///     if it is used more than once in the Formula.
    ///     Variables should be returned in canonical form, having all letters converted to uppercase.
    ///    </remarks>
    ///    <list type="bullet">
    ///      <item>new("x1+y1*z1").GetVariables() should return a set containing "X1", "Y1", and "Z1".</item>
    ///      <item>new("x1+X1" ).GetVariables() should return a set containing "X1".</item>
    ///   </list>
    /// </summary>
    /// <returns> the set of variables (string names) representing the variables referenced by the formula. </returns>
    public ISet<string> GetVariables()
    {
        HashSet<string> variables = new HashSet<string>();
        List<string> tokens = GetTokens(_formula);
        foreach (string token in tokens)
        {
            if (IsVar(token))
            {
                variables.Add(token.ToUpper());
            }
        }

        return variables;
    }

    /// <summary>
    ///   <para>
    ///     Returns a string representation of a canonical form of the formula.
    ///   </para>
    ///   <para>
    ///     The string will contain no spaces.
    ///   </para>
    ///   <para>
    ///     If the string is passed to the Formula constructor, the new Formula f
    ///     will be such that this.ToString() == f.ToString().
    ///   </para>
    ///   <para>
    ///     All the variable and number tokens in the string will be normalized.
    ///     For numbers, this means that the original string token is converted to
    ///     a number using double.Parse or double.TryParse, then converted back to a
    ///     string using double.ToString.
    ///     For variables, this means all letters are uppercased.
    ///   </para>
    ///   <para>
    ///     For example:
    ///   </para>
    ///   <code>
    ///     new("x1 + Y1").ToString() should return "X1+Y1"
    ///     new("x1 + 5.0000").ToString() should return "X1+5".
    ///   </code>
    ///   <para>
    ///     This method should execute in O(1) time.
    ///   </para>
    /// </summary>
    /// <returns>
    ///   A canonical version (string) of the formula. All "equal" formulas
    ///   should have the same value here.
    /// </returns>
    public override string ToString()
    {
        return _canonicalFormula;
    }

    /// <summary>
    ///     <para>
    ///         Reports whether f1 == f2, using the notion of equality from the <seecref="Equals"/> method.
    ///     </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are the same.</returns>
    public static bool operator ==(Formula f1, Formula f2)
    {
        return f1.Equals(f2);
    }

    /// <summary>
    ///     <para>
    ///         Reports whether f1 != f2, using the notion of equality from the <seecref="Equals"/> method.
    ///     </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are not equal to each other.</returns>
    public static bool operator !=(Formula f1, Formula f2)
    {
        return !(f1.Equals(f2));
    }

    /// <summary>
    ///     <para>
    ///         Determines if two formula objects represent the same formula.
    ///     </para>
    ///     <para>
    ///         By definition, if the parameter is null or does not reference
    ///         a Formula Object then return false.
    ///     </para>
    ///     <para>
    ///         Two Formulas are considered equal if their canonical string representations
    ///         (as defined by ToString) are equal.
    ///     </para>
    /// </summary>
    /// <param name="obj"> The other object.</param>
    /// <returns>
    /// True if the two objects represent the same formula.
    /// </returns>
    public override bool Equals(object? obj)
    {
        if (obj == null || obj is not Formula)
        {
            return false;
        }

        return obj.ToString() == _canonicalFormula;
    }

    /// <summary>
    ///     <para>
    ///         Evaluates this Formula, using the lookup delegate to determine the values of
    ///         variables.
    ///     </para>
    ///     <remarks>
    ///         When the lookup method is called, it will always be passed a normalized(capitalized)
    ///         variable name. The lookup method will throw an ArgumentException if there is
    ///         not a definition for that variable token.
    ///     </remarks>
    ///     <para>
    ///         If no undefined variables or divisions by zero are encountered when evaluating
    ///         this Formula, the numeric value of the formula is returned. Otherwise, a
    ///         FormulaError is returned (with a meaningful explanation as the Reason property).
    ///     </para>
    ///     <para>
    ///         This method should never throw an exception.
    ///     </para>
    /// </summary>
    /// <param name="lookup">
    ///     <para>
    ///         Given a variable symbol as its parameter, lookup returns the variable's value
    ///         (if it has one) or throws an ArgumentException (otherwise). This method will expect
    ///         variable names to be normalized.
    ///     </para>
    ///     </param>
    /// <returns> Either a double or a FormulaError, based on evaluating the formula.</returns>
    public object Evaluate(Lookup lookup)
    {
        List<string> tokens = GetTokens(_canonicalFormula);
        Stack<string> oper = new Stack<string>();
        Stack<double> values = new Stack<double>();
        foreach (string token in tokens)
        {
            if (double.TryParse(token, out double number))
            {
                if (oper.Count == 0)
                {
                    values.Push(number);
                }

                else if (oper.Peek() == "*" || oper.Peek() == "/")
                {
                    double value1 = values.Pop();
                    double result;

                    if (oper.Pop() == "/")
                    {
                        if (number == 0)
                        {
                            return new FormulaError("A division by zero occurs");
                        }

                        result = value1 / number;
                    }
                    else
                    {
                        result = OperMultiply(number, value1);
                    }

                    values.Push(result);
                }
                else
                {
                    values.Push(number);
                }
            }
            else if (IsVar(token))
            {
                try
                {
                    double result = lookup(token);
                    if (oper.Count == 0){}
                    else if (oper.Peek() == "*" || oper.Peek() == "/")
                    {
                        double value1 = values.Pop();
                        double value2 = result;
                        if (oper.Pop() == "/")
                        {
                            if (value1 == 0)
                            {
                                return new FormulaError("A division by zero occurs");
                            }

                            result = value1 / value2;
                        }
                        else
                        {
                            result = OperMultiply(value1, value2);
                        }
                    }
                    values.Push(result);
                }
                catch (ArgumentException)
                {
                    return new FormulaError("Undefined Variable");
                }
            }
            else if (token == "+" || token == "-")
            {
                if (oper.Count == 0)
                {
                    oper.Push(token);
                }
                else if (oper.Peek() == "+" || oper.Peek() == "-")
                {
                    values.Push(ApplyOperAddOrSub(values, oper));
                    oper.Push(token);
                }
                else
                {
                    oper.Push(token);
                }
            }
            else if (token == "*" || token == "/")
            {
                oper.Push(token);
            }
            else if (token == "(")
            {
                oper.Push(token);
            }
            else if (token == ")")
            {
                if (oper.Peek() == "+" || oper.Peek() == "-")
                {
                    values.Push(ApplyOperAddOrSub(values, oper));
                }

                oper.Pop();
                if (oper.Count == 0)
                {
                }
                else if (oper.Peek() == "*" || oper.Peek() == "/")
                {
                    double value1 = values.Pop();
                    double value2 = values.Pop();
                    double result;

                    if (oper.Pop() == "/")
                    {
                        if (value1 == 0)
                        {
                            return new FormulaError("A division by zero occurs");
                        }

                        result = value2 / value1;
                    }
                    else
                    {
                        result = OperMultiply(value1, value2);
                    }

                    values.Push(result);
                }
            }
        }

        if (oper.Count != 0)
        {
            return ApplyOperAddOrSub(values, oper);
        }

        return values.Pop();
    }

    /// <summary>
    ///     Helper Method that performs a single addition or subtraction oepration using
    ///     the top two values and the top operator from their stacks. It applies the oper
    ///     from left to right and returns the result.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="oper"></param>
    /// <returns></returns>
    private double ApplyOperAddOrSub(Stack<double> values, Stack<string> oper)
    {
        double value1 = values.Pop();
        double value2 = values.Pop();
        if (oper.Pop() == "+")
        {
            return value2 + value1;
        }
        else
        {
            return value2 - value1;
        }
    }

    /// <summary>
    ///     Helper Method that allow two values to be multiply and returning the result
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <returns></returns>
    private static double OperMultiply(double value1, double value2)
    {
        return value2 * value1;
    }

    /// <summary>
    ///     <para>
    ///         Returns a hash code for this Formula. If f1.Equals(f2), then it must be the
    ///         case that f1.GetHashCode() == f2.GetHashCode(). Ideally, the probability that two
    ///         randomly-generated unequal Formulas have the same hash code should be miniscule.
    ///     </para>
    /// </summary>
    /// <returns> The hashcode for the object. </returns>
    public override int GetHashCode()
    {
        return _canonicalFormula.GetHashCode();
    }

    /// <summary>
    ///   Reports whether "token" is a variable. It must be one or more letters
    ///   followed by one or more numbers.
    /// </summary>
    /// <param name="token"> A token that may be a variable. </param>
    /// <returns> true if the string matches the requirements, e.g., A1 or a1. </returns>
    private static bool IsVar(string token)
    {
        // notice the use of ^ and $ to denote that the entire string being matched is just the variable
        string standaloneVarPattern = $"^{VariableRegExPattern}$";
        return Regex.IsMatch(token, standaloneVarPattern);
    }

    /// <summary>
    ///   <para>
    ///     Given an expression, enumerates the tokens that compose it.
    ///   </para>
    ///   <para>
    ///     Tokens returned are:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>left paren</item>
    ///     <item>right paren</item>
    ///     <item>one of the four operator symbols</item>
    ///     <item>a string consisting of one or more letters followed by one or more numbers</item>
    ///     <item>a double literal</item>
    ///     <item>and anything that doesn't match one of the above patterns</item>
    ///   </list>
    ///   <para>
    ///     There are no empty tokens; white space is ignored (except to separate other tokens).
    ///   </para>
    /// </summary>
    /// <param name="formula"> A string representing an infix formula such as 1*B1/3.0. </param>
    /// <returns> The ordered list of tokens in the formula. </returns>
    private static List<string> GetTokens(string formula)
    {
        List<string> results = [];
        string lpPattern = @"\(";
        string rpPattern = @"\)";
        string opPattern = @"[\+\-*/]";
        string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
        string spacePattern = @"\s+";

        // Overall pattern
        string pattern = string.Format(
            "({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
            lpPattern,
            rpPattern,
            opPattern,
            VariableRegExPattern,
            doublePattern,
            spacePattern);
        // Enumerate matching tokens that don't consist solely of white space.
        foreach (string s in Regex.Split(formula, pattern,
                     RegexOptions.IgnorePatternWhitespace))
        {
            if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
            {
                results.Add(s);
            }
        }

        return results;
    }
}

/// <summary>
///   Used to report syntax errors in the argument to the Formula constructor.
/// </summary>
public class FormulaFormatException : Exception
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaFormatException"/> class.
    ///   <para>
    ///     Constructs a FormulaFormatException containing the explanatory message.
    ///   </para>
    /// </summary>
    /// <param name="message"> A developer defined message describing why the exception occured.</param>
    public FormulaFormatException(string message)
        : base(message)
    {
        // All this does is call the base constructor. No extra code needed.
    }
}

/// <summary>
///     Used as a possible return value of the Formula.Evaluate method.
/// </summary>
public class FormulaError
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="FormulaError"/> class.
    ///     <para>
    ///         Constructs a FormulaError containing the explanatory reason.
    ///     </para>
    /// </summary>
    /// <param name="message"> Contains a message for why the erroroccurred.</param>
    public FormulaError(string message)
    {
        Reason = message;
    }

    /// <summary>
    ///     Gets the reason why this FormulaError was created.
    /// </summary>
    public string Reason { get; private set; }
}

/// <summary>
///    Any method meeting this type signature can be used for
///    looking up the value of a variable.
/// </summary>
/// <exception cref="ArgumentException">
///    If a variable name is provided that is not recognized by the implementingmethod,
///    then the method should throw an ArgumentException.
/// </exception>
/// <param name="variableName">
///    The name of the variable (e.g., "A1") to lookup.
/// </param>
/// <returns> The value of the given variable (if one exists). </returns>
public delegate double Lookup(string variableName);