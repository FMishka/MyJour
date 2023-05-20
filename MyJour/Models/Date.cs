using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace MyJour.Models
{
    public class Date
    {
        public static List<SelectListItem> GetAllMonth()
        {
            List<SelectListItem> months = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                months.Add(new SelectListItem { Text = ToMonthName(i), Value = i.ToString()});
            }
            return months;
        }
        public static List<SelectListItem> GetLast2Years()
        {
            List<SelectListItem> years = new List<SelectListItem>();
            for (int i = DateTime.Now.Year; i > DateTime.Now.Year - 2; i--)
            {
                years.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }
            return years;
        }
        public static string ToMonthName(int month)
        {
            string _month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            char[] charMonth = _month.ToCharArray();
            charMonth[0] = char.ToUpper(charMonth[0]);
            return new string(charMonth);
        }
    }
}
