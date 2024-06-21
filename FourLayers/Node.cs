using System;
using System.Collections.Generic;

namespace FourLayers;

public class Node : IEquatable<Node>, IComparable<Node>
{
    public byte[,] Field { get; set; } = null!;

    public byte FieldWidth { get; set; }

    public Node Parent { get; set; }

    public byte ChildIndex { get; set; }

    public List<Node> Children { get; set; }

    public byte Depth { get; set; }

    public byte MoveFromParent { get; set; }

    public bool IsChildrenProcessed { get; set; }

    public int SameMoveCounter = 0;

    public int Rating { get; set; }

    public List<byte> Path
    {
        get
        {
            var result = new List<byte>();
            if (this.Parent == null) return result;
            result.Add(this.MoveFromParent);
            var node = this.Parent;
            while (node != null)
            {
                if (node.Parent != null)
                    result.Insert(0, node.MoveFromParent);
                node = node.Parent;
            }
            return result;
        }
    }

    public bool IsSolved => Rating == 0;
    
    public bool Equals(Node other)
    {
        return this.Rating == other!.Rating;
    }

    public int CompareTo(Node other)
    {
        return this.Rating - other.Rating;
    }

}
