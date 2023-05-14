using System.Net;

namespace HebrewHolidays_custom
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WebClient wc = new WebClient();
            var dateData = wc.DownloadString("https://www.hebcal.com/hebcal?v=1&cfg=json&maj=on&year=now&geo=geoname&geonameid=281184&start=2023-01-01&end=2050-01-01&c=off&m=0&b=1");
            var raw = System.Text.Json.JsonSerializer.Deserialize<Root>(dateData);

            var list = raw.items.Where(x => x.category == "holiday").ToList();
            list.RemoveAll(s => s.hebrew.Contains("חוה״מ"));
            list.RemoveAll(s => s.title.Contains("Purim"));
            list.RemoveAll(s => s.hebrew.Contains("תשעה באב"));
            list.RemoveAll(s => s.hebrew.Contains("הושענא רבה"));
            list.RemoveAll(s => s.hebrew.Contains("חנוכה"));

            var newlist = list.Where(s => s.hebrew.Contains("פסח ז׳")).ToList();
            foreach (var item in newlist)
            {
                list.Add(new Item()
                {
                    title = item.title,
                    date = item.date.AddDays(-1),
                    category = item.category,
                    title_orig = item.title_orig,
                    hebrew = "ערב " + item.hebrew
                });
            }
            //get only chars from item.hebrew
            list.ForEach(x => x.hebrew = new string(x.hebrew.Where(c => char.IsLetter(c) || char.IsSeparator(c)).ToArray()));

            var output = "";
            foreach (var item in list)
            {
                output += item.hebrew + "," + item.date.ToString("dd/MM/yyyy") + "," + (item.hebrew.Contains("ערב") ? "true" : "false") + Environment.NewLine;
            }

            File.WriteAllText("holidays.csv", output);
        }
    }

    public class Item
    {
        public string title { get; set; }
        public DateTime date { get; set; }
        public string category { get; set; }
        public string title_orig { get; set; }
        public string hebrew { get; set; }
    }

    public class Location
    {
        public string title { get; set; }
        public string city { get; set; }
        public string tzid { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string cc { get; set; }
        public string country { get; set; }
        public string admin1 { get; set; }
        public string asciiname { get; set; }
        public string geo { get; set; }
        public int geonameid { get; set; }
    }

    public class Range
    {
        public string start { get; set; }
        public string end { get; set; }
    }

    public class Root
    {
        public string title { get; set; }
        public DateTime date { get; set; }
        public Location location { get; set; }
        public Range range { get; set; }
        public List<Item> items { get; set; }
    }
}