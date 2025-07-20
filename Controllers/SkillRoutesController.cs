using AdmnPanel.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace AdmnPanel.Controllers
{
    public class SkillRoutesController : Controller
    {
        private IConfiguration configuration;

        public SkillRoutesController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IActionResult SkillRoutesList()
        {
            DataTable dataTable = new DataTable();
            string connectionString = configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_GETALL_SkillRoutes", connection))
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
                    TempData["ErrorMessage"] = "Error fetching SkillRoutes list.";
                    Console.WriteLine(ex.ToString());
                }
            }
            return View(dataTable);
        }

        public IActionResult SkillRouteDelete(int skillRoutesID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_Delete_SkillRoutes", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@SkillRoutesID", SqlDbType.Int).Value = skillRoutesID;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("SkillRoutesList");
        }
        public IActionResult AddEditSkillRoute(int skillRoutesID)
        {
            SkillDropDown();
            SkillRoutesModel model = new SkillRoutesModel();
            string connectionString = configuration.GetConnectionString("ConnectionString");
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_SelectByID_SkillRoutes", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SkillRoutesID", skillRoutesID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            table.Load(reader);
                        }
                    }

                    foreach (DataRow row in table.Rows)
                    {
                        model.SkillRoutesID = Convert.ToInt32(row["SkillRoutesID"]);
                        model.ControllerName = row["ControllerName"].ToString();
                        model.ActionName = row["ActionName"].ToString();
                        model.SkillID = Convert.ToInt32(row["SkillID"]);
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error fetching SkillRoute details.";
                    Console.WriteLine(ex.ToString());
                }
            }
            return View("AddEditSkillRoute", model);
        }

        public IActionResult SkillRouteSave(SkillRoutesModel model)
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

                        if (model.SkillRoutesID == 0)
                        {
                            command.CommandText = "PR_SkillRoutes_Insert";
                        }
                        else
                        {
                            command.CommandText = "PR_Update_SkillRoutes";
                            command.Parameters.Add("@SkillRoutesID", SqlDbType.Int).Value = model.SkillRoutesID;
                        }

                        command.Parameters.Add("@ControllerName", SqlDbType.NVarChar, 50).Value = model.ControllerName;
                        command.Parameters.Add("@ActionName", SqlDbType.NVarChar, 50).Value = model.ActionName;
                        command.Parameters.Add("@SkillID", SqlDbType.Int).Value = model.SkillID;

                        command.ExecuteNonQuery();
                    }
                }
                return RedirectToAction("SkillRoutesList");
            }
            SkillDropDown();
            return View("AddEditSkillRoute", model);
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
    }
}
