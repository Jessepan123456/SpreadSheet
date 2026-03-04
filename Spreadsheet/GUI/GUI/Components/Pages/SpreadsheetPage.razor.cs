// <copyright file="SpreadsheetPage.razor.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Spreadsheets;

namespace GUI.Components.Pages;

/// <summary>
/// TODO: Fill in
/// </summary>
public partial class SpreadsheetPage 
{
    /// <summary>
    /// Based on your computer, you could shrink/grow this value based on performance.
    /// </summary>
    private const int Rows = 50;

    /// <summary>
    /// Number of columns, which will be labeled A-Z.
    /// </summary>
    private const int Cols = 26;

    /// <summary>
    /// Allow access to spreadsheet methods
    /// </summary>
    private Spreadsheet _currentSheet = new Spreadsheet();

    /// <summary>
    /// Holds the selectedCell
    /// </summary>
    private String _selectedCell = "A1";
    /// <summary>
    /// Holds the selectedContent
    /// </summary>
    private String _selectedContent = "";
    
    /// <summary>
    /// Allows the contentsBox to be uniquely display
    /// </summary>
    private ElementReference _contentsBox;

    /// <summary>
    /// Provides an easy way to convert from an index to a letter (0 -> A)
    /// </summary>
    private char[] Alphabet { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    
    /// <summary>
    ///   Gets or sets the name of the file to be saved
    /// </summary>
    private string FileSaveName { get; set; } = "Spreadsheet.sprd";
    
    /// <summary>
    ///   <para> Gets or sets the data for all the cells in the spreadsheet GUI. </para>
    ///   <remarks>Backing Store for HTML</remarks>
    /// </summary>
    private string[,] CellsBackingStore { get; set; } = new string[Rows, Cols];

    private int _row = 0;
    
    private int _col = 0;
    
    /// <summary>
    /// Handler for when a cell is clicked
    /// </summary>
    /// <param name="row">The row component of the cell's coordinates</param>
    /// <param name="col">The column component of the cell's coordinates</param>
    private void CellClicked( int row, int col )
    {
        char letter = Alphabet[col];
        string cell = $"{letter}{row + 1}";
        _selectedCell = cell;

        _selectedContent = _currentSheet.GetCellContents(cell)?.ToString() ?? "";
        _row = row;
        _col = col;

        _contentsBox.FocusAsync();
    }


    /// <summary>
    /// Saves the current spreadsheet, by providing a download of a file
    /// containing the json representation of the spreadsheet.
    /// </summary>
    private async void SaveFile()
    {
        await JsRuntime.InvokeVoidAsync( "downloadFile", FileSaveName, 
            _currentSheet.JsonPath() );
    }

    /// <summary>
    /// This method will run when the file chooser is used, for loading a file.
    /// Uploads a file containing a json representation of a spreadsheet, and 
    /// replaces the current sheet with the loaded one.
    /// </summary>
    /// <param name="args">The event arguments, which contains the selected file name</param>
    private async void HandleFileChooser( EventArgs args )
    {
        try
        {
            string fileContent = string.Empty;

            InputFileChangeEventArgs eventArgs = args as InputFileChangeEventArgs ?? throw new Exception("unable to get file name");
            if ( eventArgs.FileCount == 1 )
            {
                var file = eventArgs.File;
                if ( file is null )
                {
                    return;
                }

                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);

                // fileContent will contain the contents of the loaded file
                fileContent = await reader.ReadToEndAsync();

                _currentSheet.JsonReplace(fileContent);

                _selectedCell = "A1";
                
                UpdateSpreadSheet();
                StateHasChanged();
            }
        }
        catch ( Exception e )
        {
            Debug.WriteLine( "an error occurred while loading the file..." + e );
        }
    }

    private void ContentsChangedHandler(ChangeEventArgs obj)  //might be better to implement some try catch - Max
    {
        try
        {
            string contents = obj.Value as string ?? "BIG ERROR CHECK YOUR CODE";
            _currentSheet.SetContentsOfCell(_selectedCell,contents);
            CellsBackingStore[_row, _col] = _currentSheet.GetCellValue(_selectedCell).ToString() ?? "";
            UpdateSpreadSheet();
        }
        catch (NotImplementedException)
        {
            throw new NotImplementedException();
        }
    }

    private void UpdateSpreadSheet()
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                char letter = Alphabet[c];
                string cell = $"{letter}{r + 1}";
                try
                {
                    CellsBackingStore[r, c] = _currentSheet.GetCellValue(cell)?.ToString()??"";
                }
                catch (Exception)
                {
                    CellsBackingStore[r, c] = "";
                }
            }
        }
    }
}
