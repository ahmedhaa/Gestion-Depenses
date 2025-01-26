using Gestion_Dépenses.Models.DepenseModel;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Gestion_Dépenses.Dto
{
    public class DepenseDto
    {
        [Range(0.01, double.MaxValue, ErrorMessage = "Le montant doit être un nombre positif non nul.")]
        public decimal Montant { get; set; }


        [MaxLength(100)]
        public string Commentaire { get; set; }
        [Required]
        [EnumDataType(typeof(TypeDepense))]
        [Range(0, 1, ErrorMessage = "La nature de la dépense doit être 0 (Déplacement) ou 1 (Restaurant).")]
        public TypeDepense Nature { get; set; }

       
        public int? Distance { get; set; }

     
        public int? NombreInvites { get; set; }
    }
}
