namespace SpreadsheetTests;

using Spreadsheets;
using Formula;

[TestClass]
public sealed class SpreadsheetTests
{
    // -- GetNonEmptyCells Method --
    [TestMethod]
    public void GetNamesOfAllNonEmptyCells_Empty_Equal()
    {
        var s = new Spreadsheet();

        var amount = s.GetNamesOfAllNonemptyCells();

        Assert.AreEqual(0, amount.Count());
    }

    [TestMethod]
    public void GetNamesOfAllNonEmptyCells_NonEmpty_Equal()
    {
        var s = new Spreadsheet();

        s.SetContentsOfCell("A1", "1");
        s.SetContentsOfCell("A2", "Hello");
        s.SetContentsOfCell("B3", ("=1+1"));
        var amount = s.GetNamesOfAllNonemptyCells();

        s.Save("NO.txt");
        Assert.AreEqual(3, amount.Count());
    }

    [TestMethod]
    public void GetNamesOfAllNonEmptyCells_OverRideCells_Equal()
    {
        var s = new Spreadsheet();

        s.SetContentsOfCell("A1", "1.34");
        s.SetContentsOfCell("A1", "");
        var amount = s.GetNamesOfAllNonemptyCells();
        Assert.AreEqual(0, amount.Count());
    }

    [TestMethod]
    public void GetNamesOfAllNonEmptyCells_OverRideCellsIncrease_Equal()
    {
        var s = new Spreadsheet();

        s.SetContentsOfCell("A1", "");
        s.SetContentsOfCell("A1", "9");
        var amount = s.GetNamesOfAllNonemptyCells();
        Assert.AreEqual(1, amount.Count());
    }

    // -- GetCell Method --
    [TestMethod]
    public void GetCellContentsOfCell_InvalidName_Invalid()
    {
        var s = new Spreadsheet();
        Assert.Throws<InvalidNameException>(() => s.GetCellContents("2"));
    }

