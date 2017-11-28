using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;

namespace WebApplication3
{
    public static class Queries
    {
        public static DataSet GetResultsFromStoreProcedure(string StoredProcedureName, ref Dictionary<string, string> Parameters, string OutputVariable = null)
        {
            DataSet ds = new DataSet();
            using (SqlConnection cnn = new SqlConnection(ConfigurationManager.
                ConnectionStrings["ConnectionString"].ConnectionString))
            {
                cnn.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cnn;
                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach (KeyValuePair<string, string> Parameter in Parameters)
                    {
                        cmd.Parameters.Add(Parameter.Key, SqlDbType.NVarChar);

                        if (Parameter.Value != "NULL")
                            cmd.Parameters[Parameter.Key].Value = Parameter.Value;
                        else
                            cmd.Parameters[Parameter.Key].Value = DBNull.Value;
                    }

                    cmd.CommandTimeout = int.Parse(ConfigurationManager.AppSettings.Get("CommandTimeOut"));
                    cmd.CommandText = StoredProcedureName;

                    if (OutputVariable != null)
                    {
                        cmd.Parameters.Add(OutputVariable, SqlDbType.NVarChar, 50);
                        cmd.Parameters[OutputVariable].Direction = ParameterDirection.Output;
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }

                    if (OutputVariable != null)
                    {
                        Parameters.Add(OutputVariable, cmd.Parameters[OutputVariable].Value.ToString());
                    }
                }
            }
            return ds;
        }

        public static DataTable GetResultsFromQueryString(string QueryString)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cnn = new SqlConnection(System.Configuration.ConfigurationManager.
                ConnectionStrings["ConnectionString"].ConnectionString))
            {
                cnn.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cnn;
                    cmd.CommandText = QueryString;
                    cmd.CommandTimeout = int.Parse(ConfigurationManager.AppSettings.Get("CommandTimeOut"));

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public static void ExecuteFromStoreProcedure(string StoredProcedureName, Dictionary<string, string> Parameters)
        {
            using (SqlConnection cnn = new SqlConnection(System.Configuration.ConfigurationManager.
                ConnectionStrings["ConnectionString"].ConnectionString))
            {
                cnn.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cnn;
                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach (KeyValuePair<string, string> Parameter in Parameters)
                    {
                        cmd.Parameters.Add(Parameter.Key, SqlDbType.NVarChar);

                        if (Parameter.Value != "NULL")
                            cmd.Parameters[Parameter.Key].Value = Parameter.Value;                            
                        else
                            cmd.Parameters[Parameter.Key].Value = DBNull.Value;
                    }

                    cmd.CommandText = StoredProcedureName;
                    cmd.CommandTimeout = int.Parse(ConfigurationManager.AppSettings.Get("CommandTimeOut"));
                    cmd.ExecuteNonQuery();
                }
            }
        }



        public static void ExecuteFromStoreProcedure2(string StoredProcedureName, Dictionary<string, string> Parameters, string OutputVariable = null)
        {
            using (SqlConnection cnn = new SqlConnection(ConfigurationManager.
                ConnectionStrings["ConnectionString"].ConnectionString))
            {
                cnn.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cnn;
                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach (KeyValuePair<string, string> Parameter in Parameters)
                    {
                        cmd.Parameters.Add(Parameter.Key, SqlDbType.NVarChar);

                        if (Parameter.Value != "NULL")
                            cmd.Parameters[Parameter.Key].Value = Parameter.Value;
                        else
                            cmd.Parameters[Parameter.Key].Value = DBNull.Value;
                    }

                    cmd.CommandText = StoredProcedureName;
                    cmd.CommandTimeout = int.Parse(ConfigurationManager.AppSettings.Get("CommandTimeOut"));

                    if (OutputVariable != null)
                    {
                        cmd.Parameters.Add(OutputVariable, SqlDbType.NVarChar, 50);
                        cmd.Parameters[OutputVariable].Direction = ParameterDirection.Output;
                    }
                    
                    cmd.ExecuteNonQuery();

                    if (OutputVariable != null)
                    {
                        Parameters.Add(OutputVariable, cmd.Parameters[OutputVariable].Value.ToString());
                    }
                }
            }
        }

        public static void ExecuteFromQueryString(string QueryString)
        {
            using (SqlConnection cnn = new SqlConnection(System.Configuration.ConfigurationManager.
                ConnectionStrings["ConnectionString"].ConnectionString))
            {
                cnn.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cnn;
                    cmd.CommandText = QueryString;
                    cmd.CommandTimeout = int.Parse(ConfigurationManager.AppSettings.Get("CommandTimeOut"));

                    cmd.ExecuteNonQuery();
                }
            }
        }



        public static void CSVFileDownloadFromSP(string StoredProcedureName, ref Dictionary<string, string> Parameters, string strDelimiter, string FilePath, string OutputVariable = null)
        {
            SqlDataReader reader;
            StringBuilder sb = new StringBuilder();            

            using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                cnn.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cnn;
                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach (KeyValuePair<string, string> Parameter in Parameters)
                    {
                        cmd.Parameters.Add(Parameter.Key, SqlDbType.NVarChar);

                        if (Parameter.Value != "NULL")
                            cmd.Parameters[Parameter.Key].Value = Parameter.Value;
                        else
                            cmd.Parameters[Parameter.Key].Value = DBNull.Value;
                    }

                    cmd.CommandTimeout = int.Parse(ConfigurationManager.AppSettings.Get("CommandTimeOut"));
                    cmd.CommandText = StoredProcedureName;

                    if (OutputVariable != null)
                    {
                        cmd.Parameters.Add(OutputVariable, SqlDbType.NVarChar, 50);
                        cmd.Parameters[OutputVariable].Direction = ParameterDirection.Output;
                    }
                    
                    using (StreamWriter writer = new StreamWriter(FilePath, true))
                    {
                        using (reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                object[] items = new object[reader.FieldCount];

                                sb.Clear();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    sb.Append("\"");
                                    sb.Append(reader.GetName(i));
                                    sb.Append("\"");
                                    sb.Append(strDelimiter);
                                }
                                writer.WriteLine(sb);

                                while (reader.Read())
                                {
                                    reader.GetValues(items);
                                    sb.Clear();
                                    foreach (var item in items)
                                    {
                                        sb.Append("\"");
                                        sb.Append(item);
                                        sb.Append("\"");
                                        sb.Append(strDelimiter);
                                    }
                                    writer.WriteLine(sb);
                                }
                            }
                        }
                    }                    

                    if (OutputVariable != null)
                    {
                        Parameters.Add(OutputVariable, cmd.Parameters[OutputVariable].Value.ToString());
                    }
                }
            }
        }
    }
}