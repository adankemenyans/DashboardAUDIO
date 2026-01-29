using System;

namespace DashboardAudio.Models
{
    public class AssemblyData
    {
        public DateTime DateTime { get; set; }
        public string Model { get; set; }
        public int DailyPlan { get; set; }
        public int Target { get; set; }
        public int Actual { get; set; }
        public float Weight { get; set; }
        public int Efficiency { get; set; }
    }
}