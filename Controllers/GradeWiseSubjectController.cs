using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using AdmnPanel.Models;

namespace AdmnPanel.Controllers
{
    public class GradeWiseSubjectController : Controller
    {
        private IConfiguration configuration;

        public GradeWiseSubjectController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IActionResult GradewiseSubjectList()
        {
            DataTable dataTable = new DataTable();
            string connectionString = configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_SelectAll_GradewiseSubject", connection))
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
                    TempData["ErrorMessage"] = "Error fetching GradewiseSubject list.";
                    Console.WriteLine(ex.ToString());
                }
            }
            return View(dataTable);
        }

        public IActionResult GradewiseSubjectDelete(int GradewiseSubjectID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_Delete_GradewiseSubject", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@GradewiseSubjectID", SqlDbType.Int).Value = GradewiseSubjectID;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("GradewiseSubjectList");
        }

        public IActionResult AddEditGradewiseSubject(int GradewiseSubjectID)
        {
            GradeDropDown();
            SubjectDropDown();


            string connectionString = configuration.GetConnectionString("ConnectionString");
            DataTable table = new DataTable();
            GradeWiseSubjectModel model = new GradeWiseSubjectModel();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_SelectByID_GradewiseSubject", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@GradewiseSubjectID", GradewiseSubjectID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            table.Load(reader);
                        }
                    }

                    foreach (DataRow dataRow in table.Rows)
                    {
                        model.GradewiseSubjectID = Convert.ToInt32(dataRow["GradewiseSubjectID"]);
                        model.GradeID = Convert.ToInt32(dataRow["GradeID"]);
                        model.SubjectID = Convert.ToInt32(dataRow["SubjectID"]);
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error fetching GradewiseSubject details.";
                    Console.WriteLine(ex.ToString());
                }
            }
            return View("AddEditGradewiseSubject", model);
        }

        public IActionResult GradewiseSubjectSave(GradeWiseSubjectModel model)
        {
            if (ModelState.IsValid)
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        if (model.GradewiseSubjectID == 0)
                        {
                            command.CommandText = "PR_Insert_GradewiseSubject";
                        }
                        else
                        {
                            command.CommandText = "PR_Update_GradewiseSubject";
                            command.Parameters.Add("@GradewiseSubjectID", SqlDbType.Int).Value = model.GradewiseSubjectID;
                        }

                        command.Parameters.Add("@GradeID", SqlDbType.Int).Value = model.GradeID;
                        command.Parameters.Add("@SubjectID", SqlDbType.Int).Value = model.SubjectID;

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("GradewiseSubjectList");
            }

            GradeDropDown();
            SubjectDropDown();
            return View("AddEditGradewiseSubject", model);
        }

        public void GradeDropDown()
        {
            string connectionString2 = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection2 = new SqlConnection(connectionString2);
            connection2.Open();
            SqlCommand command2 = connection2.CreateCommand();
            command2.CommandType = System.Data.CommandType.StoredProcedure;
            command2.CommandText = "PR_Grade_DropDown";
            SqlDataReader reader2 = command2.ExecuteReader();
            DataTable dataTable2 = new DataTable();
            dataTable2.Load(reader2);
            List<GradeDropDownModel> gradeList = new List<GradeDropDownModel>();
            foreach (DataRow data in dataTable2.Rows)
            {
                GradeDropDownModel model = new GradeDropDownModel();
                model.GradeID = Convert.ToInt32(data["GradeID"]);
                model.GradeName = data["Name"].ToString();
                gradeList.Add(model);
            }
            ViewBag.GradeList = gradeList;
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
