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
    ///     UI controller for the SpreadSheet
    ///     Manages cell selection, content, value display, and error handling for exception
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
        ///   <para> Gets or sets the data for all the cells in the spreadsheet GUI. </para>
        ///   <remarks>Backing Store for HTML</remarks>
        /// </summary>
        private string[,] CellsBackingStore { get; set; } = new string[Rows, Cols];

        /// <summary>
        /// Provides an easy way to convert from an index to a letter (0 -> A)
        /// </summary>
        private char[] Alphabet { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        /// <summary>
        /// Allow access to spreadsheet methods, that allow it to store cell contents
        /// and values
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
        /// Coordinates of the selected cell
        /// </summary>
        private int _row;
        private int _col;

        /// <summary>
        /// A bool that determine whether to show the Error message or not
        /// </summary>
        private bool _showError;

        /// <summary>
        /// Error message display
        /// </summary>
        private string _errorMessage = "";
        
        /// <summary>
        /// Stores the last content assigned to a cell
        /// </summary>
        private string _lastChanged = "";
        
        /// <summary>
        /// Stack That Keeps track of the history of the cell's name and content that have been edited 
        /// </summary>
        private Stack<(string,string)> _undoHistoryStack = new Stack<(string,string)>();

        /// <summary>
        ///  Stack that keeps track of the cells that have been undone and their names 
        /// </summary>
        private Stack<(string,string)> _redoHistoryStack = new Stack<(string,string)>();
        
        /// <summary>
        ///   Gets or sets the name of the file to be saved
        /// </summary>
        private string FileSaveName { get; set; } = "Spreadsheet.sprd";

        /// <summary>
        /// Saves the current spreadsheet, by providing a download of a file
        /// containing the json representation of the spreadsheet.
        /// </summary>
        private async void SaveFile()
        {
            await JsRuntime.InvokeVoidAsync("downloadFile", FileSaveName,
                _currentSheet.JsonPath());
        }
        

        /// <summary>
        /// This method will run when the file chooser is used, for loading a file.
        /// Uploads a file containing a json representation of a spreadsheet, and 
        /// replaces the current sheet with the loaded one.
        /// </summary>
        /// <param name="args">The event arguments, which contains the selected file name</param>
        private async void HandleFileChooser(EventArgs args)
        {
            try
            {
                string fileContent;

                InputFileChangeEventArgs eventArgs =
                    args as InputFileChangeEventArgs ?? throw new Exception("unable to get file name");
                if (eventArgs.FileCount == 1)
                {
                    var file = eventArgs.File;

                    using var stream = file.OpenReadStream();
                    using var reader = new StreamReader(stream);

                    // fileContent will contain the contents of the loaded file
                    fileContent = await reader.ReadToEndAsync();

                    _currentSheet.JsonReplace(fileContent);
                    FileSaveName = file.Name;
                    _selectedCell = "A1";
                    UpdateSpreadSheet();
                    StateHasChanged();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("an error occurred while loading the file..." + e);
            }
        }
        
        /// <summary>
        /// Handler for when a cell is clicked
        /// </summary>
        /// <param name="row">The row component of the cell's coordinates</param>
        /// <param name="col">The column component of the cell's coordinates</param>
        private void CellClicked(int row, int col)
        {
            char letter = Alphabet[col];
            string cell = $"{letter}{row + 1}";
            _selectedCell = cell;

            _selectedContent = _currentSheet.GetCellContents(cell).ToString() ?? "";
            _row = row;
            _col = col;
            _contentsBox.FocusAsync();
        }
        
        /// <summary>
        /// Change the contents of the selected cell by setting it s content in the
        /// SpreadSheet, then store the evaluated value of that cell into the Cell
        /// BackingStore. After it refreshes the entire spreadsheet by calling UpdateSpreadSheet
        /// If a CircularException or any other exception occurs, display an error message
        /// </summary>
        private void ContentsChangedHandler()
        {
            try
            {
                string oldContent = _currentSheet.GetCellContents(_selectedCell).ToString() ?? "";
                _currentSheet.SetContentsOfCell(_selectedCell, _selectedContent);
                CellsBackingStore[_row, _col] = _currentSheet.GetCellValue(_selectedCell).ToString() ?? "";
                UpdateSpreadSheet();
                _lastChanged = _currentSheet.GetCellContents(_selectedCell).ToString() ?? "";
                _undoHistoryStack.Push((_selectedCell, oldContent));
                _redoHistoryStack.Clear();
            }
            catch (CircularException)
            {
                _showError = true;
                _errorMessage = "Circular Dependency Error";
            }
            catch (KeyNotFoundException)
            {
                CellsBackingStore[_row, _col] =  "";
                UpdateSpreadSheet();
            }
            catch (Exception)
            {
                _showError = true;
                _errorMessage = "Invalid Formula Error";
            }
        }

        /// <summary>
        /// HIdes the error popup displayed after an exception
        /// </summary>
        private void DismissError()
        {
            _showError = false;
        }

        /// <summary>
        /// Clears the spreadsheet by looping through and setting them back to a empty string
        /// </summary>
        private void ClearSpreadsheet()
        {
            _currentSheet = new();
            _selectedContent = "";
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    CellsBackingStore[r, c] = "";
                }
            }
        }

        /// <summary>
        /// Recomputes and updates the displayed values for every cell
        /// Called after any change that may affect the cell values
        /// </summary>
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
                        CellsBackingStore[r, c] = _currentSheet.GetCellValue(cell).ToString() ?? "";

                    }
                    catch (Exception)
                    {
                        CellsBackingStore[r, c] = "";
                    }
                }
            }
        }

        /// <summary>
        /// Reverts the most recent cell edit using the undo stacks.
        /// Moves the reverted change into the redo stacks
        /// </summary>
        private void UndoContent()
        {
            if (_undoHistoryStack.Count > 0)
            {
                (string cell ,string content) poppedCell = _undoHistoryStack.Pop();
                
                string currentContent = _currentSheet.GetCellContents(poppedCell.cell).ToString() ?? "";
                _redoHistoryStack.Push((poppedCell.cell, currentContent));
                
                _currentSheet.SetContentsOfCell(poppedCell.cell, poppedCell.content);
                _selectedContent = _currentSheet.GetCellContents(poppedCell.cell).ToString() ?? "";
                _selectedCell = poppedCell.cell;
            }
            else
            {
                _currentSheet.SetContentsOfCell(_selectedCell, "");
                _selectedContent = "";
            }
            UpdateSpreadSheet();
        }

        /// <summary>
        /// Readd the previously undone edit using the redo stacks.
        /// If redo is empty, restores theh last known content.
        /// </summary>
        private void RedoContent()
        {
            if (_redoHistoryStack.Count > 0)
            {
              
                (string cell, string content) poppedCell = _redoHistoryStack.Pop();
                
                string currentContent = _currentSheet.GetCellContents(poppedCell.cell).ToString() ?? "";
                _undoHistoryStack.Push((poppedCell.cell, currentContent));
                
                _currentSheet.SetContentsOfCell(poppedCell.cell, poppedCell.content);
                _selectedContent = _currentSheet.GetCellContents(poppedCell.cell).ToString() ?? "";
                _selectedCell = poppedCell.cell;
            }
            else
            {
               _currentSheet.SetContentsOfCell(_selectedCell, _lastChanged);
               _selectedContent = _lastChanged;
            }
          
            UpdateSpreadSheet();
        }
    }
    
    