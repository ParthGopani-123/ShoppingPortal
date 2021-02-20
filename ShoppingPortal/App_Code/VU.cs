using System;
using System.Collections.Generic;
using System.Linq;
using BOL;
using System.Data;

/// <summary>
/// Summary description for VU
/// </summary>
public class VU
{
    public static bool UpdateFail = false;

    public static string FailureMessage = string.Empty;

    public static readonly string LatestUpdate = "02 Jan 2017";

    public static readonly string LatestVersion = "1.0.0.0";

    public static string GetDBVersion()
    {
        return CU.GetNameValue(eNameValue.DBVersion);
    }

    public static bool UpdateVersion(ref string Message)
    {
        UpdateFail = false;
        for (int i = 0; ; i++)
        {
            string CurrntVersion = GetDBVersion();

            if (CurrntVersion == LatestVersion || UpdateFail)
                break;

            switch (CurrntVersion)
            {
                # region Version 1.0.0.0 To 1.0.0.9

                case "1.0.0.0":
                    # region 1.0.0.0 to 1.0.0.1
                    if (!UpdateFail && !UpdateTable_1_0_0_1())
                    {
                        UpdateFail = true;
                        FailureMessage = "Error at UpdateDB 1.0.0.1";
                    }
                    if (!UpdateFail && !UpdateDBVersion_1_0_0_1())
                    {
                        UpdateFail = true;
                        FailureMessage = "Error at UpdateDBVersion 1.0.0.1";
                    }
                    # endregion
                    break;
                
                case "1.0.0.1":
                    //UpComing Version
                    break;

                default:
                    return true;

                #endregion Verson 1.0.0.0 To 1.0.0.9
            }
        }

        Message = UpdateFail ? FailureMessage : "Verson Update Successfull.";

        return UpdateFail;
    }

    #region Updated Verson

    # region Version 1.0.0.1 To 1.0.0.9

    # region Version 1.0.0.1

    private static bool UpdateTable_1_0_0_1()
    {
        try
        {
        }
        catch { return false; }

        return true;
    }

    private static bool UpdateDBVersion_1_0_0_1()
    {
        try
        {
            CU.SetNameValue(eNameValue.DBVersion, "1.0.0.1");
            return true;
        }
        catch
        {
            return false;
        }
    }

    # endregion

    # endregion

    #endregion

    #region UpdateMethod

    private static bool AddColumnInTable(string TableName, string ColumnName, string DataType, object DefaultValue)
    {
        if (IsTableContainColumn(TableName, ColumnName))
            return false;

        string Query = "ALTER TABLE " + TableName + " ADD " + ColumnName + " " + DataType;
        new Query().ExeQuery(Query);
        if (DefaultValue != null)
        {
            if (DataType.ToUpper() == "BIT")
                DefaultValue = "'" + DefaultValue.ToString().Replace("'", "''") + "'";
            else if (DataType.ToUpper() == "DATETIME")
                DefaultValue = "'" + Convert.ToDateTime(DefaultValue).ToString("MM-dd-yyyy hh:mm:ss tt") + "'";
            else if (DataType.ToUpper().Contains("VARCHAR"))
                DefaultValue = "N'" + DefaultValue.ToString().Replace("'", "''") + "'";

            Query = "UPDATE " + TableName + " SET " + ColumnName + " = " + DefaultValue;
            new Query().ExeQuery(Query);
        }
        return true;
    }

    private static void ChangeColumnDataType(string TableName, string ColumnName, string DataType, object DefaultValue)
    {
        if (!IsTableContainColumn(TableName, ColumnName))
            return;
        if (DefaultValue != null)
            new Query().ExeQuery("UPDATE " + TableName + " SET " + ColumnName + " = " + DefaultValue);

        string Query = "ALTER TABLE " + TableName + " ALTER COLUMN " + ColumnName + " " + DataType;
        new Query().ExeQuery(Query);
    }

    private static void ChangeColumnName(string TableName, string OldColumnName, string NewColumnName)
    {
        //if (!IsTableContainColumn(TableName, OldColumnName))
        //    return;

        //sp_RENAME 'ExpenseAccount.AccountId','TransactionAccountId' , 'COLUMN'

        string RENAME = "sp_RENAME";

        string Query = RENAME + " '" + TableName + "." + OldColumnName + "'" + "," + "'" + NewColumnName + "'" + " , " + "'COLUMN'";
        new Query().ExeQuery(Query);
    }

    private static bool IsTableContainColumn(string TableName, string ColumnName)
    {
        string Query = "select TOP 1 * from " + TableName;
        DataTable dt = new Query().ExeQuery(Query);

        bool Found = false;
        for (int i = 0; i < dt.Columns.Count; i++)
        {
            if (dt.Columns[i].ColumnName.ToUpper() == ColumnName.ToUpper())
            {
                Found = true;
                break;
            }
        }
        return Found;
    }

    private static void ChangeTableName(string OldTableName, string NewTableName)
    {
        //sp_RENAME ExpenseAccount  , TransactionAccount 

        string Query = "sp_RENAME " + OldTableName + " , " + NewTableName;
        new Query().ExeQuery(Query);
    }

    private static void DeleteColumn(string TableName, string ColumnName)
    {
        if (IsTableContainColumn(TableName, ColumnName))
        {
            //ALTER TABLE TM_WorkType DROP COLUMN SerialNo

            string Query = "ALTER TABLE " + TableName + " DROP COLUMN " + ColumnName;
            new Query().ExeQuery(Query);
        }
    }

    private static void ChnageColumnDataType(string TableName, string ColumnName, string DataType)
    {
        if (IsTableContainColumn(TableName, ColumnName))
        {
            string Query = "ALTER TABLE " + TableName + " ALTER COLUMN " + ColumnName + " " + DataType;
            new Query().ExeQuery(Query);
        }
    }

    private static bool CreateTable(string TableCreationScript)
    {
        try
        {
            //[dbo].[SMSFiles]
            string TableName = TableCreationScript.Split(new string[] { "[dbo].[", "]" }, StringSplitOptions.RemoveEmptyEntries)[1];

            try { DataTable dt = new Query().ExeQuery("select TOP 1 * from " + TableName.Trim()); }
            catch { new Query().ExeNonQuery(TableCreationScript); }
        }
        catch { return false; }
        return true;
    }

    #endregion
}
