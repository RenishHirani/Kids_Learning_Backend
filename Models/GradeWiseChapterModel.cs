namespace AdmnPanel.Models
{
    public class GradeWiseChapterModel
    {
        public int GradewiseChapterID { get; set; }
        public int? GradeID { get; set; }
        public string? GradeName { get; set; }
        public int? ChapterID { get; set; }
        public string? ChapterName { get; set; }
        public int Sequence { get; set; }
    }
}
