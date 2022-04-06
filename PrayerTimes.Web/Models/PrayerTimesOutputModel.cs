using PrayerTimes.Library.Calculators;
using prayertimescore.PrayerTimes.Library.Calender;

namespace PrayerTimes.Web.Models;

public class PrayerTimesOutputModel
{
    public bool Ready { get; set; }
    public SolarDate? SolarDate { get; set; }
    public LunarDate? LunarDate { get; set; }
    public DateTime GregorianDate { get; set; }
    public Prayers? Prayers { get; set; }
    public string? Imsaak { get; set; }
    public string? Fajr { get; set; }
    public string? Sunrise { get; set; }
    public string? Dhuhr { get; set; }
    public string? Asr { get; set; }
    public string? Sunset { get; set; }
    public string? Maghreb { get; set; }
    public string? Isha { get; set; }
    public string? Midnight { get; set; }
}