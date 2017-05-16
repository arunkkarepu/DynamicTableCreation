"# DynamicTableCreation"
Db:Mysql,
DatabaseName: NowFloats

Table creation(POST):
url: http://localhost:14772/api/schema/CreateSchema
body:
{
	"TableName":"Employee",
	"UserId":"1",
	"Columns":{
		"EmpID":"char(50)",
		"EmpName":"char(50)",
		"Designation":"char(50)"
	}
}

GetTables by Id(GET):
url: http://localhost:14772/api/schema/GetTables?id=1

InsertData(POST):
url: http://localhost:14772/api/schema/AddData?id=1&tableName=Employee
body:
{
	"EmpName":"Arun Kumar",
	"EmpID": 2,
	"Designation": "234"
}

GetData(GET):
http://localhost:14772/api/schema/GetData?id=1&tableName=Employee&skip=0&limit=10

UpdateData(PUT):
url: http://localhost:14772/api/schema/UpdateRecord?id=1
body:
{
	"table":"Employee",
	"update":{
		"EmpName":"Kumar"
	},
	"condition":{
		"EmpID":1,
		"Designation":"234"
	}
}

Delete particular record(DELETE):
url:http://localhost:14772/api/schema/DeleteRecord?id=1
body:
{
	"table":"Employee",
	"condition":{
		"EmpID":2,
		"Designation":"234"
	}
}

DeleteAll(DELETE):
url: http://localhost:14772/api/schema/DeleteAllRecords?id=1
body:
{
	"table":"Employee"
}





