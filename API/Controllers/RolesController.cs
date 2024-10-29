using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using API.Models;
using API.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;



namespace API.Controllers
{
    [Authorize(Roles="Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

       [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> CreateRole([FromBody] CreateRoleDto createRoleDto)
        {
            if(string.IsNullOrEmpty(createRoleDto.RoleName)){
                return BadRequest("Role name is required");
            }
            var roleExist=await _roleManager.RoleExistsAsync(createRoleDto.RoleName);
        if(roleExist)
        {
            return BadRequest("Role already exist");
        }
        var roleResult = await _roleManager.CreateAsync(new IdentityRole(createRoleDto.RoleName));

        if(roleResult.Succeeded)
        {
            return Ok(new {message="Role created successfully."});
        }
        
            return BadRequest("Role creation failed");

        }
        
         [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetRoles()
        {
            var roles =await _roleManager.Roles.Select(r=>new RoleResponseDto{
                Id=r.Id,
                Name=r.Name,
                TotalUsers= _userManager.GetUsersInRoleAsync(r.Name!).Result.Count
            }).ToListAsync();

            return Ok(roles);
        }
        [HttpDelete]

        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if(role is null)
            {
                return NotFound("Role not found.");
            }

            var result = await _roleManager.DeleteAsync(role);
            if(result.Succeeded)
            {
                return Ok(new {message="Role deleted Succesfully"});

            }
            return BadRequest("Role deletin failed");
        }
  [AllowAnonymous]      
[HttpPost("assign")]
public async Task<IActionResult> AssignRole([FromBody]RoleAssignDto roleAssignDto)
    {
    var user = await _userManager.FindByIdAsync(roleAssignDto.UserId);
    if (user is null)
    {
        return NotFound("User not found");
    }

    var role = await _roleManager.FindByIdAsync(roleAssignDto.RoleId);
    if (role is null)
    {
        return NotFound("Role not found");
    }

    var result = await _userManager.AddToRoleAsync(user, role.Name!);
    if (result.Succeeded)
    {
        return Ok(new { message = "Role assigned Successfully" });
    }

    var error = result.Errors.FirstOrDefault(); 
    return BadRequest(error!.Description);
    }

    }
}
