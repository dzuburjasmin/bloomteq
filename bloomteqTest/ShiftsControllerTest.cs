using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using bloomteq.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Net.Http.Json;

namespace InformationProtocolSubSystem.Api.Tests
{
    public class ShiftsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;


        public ShiftsControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }
        private string GenerateToken(User user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("name", user.Name),
            new Claim("userId", user.Id),
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("bloomteqbloomteqbloomteqbloomteqsecretkey123456789"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "bloomteq",
                audience: "bloomteq",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private void AddAuthentication()
        {
            var user = new User { UserName = "mock", Name = "Mock123!", Id = "mockId" };
            var token = GenerateToken(user);
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        [Fact]
        public async Task Get_Shifts_ReturnsUnauthorized_WhenNoToken()
        {
            var response = await _client.GetAsync("odata/Shifts");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Patch_Shift_ReturnsNotFound_WhenShiftDoesNotExist()
        {
            AddAuthentication();
            var invalidShiftId = Guid.NewGuid();
            var patchData = new { StartTime = DateTime.UtcNow.AddHours(2) };
            var content = new StringContent(JsonConvert.SerializeObject(patchData), Encoding.UTF8, "application/json");

            var response = await _client.PatchAsync($"odata/Shifts({invalidShiftId})", content);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
}
    }
