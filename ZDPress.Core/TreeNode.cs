using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ZDPress.Opc
{
    public class TreeNode<T>
    {
        private readonly T _value;
        private readonly List<TreeNode<T>> _children = new List<TreeNode<T>>();

        public TreeNode(T value)
        {
            _value = value;
        }

        public TreeNode<T> this[int i]
        {
            get { return _children[i]; }
        }

        public TreeNode<T> Parent { get; private set; }

        public T Value { get { return _value; } }

        public ReadOnlyCollection<TreeNode<T>> Children
        {
            get { return _children.AsReadOnly(); }
        }

        public void Clear()
        {
            _children.Clear();
        }
        public TreeNode<T> AddChild(TreeNode<T> node)
        {
            node.Parent = this;
            _children.Add(node);
            return node;
        }

        public TreeNode<T>[] AddChildren(params TreeNode<T>[] nodes)
        {
            return nodes.Select(AddChild).ToArray();
        }

        public bool RemoveChild(TreeNode<T> node)
        {
            return _children.Remove(node);
        }

        public void Traverse(Action<T> action)
        {
            action(Value);
            foreach (var child in _children)
                child.Traverse(action);
        }


        public string FullPath
        {
            get
            {
                List<T> chanks = new List<T>();
                TreeNode<T> self = this;
                while (self.Parent != null)
                {
                    chanks.Add(self.Value);
                    self = self.Parent;
                }
                chanks.Add(self.Value);
                chanks.Reverse();
                return string.Join("\t", chanks.OfType<string>());
            }
        }


        public IEnumerable<T> Flatten()
        {
            return new[] { Value }.Union(_children.SelectMany(x => x.Flatten()));
        }
    }
}
