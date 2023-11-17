namespace CoMon.Statuses.Dtos
{
    public class StatusHistoryDto
    {
        public StatusPreviewDto Status { get; set; }
        public StatusPreviewDto LatestStatus { get; set; }
        public StatusPreviewDto PreviousStatus { get; set; }
        public StatusPreviewDto NextStatus { get; set; }
    }
}
