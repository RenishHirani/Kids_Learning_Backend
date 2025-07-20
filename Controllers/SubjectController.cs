using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using AdmnPanel.Models;

namespace AdmnPanel.Controllers
{
    public class SubjectController : Controller
    {
        private IConfiguration configuration;

        public SubjectController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IActionResult SubjectList()
        {
            DataTable dataTable = new DataTable();
            string connectionString = configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_SelectAll_Subject", connection))
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
                    TempData["ErrorMessage"] = "Error fetching subject list.";
                    Console.WriteLine(ex.ToString());
                }
            }
            return View(dataTable);
        }

        public IActionResult SubjectDelete(int SubjectID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_Delete_Subject", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SubjectID", SqlDbType.Int).Value = SubjectID;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("SubjectList");
        }

        public IActionResult AddEditSubject(int SubjectID)
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            DataTable table = new DataTable();
            SubjectModel subjectModel = new SubjectModel();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_SelectByID_Subject", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SubjectID", SubjectID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            table.Load(reader);
                        }
                    }

                    foreach (DataRow dataRow in table.Rows)
                    {
                        subjectModel.SubjectID = Convert.ToInt32(dataRow["SubjectID"]);
                        subjectModel.SubjectName = dataRow["Name"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error fetching subject details.";
                    Console.WriteLine(ex.ToString());
                }
            }
            return View("AddEditSubject", subjectModel);
        }

        public IActionResult SubjectSave(SubjectModel subjectModel)
        {
            if (string.IsNullOrWhiteSpace(subjectModel.SubjectName))
            {
                ModelState.AddModelError("Name", "Subject Name is required.");
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

                        if (subjectModel.SubjectID == 0)
                        {
                            command.CommandText = "PR_Insert_Subject";
                        }
                        else
                        {
                            command.CommandText = "PR_Update_Subject";
                            command.Parameters.Add("@SubjectID", SqlDbType.Int).Value = subjectModel.SubjectID;
                        }

                        command.Parameters.Add("@Name", SqlDbType.NVarChar, 255).Value = subjectModel.SubjectName;

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("SubjectList");
            }

            return View("AddEditSubject", subjectModel);
        }

    }
}
