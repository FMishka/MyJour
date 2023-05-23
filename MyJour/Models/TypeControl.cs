using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyJour.Models
{
    public class TypeControl
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public ICollection<AcademicPerformance> AcademicPerformance { get; set;}
        static public List<SelectListItem> GetAllTypesControl(ApplicationDbContext db)
        {
            var list = db.TypeControl.Select(s => new { s.Id, s.Type });
            List<SelectListItem> classes = new List<SelectListItem>();
            foreach (var item in list)
            {
                classes.Add(new SelectListItem { Text = item.Type.ToString(), Value = item.Id.ToString() });
            }
            return classes;
        }
    }
}
