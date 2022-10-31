namespace front_to_back.Areas.Admin.ViewModels.FeaturedWorkComponent
{
    public class FeaturedWorkComponentDetailsViewModel
    {
         public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<IFormFile>? Photos { get; set; }
        public ICollection<Models.FeaturedWorkComponentPhoto> FeaturedWorkComponentPhotos { get; set; }
    }
}
