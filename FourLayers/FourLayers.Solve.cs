using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FourLayers;

public static partial class FourLayers
{
    private static int isCancelled = 0;

    public static (List<byte> Moves, Stats Stats) SolveChallenge(Challenge challenge, Stats stats) 
    {
        CurrentOrderedFieldLookUp = CreateOrderedFieldLookUp(challenge.FieldWidth);

        Node CreateRoot()
        {
            var root = new Node
            {
                Field = challenge.Field,
                FieldWidth = (byte)challenge.FieldWidth,
                Depth = 0,
                MoveFromParent = 255,
                Rating = 255,

            };
            return root;
        }

        var (chunks, kerne) = Chunk.CreateChunks(challenge.FieldWidth);
        var resultingStats = new List<Stats>();

        try
        {
            Parallel.ForEach(chunks, new ParallelOptions { MaxDegreeOfParallelism = kerne }, chunk =>
                {
                    var chunkStats = Stats.Clone(stats);
                    var startNode = CreateRoot();
                    chunk.Result = InternalSolveChallenge(startNode, challenge.MaxMoves, chunkStats, chunk.SlotStart, chunk.SlotSize);
                    resultingStats.Add(chunkStats);
                }
            );
        }
        catch (AggregateException ae)
        {
            foreach (var ex in ae.Flatten().InnerExceptions)
            {
                Console.WriteLine(ex.Message);
            }
            return (null, null);
        }
        var successFullChunk = chunks.FirstOrDefault(c => c.Result.Moves != null);
        var moves = successFullChunk?.Result.Moves;
        return (moves, Stats.Aggregate(resultingStats));
    }

    public static (List<byte> Moves, Stats Stats) InternalSolveChallenge(Node root, byte maxMoves, Stats stats, byte slotStart, byte slotSize)
    {
        var stack = new Stack<Node>();
        stack.Push(root);
        stats.NodesCreated = 1;

        while (stack.Count > 0)
        {
            if (isCancelled > 0)
                return (null, stats);

            var currentNode = stack.Pop();

            // Gelöst?
            stats.NodesCompared++;
            if (currentNode.IsSolved)
            {
                Interlocked.Increment(ref isCancelled);
                var moves = currentNode.Path;
                return (moves, stats);
            }

            // Noch keine Children?
            if (!currentNode.IsChildrenProcessed && currentNode.Children == null && currentNode.Depth < maxMoves)
            {
                currentNode.Children = currentNode.Depth == 0
                    ? PopulateNode(currentNode, slotStart, slotSize)
                    : PopulateNode(currentNode, 0, (byte)(currentNode.FieldWidth * 4));

                // Ende
                if (currentNode.Children.Count == 0) return (null, stats);

                stats.NodesCreated += currentNode.Children.Count;
                stack.Push(currentNode.Children[0]);
                continue;
            }

            // Noch Siblings?
            if (currentNode.Parent != null 
                && currentNode.Parent.Children != null 
                && currentNode.ChildIndex < currentNode.Parent.Children.Count - 1)
            {
                stack.Push(currentNode.Parent.Children[currentNode.ChildIndex + 1]);
                continue;
            }

            // Alle Siblings abgegrast
            if (currentNode.Parent == null) continue;
            currentNode.Parent.Children = null;
            currentNode.Parent.IsChildrenProcessed = true;

            stack.Push(currentNode.Parent);
        }
        return (null, stats);
    }

    private static List<Node> PopulateNode(Node currentNode, byte slotStart, byte slotSize)
    {
        var result = new List<Node>();
        byte childIndex = 0;

        for (var move = slotStart; move < slotStart + slotSize; move++)
        {
            var nextChild = new Node
            {
                Parent = currentNode,
                Depth = (byte)(currentNode.Depth + 1),
                FieldWidth = currentNode.FieldWidth,
                MoveFromParent = (byte)move,
                ChildIndex = childIndex,
                SameMoveCounter = currentNode.SameMoveCounter,
            };

            //
            // Check Cycle-Path:
            //
            if (nextChild.MoveFromParent == currentNode.MoveFromParent)
            {
                nextChild.SameMoveCounter++;
                var isCyclePath = nextChild.SameMoveCounter > currentNode.FieldWidth / 2 - 1;
                if (isCyclePath) continue;
            }
            else
                nextChild.SameMoveCounter = 0;

            //
            // Check Forth-Back:
            //
            var isForthBackMove = Math.Abs(currentNode.MoveFromParent - nextChild.MoveFromParent) == currentNode.FieldWidth;
            if (isForthBackMove) continue;
            
            var (newField, isUnchanged, wasAffectedLineInOrder) = ProcessMove(currentNode.Field, currentNode.FieldWidth, CurrentOrderedFieldLookUp, move);
            
            if (isUnchanged) continue;

            nextChild.Field = newField;

            var rating = GetRating(nextChild.Field, CurrentOrderedFieldLookUp, nextChild.FieldWidth);
            if (wasAffectedLineInOrder) rating += 64;
            nextChild.Rating = rating;
            result.Add(nextChild);
            childIndex++;
        }

        result.Sort();
        for (byte ix = 0; ix < result.Count; ix++) result[ix].ChildIndex = ix;
        
        return result;
    }

    public static int GetRating(byte[,] field, Dictionary<byte, Dictionary<byte, byte>> lookup, byte fieldWidth)
    {
        var result = 0;

        for (byte row = 0; row < fieldWidth; row++)
        {
            for (byte col = 0; col < fieldWidth; col++)
            {
                var valueSoll = lookup[row][col];
                var valueIst = field[row, col];

                var delta = Math.Abs(valueSoll - valueIst);
                result += delta;
            }
        }
        return result;
    }
}