    [TestMethod]
    public void GetCellContentsOfCell_InvalidNameCaseSen_Equal()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("a1", "1");
        Assert.AreEqual(1.0, s.GetCellContents("A1"));
    }

    [TestMethod]
    public void GetCellContentsOfCell_InvalidNameCaseSenOverRide_Equal()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("a1", "1");
        s.SetContentsOfCell("A1", "3");
        Assert.AreEqual(3.0, s.GetCellContents("a1"));
    }

    [TestMethod]
    public void GetCellContentsOfCell_InvalidNameSpace_Invalid()
    {
        var s = new Spreadsheet();
        Assert.Throws<InvalidNameException>(() => s.GetCellContents("a A 1"));
    }

    [TestMethod]
    public void GetCellContentsOfCell_OverridenCells_Equal()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("A1", "1");
        s.SetContentsOfCell("A1", "Hello");
        Assert.AreEqual("Hello", s.GetCellContents("A1"));
    }

    [TestMethod]
    public void GetCellContentsOfCell_Empty_Equal()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("A1", "");
        Assert.AreEqual("", s.GetCellContents("A1"));
    }

    [TestMethod]
    public void GetCellContentsOfCell_EquationNumber_Equal()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=3*3");
        Assert.AreEqual(new Formula("3*3"), s.GetCellContents("A1"));
    }

    // -- SetCell

    [TestMethod]
    public void SetCellContentsOfCell_UpdateStringContent_Equal()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("A1", "B1+2");
        Assert.AreEqual("B1+2", s.GetCellContents("A1"));
    }

    [TestMethod]
    public void SetCellContentsOfCell_InvalidNameNum_Invalid()
    {
        var s = new Spreadsheet();
        Assert.Throws<InvalidNameException>(() => s.SetContentsOfCell("1", "1"));
    }

    [TestMethod]
    public void SetCellContentsOfCell_InvalidNameString_Invalid()
    {
        var s = new Spreadsheet();
        Assert.Throws<InvalidNameException>(() => s.SetContentsOfCell("1A", "String"));
    }

    [TestMethod]
    public void SetCellContentsOfCell_InvalidNameFormula_Invalid()
    {
        var s = new Spreadsheet();
        Assert.Throws<InvalidNameException>(() => s.SetContentsOfCell("", "=1+1"));
    }

    [TestMethod]
    public void SetCellContentsOfCell_CircularException_Invalid()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("B1", "=A1");
        Assert.Throws<CircularException>(() => s.SetContentsOfCell("A1", "=B1"));
        var result = s.SetContentsOfCell("B1", "HI");
        CollectionAssert.AreEqual(new List<string> { "B1" }, result.ToList());
    }

    [TestMethod]
    public void SetCellContentsOfCell_CircularExceptionDeeperChain_Invalid()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("B1", "=A1");
        s.SetContentsOfCell("C1", "=B1");
        s.SetContentsOfCell("D1", "=C1");
        s.SetContentsOfCell("E1", "=D1");
        Assert.Throws<CircularException>(() => s.SetContentsOfCell("A1", "=E1"));
    }

    [TestMethod]
    public void SetCellContentsOfCell_CircularExceptionSelf_Invalid()
    {
        var s = new Spreadsheet();
        Assert.Throws<CircularException>(() => s.SetContentsOfCell("A1", "=A1"));
    }

    [TestMethod]
    public void SetCellContentsOfCell_FormulaDependencyNum_Equal()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("B1", "=A1+1+C1");
        s.SetContentsOfCell("A1", "1");
        s.SetContentsOfCell("C1", "1");

        var result = s.SetContentsOfCell("B1", "1");
        CollectionAssert.AreEqual(new List<string> { "B1" }, result.ToList());
    }

    [TestMethod]
    public void SetCellContentsOfCell_FormulaDependencyCaseSen_Equal()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("b1", "=A1+1");
        s.SetContentsOfCell("a1", "hi");

        var result = s.SetContentsOfCell("a1", "1");
        CollectionAssert.AreEqual(new List<string> { "A1", "B1" }, result.ToList());
    }

    [TestMethod]
    public void SetCellContentsOfCell_RemoveOldDependencyString_Equal()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("B1", "=A1");
        var oldResult = s.SetContentsOfCell("A1", "Hi");

        CollectionAssert.AreEqual(
            new List<string> { "A1", "B1" },
            oldResult.ToList()
        );
        var newResult = s.SetContentsOfCell("B1", "Hello");
        CollectionAssert.AreEqual(
            new List<string> { "B1" },
            newResult.ToList()
        );
    }

    [TestMethod]
    public void SetCellContentsOfCell_CorrectDependencyList_Equal()
    {
        var s = new Spreadsheet();

        s.SetContentsOfCell("B1", "=A1 * 2");

        s.SetContentsOfCell("C1", "=B1 + A1");

        var result = s.SetContentsOfCell("A1", "5");

        CollectionAssert.AreEqual(
            new List<string> { "A1", "B1", "C1" },
            result.ToList()
        );
    }

    [TestMethod]
    public void SetCellContentsOfCell_BranchingDependency_Equal()
    {
        var s = new Spreadsheet();

        s.SetContentsOfCell("A1", "=B1 * C1");

        s.SetContentsOfCell("B1", "=D1");
        s.SetContentsOfCell("C1", "=D1");

        var result = s.SetContentsOfCell("D1", "5");

        CollectionAssert.AreEqual(
            new List<string> { "D1", "C1", "B1", "A1" },
            result.ToList()
        );
    }

    [TestMethod]
    public void SetCellContentsOfCell_NoDependencyFormula_Equal()
    {
        var s = new Spreadsheet();

        s.SetContentsOfCell("A1", "B1 * C1");

        s.SetContentsOfCell("B1", "1");
        s.SetContentsOfCell("C1", "1.3");

        var result = s.SetContentsOfCell("A1", "=5");

        CollectionAssert.AreEqual(
            new List<string> { "A1" },
            result.ToList()
        );
    }

    [TestMethod]
    public void SetContentsOfCell_InvalidFormula_Throw()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=1+B1");
        Assert.Throws<FormulaFormatException>(() => s.SetContentsOfCell("B1", "=Hello"));
    }

    [TestMethod]
    public void SetContentsOfCell_FormulaCircleException_Throw()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=1");
        Assert.Throws<CircularException>(() => s.SetContentsOfCell("A1", "=A1"));
    }
    
    [TestMethod]
    public void GetCellValue_Negative_Equal()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=1-2");
        Assert.AreEqual(-1.0, s.GetCellValue("A1"));
    }

    // -- GetCellValue --
    [TestMethod]
    public void GetCellValue_StringValue_Equal()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("A1", "1+1");
        Assert.AreEqual("1+1", s.GetCellValue("A1"));
    }

    [TestMethod]
    public void GetCellValue_String_Equal()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("A1", "Hello");
        Assert.AreEqual("Hello", s.GetCellValue("A1"));
    }

    [TestMethod]
    public void GetCellValue_NumValue_Equal()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("A1", "1");
        Assert.AreEqual(1.0, s.GetCellValue("A1"));
    }

    [TestMethod]
    public void GetCellValue_FormulaNumValue_Equal()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=1+1");
        Assert.AreEqual(2.0, s.GetCellValue("A1"));
    }

    [TestMethod]
    public void GetCellValue_LongerFormula_Equal()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=1+B1");
        s.SetContentsOfCell("B1", "=1*C1");
        s.SetContentsOfCell("C1", "=1+D1");
        s.SetContentsOfCell("D1", "3");
        Assert.AreEqual(5.0, s.GetCellValue("A1"));
    }

    [TestMethod]
    public void GetCellValue_FormulaErrorValue_Equal()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=1+B1");
        s.SetContentsOfCell("B1", "Hello");
        Assert.IsInstanceOfType(s.GetCellValue("A1"), typeof(FormulaError));
    }

    [TestMethod]
    public void GetCellValue_FormulaError_Throw()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=1");
        Assert.Throws<CircularException>(() => s.SetContentsOfCell("A1", "=A1"));
    }
    
    [TestMethod]
    public void GetCellValue_NameUpperCase_Equal()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=1+B1");
        s.SetContentsOfCell("B1", "=1*1");
        Assert.AreEqual(2.0, s.GetCellValue("a1"));
    }
    // // -- Save And Load --

    [TestMethod]
    public void Save_FormulaVarValue_Equal()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=1+B1");
        s.SetContentsOfCell("B1", "=1");
        s.SetContentsOfCell("C1", "1");
        s.SetContentsOfCell("D1", "Hello");
        s.Save("save.txt");
        Assert.AreEqual(2.0, s["A1"]);
    }

    [TestMethod]
    public void SpreadSheetSave_FailsPath_Throw()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=1+B1");
        Assert.Throws<SpreadsheetReadWriteException>( () => s.Save("/some/nonsense/path.txt"));
    }
    

    [TestMethod]
    public void SaveAndLoad_SameFormulaValue_Equal()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=1+B1");
        s.SetContentsOfCell("B1", "=1");
        s.Save("S.txt");
        var t = new Spreadsheet("S.txt");
        Assert.AreEqual(2.0, t["A1"]);
    }

    [TestMethod]
    public void SaveAndLoad_MiniRepeatSaveAndLoad_Equal()
    {
        var s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=1+B1");
        s.SetContentsOfCell("B1", "=1");
        s.Save("1.txt");
        var t = new Spreadsheet("1.txt");
        t.SetContentsOfCell("C1", "Hello");
        t.Save("1.txt");
        var c = new Spreadsheet("1.txt");
        c.SetContentsOfCell("D1", "2.3");

        Assert.AreEqual(4, c.GetNamesOfAllNonemptyCells().Count());
    }

    [TestMethod]
    public void SpreadSheetLoad_LoadEmpty_Equal()
    {
        string sheet = ("{ \"Cells\" : {} }");

        File.WriteAllText("Empty.txt", sheet);

        var s = new Spreadsheet("Empty.txt");

        var amount = s.GetNamesOfAllNonemptyCells();

        Assert.AreEqual(0, amount.Count());
    }

    [TestMethod]
    public void SpreadSheetLoad_Content_Equal()
    {
        string sheet = ("{\"Cells\":{\"A1\":{\"StringForm\":\"=1\\u002BB1\"},\"B1\":{\"StringForm\":\"=1\"}}}");

        File.WriteAllText("load.txt", sheet);

        var s = new Spreadsheet("load.txt");

        Assert.AreEqual(2, s.GetNamesOfAllNonemptyCells().Count());
    }
    
    [TestMethod]
    public void SpreadSheetLoad_DoesntExist_Fails()
    {
        Assert.Throws<SpreadsheetReadWriteException>( () => new Spreadsheet("31231.txt") );
    }
    
    [TestMethod]
    public void SpreadSheetLoad_InvalidWrite_Throw()
    {
        File.WriteAllText("bad.txt", "{Json}");
        Assert.Throws<SpreadsheetReadWriteException>(() => new Spreadsheet("bad.txt"));
    }
    
    // -- StressTest --
    [TestMethod]
    public void StressTest_SaveLoad()
    {
        var s = new Spreadsheet();
        
        for ( int i = 1; i <= 100; i++)
            s.SetContentsOfCell($"A{i}", $"{i}");
        s.Save("big.txt");
        
        var t = new Spreadsheet("big.txt");
        
        Assert.AreEqual(100, t.GetNamesOfAllNonemptyCells().Count());
    }
    
    [TestMethod]
    public void StressTest_LongChain()
    {
        var s = new Spreadsheet();
        
        for ( int i = 1; i <= 100; i++)
            s.SetContentsOfCell($"A{i}", $"=A{i+1}");

        s.SetContentsOfCell("A100", "0");
        
        Assert.AreEqual(0.0, s.GetCellValue("A1"));
    }
    
    
}