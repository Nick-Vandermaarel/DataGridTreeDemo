using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Collections.Generic;

namespace DataGridTreeDemo
{
    public class RowDef
    {
        public event EventHandler<RowDef> RowExpanding;
        public event EventHandler<RowDef> RowCollapsing;

        public RowDef()
        {
            Cells = new ObservableCollection<string>();
            Children = new List<RowDef>();
        }

        public RowDef(RowDef parent)
            : this()
        {
            Parent = parent;
        }

        //TODO: Probably should have another class defining Cell, in case you want something more sophisticated than just a string
        public ObservableCollection<string> Cells { get; internal set; }

        bool? _isExpanded;
        public bool? IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    if (_isExpanded.Value)
                    {
                        RowExpanding?.Invoke(this, this);
                    }
                    else
                    {
                        RowCollapsing?.Invoke(this, this);
                    }
                }
            }
        }

        public List<RowDef> Children { get; set; }

        private RowDef _parent;
        public RowDef Parent
        {
            get { return _parent; }
            private set
            {
                _parent = value;
                if (_parent != null)
                    Level = _parent.Level + 1;
            }
        }

        public int Level { get; set; }
        public bool IsVisible { get; set; }
    }
}
