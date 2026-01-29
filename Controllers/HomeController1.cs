using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using DashboardAudio.Models;

public class HomeController : Controller
{
    private readonly IConfiguration _configuration;

    public HomeController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public JsonResult GetProductionData()
    {
        var dashboardData = new Dictionary<string, object>();

        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                for (int i = 1; i <= 11; i++)
                {
                    try
                    {
                        string tableName = $"dbo.FINAL{i}";
                        string query = $@"
                        SELECT TOP 1 
                            DailyPlan, 
                            Target, 
                            Actual, 
                            Efficiency 
                        FROM {tableName} 
                        ORDER BY ID DESC";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    var data = new
                                    {
                                        totalActual = reader["Actual"] != DBNull.Value ? Convert.ToInt32(reader["Actual"]) : 0,
                                        totalPlan = reader["Target"] != DBNull.Value ? Convert.ToInt32(reader["Target"]) : 0,
                                        dailyPlan = reader["DailyPlan"] != DBNull.Value ? Convert.ToInt32(reader["DailyPlan"]) : 0,
                                        qualityRate = reader["Efficiency"] != DBNull.Value ? Convert.ToInt32(reader["Efficiency"]) : 0,
                                        totalDefects = 0,
                                        status = "OK"
                                    };
                                    dashboardData.Add($"line{i}", data);
                                }
                                else
                                {
                                    // Tabel ada, tapi data kosong
                                    dashboardData.Add($"line{i}", new { totalActual = 0, totalPlan = 0, dailyPlan = 0, qualityRate = 0, totalDefects = 0, status = "EMPTY" });
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        System.Diagnostics.Debug.WriteLine($"Error Line {i}: {ex.Message}");

                        dashboardData.Add($"line{i}", new
                        {
                            totalActual = 0,
                            totalPlan = 0,
                            dailyPlan = 0,
                            qualityRate = 0,
                            totalDefects = 0,
                            status = "ERROR",
                            errorMessage = ex.Message 
                        });
                    }
                }
            }
        }
        catch (Exception globalEx)
        {
            return Json(new { error = "Connection Error: " + globalEx.Message });
        }

        return Json(dashboardData);
    }
}