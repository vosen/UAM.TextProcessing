using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime.Tree;
using Antlr.Runtime;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Vosen.SQLFilter
{
    public abstract class FilterTree : CommonTree, INotifyPropertyChanged
    {
        public bool IsLeaf { get; set; }

        private bool dirty;
        public bool IsDirty
        {
            get
            {
                return dirty;
            }
            internal set
            {
                if (dirty != value)
                {
                    dirty = value;
                    OnChanged("IsDirty");
                }
            }
        }        

        public FilterTree()
            : base()
        { }

        public FilterTree(CommonTree node)
            : base(node)
        { }

        public FilterTree(IToken t)
            : base(t)
        { }

        public FilterTree(FilterTree t)
            : base(t)
        {
            this.SubType = t.SubType;
            if (t.IsLeaf)
                this.IsLeaf = true;
            if (t.IsDirty)
                this.IsDirty = true;
            if(t.Children != null)
                this.AddChildren(t.Children);
        }

        public FilterTree(int type)
            : base(new CommonToken(type))
        { }

        protected override IList<ITree> CreateChildrenList()
        {
            return new ObservableCollection<ITree>();            
        }

        public override void AddChild(ITree t)
        {
            var oldChild = this.Children;
            base.AddChild(t);
            if (oldChild == null && this.Children != null)
                OnChanged("Children");
        }

        public override void ReplaceChildren(int startChildIndex, int stopChildIndex, object t)
        {
            var oldChild = this.Children;
            base.ReplaceChildren(startChildIndex, stopChildIndex, t);
            if (oldChild == null && this.Children != null)
                OnChanged("Children");
        }

        public override void  SetChild(int i, ITree t)
        {
            var oldChild = this.Children;
 	        base.SetChild(i, t);
            if (oldChild == null && this.Children != null)
                OnChanged("Children");
        }

        public override int Type
        {
            get
            {
                return base.Type;
            }
            set
            {
                this.token.Type = value;
                OnChanged("Type");
            }
        }

        private int subType = -1;
        public virtual int SubType
        {
            get
            {
                return subType;
            }
            set
            {
                subType = value;
            }
        }

        public override string Text
        {
            get
            {
                return this.token.Text;
            }
            set
            {
                this.token.Text = value;
                OnChanged("Text");
            }
        }

        internal void UnmarkWithChildren()
        {
            if(IsDirty)
            {
                IsDirty = false;
                if (Children == null)
                    return;
                foreach(FilterTree child in Children)
                {
                    child.UnmarkWithChildren();
                }
            }
        }

        public void MarkAsDirty()
        {
            this.IsDirty = true;
            var parent = Parent as FilterTree;
            if (parent != null && !parent.IsDirty)
                parent.MarkAsDirty();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnChanged(string property)
        {
            var temp = PropertyChanged;
            if (temp != null)
            {
                temp(this, new PropertyChangedEventArgs(property));
            }
        }

        public abstract Expression Compile(ParameterExpression param);

        public override string ToStringTree()
        {
            if ((this.Children == null) || (this.Children.Count == 0))
            {
                return printNode(this);
            }
            StringBuilder builder = new StringBuilder();
            if (!this.IsNil)
            {
                builder.Append("(");
                builder.Append(printNode(this));
                builder.Append(' ');
            }
            for (int i = 0; (this.Children != null) && (i < this.Children.Count); i++)
            {
                ITree tree = this.Children[i];
                if (i > 0)
                {
                    builder.Append(' ');
                }
                builder.Append(tree.ToStringTree());
            }
            if (!this.IsNil)
            {
                builder.Append(")");
            }
            return builder.ToString();
        }

        //Stolen from antlr runtime
        private static string printNode(FilterTree tree)
        {
            if (tree.IsNil)
            {
                return "nil";
            }
            if (tree.Type == 0)
            {
                return "<errornode>";
            }
            if (tree.token == null)
            {
                return string.Empty;
            }
            return tree.Text;
        }
     }
}