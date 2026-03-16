// <copyright file="Spreadsheet.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// Written by Joe Zachary for CS 3500, September 2013
// Update by Profs Kopta, de St. Germain, Martin, Fall 2021, Fall 2024, Fall 2025
// - Updated return types
// - Updated documentation

// <authors> Jesse Pan </authors>
// <date> 2/9/2026 </date>

using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Spreadsheets;

using System.Text.Json;
using Formula;
using DependencyGraph;

/// <summary>
///     <para>
///         Thrown to indicate that a change to a cell will cause a circular dependency.
///     </para>
/// </summary>
public class CircularException : Exception
{
}

/// <summary>
///     <para>
///         Thrown to indicate that a name parameter was invalid.
///     </para>
/// </summary>
public class InvalidNameException : Exception
{
}

//  <copyright file="NewSpreadsheetContent.cs" company="UofU-CS3500">
//  Copyright (c) 2025 UofU-CS3500. All rights reserved.
//  </copyright>
//  Add this exception to the namespace (not inside another class)
// /// <summary>
// ///     <para>
// ///         Thrown to indicate that a read or write attempt has failed with
// ///         an expected error message informing the user of what went wrong.
// ///     </para>
// /// </summary>
public class SpreadsheetReadWriteException : Exception
{
    /// <summary>
    ///     <para>
    ///         Creates the exception with a message defining what went wrong.
    ///     </para>
    /// </summary>
    /// <param name="msg"> An informative message to the user. </param>
    public SpreadsheetReadWriteException(string msg)
        : base(msg)
    {
    }
}

/// <summary>
///     <para>
///         A Spreadsheet object represents the state of a simple spreadsheet. A
///         spreadsheet represents an infinite number of named cells.
///     </para>
///     <para>
///         Valid Cell Names: A string is a valid cell name if and only if it is one or
///         more letters followed by one or more numbers, e.g., A5, BC27.
///     </para>
///     <para>
///         Cell names are case-insensitive, so "x1" and "X1" are the same cell name.
///         Your code should normalize (uppercased) any stored name but accept either.
///     </para>
///     <para>
///         A spreadsheet represents a cell corresponding to every possible cell name. (This
///         means that a spreadsheet contains an infinite number of cells.) In addition to
///         a name, each cell has a contents and a value. The distinction is important.
///     </para>
///     <para>
///         The <b>contents</b> of a cell can be (1) a string, (2) a double, or (3) a Formula.
///         If the contents of a cell is set to the empty string, the cell is considered empty.
///     </para>
///     <para>
///         By analogy, the contents of a cell in Excel is what is displayed on
///         the editing line when the cell is selected.
///     </para>
///     <para>
///         In a new spreadsheet, the contents of every cell is the empty string. Note:
///         this is by definition (it is IMPLIED, not stored).
///     </para>
///     <para>
///         The <b>value</b> of a cell can be (1) a string, (2) a double, or (3) a FormulaError.
///         (By analogy, the value of an Excel cell is what is displayed in that cell's position
///         in the grid.) We are not concerned with cell values yet, only with their contents,
///         but for context:
///     </para>
///     <list type="number">
///         <item>If a cell's contents is a string, its value is that string.</item>
///         <item>If a cell's contents is a double, its value is that double.</item>
///         <item>
///             <para>
///                 If a cell's contents is a Formula, its value is either a double or a FormulaError,
///                 as reported by the Evaluate method of the Formula class. For this assignment,
///                 you are not dealing with values yet.
///             </para>
///         </item>
///     </list>
///     <para>
///         Spreadsheets are never allowed to contain a combination of Formulas that establish
///         a circular dependency. A circular dependency exists when a cell depends on itself,
///         either directly or indirectly.
///         For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
///         A1 depends on B1, which depends on C1, which depends on A1. That's a circular
///         dependency.
///     </para>
/// </summary>
public class Spreadsheet
{
    /// <summary>
    ///     Stores all non-empty cells using Dictionary, each having it
    ///     own content and value. Empty cells are not stored.
    /// </summary>
    [JsonPropertyName("Cells")] [JsonInclude]
    private Dictionary<string, Cell> _cells = new();

    /// <summary>
    ///     Tracks all formula dependencies between cells. Each Cell having it
    ///     own dependency and dependees.
    /// </summary>
    private DependencyGraph _graph = new();

