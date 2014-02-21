using Gtk;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

namespace cottontail.widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class DataGrid : Bin
	{
		private int numRecords = 0;
		private int totalRecords = 0;
		private DataTable dataSource;
		private ListStore store;
		private DataGridColumn[] columns;
		private int columnCount = 0;
		private ObjectContentRenderer defaultContentRenderer;
		private Dictionary<Type, IDataGridContentRenderer> contentRenderers;
		private Dictionary<int, ConvertObjectFunc> conversionLookup = new Dictionary<int, ConvertObjectFunc> ();
		private delegate string ConvertObjectFunc (object obj);
		
		private ConvertObjectFunc byteConvertFunc = new ConvertObjectFunc (ByteConvertFunc);
		private ConvertObjectFunc sbyteConvertFunc = new ConvertObjectFunc (SByteConvertFunc);
		
		public DataGrid ()
		{
			this.Build ();
			
			contentRenderers = new Dictionary<Type, IDataGridContentRenderer> ();
			
			AddContentRenderer (new BlobContentRenderer ());
			AddContentRenderer (new BooleanContentRenderer ());
			AddContentRenderer (new ByteContentRenderer ());
			AddContentRenderer (new DecimalContentRenderer ());
			AddContentRenderer (new DoubleContentRenderer ());
			AddContentRenderer (new FloatContentRenderer ());
			AddContentRenderer (new IntegerContentRenderer ());
			AddContentRenderer (new LongContentRenderer ());
			AddContentRenderer (new StringContentRenderer ());
		}
		
		public int RecordCount {
			get { return numRecords; }
		}
		
		public void DataBind ()
		{
			if (dataSource == null) {
				Clear ();
				return;
			}
			
			int index = 0;
			Type[] storeTypes = new Type[dataSource.Columns.Count];
			columnCount = dataSource.Columns.Count;
			foreach (DataColumn col in dataSource.Columns) {
				DataGridColumn dgCol = new DataGridColumn (this, col, index);
				grid.AppendColumn (dgCol);

				if (col.DataType == typeof(byte)) {
					//special case for gchar (TODO: look up the bugzilla bug id)
					storeTypes [index] = typeof(string);
					conversionLookup.Add (index, byteConvertFunc);
				} else if (col.DataType == typeof(sbyte)) {
					storeTypes [index] = typeof(string);
					conversionLookup.Add (index, sbyteConvertFunc);
				} else if (col.DataType.IsPrimitive || col.DataType == typeof(string) || col.DataType.IsEnum) {
					storeTypes [index] = col.DataType;
				} else {
					//the ListStore doesn't allow types that can't be converted to a GType
					storeTypes [index] = typeof(object);
				}

				index++;
			}
			store = new ListStore (storeTypes);
			grid.Model = store;
			
			FillGrid (0, numRecords);
		}
		
		public DataTable DataSource {
			get { return dataSource; }
			set {
				dataSource = value;
				if (value != null)
					numRecords = dataSource.Rows.Count;
				else
					numRecords = 0;
			}
		}
		
		public int TotalRecords {
			get { return totalRecords;}
			set { 
				if (value != null)
					totalRecords = value;
				else
					totalRecords = 0;
				label.Text = String.Format ("{0} rows in total, showing {1}", totalRecords, numRecords);
			}
		}
	
		public void Clear ()
		{
			numRecords = 0;
			totalRecords = 0;
			columnCount = 0;
			
			conversionLookup.Clear ();

			if (store != null) {
				store.Clear ();
				store = null;
			}
			
			if (columns != null) {
				for (int i=0; i<columns.Length; i++) {
					if (columns [i] != null)
						grid.RemoveColumn (columns [i]);
					columns [i] = null;
				}
				columns = null;
			}
		}

		private void FillGrid (int start, int count)
		{
			grid.Model = null;
			
			int end = start + count;
			if (dataSource.Rows.Count < end)
				end = dataSource.Rows.Count;

			store.Clear ();
			for (int i=start; i<end; i++) {
				DataRow row = dataSource.Rows [i];
				
				TreeIter iter = store.Append ();
				for (int j=0; j<columnCount; j++) {
					//HACK: this is a hack for a bug that doesn't allow gchars in a liststore
					if (conversionLookup.ContainsKey (j)) {
						ConvertObjectFunc func = conversionLookup [j];
						store.SetValue (iter, j, func (row [j]));
					} else {
						store.SetValue (iter, j, row [j]);
					}
				}
			}
			
			grid.Model = store;
		}

		internal IDataGridContentRenderer GetDataGridContentRenderer (Type type)
		{
			IDataGridContentRenderer renderer = null;
			if (contentRenderers.TryGetValue (type, out renderer))
				return renderer;
			
			if (defaultContentRenderer == null)
				defaultContentRenderer = new ObjectContentRenderer ();
			return defaultContentRenderer;
		}
		
		private void AddContentRenderer (IDataGridContentRenderer renderer)
		{
			foreach (Type type in renderer.DataTypes) {
				if (contentRenderers.ContainsKey (type)) {
//					LoggingService.LogError ("Duplicate IDataGridContentRenderer for type '{0}'", type.FullName);
				} else
					contentRenderers.Add (type, renderer);
			}
		}
		
		private static string ByteConvertFunc (object obj)
		{
			if (Convert.IsDBNull (obj) || obj == null)
				return null;
			
			byte b = (byte)obj;
			return "0x" + b.ToString ("X");
		}
		
		private static string SByteConvertFunc (object obj)
		{
			if (Convert.IsDBNull (obj) || obj == null)
				return null;			
			
			sbyte b = (sbyte)obj;
			return b.ToString ("N");
		}
	}
}

