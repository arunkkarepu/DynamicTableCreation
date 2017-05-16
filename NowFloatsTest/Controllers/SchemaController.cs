using Newtonsoft.Json;
using NowFloatsTest.Database;
using NowFloatsTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NowFloatsTest.Controllers
{
    public class SchemaController : ApiController
    {
        // GET api/schema/GetAllTables
        [HttpGet]
        [ActionName("GetAllTables")]
        public IEnumerable<string> Get()
        {
            DataAccessLayer db = new DataAccessLayer();
            return db.ListTables();
        }

        // GET api/schema/GetTables?id=1
        [HttpGet]
        [ActionName("GetTables")]
        public IList<string> Get(int id)
        {
            try
            {
                DataAccessLayer db = new DataAccessLayer();
                return db.GetTablesById(id);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        // POST api/schema/CreateSchema
        [HttpPost]
        [ActionName("CreateSchema")]
        public string CreateSchema([FromBody]object strBody)
        {
            try
            {
                if (strBody != null)
                {
                    var tableData = JsonConvert.DeserializeObject<SQLTable>(strBody.ToString());
                    DataAccessLayer db = new DataAccessLayer();
                    bool success = db.CreateTable(tableData);
                    if (success)
                    {
                        db.InsertToUserTable(tableData.TableName, tableData.UserId);
                        return "Table created successfully";
                    }
                }
                return "Table creation failed";
            }
            catch(Exception ex)
            {
                return string.Format("Table creation failed, ex: {0}", ex.Message);
            }
        }

        [HttpPost]
        [ActionName("AddData")]
        public string InsertData(int id, string tableName, [FromBody] object strBody)
        {
            try
            {
                if (strBody != null)
                {
                    var tableData = JsonConvert.DeserializeObject<Dictionary<string, string>>(strBody.ToString());
                    DataAccessLayer db = new DataAccessLayer();
                    bool success = db.InsertData(tableName, tableData);
                    if (success)
                        return "Inserted successfully";
                }
            }
            catch (Exception ex)
            {
                return string.Format("Insertion failed, ex:{0}", ex.Message);
            }
            return "Insertion failed";
        }

        // GET api/schema/GetData?id=1&tableName=Employee&skip=0&limit=10
        [HttpGet]
        [ActionName("GetData")]
        public string GetData(int id, string tableName, int skip = 0, int limit = 10)
        {
            try
            {
                //check whether table exists with that id or not
                DataAccessLayer db = new DataAccessLayer();
                List<string> data = db.GetData(id, tableName, skip, limit);
                return JsonConvert.SerializeObject(data);
            }
            catch (Exception e)
            {
                return "GetData exception : " + e.Message;
            }
        }

        [HttpPut]
        [ActionName("UpdateRecord")]
        public string UpdateRecord(int id,[FromBody] object strBody)
        {
            try
            {
                if (strBody == null)
                {
                    return "No body found!";
                }
                var body = JsonConvert.DeserializeObject<Dictionary<string, object>>(strBody.ToString());
                DataAccessLayer db = new DataAccessLayer();
                bool success = db.UpdateRecord(id, body["table"].ToString(), body["update"], body["condition"]);
                if(success)
                    return "Update successfull";
                return "Failed to update";
            }
            catch(Exception e)
            {
                return "update exception : " + e.Message;
            }
}

        // DELETE api/schema/DeleteRecord?id=1
        [HttpDelete]
        [ActionName("DeleteRecord")]
        public string DeleteRecord(int id, [FromBody] object strBody)
        {
            try
            {
                if (strBody == null)
                {
                    return "No body found!";
                }
                var body = JsonConvert.DeserializeObject<Dictionary<string, object>>(strBody.ToString());
                DataAccessLayer db = new DataAccessLayer();
                bool success = db.DeleteRecord(id, body["table"].ToString(), body["condition"]);
                if (success)
                    return "Record deleted successfully";
                return "Failed to delete";
            }
            catch(Exception e)
            {
                return "Delete exception : " + e.Message;
            }
        }

        [HttpDelete]
        [ActionName("DeleteAllRecords")]
        public string DeleteAllRecords(int id, [FromBody] object strBody)
        {
            try
            {
                if (strBody == null)
                {
                    return "No body found!";
                }
                var body = JsonConvert.DeserializeObject<Dictionary<string, object>>(strBody.ToString());
                DataAccessLayer db = new DataAccessLayer();
                bool success = db.DeleteRecord(id, body["table"].ToString(), null);
                if (success)
                    return "Record deleted successfully";
                return "Failed to delete";
            }
            catch (Exception e)
            {
                return "Delete exception : " + e.Message;
            }
        }
    }
}