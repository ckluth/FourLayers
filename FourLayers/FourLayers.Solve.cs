
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FourLayers;

public static partial class FourLayers
{
    private static int isCancelled;

    public static (List<byte> Moves, Stats Stats) SolveChallenge(Challenge challenge, Stats stats)
    {
        isCancelled = 0;

        Node CreateRoot()
        {
            var root = new Node
            {
                Field = challenge.Field,
                FieldWidth = (byte)challenge.FieldWidth,
                Depth = 0,
                MaxDepth = challenge.MaxMoves,
                MoveFromParent = 255,
                Disorder = 255,

            };
            return root;
        }

        var (slots, kerne) = Slot.CreateSlots(challenge.FieldWidth);
        var resultingStats = new List<Stats>();

        var sw = Stopwatch.StartNew();

        try
        {
            Parallel.ForEach(slots, new ParallelOptions { MaxDegreeOfParallelism = kerne }, slot =>
            {
                    var slotStats = Stats.Clone(stats);
                    var startNode = CreateRoot();
                    slot.Result = InternalSolveChallenge(startNode, challenge.MaxMoves, slotStats, slot.SlotStart,
                        slot.SlotSize);
                    resultingStats.Add(slotStats);
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

        try
        {

            var successFullSlot = slots.FirstOrDefault(c => c.Result.Moves != null);
            var moves = successFullSlot?.Result.Moves;

            var resultStats = Stats.Aggregate(resultingStats);
            resultStats.Moves = moves;
            resultStats.Duration = sw.Elapsed;
            return (moves, resultStats);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.ReadKey();
            return (null, null);
        }
        
    }
    
    private static (List<byte> Moves, Stats Stats) InternalSolveChallenge(Node root, byte maxMoves, Stats stats, byte slotStart, byte slotSize)
    {
        var stack = new Stack<Node>();
        stack.Push(root);
        
        stats.NodesCreated = 1;
        stats.NodesFollowed = 1;
        
        var orderedFieldlookup = CreateOrderedField(root.FieldWidth);

        while (stack.Count > 0)
        {
            if (isCancelled > 0)
                return (null, stats);

            var currentNode = stack.Pop();

            if (!currentNode.AreAllChildrenProcessed)
            {
                // Gelöst?
                stats.NodesCompared++;

                if (currentNode.IsSolved)
                {
                    Interlocked.Increment(ref isCancelled);
                    var moves = currentNode.Path;
                    return (moves, stats);
                }
            }

            // Noch keine Children?
            if (!currentNode.AreAllChildrenProcessed && currentNode.Children == null && currentNode.Depth < maxMoves)
            {
                currentNode.Children = currentNode.Depth == 0
                    ? PopulateNode(currentNode, slotStart, slotSize, currentNode.MaxDepth, orderedFieldlookup)
                    : PopulateNode(currentNode, 0, (byte)(currentNode.FieldWidth * 4), currentNode.MaxDepth, orderedFieldlookup);

                // currentNode.Children == null --> Too much Disorder for remaining Moves  
                if (currentNode.Children != null)
                {
                    stats.NodesCreated += (currentNode.FieldWidth * 4);
                    stats.NodesFollowed += currentNode.Children.Count;
                    stats.NodesDiscarded += ((currentNode.FieldWidth * 4) - currentNode.Children.Count);
                    stack.Push(currentNode.Children[0]);
                    continue;
                }
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
            currentNode.Parent.AreAllChildrenProcessed = true;

            stack.Push(currentNode.Parent);

        }
        return (null, stats);
    }

    private static List<Node> PopulateNode(Node currentNode, byte slotStart, byte slotSize, byte maxDepth, byte [,] orderedFieldLookup)
    {
        if (currentNode.Depth > 0)
        {
            // Too much Disorder for remaining Moves?
            // Der entscheidende Booster! ;)
            var depthsLeft = maxDepth - currentNode.Depth;
            var maxSolvableDisorder = (currentNode.FieldWidth - 2) * depthsLeft;
            if (currentNode.Disorder > maxSolvableDisorder)
            {
                return null;
            }
        }

        var result = new List<Node>();
        byte childIndex = 0;

        // Für jeden move des CurrentNode einen neuen ChildNode erzeugen und ggf. verwerfen, oder sortierte den ChildNodes hinzufügen 
        for (var move = slotStart; move < slotStart + slotSize; move++)
        {
            var nextChild = new Node
            {
                Parent = currentNode,
                Depth = (byte)(currentNode.Depth + 1),
                MaxDepth = currentNode.MaxDepth,
                FieldWidth = currentNode.FieldWidth,
                MoveFromParent = (byte)move,
                ChildIndex = childIndex,
                SameMoveCounter = currentNode.SameMoveCounter,
                OneDirectionDelta  = currentNode.OneDirectionDelta,
            };

            //
            // Check Nonsense-Move "Cycle-Path":
            //
            if (nextChild.MoveFromParent == currentNode.MoveFromParent)
            {
                nextChild.SameMoveCounter++;
                var isCyclePath = nextChild.SameMoveCounter > currentNode.FieldWidth / 2 - 1;
                if (isCyclePath) continue;
            }
            else
                nextChild.SameMoveCounter = 0;
            
            // Check Nonsense-Move "Forth-Back":
            var isForthBackMove = Math.Abs(currentNode.MoveFromParent - nextChild.MoveFromParent) == currentNode.FieldWidth;
            if (isForthBackMove) continue;
            
            // ProcessMove
            var (newField, isUnchanged, wasAffectedLineInOrder, isColumnMove, lineDelta) = ProcessMove(currentNode.Field, currentNode.FieldWidth, orderedFieldLookup, move);
            
            // Check Nonsense-Move "Unchanged":
            if (isUnchanged) continue;
            
            // Check Nonsense-Move "Line-Delta==0"
            // Ohne Richtungswechsel so lange hin -und her-geschoben, so dass wieder der Ausgangszustand erreicht wurde?
            nextChild.MoveFromParentWasColumnMove = isColumnMove;
            var wasDirectionChange = nextChild.Parent != null && nextChild.Parent.MoveFromParentWasColumnMove != nextChild.MoveFromParentWasColumnMove;
            nextChild.OneDirectionDelta = wasDirectionChange ? lineDelta : nextChild.OneDirectionDelta |= lineDelta;
            if (nextChild.OneDirectionDelta == 0) continue;

            if (!wasDirectionChange)
            {

            }

            // Calc Disorder and Add
            var disorder = CalcDisorder(newField, orderedFieldLookup, nextChild.FieldWidth);
            nextChild.Disorder = disorder;
            nextChild.Field = newField;
            if (wasAffectedLineInOrder) nextChild.Handicap = 64;
            result.Add(nextChild);
            childIndex++;
        }

        if (result.Count == 0) return null;

        // Sort (by Disorder and Handicap)
        result.Sort();

        // Update ChildIndex-Values
        for (byte ix = 0; ix < result.Count; ix++) result[ix].ChildIndex = ix;
        
        return result;
    }
    
    public static int CalcDisorder(byte[,] field, byte[,] orderedFieldlookup, byte fieldWidth)
    {
        // Für jeden Cell-Value das Delta zum jeweiligen "Orderd-Cell-Value" ermitteln und auf-addieren:
        var result = 0;
        for (byte row = 0; row < fieldWidth; row++)
        {
            for (byte col = 0; col < fieldWidth; col++)
            {
                var valueSoll = orderedFieldlookup[row,col];
                var valueIst = field[row, col];
                var delta = Math.Abs(valueSoll - valueIst);
                result += delta;
            }
        }
        return result;
    }

    public static (int Disorder, ulong HashLow, ulong HashHigh) GetDisorderAndHash(byte[,] field, Dictionary<byte, Dictionary<byte, byte>> lookup, byte fieldWidth)
    {
        //
        // Ermittelt für das gegebene Field eine eindeutige 128 Bit-Prüfsumme (in Form von 2 64-Bit-Werten) 
        // Aus Performance-Gründe wird in dem Loop auch direkt der Disorder-Wert berechnet.
        //
        // Aktuell nicht genutzt.
        //
        // Wurde für das Feature "Already-Visited" benötigt; aus Performance und -Memory-Gründen (bislang?) kein sinnvolle Implementierung gefunden...
        //
        // Könnte noch nüztlich sein...
        //

        var disorder = 0;
        var ix = 0;
        ulong hashLow = 0;
        ulong hashHigh = 0;
        var half = fieldWidth * fieldWidth / 2;

        for (byte row = 0; row < fieldWidth; row++)
        {
            for (byte col = 0; col < fieldWidth; col++)
            {
                var valueSoll = lookup[row][col];
                var valueIst = field[row, col];

                var delta = Math.Abs(valueSoll - valueIst);
                disorder += delta;

                if (ix < half)
                {
                    hashLow |= ((ulong)valueIst << (2 * ix));
                }
                else
                {
                    hashHigh |= ((ulong)valueIst << (2 * (ix - half)));
                }
                ix++;
            }
        }
        return (disorder, hashLow, hashHigh);
    }
}

