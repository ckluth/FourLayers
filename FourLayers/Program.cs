using System;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace FourLayers;

public class Program
{
    public static void Main(string[] args)
    {
        var challengeNr = args != null && args.Length > 0 ? byte.Parse(args[0]) : (byte)14;

        RunChallenge(challengeNr);
        
        Console.WriteLine("[PRESS KEY]");
        Console.ReadKey();
    }

    private static void RunChallenge(byte challengeNr)
    {
        var challenge = FourLayers.Challenges.Single(ch => ch.Number == challengeNr);
        var field = challenge.Field;

        var st = new Stats(challengeNr, challenge.MaxMoves, challenge.FieldWidth);
        
        var str = PrintSessionParameter(st);
        Console.WriteLine(str);

        FourLayers.PrintField(field);

        var sw = Stopwatch.StartNew();

        Console.WriteLine($"Started: {DateTime.Now:HH:mm:ss}");
        Console.WriteLine();
        Console.WriteLine("Processing...");

        MaximizeConsoleWindow();

        var (moves, stats) = FourLayers.SolveChallenge(challenge, st);
        
        Console.WriteLine();
        Console.WriteLine("+++ +++ +++ [ELAPSED: " + sw.Elapsed + "] +++ +++ +++ \r\n");
        
        if (moves == null)
        {
            Console.WriteLine("---NO SOLUTION---\r\n");
            Console.WriteLine(PrintSessionResults(stats));
        }
        else
        {
            Console.WriteLine(PrintSessionResults(stats));
            Console.WriteLine("Moves " + moves.Select(m => m.ToString()).Aggregate((n1, n2) => n1 + "-" + n2));
            Console.WriteLine();

            var movesStrs = FourLayers.DecodeMoves(moves, challenge.FieldWidth);
            movesStrs.ForEach(Console.WriteLine);
            Console.WriteLine();

            field = FourLayers.ProcessMoves(field, moves);
            FourLayers.PrintField(field);
        }
        
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
        sb.AppendLine($"{nameof(stats.NodesCreated).PadRight(len, ' ')}: {stats.NodesCreated:N0}");
        sb.AppendLine($"{nameof(stats.NodesCompared).PadRight(len, ' ')}: {stats.NodesCompared:N0}");
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