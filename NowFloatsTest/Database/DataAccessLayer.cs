using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;
using NowFloatsTest.Models;
using System.Text;
using Newtonsoft.Json;
using NowFloatsTest.Util;

namespace NowFloatsTest.Database
{
    public class DataAccessLayer
    {
        private MySqlConnection _connection;

        public DataAccessLayer()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NowFloatsConn"].ConnectionString;
            _connection = new MySqlConnection(connectionString);
        }
        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                _connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                return false;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                _connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                return false;
            }
        }

        public bool CreateTable(SQLTable data)
        {
            try
            {
                StringBuilder tableQuery = new StringBuilder();
                tableQuery.AppendFormat("CREATE TABLE {0} (", data.TableName);

                if(data.Columns.Count == 0)
                {
                    return false;
                }
                else
                {
                    int i = 0;
                    foreach(var column in data.Columns)
                    {
                        i++;
                        if(i == data.Columns.Count)
                        {
                            tableQuery.Append(column.Key + " " + column.Value + ");");
                        }
                        else
                        {
                            tableQuery.Append(column.Key + " " + column.Value + ",");
                        }
                    }
                }
                //open connection
                if (this.OpenConnection() == true)
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(tableQuery.ToString(), _connection);

                    //Execute command
                    cmd.ExecuteNonQuery();

                    //close connection
                    this.CloseConnection();

                    return true;
                }
                return false;
            }
            catch(MySqlException mex)
            {
                this.CloseConnection();
                throw mex;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void InsertToUserTable(string tablename, string userId)
        {
            try
            {
                string query = string.Format("INSERT INTO UserTables(UserId, TableName) Values({0},'{1}')", userId, tablename);

                if (this.OpenConnection() == true)
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, _connection);

                    //Execute command
                    cmd.ExecuteNonQuery();

                    //close connection
                    this.CloseConnection();
                }
            }
            catch(MySqlException ex)
            {
                this.CloseConnection();
                throw ex;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public bool InsertData(string tableName, Dictionary<string, string> columns)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendFormat("INSERT INTO {0} (", tableName);

                if (columns.Count == 0)
                    return false;
                else
                {
                    int i = 0;
                    foreach(var key in columns.Keys)
                    {
                        i++;
                        if(i == columns.Count)
                            query.Append(key + ") Values (");
                        else
                            query.Append(key + ",");
                    }
                    i = 0;
                    foreach (var value in columns.Values)
                    {
                        i++;
                        if (i == columns.Count)
                            query.AppendFormat("'{0}');", value);
                        else
                            query.AppendFormat("'{0}',", value);
                    }
                }
                if (this.OpenConnection() == true)
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query.ToString(), _connection);

                    //Execute command
                    cmd.ExecuteNonQuery();

                    //close connection
                    this.CloseConnection();
                    return true;
                }
                return false;
            }
            catch (MySqlException ex)
            {
                this.CloseConnection();
                throw ex;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool UpdateRecord(int userId, object tableName, object updateObj, object condObj)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendFormat("UPDATE {0} SET ", tableName);

                var updateDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(updateObj.ToString());
                var conditionDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(condObj.ToString());

                if (updateDic != null && conditionDic != null && updateDic.Count > 0 && conditionDic.Count > 0)
                {
                    int i = 0;
                    foreach(var item in updateDic)
                    {
                        i++;
                        if (i == updateDic.Count)
                            query.AppendFormat("{0}='{1}' WHERE ", item.Key, item.Value);
                        else
                            query.AppendFormat("{0}='{1}', ", item.Key, item.Value);
                    }

                    i = 0;
                    foreach (var item in conditionDic)
                    {
                        i++;
                        if (i == conditionDic.Count)
                            query.AppendFormat("{0}='{1}';", item.Key, item.Value);
                        else
                            query.AppendFormat("{0}='{1}' AND ", item.Key, item.Value);
                    }
                }

                if (this.OpenConnection() == true)
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query.ToString(), _connection);

                    //Execute command
                    cmd.ExecuteNonQuery();

                    //close connection
                    this.CloseConnection();
                    return true;
                }
                return false;
            }
            catch (MySqlException ex)
            {
                this.CloseConnection();
                throw ex;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<string> GetData(int id, string tableName, int skip = 0, int limit = 10)
        {
            try
            {
                string query = string.Format("SELECT * FROM {0} LIMIT {1}, {2}", tableName, skip, skip+limit);
                List<string> dataList = new List<string>();
                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, _connection);

                    //Execute command
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            List<KeyValuePair<string, object>> data = new List<KeyValuePair<string, object>>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                KeyValuePair<string, object> keyValue = new KeyValuePair<string, object>(reader.GetName(i), reader.GetValue(i));
                                data.Add(keyValue);
                            }

                            JsonSerializerSettings settings = new JsonSerializerSettings { Converters = new[] { new KeyValueJsonConvertor() } };
                            string json = JsonConvert.SerializeObject(data, settings);
                            dataList.Add(json);
                        }
                    }

                    this.CloseConnection();
                }
                return dataList;
            }
            catch (MySqlException ex)
            {
                this.CloseConnection();
                throw ex;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IList<string> GetTablesById(int userid)
        {
            try
            {
                string query = string.Format("SELECT TableName FROM UserTables WHERE UserId={0}", userid);
                List<string> tables = new List<string>();
                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, _connection);

                    //Execute command
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            tables.Add(reader["TableName"].ToString());
                        }
                    }
                    
                    this.CloseConnection();
                }
                return tables;
            }
            catch (MySqlException ex)
            {
                this.CloseConnection();
                throw ex;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IList<string> ListTables()
        {
            try
            {
                List<string> tables = new List<string>();
                if (this.OpenConnection() == true)
                {
                    DataTable dt = _connection.GetSchema("Tables");
                    foreach (DataRow row in dt.Rows)
                    {
                        string tablename = (string)row[2];
                        tables.Add(tablename);
                    }

                    this.CloseConnection();
                }
                return tables;
            }
            catch(MySqlException ex)
            {
                this.CloseConnection();
                throw ex;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        //Delete statement
        public bool DeleteRecord(int userId, string tableName, object condObj)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendFormat("DELETE FROM {0}", tableName);

                Dictionary<string, string> conditionDic = null;
                if (condObj != null)
                    conditionDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(condObj.ToString());
                if (conditionDic != null && conditionDic.Count > 0)
                {
                    int i = 0;
                    query.Append(" WHERE ");
                    foreach (var item in conditionDic)
                    {
                        i++;
                        if (i == conditionDic.Count)
                            query.AppendFormat("{0}='{1}';", item.Key, item.Value);
                        else
                            query.AppendFormat("{0}='{1}' AND ", item.Key, item.Value);
                    }
                }
                if (this.OpenConnection() == true)
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query.ToString(), _connection);

                    //Execute command
                    cmd.ExecuteNonQuery();

                    //close connection
                    this.CloseConnection();

                    return true;
                }
                return false;
            }
            catch (MySqlException ex)
            {
                this.CloseConnection();
                throw ex;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}