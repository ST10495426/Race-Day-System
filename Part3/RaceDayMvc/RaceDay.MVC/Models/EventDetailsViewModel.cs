namespace RaceDay.MVC.Models
{
    public class EventDetailsViewModel
    {
        public int EventID { get;set;}
        public string Name {get;set; }
        public string Description {get; set; }
        public DateTime Date { get;set; }
        public string Location { get; set;}
        public string Distance { get;set;}
        public string EventTypeName {get; set; }
        public List<CategoryViewModel> Categories { get;set; } = new();
    }

    public class CategoryViewModel
    {
        public int CategoryID {get;set; }
        public string Name { get;set;}
        public string CategoryType { get; set;}
        public int? MinValue { get;set; }
        public int? MaxValue {get; set; }
    }
}