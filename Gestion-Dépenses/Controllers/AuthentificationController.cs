using Gestion_Dépenses.Models.UserModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.Metrics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;


namespace Gestion_Dépenses.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthentificationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;


        public AuthentificationController(UserManager<User> userManager, SignInManager<User> signInManager,RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }
        //connexion d'un utilisateur
        [HttpPost("login")]
            public async Task<IActionResult> Login([FromBody] Models.UserModel.LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new { message = "Email ou mot de passe incorrect." });

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                // Génération du jeton JWT
                var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) 
        };

                // Ajout des rôles de l'utilisateur 
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                var token = new JwtSecurityToken(
                   issuer: "YourIssuer", 
                   audience: "YourAudience", 
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }

            return Unauthorized(new { message = "Email ou mot de passe incorrect." });
        }

        //enregistrer un nouvel utilisateur
        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
                [SwaggerOperation(Summary = "Ajout nouvel utilisateur", Description = "Ajouter un nouvel utilisateur")]

        public async Task<IActionResult> Register([FromBody] Models.UserModel.RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Vérifiez si le rôle choisi existe
            var roleExists = await _roleManager.RoleExistsAsync(model.Role);
            if (!roleExists)
            {
                return BadRequest($"The role '{model.Role}' does not exist.");
            }

            // Créez un nouvel utilisateur
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email
            };

           
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Ajoutez l'utilisateur au rôle spécifié
                var roleResult = await _userManager.AddToRoleAsync(user, model.Role);

                if (roleResult.Succeeded)
                {
                    return Ok(new { Message = "User registered successfully", UserId = user.Id });
                }

                // Supprimez l'utilisateur si l'ajout au rôle échoue
                await _userManager.DeleteAsync(user);
                return BadRequest(new { Message = "Failed to assign role to user", Errors = roleResult.Errors });
            }

            
            return BadRequest(new { Message = "Failed to register user", Errors = result.Errors });
        }

    }
}
