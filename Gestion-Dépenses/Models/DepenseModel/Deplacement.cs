using System.ComponentModel.DataAnnotations;

namespace Gestion_Dépenses.Models.DepenseModel
{
    public class Deplacement : Depense
    {
        [Range(1, int.MaxValue, ErrorMessage = "La distance doit être un entier positif.")]
        public int? Distance { get; set; }
        public  DateTime Date { get; set; } 

    }
}
