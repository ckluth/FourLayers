using System.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Numerics;
using System.Text.RegularExpressions;

namespace FourLayers;

public static partial class FourLayers
{
    static FourLayers()
    {
        InitChallenges();
    }
    
    public static byte[,] ProcessMoves(byte[,] matrix, IEnumerable<byte> moves)
    {
        var result = (byte[,])matrix.Clone();
        var fieldWidth = (byte)matrix.GetLength(0);
        var lookup = CreateOrderedField(fieldWidth);
        
        foreach (var move in moves)
        {
            var (mx, _, _, _, _) = ProcessMove(result, fieldWidth, lookup, move);
            result = mx;

        }
        return result;
    }

    public static 
        (byte[,] NewField, bool IsUnchanged, bool WasAffectedLineInOrder, bool IsColumnMove, int lineDelta) 
        ProcessMove(byte[,] oldField, byte fieldWidth, byte [,] orderedFieldLookup, byte move)
    {
        var newField = (byte[,])oldField.Clone();

        var isColumn = move < fieldWidth * 2;
        var isForwards = isColumn ? move < fieldWidth : move < fieldWidth * 3;

        var isUnchanged = true;

        if (isColumn && isForwards) // Column, Down
        {
            var last = newField[fieldWidth - 1, move];
            var wasAffectedLineInOrder = last == orderedFieldLookup[(byte)(fieldWidth - 1), move];
            for (var i = fieldWidth - 1; i > 0; i--)
            {
                isUnchanged = isUnchanged && newField[i - 1, move] == last;
                wasAffectedLineInOrder = wasAffectedLineInOrder && newField[i - 1, move] == orderedFieldLookup[(byte)(i - 1), move];
                newField[i, move] = newField[i - 1, move];

            }
            newField[0, move] = last;
            var lineDelta = move << (fieldWidth / 2 - 2);
            return (newField, isUnchanged, wasAffectedLineInOrder, true, lineDelta);
        }

        if (isColumn) // Column, Up
        {
            var col = move - fieldWidth;
            var first = newField[0, col];
            var wasAffectedLineInOrder = first == orderedFieldLookup[0,(byte)col];
            for (var i = 0; i < fieldWidth - 1; i++)
            {
                isUnchanged = isUnchanged && newField[i + 1, col] == first;
                wasAffectedLineInOrder = wasAffectedLineInOrder && newField[i + 1, col] == orderedFieldLookup[(byte)(i + 1), (byte)col];

                newField[i, col] = newField[i + 1, col];
            }
            newField[fieldWidth - 1, col] = first;
            var lineDelta = -(col << (fieldWidth / 2 - 2));
            return (newField, isUnchanged, wasAffectedLineInOrder, true, lineDelta);
        }

        if (isForwards) // Row, Right
        {
            var row = move - fieldWidth * 2;
            var last = newField[row, fieldWidth - 1];

            var wasAffectedLineInOrder = last == orderedFieldLookup[(byte)row,(byte)(fieldWidth - 1)];
            
            for (var i = fieldWidth - 1; i > 0; i--)
            {
                isUnchanged = isUnchanged && newField[row, i - 1] == last;
                wasAffectedLineInOrder = wasAffectedLineInOrder && newField[row, i - 1] == orderedFieldLookup[(byte)row, (byte)(i - 1)];

                newField[row, i] = newField[row, i - 1];
            }
            newField[row, 0] = last;
            var lineDelta = row << (fieldWidth / 2 - 2);
            return (newField, isUnchanged, wasAffectedLineInOrder, false, lineDelta);
        }

        { // Row, Left
            
            var row = move - fieldWidth * 3;
            var first = newField[row, 0];

            var wasAffectedLineInOrder = first == orderedFieldLookup[(byte)row, 0];

            for (var i = 0; i < fieldWidth - 1; i++)
            {
                isUnchanged = isUnchanged && newField[row, i + 1] == first;
                wasAffectedLineInOrder = wasAffectedLineInOrder && newField[row, i + 1] == orderedFieldLookup[(byte)row, (byte)(i + 1)];


                newField[row, i] = newField[row, i + 1];
            }
            newField[row, fieldWidth - 1] = first;
            var lineDelta = -(row << (fieldWidth / 2 - 2));
            return (newField, isUnchanged, wasAffectedLineInOrder, false, lineDelta);
        }
    }

