using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;

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
    public string MaxNodesStr => $"{MaxNodes:N0}";
    public decimal NodesCreated { get; set; }
    public decimal NodesDiscarded { get; set; }
    public decimal NodesFollowed { get; set; }
    public decimal NodesCompared { get; set; }
    public TimeSpan Duration { get; set; }
    public List<byte> Moves { get; set; }
    public string MovesStr => this.Moves == null ? "---NO SOLUTION---" : $"{this.Moves.Count} Moves: " + this.Moves.Select(m => m.ToString()).Aggregate((n1, n2) => n1 + "-" + n2);

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
            NodesFollowed = stats.NodesFollowed,
            NodesDiscarded = stats.NodesDiscarded,
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
            result.NodesFollowed = decimal.Add(result.NodesFollowed, stat.NodesFollowed);
            result.NodesDiscarded = decimal.Add(result.NodesDiscarded, stat.NodesDiscarded);
        }

        return result;
    }

    public static string Serialze2Json(List<Stats> statsList)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var jsonString = JsonSerializer.Serialize(statsList, options);
        return jsonString;
    }

    public static string Serialze2Csv(List<Stats> statsList)
    {
        var sb = new StringBuilder();

        var header = $"{nameof(Stats.ChallengeNr)};{nameof(Stats.FieldWidth)};{nameof(Stats.MaxMoves)};{nameof(Stats.MaxNodes)};{nameof(Stats.NodesCreated)};{nameof(Stats.NodesFollowed)};{nameof(Stats.NodesCompared)};{nameof(Stats.NodesDiscarded)};{nameof(Stats.MovesStr)};{nameof(Stats.Duration)}";
        sb.AppendLine(header);

        foreach (var stats in statsList)
        {
            var line = $"{stats.ChallengeNr};{stats.FieldWidth};{stats.MaxMoves:N0};{stats.MaxNodes:N0};{stats.NodesCreated:N0};{stats.NodesFollowed:N0};{stats.NodesCompared:N0};{stats.NodesDiscarded:N0};{stats.MovesStr};{stats.Duration.TotalHours:00}:{stats.Duration.Minutes:00}:{stats.Duration.Seconds:00}:{stats.Duration.Milliseconds:000}";
            sb.AppendLine(line);
        }

        return sb.ToString();
    }

}