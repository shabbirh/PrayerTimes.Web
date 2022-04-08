using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NodaTime;
using PrayerTimes.Library.Calculators;
using PrayerTimes.Library.Enumerations;
using PrayerTimes.Library.Helpers;
using PrayerTimes.Library.Models;
using PrayerTimes.Web.Helpers;
using PrayerTimes.Web.Models;
using TimeZoneNames;

namespace PrayerTimes.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public PrayerTimeCalculatorModel PrayerTimeCalculatorModel { get; set; }

        private static List<InternalTimeZoneInfo> TimeZoneList { get; set; } = new List<InternalTimeZoneInfo>();

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
                prayerTimeCalculatorModel.TimeZone += 1.0;
            }

            if (prayerTimeCalculatorModel.CalculationMethodPreset == CalculationMethodPreset.Custom)
            {
                prayerTimeCalculatorModel.CalculationMethodPreset = CalculationMethodPreset.IthnaAshari;
            }

            var when = Instant.FromDateTimeUtc(prayerTimeCalculatorModel.CalculationDate.ToUniversalTime());
            var settings = new PrayerCalculationSettings();
            settings.CalculationMethod.SetCalculationMethodPreset(when, prayerTimeCalculatorModel.CalculationMethodPreset);
            var geo = new Geocoordinate(prayerTimeCalculatorModel.Latitude, prayerTimeCalculatorModel.Longitude, prayerTimeCalculatorModel.Altitude);

            var prayer = Prayers.On(when, settings, geo, prayerTimeCalculatorModel.TimeZone);
            var solarDate = prayertimescore.PrayerTimes.Library.Calender.Calendar.ConvertToPersian(prayerTimeCalculatorModel.CalculationDate);
            var lunarDate = prayertimescore.PrayerTimes.Library.Calender.Calendar.ConvertToIslamic(prayerTimeCalculatorModel.CalculationDate.AddDays(prayerTimeCalculatorModel.LunarHijriOffset));
            var gregorianDate = prayerTimeCalculatorModel.CalculationDate;

            var solarDateString = $" {solarDate.ToString("english_day")} {solarDate.ToString("english_month")} {solarDate.ToString("english_year")}";

            var sb = new StringBuilder();
            sb.AppendLine(
                "<div class=\"alert alert-success\" role=\"alert\">");
            sb.AppendLine("<table class=\"table table-dark table-hover table-bordered mb-12\"><tbody>");

            sb.AppendLine("<table class=\"table table-dark table-hover table-bordered mb-12\"><tbody>");
            sb.AppendLine("<tr><th>Method and Parameters</th></tr><tr>");
            sb.AppendLine("<td><table class\"table table-light table-bordered\"><tr>");
            sb.AppendLine($"<td scope=\"row\" class=\"col-1\">&nbsp;</td><td scope=\"row\" class=\"col-4\"><b>Calculation Method</b></td><td scope=\"row\" class=\"col-1\">&nbsp;</td><td class=\"col-6\" >{GetDescription(prayerTimeCalculatorModel.CalculationMethodPreset)}</td>");
            sb.AppendLine("</tr><tr>");
            sb.AppendLine($"<td scope=\"row\" class=\"col-1\">&nbsp;</td><td scope=\"row\" class=\"col-4\"><b>Calculation Date</b></td><td scope=\"row\" class=\"col-1\">&nbsp;</td><td class=\"col-6\" >{prayerTimeCalculatorModel.CalculationDate:D}</td>");
            sb.AppendLine("</tr><tr>");
            sb.AppendLine($"<td scope=\"row\" class=\"col-1\">&nbsp;</td><td scope=\"row\" class=\"col-4\"><b>Lunar Hijri Offset</b></td><td scope=\"row\" class=\"col-1\">&nbsp;</td><td class=\"col-6\" >{GenerateLunarHijriOffsetString(prayerTimeCalculatorModel.LunarHijriOffset)}</td>");
            sb.AppendLine("</tr><tr>");
            sb.AppendLine($"<td scope=\"row\" class=\"col-1\">&nbsp;</td><td scope=\"row\" class=\"col-4\"><b>Timezone</b></td><td scope=\"row\" class=\"col-1\">&nbsp;</td><td class=\"col-6\" >{GetTimeZoneFromOffset(prayerTimeCalculatorModel.TimeZone, prayerTimeCalculatorModel.DayLightSavingsTime)} (DST: {GetDstStatus(prayerTimeCalculatorModel.DayLightSavingsTime)})</td>");
            sb.AppendLine("</tr><tr>");
            sb.AppendLine($"<td scope=\"row\" class=\"col-1\">&nbsp;</td><td scope=\"row\" class=\"col-4\"><b>Geo Coordinates</b></td><td scope=\"row\" class=\"col-1\">&nbsp;</td><td class=\"col-6\" >{GetCoordinates(prayerTimeCalculatorModel.Latitude, prayerTimeCalculatorModel.Longitude, prayerTimeCalculatorModel.Altitude)}</td>");
            sb.AppendLine("</tr><tr>");
            sb.AppendLine($"<td scope=\"row\" class=\"col-1\">&nbsp;</td><td scope=\"row\" class=\"col-4\"><b>Geo Raw</b></td><td scope=\"row\" class=\"col-1\">&nbsp;</td><td class=\"col-6\" >{GetGeoRaw(prayerTimeCalculatorModel.Latitude, prayerTimeCalculatorModel.Longitude, prayerTimeCalculatorModel.Altitude)}</td>");
            sb.AppendLine("</tr></table></td><tr>");

            sb.AppendLine("<table class=\"table table-dark table-hover table-bordered mb-12\"><tbody>");
            sb.AppendLine("<tr><th>Dates</th></tr><tr>");
            sb.AppendLine("<td><table class\"table table-light table-bordered\"><tr>");
            sb.AppendLine($"<td scope=\"row\" class=\"col-1\">&nbsp;</td><td scope=\"row\" class=\"col-4\"><b>Gregorian</b></td><td scope=\"row\" class=\"col-1\">&nbsp;</td><td class=\"col-6\" >{gregorianDate:D}</td>");
            sb.AppendLine("</tr><tr>");
            sb.AppendLine($"<td scope=\"row\" class=\"col-1\">&nbsp;</td><td scope=\"row\" class=\"col-4\"><b>Solar Hijri</b></td><td scope=\"row\" class=\"col-1\">&nbsp;</td><td class=\"col-6\" >{solarDateString}</td>");
            sb.AppendLine("</tr><tr>");
            sb.AppendLine($"<td scope=\"row\" class=\"col-1\">&nbsp;</td><td scope=\"row\" class=\"col-4\"><b>Lunar Hijri</b></td><td scope=\"row\" class=\"col-1\">&nbsp;</td><td class=\"col-6\" >{lunarDate.ToString("english_formatted")}</td>");
            sb.AppendLine("</tr></table></td><tr>");

            sb.AppendLine("<tr><th>Prayer Times</th></tr><tr>");
            sb.AppendLine("<td><table class\"table table-light table-bordered\"><tr>");
            sb.AppendLine($"<td scope=\"row\" class=\"col-1\">&nbsp;</td><td scope=\"row\" class=\"col-4\"><b>Imsaak</b></td><td scope=\"row\" class=\"col-1\">&nbsp;</td><td class=\"col-6\" >{GetPrayerTimeString(prayer.Imsak, prayerTimeCalculatorModel.TimeZone)}</td>");
            sb.AppendLine("</tr><tr>");
            sb.AppendLine($"<td scope=\"row\" class=\"col-1\">&nbsp;</td><td scope=\"row\" class=\"col-4\"><b>Fajr</b></td><td scope=\"row\" class=\"col-1\">&nbsp;</td><td class=\"col-6\" >{GetPrayerTimeString(prayer.Fajr, prayerTimeCalculatorModel.TimeZone)}</td>");
            sb.AppendLine("</tr><tr>");
            sb.AppendLine($"<td scope=\"row\" class=\"col-1\">&nbsp;</td><td scope=\"row\" class=\"col-4\"><b>Sunrise</b></td><td scope=\"row\" class=\"col-1\">&nbsp;</td><td class=\"col-6\" >{GetPrayerTimeString(prayer.Sunrise, prayerTimeCalculatorModel.TimeZone)}</td>");
            sb.AppendLine("</tr><tr>");
            sb.AppendLine($"<td scope=\"row\" class=\"col-1\">&nbsp;</td><td scope=\"row\" class=\"col-4\"><b>Dhuhr</b></td><td scope=\"row\" class=\"col-1\">&nbsp;</td><td class=\"col-6\" >{GetPrayerTimeString(prayer.Dhuhr, prayerTimeCalculatorModel.TimeZone)}</td>");
            sb.AppendLine("</tr><tr>");
            sb.AppendLine($"<td scope=\"row\" class=\"col-1\">&nbsp;</td><td scope=\"row\" class=\"col-4\"><b>Asr</b></td><td scope=\"row\" class=\"col-1\">&nbsp;</td><td class=\"col-6\" >{GetPrayerTimeString(prayer.Asr, prayerTimeCalculatorModel.TimeZone)}</td>");
            sb.AppendLine("</tr><tr>");
            sb.AppendLine($"<td scope=\"row\" class=\"col-1\">&nbsp;</td><td scope=\"row\" class=\"col-4\"><b>Sunset</b></td><td scope=\"row\" class=\"col-1\">&nbsp;</td><td class=\"col-6\" >{GetPrayerTimeString(prayer.Sunset, prayerTimeCalculatorModel.TimeZone)}</td>");
            sb.AppendLine("</tr><tr>");
            sb.AppendLine($"<td scope=\"row\" class=\"col-1\">&nbsp;</td><td scope=\"row\" class=\"col-4\"><b>Maghrib</b></td><td scope=\"row\" class=\"col-1\">&nbsp;</td><td class=\"col-6\" >{GetPrayerTimeString(prayer.Maghrib, prayerTimeCalculatorModel.TimeZone)}</td>");
            sb.AppendLine("</tr><tr>");
            sb.AppendLine($"<td scope=\"row\" class=\"col-1\">&nbsp;</td><td scope=\"row\" class=\"col-4\"><b>Isha</b></td><td scope=\"row\" class=\"col-1\">&nbsp;</td><td class=\"col-6\" >{GetPrayerTimeString(prayer.Isha, prayerTimeCalculatorModel.TimeZone)}</td>");
            sb.AppendLine("</tr><tr>");
            sb.AppendLine($"<td scope=\"row\" class=\"col-1\">&nbsp;</td><td scope=\"row\" class=\"col-4\"><b>Midnight</b></td><td scope=\"row\" class=\"col-1\">&nbsp;</td><td class=\"col-6\" >{GetPrayerTimeString(prayer.Midnight, prayerTimeCalculatorModel.TimeZone)}</td>");
            sb.AppendLine("</tr></table></td><tr>");
            sb.AppendLine("</tbody></tr>");
            sb.AppendLine("</table>");
            sb.AppendLine("</div>");
            sb.AppendLine("<hr/>");

            PrayerTimes = sb.ToString();
        }

        private string GetGeoRaw(double latitude, double longitude, double altitude) => 
            altitude > 0 ? 
                $"Latitude: {latitude:F5}, Longitude: {longitude:F5}, Altitude: {altitude:F5} meters" :
                $"Latitude: {latitude:F5}, Longitude: {longitude:F5}";

        private string GenerateLunarHijriOffsetString(int lunarHijriOffset)
        {
            var offsetString = $"{lunarHijriOffset}";
            if (offsetString.Contains("-"))
            {
                var offsetUnsigned = int.Parse(offsetString.Replace("-", ""));
                return offsetUnsigned > 1 ? $"{lunarHijriOffset} days" : $"{lunarHijriOffset} day";
            }
            else if (lunarHijriOffset == 0)
            {
                return $"{lunarHijriOffset} days";
            }
            else
            {
                return lunarHijriOffset > 1 ? $"{lunarHijriOffset} days" : $"{lunarHijriOffset} day";
            }
        }

        private string GetCoordinates(double latitude, double longitude, double altitude)
        {
            var latitudeCoords = new Coordinate(latitude, CoordinatesPosition.N).ToString();
            var longitudeCoords = new Coordinate(longitude, CoordinatesPosition.E).ToString();

            return altitude > 0 ?
                $"{latitudeCoords}, {longitudeCoords}, {altitude:F4}m" :
                $"{latitudeCoords}, {longitudeCoords}";
            
        }

        private string GetDstStatus(bool dayLightSavingsTime)
        {
            return dayLightSavingsTime ? "ON" : "OFF";
        }

        private string GetTimeZoneFromOffset(double timeZone, bool isDaylightSavings)
        {
            if (isDaylightSavings)
            {
                timeZone -= 1;
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            var result = TimeZoneList.FirstOrDefault(x => x.Offset == timeZone);
            return result != null ? result.TimeZoneName : "Unknown TimeZone";
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

            foreach (var (_, value) in tzList)
            {
                try
                {
                    var thisInternalItem = new InternalTimeZoneInfo
                    {
                        TimeZoneName = value
                    };

                    var thisItem = new SelectListItem
                    {
                        Disabled = false,
                        Group = null,
                        Selected = false,
                        Text = value,
                    };

                    var thisOffsetInitial = value
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

                        thisInternalItem.Offset = returnOffSetAsDouble;
                    }
                    else
                    {
                        thisItem.Value = "0.00";
                        thisInternalItem.Offset = 0;
                    }

                    returnList.Add(thisItem);
                    TimeZoneList.Add(thisInternalItem);
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return returnList;
        }

        internal static string GetDescription(Enum value)
        {
            var attributes = value.GetType().GetField(value.ToString())?.GetCustomAttributes(typeof(DisplayAttribute), false);
            return (attributes is {Length: 0} ? value.ToString() : ((DisplayAttribute)attributes?[0]!)?.Name) ?? string.Empty;
        }

        public static IEnumerable<SelectListItem> GetCalculationModePresets()
        {
            var values = Enum.GetValues(typeof(CalculationMethodPreset)).Cast<CalculationMethodPreset>();
            var returnList = (from value in values
                              where value != CalculationMethodPreset.Custom
                              select new SelectListItem
                              {
                                  Disabled = false,
                                  Group = null,
                                  Selected = false,
                                  Text = GetDescription(value),
                                  Value = Enum.GetName(value)
                              }).ToList();
            return returnList;
        }
    }
}