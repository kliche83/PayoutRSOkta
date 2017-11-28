using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication3
{
    public class PMPaycheck
    {
        public string CheckDate { get; set; }
        public int TrainerId { get; set; }
        public string TrainerName { get; set; }
        public string Manager { get; set; }
        public string Role { get; set; }
        public decimal? Salary { get; set; }
        public decimal? Override { get; set; }
        public decimal? Commission { get; set; }
        public decimal? OverPay { get; set; }
        public decimal? UnderPay { get; set; }
        public decimal? OTPremium { get; set; }
        public decimal? VacationPay { get; set; }
        public decimal? TravelTime { get; set; }
        public decimal? ClawBack { get; set; }
        public decimal? Adjustment { get; set; }
        public string AdjustmentComment { get; set; }
        public decimal? Paycheck { get; set; }
    }
}