    public static byte[,] CreateOrderedField(int fieldWidth)
    {
        /*
           Creates an orderd, layered Field for a given fieldWidth.

           Sample for fieldWith 6 (=> layerCount 3):

             {2,2,2,2,2,2},
             {2,1,1,1,1,2},
             {2,1,0,0,1,2},
             {2,1,0,0,1,2},
             {2,1,1,1,1 2},
             {2,2,2,2,2,2},

             Algo:
               -   Loop over rows and cols
               -     Input: row and col
               -     First we reduce row and col to the upper-left quadrant
               -     Then each line - row and col - belongs to a layer which is the distance to the "center-cell" of the quadrant(the lower-right cell)
               -     The greater of both layer-values ist the layer-value for the currrent cell[row, col]
         */

        var result = new byte[fieldWidth, fieldWidth];
        var layerCount = fieldWidth / 2;

        for (byte row = 0; row < fieldWidth; row++)
        {
            var quadrantenRow = row < layerCount ? row : layerCount - ((row % layerCount) + 1);
            var rowLayer = layerCount - quadrantenRow - 1;
            
            for (byte col = 0; col < fieldWidth; col++)
            {
                var quadrantenCol = col < layerCount ? col : layerCount - ((col % layerCount) + 1);
                var colLayer = layerCount - quadrantenCol - 1;

                var layer = Math.Max(rowLayer, colLayer);
                result[row, col] = (byte)layer;

            }
        }
        return result;
    }

    public static void PrintField(byte[,] matrix, bool useColors=true)
    {
        var colors = new ConsoleColor[]
        {
            ConsoleColor.Blue,
            ConsoleColor.Green,
            ConsoleColor.Yellow,
            ConsoleColor.Red,
        };

        var prevCol = Console.ForegroundColor;
        var size = matrix.GetLength(0);
        var offset = 0;
        if (size < 8) offset++;
        if (size < 6) offset++;

        for (var row = 0; row < size; row++)
        {
            for (var col = 0; col < size; col++)
            {
                var layer = matrix[row, col];
                Console.ForegroundColor = colors[layer + offset];
                Console.Write(useColors ? "\u25a0 " : layer + " ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
        Console.ForegroundColor = prevCol;
    }
    
    public static List<string> DecodeMoves(IEnumerable<byte> moves, byte fieldWidth)
    {
        //
        // Übersetzt eine Sequenz von Moves in eine Liste von "human-readable"-Strings
        //

        /*----------------------------------------------------------------
           Move-Codierung:

           Anzahl der möglichen Züge: FieldWidth * 4

           Erstes Viertel : Cols Forth
           Zweites Viertel: Cols Back
           Drittes Viertel: Rows Forth
           Viertes Viertel: Rows Back

           Sample für FieldWidth 4:

           0 -> Col 0 Down
           1 -> Col 1 Down
           2 -> Col 2 Down
           3 -> Col 3 Down

           4 -> Col 0 Up
           5 -> Col 1 Up
           6 -> Col 2 Up
           7 -> Col 3 Up

           8  -> Row 0 Right
           9  -> Row 1 Right
           10 -> Row 2 Right
           11 -> Row 3 Right

           12 -> Row 0 Left
           13 -> Row 1 Left
           14 -> Row 2 Left
           15 -> Row 3 Left

         ----------------------------------------------------------------------
        */

        var result = new List<string>();

        foreach (var move in moves)
        {
            var isColumn = move < fieldWidth * 2;
            var isForwards = isColumn ? move < fieldWidth : move < fieldWidth * 3;

            if (isColumn && isForwards) // Column, Down
            {
                result.Add("Col-" + (move + 1) + " DOWN");
            }
            else if (isColumn) // Column, Up
            {
                result.Add("Col-" + ((move - fieldWidth) + 1) + " UP");
            }
            else if (isForwards) // Row, Right
            {
                result.Add("Row-" + ((move - fieldWidth * 2) + 1) + " RIGHT");
            }
            else
            {
                // Row, Left
                result.Add("Row-" + ((move - fieldWidth * 3) + 1) + " LEFT");
            }

        }
        return result;
    }

    public static (byte[,] Field, byte FieldWidth) DeserializeField(string strField)
    {
        string ExtractDigits(string input)
        {
            var result = "";
            foreach (Match match in new Regex(@"\d").Matches(input))
                result += match.Value;
            return result;
        }
        var rows = strField.Split("\r\n").Select(ExtractDigits).Where(s => !string.IsNullOrEmpty(s.Trim())).ToArray();
        var fieldWidth = rows.Length;
        var result = new byte[fieldWidth,fieldWidth];
        for (var row = 0; row < fieldWidth; row++)
        {
            for (var col = 0; col < fieldWidth; col++)
            {
                var ch = rows[row][col];
                result[row, col] = (byte)(ch - '0');
            }
        }
        return (result, (byte)result.GetLength(0));
    }
}