using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace ExamWpfApp;

public class Cell
{
    public int Row { get; set; }
    public int Column { get; set; }
    public CellState State;

    public Cell(int row, int column, CellState state)
    {
        Row = row;
        Column = column;
        State = state;
    }

    public Cell() : this(0, 0, CellState.Empty)
    {
        
    }
}

public class Turn
{
    public Cell StartCell { get; set; }
    public Cell EndCell { get; set; }

    public Turn(Cell startCell, Cell endCell)
    {
        StartCell = startCell;
        EndCell = endCell;
    }

    public bool CanMove()
    {
        switch (StartCell.State)
        {
            case CellState.Empty:
                break;
            case CellState.Black:
                // if (StartCell.Row)
                // {
                //     
                // }
                break;
            case CellState.BlackQueen:
                break;
            case CellState.White:
                break;
            case CellState.WhiteQueen:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        return false;
    }
}

public class CheckerBoard
{
    private CellState[,] _board;
    private int _boardSize;

    public CheckerBoard()
    {
        _boardSize = 8;
        for (int row = 0; row < _boardSize; row++)
        {
            for (int col = 0; col < _boardSize; col++)
            {
                _board[row, col] = CellState.Empty;
            }
        }
    }

    public void SetState(int row, int col, CellState state) => _board[row, col] = state;
    public CellState GetState(int row, int col) => _board[row, col];
    
    
}