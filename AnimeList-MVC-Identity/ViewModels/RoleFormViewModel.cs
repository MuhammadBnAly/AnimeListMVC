using System.ComponentModel.DataAnnotations;

namespace AnimeList_MVC_Identity.ViewModels
{
    public class RoleFormViewModel
    {
        [Required, StringLength(50)]
        public string Name { get; set; }
    }
}
