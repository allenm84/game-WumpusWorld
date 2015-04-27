using System;
using System.Collections.Generic;
using System.Text;

namespace WumpusWorld
{
  /// <summary>
  /// Represents a node in a grid that is used for the Dijkstra searching algorithm.
  /// </summary>
  public class DijkstraNode : IComparable<DijkstraNode>
  {
    private bool mIsIllegal;
    private Index mIndex;
    private int mHashCode;

    /// <summary>The total distance to the goal.</summary>
    protected int mDistance = int.MaxValue;

    /// <summary>The parent of this node.</summary>
    protected DijkstraNode mParent = null;

    /// <summary>
    /// Gets the current distance this node has to reach the goal.
    /// </summary>
    public int Distance
    {
      get { return mDistance; }
      internal set { mDistance = value; }
    }

    /// <summary>
    /// Gets the parent of this node. This is set after Dijkstra has been run.
    /// </summary>
    public DijkstraNode Parent
    {
      get { return mParent; }
      internal set { mParent = value; }
    }

    /// <summary>
    /// Gets a value indicating if this node is illegal, meaning it can't be used to form a path
    /// through the grid. A value of true means this node is blocked.
    /// </summary>
    public virtual bool IsIllegal
    {
      get { return mIsIllegal; }
    }

    /// <summary>
    /// Gets the location of the this node in the grid.
    /// </summary>
    public Index Index
    {
      get { return mIndex; }
    }
    
    /// <summary>
    /// Creates a new Dijkstra node from an index setting whether the node is illegal or not.
    /// </summary>
    /// <param name="column">The zero-based column index of this node</param>
    /// <param name="row">The zero-based row index of this node</param>
    /// <param name="isIllegal">Whether or not this node is illegal.</param>
    public DijkstraNode(int column, int row, bool isIllegal)
      : this(new Index(column, row), isIllegal)
    {
    }

    /// <summary>
    /// Creates a new Dijkstra node from an index setting whether the node is illegal or not.
    /// </summary>
    /// <param name="index">The zero-based index of this node.</param>
    /// <param name="isIllegal">Whether or not this node is illegal.</param>
    public DijkstraNode(Index index, bool isIllegal)
    {
      mIndex = index;
      mIsIllegal = isIllegal;
      mHashCode = index.GetHashCode();
    }

    /// <summary>
    /// Resets this node to it's initial state by clearing the Dijkstra searching data.
    /// </summary>
    public void Reset()
    {
      mDistance = int.MaxValue;
      mParent = null;
    }

    /// <summary>
    /// Gets the all the possible adjacent indices of this node's neighbors. This doesn't check to see 
    /// if the indices are valid.
    /// </summary>
    /// <returns>A collection of indices of the possible neighbors.</returns>
    public virtual Index[] GetAdjacentPoints()
    {
      List<Index> retval = new List<Index>(4);
      Index pt = Index;

      retval.Add(new Index(pt.Column - 1, pt.Row));
      retval.Add(new Index(pt.Column + 1, pt.Row));
      retval.Add(new Index(pt.Column, pt.Row - 1));
      retval.Add(new Index(pt.Column, pt.Row + 1));

      return retval.ToArray();
    }

    /// <summary>
    /// Gets a unique code representing this node.
    /// </summary>
    /// <returns>The unique code of this node.</returns>
    public override int GetHashCode()
    {
      return mHashCode;
    }

    /// <summary>
    /// Determines if this node is equal to obj.
    /// </summary>
    /// <param name="obj">The object to test.</param>
    /// <returns>A value indicating if this is equal to obj.</returns>
    public override bool Equals(object obj)
    {
      DijkstraNode node = obj as DijkstraNode;
      if (node == null) return false;
      return node.mHashCode.Equals(mHashCode);
    }

    /// <summary>
    /// Compares the distance of this node with another node.
    /// </summary>
    /// <param name="other">The other node to compare with.</param>
    /// <returns>A comparison value.</returns>
    public int CompareTo(DijkstraNode other)
    {
      return mDistance.CompareTo(other.mDistance);
    }
  }
}
