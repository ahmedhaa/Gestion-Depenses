using Gestion_Dépenses.Data;
using Gestion_Dépenses.Dto;
using Gestion_Dépenses.Models.DepenseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Gestion_Dépenses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepensesController : ControllerBase
    {
        private readonly DepenseDbContext _context;
        public DepensesController(DepenseDbContext context)
        {
            _context = context;
        }

        //  Lister les dépenses avec pagination
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Non autorisé.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Accès interdit : seuls les administrateurs peuvent accéder à cette ressource.")]

        [SwaggerOperation(Summary = "Liste des dépenses", Description = "Récupère une liste de dépenses avec pagination.")]
        public async Task<IActionResult> GetDepenses(int page = 1, int pageSize = 10)
        {
            // Total des dépenses pour la pagination
            var totalCount = await _context.Depenses.CountAsync();
            var skip = (page - 1) * pageSize;

            // Récupérer les dépenses avec pagination
            var depenses = await _context.Depenses
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            // Convertir les dépenses en DTO 
            var depensesDto = depenses.Select(depense =>
            {
                var depenseDto = new Dictionary<string, object>();

                // Ajouter toutes les propriétés 
                foreach (var prop in depense.GetType().GetProperties())
                {
                    // On vérifie si la propriété n'est pas nulle avant d'ajouter
                    var value = prop.GetValue(depense);
                    if (value != null && prop.Name != "Nature" && prop.Name != "Distance" && prop.Name != "NombreInvites" && prop.Name != "Date") 
                    {
                        depenseDto.Add(prop.Name, value);
                    }
                }

                // Ajouter la nature avec son nom 
                depenseDto.Add("Nature", Enum.GetName(typeof(TypeDepense), depense.Nature));

                // Ajouter la propriété  'NombreInvites' si la dépense est de type 'Restaurant'
                if (depense is Restaurant r && r.NombreInvites.HasValue)
                {
                    depenseDto.Add("NombreInvites", r.NombreInvites);
                }

                // Ajouter la propriété  'Distance' si la dépense est de type 'Deplacement'
                if (depense is Deplacement d && d.Distance.HasValue)
                {
                    depenseDto.Add("Distance", d.Distance);
                }
                depenseDto.Add("Date", depense.Date);

                return depenseDto;
            }).ToList();

            var result = new
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Data = depensesDto
            };

            return Ok(result);
        }


        //  Récupérer une dépense par ID
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Récupérer une dépense", Description = "Récupère une dépense spécifique par son ID.")]
        public async Task<ActionResult<DepenseDto>> GetDepense(int id)
        {
            var depense = await _context.Depenses.FindAsync(id);
            if (depense == null)
            {
                return NotFound();
            }

            // Utilisation de Enum.GetName pour obtenir le nom de l'énumération
            var depenseDto = new Dictionary<string, object>();
      foreach (var prop in depense.GetType().GetProperties())
            {
                // On vérifie si la propriété n'est pas nulle avant d'ajouter
                var value = prop.GetValue(depense);
                if (value != null && prop.Name != "Nature" && prop.Name != "Distance" && prop.Name != "NombreInvites" && prop.Name != "Date")  // Ne pas ajouter "Nature" et "Distance" ici
                {
                    depenseDto.Add(prop.Name, value);
                }
            }
            depenseDto.Add("Nature", Enum.GetName(typeof(TypeDepense), depense.Nature));

            // Ajouter 'Distance' uniquement si elle a une valeur
            if (depense is Deplacement d && d.Distance.HasValue)
            {
                depenseDto.Add("Distance", d.Distance);
            }

            // Ajouter 'NombreInvites' uniquement si elle a une valeur
            if (depense is Restaurant r && r.NombreInvites.HasValue)
            {
                depenseDto.Add("NombreInvites", r.NombreInvites);
            }
            depenseDto.Add("Date", depense.Date);
            return Ok(depenseDto);
        }


        // Ajouter une dépense
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [SwaggerOperation(Summary = "Ajouter une dépense", Description = "Ajoute une nouvelle dépense (La nature de la dépense: 0 pour Déplacement et 1 pour Restaurant),La distance, obligatoire et doit etre positif si la nature est Déplacement (0).Le nombre d'invités doit etre un nombre entier positif ou 0 si la nature est Restaurant (1)")]
        public async Task<IActionResult> AddDepense([FromBody] DepenseDto depenseDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Depense depense;
  
                // Vérifier le type de dépense 
                if (depenseDto.Nature == TypeDepense.Deplacement)
            {
                if (!depenseDto.Distance.HasValue || depenseDto.Distance.Value <= 0)
                    return BadRequest("La distance est obligatoire pour un déplacement et doit etre >0");

                depense = new Deplacement
                {
                    Montant = depenseDto.Montant,
                    Date = DateTime.Now,
                    Commentaire = depenseDto.Commentaire,
                    Nature = depenseDto.Nature,
                    Distance = depenseDto.Distance.Value
                };
            }
            else if (depenseDto.Nature == TypeDepense.Restaurant)
            {
                if (!depenseDto.NombreInvites.HasValue || depenseDto.NombreInvites < 0)
                    return BadRequest("Le nombre d'invités doit être un entier positif ou égal à 0.");

                depense = new Restaurant
                {
                    Montant = depenseDto.Montant,
                    Date = DateTime.Now,
                    Commentaire = depenseDto.Commentaire,
                    Nature = depenseDto.Nature,
                    NombreInvites = depenseDto.NombreInvites.Value
                };
            }
            else
            {
                return BadRequest("Nature de dépense non valide. il doit etre 0:deplacement ou 1:restaurant");
            }

            // Ajouter la dépense à la base de données
            _context.Depenses.Add(depense);
            await _context.SaveChangesAsync();

            // Retourner la réponse avec l'ID de la dépense ajoutée
            return CreatedAtAction(nameof(GetDepense), new { id = depense.Id }, depense);
        }


        // Supprimer une dépense
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Supprimer une dépense", Description = "Supprime une dépense de la base de données par son ID.")]
        public async Task<IActionResult> DeleteDepense(int id)
#pragma warning restore CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement
        {
            var depense = await _context.Depenses.FindAsync(id);
            if (depense == null)
            {
                return NotFound(new { Message = "La dépense n'a pas été trouvée." });
            }

            _context.Depenses.Remove(depense);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "La dépense a été supprimée avec succès." });
        }
    }
}
