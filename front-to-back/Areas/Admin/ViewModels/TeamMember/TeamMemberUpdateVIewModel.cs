using System.ComponentModel.DataAnnotations.Schema;

namespace front_to_back.Areas.Admin.ViewModels.TeamMember
{
    public class TeamMemberUpdateVIewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? Position { get; set; }
        public string? PhotoName { get; set; }

        
        public IFormFile? Photo { get; set; }
    }
}
