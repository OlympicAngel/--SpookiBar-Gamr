using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.OleDb;
using System.Data;

public class DbCtrl
{
	OleDbDataReader reader;
    OleDbConnection connection;
    OleDbCommand command;
    public OleDbDataReader GetReader()
    { return this.reader; }

    public void ConectedDb(string sql)
    {
        this.connection = new OleDbConnection();//provider חיבור לממסד
        this.connection.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source='D:\Spook-I Bar (c#) - The most awsome game made in programing class\spooki bar\spooki bar\DataAction\Database.accdb'";//מיקום הממסד
        this.command = new OleDbCommand();//provider  אחרי על הגדרת פעולות פנימיות/חיצוניות
        this.command.CommandText = sql;//SQL
        this.command.Connection = this.connection;//מגדיר חיבור
        this.connection.Open();//פתיחת חיבור לממסד
        this.reader = this.command.ExecuteReader();//OleDbDataReader - DataCache מכבל את המידע בצורת טבלא || command.ExecuteReader() - ביצוע הפעולה והחזרת נתונים
        // command.ExecuteNonQuery(); מחזיר מספר שורות ששונו
        // command.ExecuteScalar(); מחזיר אובייקט
    }

    public void CloseConactionDb()
    {
        this.reader.Close();//סגירת ממסד
        this.connection.Close();//סגירת ממסד
    }

}