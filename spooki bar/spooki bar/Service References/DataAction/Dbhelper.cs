using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.Data.SqlClient;

/// <summary>
/// Summary description for Dbhelper
/// </summary>
public class Dbhelper
{
    private OleDbCommand com;
    private OleDbConnection con;
    private OleDbDataReader reader;

	public Dbhelper()
	{
        this.com = new OleDbCommand();
        this.con = new OleDbConnection();
        string connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + System.AppDomain.CurrentDomain.BaseDirectory + @"\DataAction\Database.accdb'";
        this.con.ConnectionString = connStr;
        this.com.Connection = this.con; 
	}

    public OleDbDataReader GetData(string sql)
    {
        this.com.CommandText = sql;
        this.con.Open();
        this.reader = this.com.ExecuteReader();
        return this.reader;
    }
    public bool ChangeDB(string sql)
    {
        this.com.CommandText = sql;
        this.con.Open();
        return this.com.ExecuteNonQuery() > 0;
    }
    public void CloseConnection()
    {
        this.con.Close();
    }
} 