using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NodaTime;
using PrayerTimes.Library.Calculators;
using PrayerTimes.Library.Helpers;
using PrayerTimes.Library.Models;
using PrayerTimes.Web.Models;
using TimeZoneNames;

namespace PrayerTimes.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public PrayerTimeCalculatorModel PrayerTimeCalculatorModel { get; set; }

        public string PrayerTimes { get; set; }

        public PrayerTimesOutputModel PrayerTimesOutput { get; set; }
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public void OnPostSubmit(PrayerTimeCalculatorModel prayerTimeCalculatorModel)
        {
            if (prayerTimeCalculatorModel.DayLightSavingsTime)
            {
                prayerTimeCalculatorModel.TimeZone = prayerTimeCalculatorModel.TimeZone + 1.0;
            }

            var when = Instant.FromDateTimeUtc(prayerTimeCalculatorModel.CalculationDate.ToUniversalTime());
            var settings = new PrayerCalculationSettings();
            settings.CalculationMethod.SetCalculationMethodPreset(when, prayerTimeCalculatorModel.CalculationMethodPreset);
            var geo = new Geocoordinate(prayerTimeCalculatorModel.Latitude, prayerTimeCalculatorModel.Longitude, prayerTimeCalculatorModel.Altitude);

            var prayer = Prayers.On(when, settings, geo, prayerTimeCalculatorModel.TimeZone);
            var solarDate = prayertimescore.PrayerTimes.Library.Calender.Calendar.ConvertToPersian(when.ToDateTimeUtc());
            var lunarDate = prayertimescore.PrayerTimes.Library.Calender.Calendar.ConvertToIslamic(when.ToDateTimeUtc().AddDays(prayerTimeCalculatorModel.LunarHijriOffset));
            var gregorianDate = when.ToDateTimeUtc();

            var solarDateString = $" {solarDate.ToString("english_day")} {solarDate.ToString("english_month")} {solarDate.ToString("english_year")}";

            var sb = new StringBuilder();
            sb.AppendLine(
                $"<b>Prayer times for location: {geo.Latitude:F4}, {geo.Longitude:F4} and altitude {geo.Altitude:F4}</b><br/>");
            sb.AppendLine($"<b>Date: {gregorianDate:D} | Solar Hijri: {solarDateString} | Lunar Hijri: {lunarDate.ToString("english_formatted")}</b><br/>");
            sb.AppendLine("<table class=\"table-info table-success table-bordered\">");
            sb.AppendLine("<tr>");
            sb.AppendLine(
                "<th>Imsaak</th><th>Fajr</th><th>Sunrise</th><th>Dhuhr</th><th>Asr</th><th>Sunset</th><th>Maghreb</th><th>Isha</th><th>Midnight</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("<tr>");
            sb.AppendLine($"<td>{GetPrayerTimeString(prayer.Imsak, prayerTimeCalculatorModel.TimeZone)}</td>");
            sb.AppendLine($"<td>{GetPrayerTimeString(prayer.Fajr, prayerTimeCalculatorModel.TimeZone)}</td>");
            sb.AppendLine($"<td>{GetPrayerTimeString(prayer.Sunrise, prayerTimeCalculatorModel.TimeZone)}</td>");
            sb.AppendLine($"<td>{GetPrayerTimeString(prayer.Dhuhr, prayerTimeCalculatorModel.TimeZone)}</td>");
            sb.AppendLine($"<td>{GetPrayerTimeString(prayer.Asr, prayerTimeCalculatorModel.TimeZone)}</td>");
            sb.AppendLine($"<td>{GetPrayerTimeString(prayer.Sunset, prayerTimeCalculatorModel.TimeZone)}</td>");
            sb.AppendLine($"<td>{GetPrayerTimeString(prayer.Maghrib, prayerTimeCalculatorModel.TimeZone)}</td>");
            sb.AppendLine($"<td>{GetPrayerTimeString(prayer.Isha, prayerTimeCalculatorModel.TimeZone)}</td>");
            sb.AppendLine($"<td>{GetPrayerTimeString(prayer.Midnight, prayerTimeCalculatorModel.TimeZone)}</td>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</table>");
          
            PrayerTimes = sb.ToString();
        }

        private static string GetPrayerTimeString(Instant instant, double timeZone)
        {
            var zoned = instant.InZone(DateTimeZone.ForOffset(Offset.FromTimeSpan(TimeSpan.FromHours(timeZone))));
            return zoned.ToString("HH:mm", CultureInfo.InvariantCulture);
        }

        public static IEnumerable<SelectListItem> GetCountriesList()
        {
            var returnList = new List<SelectListItem>();

            var tzList = TZNames.GetDisplayNames("en_GB", true);

            foreach (var tz in tzList)
            {
                try
                {
                    var thisItem = new SelectListItem
                    {
                        Disabled = false,
                        Group = null,
                        Selected = false,
                        Text = tz.Value,
                    };

                    var thisOffsetInitial = tz.Value
                        .Split(" ")[0]
                        .Replace("UTC", "")
                        .Replace("(", "")
                        .Replace(")", "")
                        .Replace(":", ".");

                    if (!string.IsNullOrEmpty(thisOffsetInitial))
                    {
                        var offsetHours = double.Parse(thisOffsetInitial.Split(".").First());
                        var offsetMinutes = double.Parse(thisOffsetInitial.Split(".").Last());
                        var offsetFraction = offsetMinutes == 0 ? 0 : (offsetMinutes / 60) * 100;

                        var returnOffSetAsDouble = double.Parse($"{offsetHours}.{offsetFraction}");

                        thisItem.Value = $"{returnOffSetAsDouble:F}";
                    }
                    else
                    {
                        thisItem.Value = "0.00";
                    }

                    returnList.Add(thisItem);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return returnList;
        }
    }
}