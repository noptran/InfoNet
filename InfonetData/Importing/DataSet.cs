using System;
using System.Data;
using System.Data.OleDb;

namespace Infonet.Data.Importing {
	public class DataSet : System.Data.DataSet {
		#region fields
		private OleDbConnection _oleDbConnection;
		private short _sqlConnectionTimeout = 15;
		private short _sqlCommandTimeout = 30;
		#endregion

		#region constructing/disposing
		public DataSet(string oleDbConnectionString) {
			OleDbConnection = new OleDbConnection(oleDbConnectionString);
		}

		protected override void Dispose(bool disposing) {
			if (disposing && _oleDbConnection != null) {
				_oleDbConnection.Dispose();
				_oleDbConnection = null;
			}
			base.Dispose(disposing);
		}
		#endregion

		public OleDbConnection OleDbConnection {
			get { return _oleDbConnection; }
			private set {
				_oleDbConnection = value;
				try {
					if (_oleDbConnection.State == ConnectionState.Closed)
						_oleDbConnection.Open();
				} catch (OleDbException e) {
					throw new ImportException("Unrecognized database format", e);
				}
			}
		}

		public short SqlCommandTimeout {
			set {
				if (value >= 0)
					_sqlCommandTimeout = value;
			}
		}

		public short SqlConnectionTimeout {
			get { return _sqlConnectionTimeout; }
		}

		//KMS DO select *
		public DataTable GetTable(string tableName) {
			try {
				var myTable = new DataTable(tableName);
				var myDataAdapter = new OleDbDataAdapter("Select * from " + tableName, _oleDbConnection);
				myDataAdapter.Fill(myTable);
				Tables.Add(myTable);
				return myTable;
			} catch (Exception) {
				return null;
			}
		}
	}
}