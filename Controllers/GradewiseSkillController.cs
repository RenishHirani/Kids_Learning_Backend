using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using AdmnPanel.Models;

namespace AdmnPanel.Controllers
{
    public class GradewiseSkillController : Controller
    {
        private IConfiguration configuration;

        public GradewiseSkillController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IActionResult GradewiseSkillList()
        {
            DataTable dataTable = new DataTable();
            string connectionString = configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_GetAll_GradeWiseSkill", connection))
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
                    TempData["ErrorMessage"] = "Error fetching GradeWiseSkill list.";
                    Console.WriteLine(ex.ToString());
                }
            }
            return View(dataTable);
        }

        public IActionResult GradewiseSkillDelete(int GradewiseSkillID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_Delete_GradeWiseSkill", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@GradeWiseSkillID", SqlDbType.Int).Value = GradewiseSkillID;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("GradewiseSkillList");
        }

        public IActionResult AddEditGradewiseSkill(int GradewiseSkillID)
        {
            GradeDropDown();
            SkillDropDown();
            ChapterDropDown();


            string connectionString = configuration.GetConnectionString("ConnectionString");
            DataTable table = new DataTable();
            GradeWiseSkillModel model = new GradeWiseSkillModel();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_SelectByID_GradeWiseSkill", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@GradeWiseSkillID", GradewiseSkillID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            table.Load(reader);
                        }
                    }

                    foreach (DataRow dataRow in table.Rows)
                    {
                        model.GradeWiseSkillID = Convert.ToInt32(dataRow["GradeWiseSkillID"]);
                        model.GradeID = Convert.ToInt32(dataRow["GradeID"]);
                        model.SkillID = Convert.ToInt32(dataRow["SkillID"]);
                        model.ChapterID = Convert.ToInt32(dataRow["ChapterID"]);
                        model.Sequence = Convert.ToInt32(dataRow["Sequence"]);
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error fetching GradewiseSkill details.";
                    Console.WriteLine(ex.ToString());
                }
            }
            return View("AddEditGradewiseSkill", model);
        }

        public IActionResult GradewiseSkillSave(GradeWiseSkillModel model)
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

                        if (model.GradeWiseSkillID == 0)
                        {
                            command.CommandText = "PR_Insert_GradeWiseSkill";
                        }
                        else
                        {
                            command.CommandText = "PR_Update_GradeWiseSkill";
                            command.Parameters.Add("@GradeWiseSkillID", SqlDbType.Int).Value = model.GradeWiseSkillID;
                        }

                        command.Parameters.Add("@GradeID", SqlDbType.Int).Value = model.GradeID;
                        command.Parameters.Add("@SkillID", SqlDbType.Int).Value = model.SkillID;
                        command.Parameters.Add("@ChapterID", SqlDbType.Int).Value = model.ChapterID;
                        command.Parameters.Add("@Sequence", SqlDbType.Int).Value = model.Sequence;

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("GradewiseSkillList");
            }

            GradeDropDown();
            SkillDropDown();
            ChapterDropDown();
            return View("AddEditGradewiseSkill", model);
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

        public void SkillDropDown()
        {
            string connectionString2 = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection2 = new SqlConnection(connectionString2);
            connection2.Open();
            SqlCommand command2 = connection2.CreateCommand();
            command2.CommandType = System.Data.CommandType.StoredProcedure;
            command2.CommandText = "PR_Skill_DropDown";
            SqlDataReader reader2 = command2.ExecuteReader();
            DataTable dataTable2 = new DataTable();
            dataTable2.Load(reader2);
            List<SkillDropDownModel> skillList = new List<SkillDropDownModel>();
            foreach (DataRow data in dataTable2.Rows)
            {
                SkillDropDownModel model = new SkillDropDownModel();
                model.SkillID = Convert.ToInt32(data["SkillID"]);
                model.SkillName = data["SkillName"].ToString();
                skillList.Add(model);
            }
            ViewBag.SkillList = skillList;
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