    /// <summary>
    ///     Represent a single cell and stores its contents and it values
    /// </summary>
    public class Cell
    {
        /// <summary>
        ///     Contain the content of that cell
        /// </summary>
        [JsonIgnore]
        public object Content { get; set; }

        /// <summary>
        ///     Contain the value of that cell based on its content
        /// </summary>
        [JsonIgnore]
        public object Value { get; set; }

        /// <summary>
        ///     Stores the string version of the content of that cell
        /// </summary>
        [JsonPropertyName("StringForm")]
        public string StringFormat { get; set; }

        /// <summary>
        ///     Setting it cell object content to its Content, Value, and StringFormat
        /// </summary>
        /// <param name="content"></param>
        public Cell(object content)
        {
            Content = content;
            Value = content;
            StringFormat = "";
        }
    }

    /// <summary>
    ///     A valid cell name need to start with a letter and followed
    ///     by letters or digits.
    /// </summary>
    private const string VariableRegExPattern = @"^[A-Z][A-Z0-9]*$";

    /// <summary>
    ///     Helper method that help determine whether a cell name is invalid.
    ///     A name is considered invalid if it does not match the valid name
    ///     rule.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private bool IsNameInvalid(string name)
    {
        if (!Regex.IsMatch(name, VariableRegExPattern) || name == "")
            return true;
        return false;
    }

    /// <summary>
    ///     Helper method that calls IsNameInvalid and check if the name provided is valid or not.
    ///     If invalid it throw InvalidNameException and stops running.
    /// </summary>
    /// <param name="name"></param>
    /// <exception cref="InvalidNameException"></exception>
    private void ValidateName(string name)
    {
        if (IsNameInvalid(name)) throw new InvalidNameException();
    }

    /// <summary>
    ///     Returns the contents (as opposed to the value) of the named cell.
    /// </summary>
    /// <exception cref="InvalidNameException">
    ///     Thrown if the name is invalid.
    /// </exception>
    /// <param name="name">The name of the spreadsheet cell to query. </param>
    /// <returns>
    ///     The contents as either a string, a double, or a Formula.
    ///     See the class header summary.
    /// </returns>
    public object GetCellContents(string name)
    {
        name = name.ToUpper();
        ValidateName(name);

        if (!_cells.ContainsKey(name)) return "";

        if (_cells[name].Content is Formula)
        {
            return "=" +  _cells[name].Content;
        }
        return _cells[name].Content;
    }

    /// <summary>
    ///     Provides a copy of the normalized names of all the cells in the spreadsheet
    ///     that contain information (i.e., non-empty cells).
    /// </summary>
    /// <returns>
    ///     A set of the names of all the non-empty cells in the spreadsheet.
    /// </returns>
    public ISet<string> GetNamesOfAllNonemptyCells()
    {
        return new HashSet<string>(_cells.Keys);
    }

    /// <summary>
    ///     Helper method that help turn a IEnumerable list into a IList and return the
    ///     IList.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>
    ///     IList
    /// </returns>
    private IList<string> NewList(string name)
    {
        IList<string> dependList = new List<string>();
        var requiredCells = GetCellsToRecalculate(name);
        foreach (var cell in requiredCells) dependList.Add(cell);

        return dependList;
    }

    /// <summary>
    ///     Set the contents of the named cell to the given number.
    /// </summary>
    /// <exception cref="InvalidNameException">
    ///     If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="number"> The new contents of the cell. </param>
    /// <returns>
    ///     <para>
    ///         This method returns an ordered list consisting of the passed in name
    ///         followed by the names of all other cells whose value depends, directly
    ///         or indirectly, on the named cell.
    ///     </para>
    ///     <para>
    ///         The order must correspond to a valid dependency ordering forrecomputing
    ///         all the cells, i.e., if you re-evaluate each cell in the order of thelist,
    ///         the overall spreadsheet will be correctly updated.
    ///     </para>
    ///     <para>
    ///         For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1,the
    ///         list [A1, B1, C1] is returned, i.e., A1 was changed, so then A1 must be
    ///         evaluated, followed by B1, followed by C1.
    ///     </para>
    /// </returns>
    private IList<string> SetCellContents(string name, double number)
    {
        ValidateName(name);

        _graph.ReplaceDependees(name, Enumerable.Empty<string>());
        _cells[name] = new Cell(number);
        return NewList(name);
    }

