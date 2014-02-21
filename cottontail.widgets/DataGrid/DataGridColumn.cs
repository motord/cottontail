using Gtk;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

namespace cottontail.widgets
{
	public class DataGridColumn : TreeViewColumn
	{
		private DataGrid grid;
		private DataColumn column;
		private int columnIndex;
		private IDataGridContentRenderer contentRenderer;
		private static IDataGridContentRenderer nullRenderer;
		
		static DataGridColumn ()
		{
			nullRenderer = new NullContentRenderer ();
		}
		
		public DataGridColumn (DataGrid grid, DataColumn column, int columnIndex)
		{
			this.grid = grid;
			this.column = column;
			this.columnIndex = columnIndex;
			
			contentRenderer = grid.GetDataGridContentRenderer (column.DataType);

			Title = column.ColumnName.Replace ("_", "__"); //underscores are normally used for underlining, so needs escape char
			Clickable = true;
			
			CellRendererText textRenderer = new CellRendererText ();
			PackStart (textRenderer, true);
			SetCellDataFunc (textRenderer, new CellLayoutDataFunc (ContentDataFunc));
		}
		
		public int ColumnIndex {
			get { return columnIndex; }
		}
		
		public IComparer ContentComparer {
			get { return contentRenderer; }
		}
		
		public Type DataType {
			get { return column.DataType; }
		}

		private void ContentDataFunc (CellLayout layout, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			object dataObject = model.GetValue (iter, columnIndex);
			if (dataObject == null)
				nullRenderer.SetContent (cell as CellRendererText, dataObject);
			else
				contentRenderer.SetContent (cell as CellRendererText, dataObject);
		}

	}
}

