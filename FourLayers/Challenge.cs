using System;

namespace FourLayers;

public class Challenge
{
    public int Number { get; set; }

    public byte[,] Field { get; set; }
    
    public byte FieldWidth => (byte)Field.GetLength(0);

    public byte MaxMoves { get; set; }
}

