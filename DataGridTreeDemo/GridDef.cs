using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Linq;

namespace DataGridTreeDemo
{
    public class GridDef : INotifyPropertyChanged
    {
        public GridDef()
        {
            Columns = new List<ColumnDef>();
            Source = new ObservableCollection<RowDef>();

            LoadData();
        }

        public ObservableCollection<RowDef> Source { get; }

        public List<ColumnDef> Columns { get; private set; }

        public IEnumerable<RowDef> Display
        {
            get
            {
                //TODO: How to do this with multiple roots?
                return IterateTree(Source[0]);
            }
        }

        private void RowDef_RowExpanding(object sender, RowDef row)
        {
            foreach (RowDef child in row.Children)
                child.IsVisible = true;
            OnPropertyChanged(nameof(Display));
        }

        private void RowDef_RowCollapsing(object sender, RowDef row)
        {
            foreach (RowDef child in row.Children)
            {
                if (row.IsExpanded.HasValue && row.IsExpanded.Value)
                    RowDef_RowCollapsing(this, child);
                child.IsVisible = false;
            }
            OnPropertyChanged(nameof(Display));
        }

        private void LoadData()
        {
            Columns = new List<ColumnDef>() {
                new ColumnDef() { Title = "Col1" },
                new ColumnDef() { Title = "Col2" }
            };
            XDocument doc = XDocument.Load("Data.xml");
            LoadData(Source, doc.Element("Rows").Elements("Row"), null);
            Source[0].IsVisible = true;
        }

        private int LoadData(IList<RowDef> srce, IEnumerable<XElement> rows, RowDef parent)
        {
            int count = 0;
            foreach (XElement r in rows)
            {
                var colVals = new List<string>
                {
                    r.Attribute("Val1").Value,
                    r.Attribute("Val2").Value
                };

                RowDef row = InitRow(parent, colVals);
                srce.Add(row);
                ++count;

                int children = LoadData(row.Children, r.Elements("Row"), row);
                if (children > 0)
                    row.IsExpanded = false;
            }
            return count;
        }

        private RowDef InitRow(RowDef parent, IEnumerable<string> values)
        {
            RowDef row = new RowDef(parent)
            {
                IsVisible = false,
                Cells = new ObservableCollection<string>(values),
            };

            row.RowCollapsing += RowDef_RowCollapsing;
            row.RowExpanding += RowDef_RowExpanding;

            return row;
        }

        private IEnumerable<RowDef> IterateTree(RowDef parent)
        {
            if (!parent.IsVisible)
                yield break;
            yield return parent;
            foreach (RowDef child in parent.Children)
            {
                foreach (RowDef r in IterateTree(child))
                {
                    yield return r;
                }
            }
        }

        protected virtual void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members
    }
}