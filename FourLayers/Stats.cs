using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Net.WebSockets;
using System.Numerics;
using System.Text;

namespace FourLayers;

public class Stats
{
    private Stats() {}

    public Stats(byte challengeNr, byte maxMoves, byte fieldWidth)
    {
        this.ChallengeNr = challengeNr;
        this.MaxMoves = maxMoves;
        this.FieldWidth = fieldWidth;

        this.MaxNodes = 1;
        for (var i = 0; i < maxMoves; i++)
        {
            MaxNodes = BigInteger.Add(MaxNodes, BigInteger.Pow(this.FieldWidth*4, i + 1));
        }
    }
    
    public byte ChallengeNr { get; init; }
    public byte FieldWidth { get; init; }
    public byte MaxMoves { get; init; }
    public BigInteger MaxNodes { get; init; }
    public decimal NodesCreated { get; set; }
    public decimal NodesCompared { get; set; }

    public static Stats Clone(Stats stats)
    {
        var result = new Stats
        {
            ChallengeNr = stats.ChallengeNr,
            FieldWidth = stats.FieldWidth,
            MaxMoves = stats.MaxMoves,
            MaxNodes = stats.MaxNodes,
            NodesCreated = stats.NodesCreated,
            NodesCompared = stats.NodesCompared,
        };

        return result;
    }

    public static Stats Aggregate(List<Stats> stats)
    {
        var result = Clone(stats.First());
        foreach (var stat in stats)
        {
            result.NodesCreated = decimal.Add(result.NodesCreated, stat.NodesCreated);
            result.NodesCompared = decimal.Add(result.NodesCompared, stat.NodesCompared);
        }

        return result;
    }


}