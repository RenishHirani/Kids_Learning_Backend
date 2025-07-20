using AdmnPanel.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace AdmnPanel.Controllers
{
    public class GradeWiseChapterController : Controller
    {
        private IConfiguration configuration;

        public GradeWiseChapterController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IActionResult GradewiseChapterList()
        {
            DataTable dataTable = new DataTable();
            string connectionString = configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_SelectAll_GradewiseChapter", connection))
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
                    TempData["ErrorMessage"] = "Error fetching GradewiseChapter list.";
                    Console.WriteLine(ex.ToString());
                }
            }
            return View(dataTable);
        }

        public IActionResult GradewiseChapterDelete(int GradewiseChapterID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_Delete_GradewiseChapter", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@GradewiseChapterID", SqlDbType.Int).Value = GradewiseChapterID;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("GradewiseChapterList");
        }

        public IActionResult AddEditGradewiseChapter(int GradewiseChapterID)
        {
            GradeDropDown();
            ChapterDropDown();

            string connectionString = configuration.GetConnectionString("ConnectionString");
            DataTable table = new DataTable();
            GradeWiseChapterModel model = new GradeWiseChapterModel();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_SelectByID_GradewiseChapter", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@GradewiseChapterID", GradewiseChapterID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            table.Load(reader);
                        }
                    }

                    foreach (DataRow dataRow in table.Rows)
                    {
                        model.GradewiseChapterID = Convert.ToInt32(dataRow["GradewiseChapterID"]);
                        model.GradeID = Convert.ToInt32(dataRow["GradeID"]);
                        model.ChapterID = Convert.ToInt32(dataRow["ChapterID"]);
                        model.Sequence = Convert.ToInt32(dataRow["Sequence"]);
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error fetching GradewiseChapter details.";
                    Console.WriteLine(ex.ToString());
                }
            }
            return View("AddEditGradewiseChapter", model);
        }

        public IActionResult GradewiseChapterSave(GradeWiseChapterModel model)
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

                        if (model.GradewiseChapterID == 0)
                        {
                            command.CommandText = "PR_Insert_GradewiseChapter";
                        }
                        else
                        {
                            command.CommandText = "PR_Update_GradewiseChapter";
                            command.Parameters.Add("@GradewiseChapterID", SqlDbType.Int).Value = model.GradewiseChapterID;
                        }

                        command.Parameters.Add("@GradeID", SqlDbType.Int).Value = model.GradeID ;
                        command.Parameters.Add("@ChapterID", SqlDbType.Int).Value = model.ChapterID;
                        command.Parameters.Add("@Sequence", SqlDbType.Int).Value = model.Sequence;

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("GradewiseChapterList");
            }

            GradeDropDown();
            ChapterDropDown();
            return View("AddEditGradewiseChapter", model);
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

        public void ChapterDropDown()
        {
            string connectionString2 = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection2 = new SqlConnection(connectionString2);
            connection2.Open();
            SqlCommand command2 = connection2.CreateCommand();
            command2.CommandType = System.Data.CommandType.StoredProcedure;
            command2.CommandText = "PR_Chapter_DropDown";
            SqlDataReader reader2 = command2.ExecuteReader();
            DataTable dataTable2 = new DataTable();
            dataTable2.Load(reader2);
            List<ChapterDropDownModel> chapterList = new List<ChapterDropDownModel>();
            foreach (DataRow data in dataTable2.Rows)
            {
                ChapterDropDownModel model = new ChapterDropDownModel();
                model.ChapterID = Convert.ToInt32(data["ChapterID"]);
                model.ChapterName = data["ChapterName"].ToString();
                chapterList.Add(model);
            }
            ViewBag.ChapterList = chapterList;
        }
    }
}
