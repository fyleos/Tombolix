using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeUp.Server.Data;
using TradeUp.Server.Models;
using TradeUp.Server.Services;
using TradeUp.Shared.Models;

namespace TradeUp.Server.Controllers
{
    public class UsersController : BaseApiV1Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;


        public UsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, LicenceService licenceService, UserContextService userContextService): base(licenceService, userContextService)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            ApplicationUser? user = null;

            var idClaim = User.Claims.First(t => t.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            user = await _userManager.FindByIdAsync(idClaim.Value);

            if (user == null)
            {
                return Unauthorized();
            }

            var returnedUser = new UserDto() 
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.UserName,
                DisplayName = user.DisplayName,
                EmailConfirmed = user.EmailConfirmed,
                Roles = await _userManager.GetRolesAsync(user)
            };

            return Ok(returnedUser);
        }

        [HttpGet("issuperadmin/{id}")]
        public async Task<ActionResult<bool>> IsSuperAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(await _userManager.IsInRoleAsync(user, nameof(Roles.SuperAdmin)));
        }

        [HttpGet("getoptions/{id}")]
        public async Task<ActionResult<SharedUserOption>> GetUserOption(string id)
        {
            var sharedUserOptions = await _context.SharedUserOptions.ToListAsync();

            if (sharedUserOptions == null)
            {
                return NotFound();
            }

            var sharedUserOption = sharedUserOptions.FirstOrDefault(option => option.UserId == id);

            if (sharedUserOption == null)
            {
                sharedUserOption = new SharedUserOption { UserId = id };
            }

            return Ok(sharedUserOption);
        }

        [HttpPut("setoptions/{id}")]
        public async Task<ActionResult> SetUserOption([FromRoute]string id,[FromBody] SharedUserOption sharedUserOption)
        {
            if (id != sharedUserOption.UserId)
            {
                return BadRequest();
            }

            var sharedUserOptions = await _context.SharedUserOptions.ToListAsync();
            if(sharedUserOptions is not null)
            {
                var dbUserOption = sharedUserOptions.FirstOrDefault(option => option.UserId == id);
                if (dbUserOption is not null)
                {
                    dbUserOption.CurrentTheme = sharedUserOption.CurrentTheme;
                    _context.Entry(dbUserOption).State = EntityState.Modified;
                }
                else
                {
                    _context.SharedUserOptions.Add(sharedUserOption);
                }
            }
            else
            {
                _context.SharedUserOptions.Add(sharedUserOption);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SharedUserOptionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            IList<string> roles = await _userManager.GetRolesAsync(user);

            return Ok(new UserDto { Id = user.Id, Email = user.Email, Name = user.UserName, EmailConfirmed = user.EmailConfirmed, Roles = roles });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = _userManager.Users.Select(user => new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.UserName,
                DisplayName = user.DisplayName,
                EmailConfirmed = user.EmailConfirmed,
                Roles = _userManager.GetRolesAsync(user).Result
            }).ToList();

            return Ok(users);
        }

        [HttpGet("roletypes")]
        public async Task<ActionResult<IEnumerable<string>>> GetUserTypes()
        {
            List<string> types = new List<string>();

            await Task.Run(() =>
            {
                types.Add(nameof(Roles.SuperAdmin));
                types.Add(nameof(Roles.Essentiel));
                types.Add(nameof(Roles.Pro));
                types.Add(nameof(Roles.Premium));

            });            

            return Ok(types);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(string id, UserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Email = userDto.Email;
            user.UserName = userDto.Email;
            user.DisplayName = userDto.DisplayName;
            user.EmailConfirmed = userDto.EmailConfirmed;
            await _userManager.UpdateAsync(user);

            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRolesAsync(user, userDto.Roles);
            return NoContent();
        }

        private bool SharedUserOptionExists(string id)
        {
            return _context.SharedUserOptions.Any(e => e.UserId == id);
        }
    }
}