    /// <summary>
    ///     The contents of the named cell becomes the given text.
    /// </summary>
    /// <exception cref="InvalidNameException">
    ///     If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="text"> The new contents of the cell. </param>
    /// <returns>
    ///     The same list as defined in <see cref="SetCellContents(string,double)" />.
    /// </returns>
    private IList<string> SetCellContents(string name, string text)
    {
        ValidateName(name);

        _graph.ReplaceDependees(name, Enumerable.Empty<string>());
        _cells[name] = new Cell(text);
        if (text == "") _cells.Remove(name);

        return NewList(name);
    }

    /// <summary>
    ///     Set the contents of the named cell to the given formula.
    /// </summary>
    /// <exception cref="InvalidNameException">
    ///     If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    ///     <para>
    ///         If changing the contents of the named cell to be the formula would
    ///         cause a circular dependency, throw a CircularException, and no
    ///         change is made to the spreadsheet.
    ///     </para>
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="formula"> The new contents of the cell. </param>
    /// <returns>
    ///     The same list as defined in <see cref="SetCellContents(string,double)" />.
    /// </returns>
    private IList<string> SetCellContents(string name, Formula formula)
    {
        ValidateName(name);

        var oldDep = _graph.GetDependees(name);
        _graph.ReplaceDependees(name, Enumerable.Empty<string>());
        IList<string> dependList = new List<string>();
        foreach (var v in formula.GetVariables()) _graph.AddDependency(v, name);

        try
        {
            var requiredCells = GetCellsToRecalculate(name);
            foreach (var cell in requiredCells) dependList.Add(cell);
        }
        catch (CircularException)
        {
            _graph.ReplaceDependees(name, oldDep);
            throw new CircularException();
        }

        _cells[name] = new Cell(formula);
        return dependList;
    }

    /// <summary>
    ///     Returns an enumeration, without duplicates, of the names of all cellswhose
    ///     values depend directly on the value of the named cell.
    /// </summary>
    /// <param name="name"> This <b>MUST</b> be a valid name. </param>
    /// <returns>
    ///     <para>
    ///         Returns an enumeration, without duplicates, of the names of all cells
    ///         that contain formulas containing name.
    ///     </para>
    ///     <para>For example, suppose that: </para>
    ///     <list type="bullet">
    ///         <item>A1 contains 3</item>
    ///         <item>B1 contains the formula A1 * A1</item>
    ///         <item>C1 contains the formula B1 + A1</item>
    ///         <item>D1 contains the formula B1 - C1</item>
    ///     </list>
    ///     <para> The direct dependents of A1 are B1 and C1. </para>
    /// </returns>
    private IEnumerable<string> GetDirectDependents(string name)
    {
        return _graph.GetDependents(name.ToUpper());
    }

    /// <summary>
    ///     <para>
    ///         This method is implemented for you, but makes use of yourGetDirectDependents.
    ///     </para>
    ///     <para>
    ///         Returns an enumeration of the names of all cells whose values must
    ///         be recalculated, assuming that the contents of the cell referred
    ///         to by name has changed. The cell names are enumerated in an order
    ///         in which the calculations should be done.
    ///     </para>
    ///     <exception cref="CircularException">
    ///         If the cell referred to by name is involved in a circular dependency,
    ///         throws a CircularException.
    ///     </exception>
    ///     <para>
    ///         For example, suppose that:
    ///     </para>
    ///     <list type="number">
    ///         <item>
    ///             A1 contains 5
    ///         </item>
    ///         <item>
    ///             B1 contains the formula A1 + 2.
    ///         </item>
    ///         <item>
    ///             C1 contains the formula A1 + B1.
    ///         </item>
    ///         <item>
    ///             D1 contains the formula A1 * 7.
    ///         </item>
    ///         <item>
    ///             E1 contains 15
    ///         </item>
    ///     </list>
    ///     <para>
    ///         If A1 has changed, then A1, B1, C1, and D1 must be recalculated,
    ///         and they must be recalculated in an order which has A1 first, and B1before C1
    ///         (there are multiple such valid orders).
    ///         The method will produce one of those enumerations.
    ///     </para>
    ///     <para>
    ///         PLEASE NOTE THAT THIS METHOD DEPENDS ON THE METHOD GetDirectDependents.
    ///         IT WON'T WORK UNTIL GetDirectDependents IS IMPLEMENTED CORRECTLY.
    ///     </para>
    /// </summary>
    /// <param name="name"> The name of the cell. Requires that name be a valid cell name.</param>
    /// <returns>
    ///     Returns an enumeration of the names of all cells whose values must
    ///     be recalculated.
    /// </returns>
    private IEnumerable<string> GetCellsToRecalculate(string name)
    {
        LinkedList<string> changed = new();
        HashSet<string> visited = [];
        Visit(name, name, visited, changed);
        return changed;
    }

