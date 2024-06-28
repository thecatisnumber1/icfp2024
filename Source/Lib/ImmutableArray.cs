using System;

/// <summary>
/// Represents an immutable array with efficient modification and access operations.
/// </summary>
/// <typeparam name="T">The type of elements in the array.</typeparam>
public class ImmutableArray<T>
{
    private int BlockSize; // Size of leaf nodes
    private int BranchingFactor; // Number of children for internal nodes

    private class Node
    {
        public T[] Values;
        public Node[] Children;
        public Node(T[] values, int branchingFactor)
        {
            Values = values;
            Children = new Node[branchingFactor];
        }
    }

    private T[] initialArray;
    private Node root;
    private int size;

    /// <summary>
    /// Initializes a new instance of the ImmutableArray<T> class.
    /// </summary>
    /// <param name="initialArray">The initial array to create the immutable array from.</param>
    /// <param name="blockSize">The size of leaf nodes. Default is 32.</param>
    /// <param name="branchingFactor">The number of children for internal nodes. Default is 32.</param>
    public ImmutableArray(T[] initialArray, int blockSize = 32, int branchingFactor = 32)
    {
        this.initialArray = initialArray;
        this.size = initialArray.Length;
        this.BlockSize = blockSize;
        this.BranchingFactor = branchingFactor;
        this.root = null;
    }

    /// <summary>
    /// Gets the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when index is out of range.</exception>
    public T Get(int index)
    {
        if (index < 0 || index >= size)
        {
            throw new IndexOutOfRangeException("Index was outside the bounds of the array.");
        }
        if (root == null)
        {
            return initialArray[index];
        }
        else
        {
            return AccessInternal(root, 0, size - 1, index);
        }
    }

    /// <summary>
    /// Creates a new ImmutableArray<T> with the element at the specified index set to the given value.
    /// </summary>
    /// <param name="index">The zero-based index of the element to set.</param>
    /// <param name="value">The new value for the specified element.</param>
    /// <returns>A new ImmutableArray<T> with the specified element modified.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when index is out of range.</exception>
    public ImmutableArray<T> Set(int index, T value)
    {
        if (index < 0 || index >= size)
        {
            throw new IndexOutOfRangeException("Index was outside the bounds of the array.");
        }
        Node newRoot;
        if (root == null)
        {
            newRoot = BuildInitialTree(initialArray, 0, size - 1);
        }
        else
        {
            newRoot = root;
        }
        newRoot = ModifyInternal(newRoot, 0, size - 1, index, value);
        return new ImmutableArray<T>(newRoot, size, initialArray, BlockSize, BranchingFactor);
    }

    // Internal method to access an element in the tree structure
    private T AccessInternal(Node node, int start, int end, int index)
    {
        if (node == null)
        {
            return initialArray[index];
        }
        if (end - start + 1 <= BlockSize)
        {
            return node.Values[index - start];
        }

        int segmentSize = (end - start + 1 + BranchingFactor - 1) / BranchingFactor;
        int segmentIndex = (index - start) / segmentSize;
        int segmentStart = start + segmentIndex * segmentSize;
        int segmentEnd = Math.Min(segmentStart + segmentSize - 1, end);

        return AccessInternal(node.Children[segmentIndex], segmentStart, segmentEnd, index);
    }

    // Internal method to modify an element in the tree structure
    private Node ModifyInternal(Node node, int start, int end, int index, T newValue)
    {
        if (end - start + 1 <= BlockSize)
        {
            T[] newBlock = new T[node == null ? end - start + 1 : node.Values.Length];
            if (node != null)
                Array.Copy(node.Values, newBlock, newBlock.Length);
            else
                Array.Copy(initialArray, start, newBlock, 0, newBlock.Length);
            newBlock[index - start] = newValue;
            return new Node(newBlock, BranchingFactor);
        }

        Node newNode = new Node(null, BranchingFactor);
        int segmentSize = (end - start + 1 + BranchingFactor - 1) / BranchingFactor;
        int segmentIndex = (index - start) / segmentSize;

        for (int i = 0; i < BranchingFactor; i++)
        {
            int currentSegmentStart = start + i * segmentSize;
            if (currentSegmentStart > end) break;
            int currentSegmentEnd = Math.Min(currentSegmentStart + segmentSize - 1, end);

            if (i == segmentIndex)
            {
                newNode.Children[i] = ModifyInternal(node?.Children[i], currentSegmentStart, currentSegmentEnd, index, newValue);
            }
            else
            {
                newNode.Children[i] = node?.Children[i] ?? BuildInitialTree(initialArray, currentSegmentStart, currentSegmentEnd);
            }
        }

        return newNode;
    }

    // Builds the initial tree structure from the array
    private Node BuildInitialTree(T[] array, int start, int end)
    {
        if (start > end)
        {
            return null;
        }
        if (end - start + 1 <= BlockSize)
        {
            T[] block = new T[end - start + 1];
            Array.Copy(array, start, block, 0, block.Length);
            return new Node(block, BranchingFactor);
        }

        Node node = new Node(null, BranchingFactor);
        int segmentSize = (end - start + 1 + BranchingFactor - 1) / BranchingFactor;
        for (int i = 0; i < BranchingFactor; i++)
        {
            int segmentStart = start + i * segmentSize;
            if (segmentStart > end) break;
            int segmentEnd = Math.Min(segmentStart + segmentSize - 1, end);
            node.Children[i] = BuildInitialTree(array, segmentStart, segmentEnd);
        }

        return node;
    }

    /// <summary>
    /// Gets the number of elements in the ImmutableArray<T>.
    /// </summary>
    public int Length => size;

    // Private constructor for creating new instances during modifications
    private ImmutableArray(Node root, int size, T[] initialArray, int blockSize, int branchingFactor)
    {
        this.root = root;
        this.size = size;
        this.initialArray = initialArray;
        this.BlockSize = blockSize;
        this.BranchingFactor = branchingFactor;
    }
}