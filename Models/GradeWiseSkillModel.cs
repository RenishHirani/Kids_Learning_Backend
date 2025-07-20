using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace AdmnPanel.Models
{
    public class GradeWiseSkillModel
    {
        public int GradeWiseSkillID { get; set; }

        public int? ChapterID { get; set; }

        public string? ChpaterName { get; set; }

        public int? GradeID { get; set; }

        public string? GradeName { get; set; }

        public int? SkillID { get; set; }

        public string? SkillName { get; set; }

        public int Sequence { get; set; }


    }

    public class SkillDropDownModel
    {
        public int SkillID { get; set; }

        public string SkillName { get; set; }
    }
}
