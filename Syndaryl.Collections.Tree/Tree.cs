using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syndaryl.Collections
{
    /// <summary>
    /// A simple unbalanced tree data structure, based around LinkedList<>. Uses recursion.
    /// Based on code from http://stackoverflow.com/questions/66893/tree-data-structure-in-c-sharp
    /// </summary>
    /// <typeparam name="T">The type of data contained in the tree.</typeparam>
    public class TreeRoot<T>
    {
        private TreeNode<T> node;

        public TreeNode<T> Node
        {
            get
            {
                return node;
            }

            private set
            {
                node = value;
            }
        }

        public TreeRoot(T data)
        {
            Node = new TreeNode<T>( data );
        }

        public TreeRoot()
        {
        }

        public TreeNode<T> SetNode(T data)
        {
            Node = new TreeNode<T>(data);
            return Node;
        }

        /// <summary>
        /// Traverse the entire tree and perform the provided Action on each node (recursion).
        /// </summary>
        /// <param name="node"></param>
        /// <param name="visitor"></param>
        public void Traverse(Action<T> visitor)
        {
            Node.Traverse(visitor);
        }
    }


    /// <summary>
    /// A node in the Tree.
    /// </summary>
    /// <typeparam name="T">The type of data contained in the node.</typeparam>
    public class TreeNode<T>
    {
        private readonly LinkedList<TreeNode<T>> children;

        public void AddChildren(IEnumerable<T> children)
        {
            if (children == null) return;
            foreach (var child in children)
            {
                AddChild(child);
            }
        }

        public T Data { get; set; }

        public int Count()
        {
            return children.Count;
         }

        public int Count(Func<TreeNode<T>, bool> Predicate)
        {
            return children.Count(Predicate);
        }

        /// <summary>
        /// Constructor. Initializes the data in this node, produces an empty list of children.
        /// </summary>
        /// <param name="data"></param>
        public TreeNode(T data)
        {
            this.Data = data;
            children = new LinkedList<TreeNode<T>>();
        }

        /// <summary>
        /// Adds a child to this node, initializing it with data and no children.
        /// </summary>
        /// <param name="data"></param>
        public void AddChild(T data)
        {
            children.AddFirst(new TreeNode<T>(data));
        }

        public void RemoveChild(int index)
        {
            if (children.Count < index)
                throw new IndexOutOfRangeException();
            try { 
            var result = children.GetNode(index);
            children.Remove(result);
            }
            catch (IndexOutOfRangeException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Gets the specified child TreeNode. If this node does not exist, throws IndexOutOfRangeException.
        /// </summary>
        /// <param name="index">Zero-based index of the child to return</param>
        /// <returns></returns>
        public TreeNode<T> GetChild(int index)
        {
            if (children.Count < index)
                throw new IndexOutOfRangeException();
            return children.GetNode(index).Value;
        }

        /// <summary>
        /// Produces an IEnumerable of all the children of this node (as TreeNodes). To get as type T instead, use .GetChildren().Select(x => x.Data);
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TreeNode<T>> GetChildren()
        {
            foreach (TreeNode<T> n in children)
                yield return n;
        }

        /// <summary>
        /// Traverse all children from this node and perform the provided Action on this node, and on each child node (recursion).
        /// </summary>
        /// <param name="node"></param>
        /// <param name="visitor"></param>
        public void Traverse(Action<T> visitor)
        {
            visitor(this.Data);
            foreach (TreeNode<T> kid in this.children)
                Traverse(kid, visitor);
        }

        /// <summary>
        /// Traverse all children from specified node and perform the provided Action on specified node, and on each child node (recursion).
        /// </summary>
        /// <param name="node"></param>
        /// <param name="visitor"></param>
        public static void Traverse(TreeNode<T> node, Action<T> visitor)
        {
            visitor(node.Data);
            foreach (TreeNode<T> kid in node.children)
                Traverse(kid, visitor);
        }
    }

    internal static class Extensions
    {
        /// <summary>
        /// Returns the LinkedListNode&lt;T&gt;> at the specified index, if it exists. Throws IndexOutOfRangeException if it does not exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="@this"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal static LinkedListNode<T> GetNode<T>(this LinkedList<T> @this, int index )
        {
            if (index < 0)
            {
                throw new IndexOutOfRangeException("Index must be a positive integer.");
            }
            if (@this.Count < index)
            {
                throw new IndexOutOfRangeException("Provided linked list does not have " + index.ToString() + " members.");
            }
            int i = 0;
            LinkedListNode<T> result = null;
            for (var recentNode = @this.First;
                    recentNode != null;
                    recentNode = recentNode.Next)
            {
                i++;
                if (index == i)
                {
                    result = recentNode;
                    recentNode = null; // terminate early
                }
            }
            if (result != null) // null result indicates didn't find the node
            {
                return result;
            }
            throw new IndexOutOfRangeException("Provided linked list does not have " + index.ToString() + " members.");
        }
    }
}
