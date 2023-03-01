using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Security.Cryptography;
using WebApplication2.Data;
using WebApplication2.Models;
using System.Text;
using System.Collections;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TesterController : ControllerBase
    {
        public static User U1 = new User();
        private readonly IConfiguration _configuration;
        private readonly LoginContext _context;
        public TesterController(IConfiguration configuration,LoginContext loginContext)
        {
            _configuration  = configuration;
            _context = loginContext;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(userDto request)
        {
            CreatePassHash(request.Password!, out byte[] passwordHash, out byte[] passwordSalt);

            U1.Username = request.Username;
            U1.PasswordHash = passwordHash;
            U1.PasswordSalt = passwordSalt;
            U1.Email = request.Email;
            //U1.PasswordHash = U1.PasswordHash;
            //var passwordhash = System.Convert.ToBase64String(U1.PasswordHash);

            //U1.PasswordSalt = passwordsalt;

            dynamic userDetails = new UserDetails { UserName = U1.Username, PasswordHash = U1.PasswordHash, Email = U1.Email, PasswordSalt = U1.PasswordSalt };
            //, Email = U1.Email
            _context.UserDetails.Add(userDetails);
            await _context.SaveChangesAsync();

            return Ok(U1);
        }
        private void CreatePassHash(string password, out byte[] passwordHash, out byte[] passwordsalts)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordsalts = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                //string passwordhash = Encoding.ASCII.GetString(U1.PasswordHash!);
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(LoginUserDto request)
        {
            dynamic finalToken = "";
            using (var context = new LoginContext(new DbContextOptions<LoginContext>()))
            {
                var user = await context.UserDetails.FirstOrDefaultAsync(u=>u.UserName==request.Username);
                if (user == null)
                {
                    return NotFound();  
                }

                if (!VerifyPassHash(request.Password!, user.PasswordHash!, user.PasswordSalt!))
                {
                    return BadRequest("Password chuklay");
                }
                string token = CreateToken(user);
                user.Token = token;
                context.SaveChanges();
                finalToken = token;
            }

            return Ok(finalToken);
        }

        private string CreateToken(UserDetails user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!)
            };


            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));


            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;

        }

        

    private bool VerifyPassHash(string password, byte[] passwordHash, byte[] passwordsalt)
        {
            using (var hmac = new HMACSHA512(passwordsalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

      




        }

}
