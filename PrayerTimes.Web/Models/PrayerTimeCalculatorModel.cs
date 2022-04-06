using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using PrayerTimes.Library.Enumerations;

namespace PrayerTimes.Web.Models;

public class PrayerTimeCalculatorModel
{
    [BindProperty]
    [Required]
    public double Latitude { get; set; }

    [BindProperty]
    [Required]
    public double Longitude { get; set; }

    [BindProperty]
    [Required]
    public double Altitude { get; set; }

    [BindProperty, DataType(DataType.Date)]
    [Required]
    public DateTime CalculationDate { get; set; }

    [BindProperty]
    [Required]
    public double TimeZone { get; set; }

 
    [BindProperty]
    [Required]
    public int LunarHijriOffset { get; set; }

    [BindProperty]
    [Required]
    public bool DayLightSavingsTime { get; set; }

    [BindProperty]
    [Required]
    public CalculationMethodPreset CalculationMethodPreset { get; set; }






}