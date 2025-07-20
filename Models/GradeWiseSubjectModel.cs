namespace AdmnPanel.Models
{
    public class GradeWiseSubjectModel
    {
        public int GradewiseSubjectID { get; set; }
        public int? GradeID { get; set; }

        public string? GradeName { get; set; }
        public int? SubjectID { get; set; }

        public string? SubjectName { get; set; }
    }

    public class GradeDropDownModel
    {
        public int GradeID { get; set; }

        public string GradeName { get; set; }
    }

    public  class SubjectDropDownModel
    {
        public int SubjectID { get; set; }

        public string SubjectName { get; set; }
    }
}
