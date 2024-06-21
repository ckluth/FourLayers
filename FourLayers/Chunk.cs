using System.Collections.Generic;

namespace FourLayers;

public class Chunk
{
    const int Kerne = 16;

    public byte SlotStart { get; set; }

    public byte SlotSize { get; set; }

    public (List<byte> Moves, Stats Stats) Result { get; set; }

    public static (List<Chunk> chunks, int kerne) CreateChunks(byte fieldWidth)
    {
        var chunks = new List<Chunk>();
        
        if (fieldWidth == 4)
        {
            for (var i = 0; i < Kerne; i++)
            {
                chunks.Add(new Chunk
                {
                    SlotStart = (byte)i,
                    SlotSize = 1,
                });
        
            }
        }
        else if (fieldWidth == 6)
        {
            for (var i = 0; i < 8; i++)
            {
                chunks.Add(new Chunk
                {
                    SlotStart = (byte)i,
                    SlotSize = 1,
                });
            }

            for (var i = 8; i < Kerne; i++)
            {
                chunks.Add(new Chunk
                {
                    SlotStart = (byte)(8 + ((i - 8) * 2)),
                    SlotSize = 2,
                });
            }
        }
        else
        {
            for (var i = 0; i < Kerne; i++)
            {
                chunks.Add(new Chunk
                {
                    SlotStart = (byte)(i * 2),
                    SlotSize = 2,
                });
            }
        }
        return (chunks, Kerne);
    }
}