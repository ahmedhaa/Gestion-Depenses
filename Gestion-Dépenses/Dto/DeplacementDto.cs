using System.ComponentModel.DataAnnotations;

namespace Gestion_Dépenses.Dto
{
    public class DeplacementDto : DepenseDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "La distance doit être un entier positif.")]
        public int? Distance { get; set; }
    }
}
