using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;
using System.Text.Json;

namespace FourLayers;

public class Program
{
    public static void Main(string[] args)
    {
        Thread.Sleep(100);

#pragma warning disable CA1416
        Console.Beep(800, 300);
#pragma warning restore CA1416
        MaximizeConsoleWindow();

        Main_RunOneChallenge(args, 9);

        //Main_RunChallengeSequence([19]);
        //Main_RunChallengeSequence();

#pragma warning disable CA1416
        Console.Beep(1200, 1000);
#pragma warning restore CA1416

        Console.WriteLine("[PRESS KEY]");
        Console.ReadKey();
    }

    public static void Main_RunOneChallenge(string[] args, byte nr)
    {
        var challengeNr = args != null && args.Length > 0 ? byte.Parse(args[0]) : nr;
        var challenge = FourLayers.Challenges.Single(ch => ch.Number == challengeNr);

        RunOneChallenge(challenge, Console.WriteLine);
    }

    public static void Main_RunChallengeSequence(List<byte> challengeNumbers = null)
    {
        if (challengeNumbers == null)
        {
            challengeNumbers = new List<byte>();
            for (byte i = 1; i <= 19; i++) challengeNumbers.Add(i);
            challengeNumbers = challengeNumbers.Except(new List<byte> { 18 }).ToList();
            //challengeNumbers = challengeNumbers.Except(new List<byte> { 18, 9, 15, 16 }).ToList();
        }

        DoRunChallengeSequence(challengeNumbers);
    }

    public static void DoRunChallengeSequence(List<byte> challengeNumbers)
    {
        var challenges = challengeNumbers.Select(ch => FourLayers.Challenges.Single(c => c.Number == ch)).ToList();
        var allStats = challenges.Select(challenge => RunOneChallenge(challenge, Console.WriteLine)).ToList();
        
        var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var challengNumbersStr = challenges.Select(c => c.Number.ToString()).Aggregate((n1, n2) => n1 + "-" + n2);
        var fname = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}-({challengNumbersStr}).json";
        var fpath = Path.Combine(folder!, fname);
        var jsonString = Stats.Serialze2Json(allStats);
        File.WriteAllText(fpath, jsonString, Encoding.UTF8);
        Console.WriteLine($"Stats written to {fpath}\r\n");
        
        fname = fname.Replace(".json", ".csv");
        fpath = Path.Combine(folder!, fname);
        var csvString = Stats.Serialze2Csv(allStats);
        File.WriteAllText(fpath, csvString, Encoding.UTF8);
        Console.WriteLine($"Stats written to {fpath}\r\n");
    }

    private static Stats RunOneChallenge(Challenge challenge, Action<string> cw = null)
    {
        var field = challenge.Field;

        var statsIn = new Stats(challenge.Number, challenge.MaxMoves, challenge.FieldWidth);

        var str = PrintSessionParameter(statsIn);

        cw?.Invoke(str);

        if (cw != null)
            FourLayers.PrintField(field);

        cw?.Invoke($"Started: {DateTime.Now:HH:mm:ss}");
        cw?.Invoke("\r\nProcessing...\r\n");

        var (moves, stats) = FourLayers.SolveChallenge(challenge, statsIn);

        cw?.Invoke(PrintSessionResults(stats));

        if (cw != null && moves != null)
        {
            var movesStrs = FourLayers.DecodeMoves(moves, challenge.FieldWidth);
            movesStrs.ForEach(cw);
            cw?.Invoke(string.Empty);
            field = FourLayers.ProcessMoves(field, moves);
            FourLayers.PrintField(field);
        }

        return stats;
    }

    public static string PrintSessionParameter(Stats stats)
    {
        var sb = new StringBuilder();
        const byte len = 15;
        sb.AppendLine($"{nameof(stats.ChallengeNr).PadRight(len, ' ')}: {stats.ChallengeNr:N0}.");
        sb.AppendLine("");
        sb.AppendLine($"{nameof(stats.FieldWidth).PadRight(len, ' ')}: {stats.FieldWidth:N0}");
        sb.AppendLine($"{nameof(stats.MaxMoves).PadRight(len, ' ')}: {stats.MaxMoves:N0}");
        sb.AppendLine("");
        sb.AppendLine($"{nameof(stats.MaxNodes).PadRight(len, ' ')}: {stats.MaxNodes:N0}");
        return sb.ToString();
    }
    
    public static string PrintSessionResults(Stats stats)
    {
        var sb = new StringBuilder();
        const byte len = 15;
        sb.AppendLine($"{nameof(stats.Duration).PadRight(len, ' ')}: {stats.Duration}.");
        sb.AppendLine($"{nameof(stats.NodesCreated).PadRight(len, ' ')}: {stats.NodesCreated:N0}");
        sb.AppendLine($"{nameof(stats.NodesFollowed).PadRight(len, ' ')}: {stats.NodesFollowed:N0}");
        sb.AppendLine($"{nameof(stats.NodesCompared).PadRight(len, ' ')}: {stats.NodesCompared:N0}");
        sb.AppendLine($"{nameof(stats.NodesDiscarded).PadRight(len, ' ')}: {stats.NodesDiscarded:N0}");
        sb.AppendLine("\r\n"+stats.MovesStr);
        return sb.ToString();
    }
    
    #region MaximizeConsoleWindow

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    [DllImport("user32.dll")]
    static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);
    [DllImport("user32.dll")]
    static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);
    private static void MaximizeConsoleWindow()
    {
        Thread.Sleep(100);
        const int SW_MAXIMIZE = 3;
        const int SW_RESTORE = 9;
        var consoleWindowHandle = GetForegroundWindow();
        ShowWindow(consoleWindowHandle, SW_RESTORE);        
        ShowWindow(consoleWindowHandle, SW_MAXIMIZE);
        GetWindowRect(consoleWindowHandle, out var screenRect);
        var width = screenRect.Right - screenRect.Left;
        var height = screenRect.Bottom - screenRect.Top;
        MoveWindow(consoleWindowHandle, (int)screenRect.Left, (int)screenRect.Top, (int)width, (int)height, true);
    }
    public struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    #endregion

}