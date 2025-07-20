using AdmnPanel.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace AdmnPanel.Controllers
{
    public class GradeController : Controller
    {
       
        private IConfiguration configuration;

        public GradeController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public IActionResult GradeList()
        {
            DataTable dataTable = new DataTable();
            string connectionString = configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_SelectAll_Grade", connection))
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
                    TempData["ErrorMessage"] = "Error fetching grade list.";
                    Console.WriteLine(ex.ToString());
                }
            }
            return View(dataTable);
        }

        public IActionResult GradeDelete(int GradeID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_Delete_Grade", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@GradeID", SqlDbType.Int).Value = GradeID;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("GradeList");
        }

        public IActionResult AddEditGrade(int GradeID)
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            DataTable table = new DataTable();
            GradeModel gradeModel = new GradeModel();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_SelectByID_Grade", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@GradeID", GradeID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            table.Load(reader);
                        }
                    }

                    foreach (DataRow dataRow in table.Rows)
                    {
                        gradeModel.GradeID = Convert.ToInt32(dataRow["GradeID"]);
                        gradeModel.GradeName = dataRow["Name"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error fetching grade details.";
                    Console.WriteLine(ex.ToString());
                }
            }
            return View("AddEditGrade", gradeModel);
        }

        public IActionResult GradeSave(GradeModel gradeModel)
        {
            if (string.IsNullOrWhiteSpace(gradeModel.GradeName))
            {
                ModelState.AddModelError("Name", "Grade Name is required.");
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

                        if (gradeModel.GradeID == 0)
                        {
                            command.CommandText = "PR_Insert_Grade";
                        }
                        else
                        {
                            command.CommandText = "PR_Update_Grade";
                            command.Parameters.Add("@GradeID", SqlDbType.Int).Value = gradeModel.GradeID;
                        }

                        command.Parameters.Add("@Name", SqlDbType.NVarChar, 255).Value = gradeModel.GradeName;

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("GradeList");
            }

            return View("AddEditGrade", gradeModel);
        }

    }
}
