using System.ComponentModel.DataAnnotations;

namespace Gestion_Dépenses.Dto
{
    public class RestaurantDto : DepenseDto
    {
        [Range(0, int.MaxValue, ErrorMessage = "Le nombre d'invités doit être un entier positif ou 0.")]
        public int? NombreInvites { get; set; }
    }
}