    /// <summary>
    ///     A helper for the GetCellsToRecalculate method.
    ///     It walks from cell to cell that depend on the given cell, adds each visited cell to the
    ///     recalculation list in correct order. While it checks if a CircularException occur, if so
    ///     throw a CircularException.
    /// </summary>
    private void Visit(string start, string name, ISet<string> visited,
        LinkedList<string> changed)
    {
        visited.Add(name);
        foreach (var n in GetDirectDependents(name))
        {
            if (n.Equals(start)) throw new CircularException();

            if (!visited.Contains(n)) Visit(start, n, visited, changed);
        }

        changed.AddFirst(name);
    }

    /// <summary>
    ///     <para>
    ///         Return the value of the named cell, as defined by
    ///         <see cref="GetCellValue(string)" />.
    ///     </para>
    /// </summary>
    /// <param name="name"> The cell in question. </param>
    /// <returns>
    ///     <see cref="GetCellValue(string)" />
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///     If the provided name is invalid, throws an InvalidNameException.
    /// </exception>
    public object this[string name] => GetCellValue(name);

    /// <summary>
    ///     True if this spreadsheet has been changed since it was
    ///     created or saved (whichever happened most recently),
    ///     False otherwise.
    /// </summary>
    [JsonIgnore]
    public bool Changed { get; private set; }

    /// <summary>
    ///     A default constructor for spreadsheet if a filename is not provided
    /// </summary>
    public Spreadsheet()
    {
    }

    /// <summary>
    ///     Constructs a spreadsheet using the saved data in the file referred to by
    ///     the given filename.
    ///     <see cref="Save(string)" />
    /// </summary>
    /// <exception cref="SpreadsheetReadWriteException">
    ///     Thrown if the file can not be loaded into a spreadsheet for any reason
    /// </exception>
    /// <param name="filename">The path to the file containing the spreadsheet to load</param>
    public Spreadsheet(string filename)
    {
        try
        {
            string json = File.ReadAllText(filename);
            Spreadsheet? info = JsonSerializer.Deserialize<Spreadsheet>(json);
            
            if (info != null && info._cells.Count != 0)
            {
                foreach (var name in info._cells.Keys)
                {
                    SetContentsOfCell(name, info._cells[name].StringFormat);
                }
            }

            Changed = false;
        }
        catch (Exception)
        {
            throw new SpreadsheetReadWriteException("Failed to load file");
        }
    }

    public void JsonReplace(string json)
    {
        try
        {
            Spreadsheet? info = JsonSerializer.Deserialize<Spreadsheet>(json);

            if (info != null && info._cells.Count != 0)
            {
                foreach (var name in info._cells.Keys)
                {
                    SetContentsOfCell(name, info._cells[name].StringFormat);
                }
            }

            Changed = false;
        }
        catch (Exception)
        {
            throw new SpreadsheetReadWriteException("Failed to load file");
        }
    }

    /// <summary>
    ///     Saves this spreadsheet to a file
    /// </summary>
    /// <param name="filename"> The name (with path) of the file to save to.</param>
    /// <exception cref="SpreadsheetReadWriteException">
    ///     If there are any problems opening, writing, or closing the file,
    ///     the method should throw a SpreadsheetReadWriteException with an
    ///     explanatory message.
    /// </exception>
    public void Save(string filename)
    {
        try
        {
            string json = JsonPath();
            File.WriteAllText(filename, json);
            Changed = false;
        }
        catch (Exception)
        {
            throw new SpreadsheetReadWriteException("A problem has occur when saving");
        }
    }

    public string JsonPath()
    {
        return JsonSerializer.Serialize(this);
    }
    

    /// <summary>
    ///     <para>
    ///         Return the value of the named cell.
    ///     </para>
    /// </summary>
    /// <param name="name"> The cell in question. </param>
    /// <returns>
    ///     Returns the value (as opposed to the contents) of the named cell. The return
    ///     value should be either a string, a double, or a CS3500.Formula.FormulaError.
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///     If the provided name is invalid, throws an InvalidNameException.
    /// </exception>
    public object GetCellValue(string name)
    {
        name = name.ToUpper();
        ValidateName(name);

