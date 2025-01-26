using System.ComponentModel.DataAnnotations;

namespace Gestion_Dépenses.Models.DepenseModel
{
    public class Restaurant : Depense
    {
        [Range(0, int.MaxValue, ErrorMessage = "Le nombre d'invités doit être un entier positif ou 0.")]
        public int? NombreInvites { get; set; }
        public  DateTime Date { get; set; }

    }
}
