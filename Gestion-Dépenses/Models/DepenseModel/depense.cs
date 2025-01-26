using DocumentFormat.OpenXml.Vml.Spreadsheet;
using NSwag.Annotations;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Gestion_Dépenses.Models.DepenseModel
{

    //classe depense
    public abstract class Depense
    {
        public int Id { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Le montant doit être un nombre positif non nul.")]
        public decimal Montant { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string Commentaire { get; set; }

        [EnumDataType(typeof(TypeDepense))]
        [Range(0, 1, ErrorMessage = "La nature de la dépense doit être 0 (Déplacement) ou 1 (Restaurant).")]
        public TypeDepense Nature { get; set; }
    }




}