        return _cells[name].Value;
    }

    /// <summary>
    ///     <para>
    ///         Set the contents of the named cell to be the provided string
    ///         which will either represent (1) a string, (2) a number, or
    ///         (3) a formula (based on the prepended '=' character).
    ///     </para>
    ///     <para>
    ///         Rules of parsing the input string:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             <para>
    ///                 If 'content' parses as a double, the contents of the named
    ///                 cell becomes that double.
    ///             </para>
    ///         </item>
    ///         <item>
    ///             If the string does not begin with an '=', the contents of the
    ///             named cell becomes 'content'.
    ///         </item>
    ///         <item>
    ///             <para>
    ///                 If 'content' begins with the character '=', an attempt is made
    ///                 to parse the remainder of content into a Formula f using the Formula
    ///                 constructor. There are then three possibilities:
    ///             </para>
    ///             <list type="number">
    ///                 <item>
    ///                     If the remainder of content cannot be parsed into a Formula, a
    ///                     CS3500.Formula.FormulaFormatException is thrown.
    ///                 </item>
    ///                 <item>
    ///                     Otherwise, if changing the contents of the named cell to be f
    ///                     would cause a circular dependency, a CircularException is thrown,
    ///                     and no change is made to the spreadsheet.
    ///                 </item>
    ///                 <item>
    ///                     Otherwise, the contents of the named cell becomes f.
    ///                 </item>
    ///             </list>
    ///         </item>
    ///     </list>
    /// </summary>
    /// <returns>
    ///     <para>
    ///         The method returns a list consisting of the name plus the names
    ///         of all other cells whose value depends, directly or indirectly,
    ///         on the named cell. The order of the list should be any order
    ///         such that if cells are re-evaluated in that order, their dependencies
    ///         are satisfied by the time they are evaluated.
    ///     </para>
    ///     <example>
    ///         For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    ///         list {A1, B1, C1} is returned.
    ///     </example>
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///     If name is invalid, throws an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    ///     If a formula would result in a circular dependency, throws CircularException.
    /// </exception>
    public IList<string> SetContentsOfCell(string name, string content)
    {
        name = name.ToUpper();
        ValidateName(name);
        if (content == "")
        {
            return SetAndRecalculate(name, content, content);
        }

        Changed = true;

        if (double.TryParse(content, out double number))
        {
            return SetAndRecalculate(name, number, content);
        }

        if (content[0] == '=')
        {
            string newContent = content.Remove(0, 1);
            Formula f = new Formula(newContent);
            return SetAndRecalculate(name, f, content);
        }

        return SetAndRecalculate(name, content, content);
    }

    /// <summary>
    ///     Helper Method that updates the contents, recomputes all cells that
    ///     depend on it, and stores the string format version of the content.
    ///     The content can be double, string, or Formula.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="content"></param>
    /// <param name="stringFormat"></param>
    /// <returns>
    ///     A list of cell naes that require recalculation
    /// </returns>
    private IList<string> SetAndRecalculate(string name, object content, string stringFormat)
    {
        IList<string> list;

        if (content is double d)
            list = SetCellContents(name, d);
        else if (content is string s)
            list = SetCellContents(name, s);
        else
            list = SetCellContents(name, (Formula)content);

        foreach (var n in list)
        {
            if (_cells.ContainsKey(n))
            {
                var c = _cells[n].Content;

                if (c is Formula f)
                    _cells[n].Value = f.Evaluate(Lookup);
                else
                    _cells[n].Value = c;
            }
        }

        if(_cells.ContainsKey(name)) _cells[name].StringFormat = stringFormat;
        return list;
    }

    /// <summary>
    ///     Helper method that return the number value of the cell for formula
    ///     Throws and ArgumentException if the cell is empty or its value is not
    ///     a double, causing the formulaError
    /// </summary>
    /// <param name="name"></param>
    /// <returns>
    ///     The double value stored in the cell
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    private double Lookup(string name)
    {
        if (!_cells.ContainsKey(name))
        {
            throw new ArgumentException();
        }

        var value = _cells[name].Value;
        if (value is double doubleValue)
        {
            return doubleValue;
        }

        throw new ArgumentException();
    }
}