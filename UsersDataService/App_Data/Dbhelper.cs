using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.OleDb;
using System.Configuration;


public class Dbhelper
{
    private OleDbCommand com;
    private OleDbConnection con;
    private OleDbDataReader reader;

	public Dbhelper()
	{
        this.com = new OleDbCommand();
        this.con = new OleDbConnection();
        string connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + System.AppDomain.CurrentDomain.BaseDirectory + @"\App_Data\Database.accdb;Persist Security Info=True";
        this.con.ConnectionString = connStr;
        this.com.Connection = this.con; 
	}
    
    public OleDbDataReader GetData(string sql)
    {
        while (this.con.State == ConnectionState.Executing || this.con.State == ConnectionState.Fetching)
        { con.ResetState(); }
        con.Close();
        con.ResetState();
        this.com.CommandText = sql;
        con.Open();
        reader = this.com.ExecuteReader();
        return reader;
    }
    public bool ChangeDB(string sql)
    {
        while (this.con.State == ConnectionState.Executing || this.con.State == ConnectionState.Fetching)
        { con.ResetState(); }
        con.Close();
        this.com.CommandText = sql;
        this.con.Open();
        try
        {
            return this.com.ExecuteNonQuery() > 0;
        }
        finally
        {
            CloseConnection();
        }
    }
    public void CloseConnection()
    {
        this.con.Close();
    }
} 