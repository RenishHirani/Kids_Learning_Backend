using AdmnPanel.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace AdmnPanel.Controllers
{
    public class SkillController : Controller
    {
        private IConfiguration configuration;

        public SkillController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IActionResult SkillList()
        {
            DataTable dataTable = new DataTable();
            string connectionString = configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_SelectAll_Skill", connection))
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
                    TempData["ErrorMessage"] = "Error fetching skill list.";
                    Console.WriteLine(ex.ToString());
                }
            }
            return View(dataTable);
        }

        public IActionResult SkillDelete(int SkillID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_Delete_Skill", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SkillID", SqlDbType.Int).Value = SkillID;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("SkillList");
        }

        public IActionResult AddEditSkill(int SkillID)
        {
            

            string connectionString = configuration.GetConnectionString("ConnectionString");
            DataTable table = new DataTable();
            SkillModel model = new SkillModel();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_SelectByID_Skill", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SkillID", SkillID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            table.Load(reader);
                        }
                    }

                    foreach (DataRow dataRow in table.Rows)
                    {
                        model.SkillID = Convert.ToInt32(dataRow["SkillID"]);
                        model.SkillName = dataRow["SkillName"].ToString();
                        model.Sequence = Convert.ToInt32(dataRow["Sequence"]);
                        model.ShortDescription = dataRow["ShortDescription"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error fetching skill details.";
                    Console.WriteLine(ex.ToString());
                }
            }
            return View("AddEditSkill", model);
        }

        public IActionResult SkillSave(SkillModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SkillName))
            {
                ModelState.AddModelError("SkillName", "Skill Name is required.");
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

                        if (model.SkillID == 0)
                        {
                            command.CommandText = "PR_Insert_Skill";
                        }
                        else
                        {
                            command.CommandText = "PR_Update_Skill";
                            command.Parameters.Add("@SkillID", SqlDbType.Int).Value = model.SkillID;
                        }

                        command.Parameters.Add("@SkillName", SqlDbType.NVarChar, 255).Value = model.SkillName;
                        command.Parameters.Add("@Sequence", SqlDbType.Int).Value = model.Sequence;
                        command.Parameters.Add("@ShortDescription", SqlDbType.NVarChar, -1).Value = model.ShortDescription;

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("SkillList");
            }
            return View("AddEditSkill", model);
        }


       
    }
}
