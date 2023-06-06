using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MyJour.Models
{
    public class TypeControl
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public ICollection<AcademicPerformance> AcademicPerformance { get; set;}
        static public List<SelectListItem> GetAllTypesControl(ApplicationDbContext db, int? selectedId = null)
        {
            var list = db.TypeControl.Select(s => new { s.Id, s.Type }).AsNoTracking();
            List<SelectListItem> classes = new List<SelectListItem>();
            foreach (var item in list)
            {
                if (item.Id.ToString() == selectedId.ToString())
                {
                    classes.Add(new SelectListItem { Text = item.Type.ToString(), Value = item.Id.ToString(), Selected = true });
                }
                else
                {
                    classes.Add(new SelectListItem { Text = item.Type.ToString(), Value = item.Id.ToString() });
                }
            }
            return classes;
        }
    }
}
