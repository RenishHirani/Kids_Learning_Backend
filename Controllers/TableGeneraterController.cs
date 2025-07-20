using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.Reflection.Emit;
using System;
using System.Text;
using System.Xml.Linq;

namespace TableGeneratorApp.Controllers
{
    public class TableGeneraterController : Controller
    {
        private static string GeneratedTableScript = string.Empty;
        private static List<(string FieldName, string DataType)> AdditionalFields = new List<(string, string)>();
        private static string TableName = string.Empty;

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GenerateTable(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                ViewBag.Error = "Please enter a valid table name.";
                return View("Index");
            }

            TableName = tableName;
            AdditionalFields.Clear();
            GeneratedTableScript = GenerateTableSQL();
            ViewBag.GeneratedTableScript = GeneratedTableScript;
            return View("Index");
        }

        [HttpPost]
        public IActionResult AddField(string fieldName, string dataType)
        {
            if (string.IsNullOrEmpty(fieldName) || string.IsNullOrEmpty(dataType))
            {
                ViewBag.Error = "Please enter a valid field name and datatype.";
            }
            else
            {
                AdditionalFields.Add((fieldName, dataType));
            }

            GeneratedTableScript = GenerateTableSQL();
            ViewBag.GeneratedTableScript = GeneratedTableScript;
            ViewBag.AdditionalFields = AdditionalFields;
            ViewBag.SQLScripts = GenerateProceduresSQL();
            return View("Index");
        }

        private string GenerateTableSQL()
        {
            StringBuilder sb = new StringBuilder();
            string idColumn = TableName + "Id";

            sb.AppendLine($"CREATE TABLE {TableName} (");
            sb.AppendLine($"    {idColumn} INT PRIMARY KEY IDENTITY(1,1),");
            sb.AppendLine("    Name NVARCHAR(100),");
            sb.AppendLine("    CreatedDate DATETIME DEFAULT GETDATE(),");

            if (AdditionalFields.Count > 0)
            {
                foreach (var field in AdditionalFields)
                {
                    sb.AppendLine($"    {field.FieldName} {field.DataType},");
                }
            }

            sb.AppendLine(");\n");

            return sb.ToString();
        }

        private string GenerateProceduresSQL()
        {
            StringBuilder sb = new StringBuilder();
            string idColumn = TableName + "Id";
            string columns = "Name, CreatedDate" + (AdditionalFields.Count > 0 ? ", " + string.Join(", ", AdditionalFields.ConvertAll(f => f.FieldName)) : "");
            string parameters = "@Name, GETDATE()" + (AdditionalFields.Count > 0 ? ", " + string.Join(", ", AdditionalFields.ConvertAll(f => "@" + f.FieldName)) : "");

            sb.AppendLine($"CREATE PROCEDURE Insert_{TableName} @{columns.Replace(",", " NVARCHAR(100),")} NVARCHAR(100) AS");
            sb.AppendLine("BEGIN");
            sb.AppendLine($"    INSERT INTO {TableName} ({columns}) VALUES ({parameters})");
            sb.AppendLine("END\n");

            sb.AppendLine($"CREATE PROCEDURE Update_{TableName} @{idColumn} INT, @{columns.Replace(",", " NVARCHAR(100),")} NVARCHAR(100) AS");
            sb.AppendLine("BEGIN");
            sb.AppendLine($"    UPDATE {TableName} SET Name = @Name" +
                (AdditionalFields.Count > 0 ? ", " + string.Join(", ", AdditionalFields.ConvertAll(f => f.FieldName + " = @" + f.FieldName)) : "") +
                $" WHERE {idColumn} = @{idColumn}");
            sb.AppendLine("END\n");

            sb.AppendLine($"CREATE PROCEDURE Delete_{TableName} @{idColumn} INT AS");
            sb.AppendLine("BEGIN");
            sb.AppendLine($"    DELETE FROM {TableName} WHERE {idColumn} = @{idColumn}");
            sb.AppendLine("END\n");

            sb.AppendLine($"CREATE PROCEDURE Get_{TableName}ById @{idColumn} INT AS");
            sb.AppendLine("BEGIN");
            sb.AppendLine($"    SELECT {columns} FROM {TableName} WHERE {idColumn} = @{idColumn}");
            sb.AppendLine("END\n");

            sb.AppendLine($"CREATE PROCEDURE Get_All_{TableName} AS");
            sb.AppendLine("BEGIN");
            sb.AppendLine($"    SELECT {columns} FROM {TableName}");
            sb.AppendLine("END\n");

            return sb.ToString();
        }
    }
}
