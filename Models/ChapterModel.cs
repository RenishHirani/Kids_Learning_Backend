namespace AdmnPanel.Models
{
    public class ChapterModel
    {
        public int ChapterID { get; set; }
        public string ChapterName { get; set; }
        public int? SubjectID { get; set; }
        public string? SubjectName { get; set; }
        public int Sequence { get; set; }
    }
}
