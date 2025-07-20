using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using AdmnPanel.Models;

namespace AdmnPanel.Controllers
{
    public class ChapterController : Controller
    {
        private IConfiguration configuration;

        public ChapterController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IActionResult ChapterList()
        {
            DataTable dataTable = new DataTable();
            string connectionString = configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_SelectAll_Chapter", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            dataTable.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error fetching chapter list.";
                    Console.WriteLine(ex.ToString());
                }
            }
            return View(dataTable);
        }

        public IActionResult ChapterDelete(int ChapterID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_Delete_Chapter", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@ChapterID", SqlDbType.Int).Value = ChapterID;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("ChapterList");
        }

        public IActionResult AddEditChapter(int ChapterID)
        {
            SubjectDropDown();

            string connectionString = configuration.GetConnectionString("ConnectionString");
            DataTable table = new DataTable();
            ChapterModel model = new ChapterModel();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_SelectByID_Chapter", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ChapterID", ChapterID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            table.Load(reader);
                        }
                    }

                    foreach (DataRow dataRow in table.Rows)
                    {
                        model.ChapterID = Convert.ToInt32(dataRow["ChapterID"]);
                        model.ChapterName = dataRow["ChapterName"].ToString();
                        model.SubjectID = Convert.ToInt32(dataRow["SubjectID"]);
                        model.Sequence = Convert.ToInt32(dataRow["Sequence"]);
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error fetching chapter details.";
                    Console.WriteLine(ex.ToString());
                }
            }
            return View("AddEditChapter", model);
        }

        public IActionResult ChapterSave(ChapterModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ChapterName))
            {
                ModelState.AddModelError("ChapterName", "Chapter Name is required.");
            }

            if (ModelState.IsValid)
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        if (model.ChapterID == 0)
                        {
                            command.CommandText = "PR_Insert_Chapter";
                        }
                        else
                        {
                            command.CommandText = "PR_Update_Chapter";
                            command.Parameters.Add("@ChapterID", SqlDbType.Int).Value = model.ChapterID;
                        }

                        command.Parameters.Add("@ChapterName", SqlDbType.NVarChar, 255).Value = model.ChapterName;
                        command.Parameters.Add("@SubjectID", SqlDbType.Int).Value = model.SubjectID;
                        command.Parameters.Add("@Sequence", SqlDbType.Int).Value = model.Sequence;

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("ChapterList");
            }
            SubjectDropDown();
            return View("AddEditChapter", model);
        }

        public void SubjectDropDown()
        {
            string connectionString2 = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection2 = new SqlConnection(connectionString2);
            connection2.Open();
            SqlCommand command2 = connection2.CreateCommand();
            command2.CommandType = System.Data.CommandType.StoredProcedure;
            command2.CommandText = "PR_Subject_DropDown";
            SqlDataReader reader2 = command2.ExecuteReader();
            DataTable dataTable2 = new DataTable();
            dataTable2.Load(reader2);
            List<SubjectDropDownModel> subjectList = new List<SubjectDropDownModel>();
            foreach (DataRow data in dataTable2.Rows)
            {
                SubjectDropDownModel model = new SubjectDropDownModel();
                model.SubjectID = Convert.ToInt32(data["SubjectID"]);
                model.SubjectName = data["Name"].ToString();
                subjectList.Add(model);
            }
            ViewBag.SubjectList = subjectList;
        }
    }
}
