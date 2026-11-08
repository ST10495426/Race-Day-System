namespace RaceDay.MVC.Models
{
    public class EventListViewModel
    {
        public int EventID {get; set;}
        public string Name {get; set; }
        public string Description { get; set;}
        public DateTime Date {get;set; }
        public string Location {get; set;}
        public string Distance {get; set;}
        public string? BannerUrl {get;set; }
        public string EventTypeName { get;set;}
    }
}