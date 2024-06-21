using System;
using System.Collections.Generic;

namespace FourLayers;

public class Slot
{
    //
    // Für die Parallelisierung
    //

    public byte SlotStart { get; set; }

    public byte SlotSize { get; set; }

    public (List<byte> Moves, Stats Stats) Result { get; set; }

    public static (List<Slot> Slots, int Kerne) CreateSlots(byte fieldWidth, byte? processorCount = null)
    {
        processorCount ??= (byte)Environment.ProcessorCount;

        var moveCount = fieldWidth * 4;
        var result = new List<Slot>();
        var baseSlotSize = moveCount / processorCount.Value;
        var remainder = moveCount % processorCount.Value;
        var currentStart = 0;
        for (var i = 0; i < processorCount.Value && currentStart < moveCount; i++)
        {
            var slotSize = baseSlotSize + (i < remainder ? 1 : 0);
            if (slotSize <= 0) continue;
            result.Add(new Slot
            {
                SlotStart = (byte)currentStart,
                SlotSize = (byte)slotSize
            });
            currentStart += slotSize;
        }

        return (result, processorCount.Value);
    }
}