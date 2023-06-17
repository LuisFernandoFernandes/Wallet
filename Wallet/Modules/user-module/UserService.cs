using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using NuGet.Protocol.Plugins;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http.ModelBinding;
using System.Xml.Linq;
using Wallet.Tools.database;
using Wallet.Tools.generic_module;
using Wallet.Tools.session_control;
using Wallet.Tools.validation_dictionary;

namespace Wallet.Modules.user_module
{
    public class UserService : GenericService<User>, IUserService
    {
        #region Variáveis
        private IValidationDictionary _validatonDictionary;
        private Context _context;
        private readonly IConfiguration _configuration;
        ModelStateDictionary modelState = new ModelStateDictionary();
        #endregion

        #region Construtor
        public UserService(Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        #endregion


        public async Task<User> Creat(UserDTO userDto)
        {

            CreatePasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User()
            {
                Name = userDto.Name,
                UserName = userDto.UserName,
                Email = userDto.Email,
                CPF = userDto.CPF,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            await InsertOrUpdate(user);
            return user;
        }

        public async Task<List<User>> Read(string? id = null, string? ticker = null)
        {
            var list = new List<User>();


            if (id is null && ticker is null)
            {
                list = await _context.User.AsQueryable().ToListAsync();
            }
            else
            {
                //     list = await _context.User.AsQueryable().Where(a => a.Id == id || a.Ticker == ticker).ToListAsync();
            }

            if (list.Count > 0)
            {
                return list;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }


        public async Task<User> Update(string id, User user)
        {

            //precisa validar se a senha foi alterada, caso positivo a senha precisa ser encriptada antes de ir para o InsertOrUpdate.
            //deveria receber o dto do create tambem, assim chamaria o método para verificar a senha, se a senha não conferir é pq ela está sendo alterada, ou fazer um método só pra alterar
            // a senha, fica melhor
            var oldUser = await _context.User.AsQueryable().Where(a => a.Id == id).FirstOrDefaultAsync();



            user.Id = oldUser.Id;
            //  user.Ticker ??= oldUser.Ticker;
            //user.Description ??= oldUser.Description;
            //user.Class ??= oldUser.Class;
            await InsertOrUpdate(user);
            return user;
        }

        public async Task<User> Delete(string id)
        {
            User user = await _context.User.AsQueryable().Where(a => a.Id == id).FirstOrDefaultAsync();
            await Remove(user);
            return user;
        }

        private async Task InsertOrUpdate(User user)
        {
            if (!modelState.IsValid)
            {
                throw new ArgumentException();
            }

            if (await _context.User.AsQueryable().AnyAsync(a => a.Id == user.Id))
            {
                await UpdateAsync(user, _context);
            }
            else if (await _context.User.AsQueryable().AnyAsync(a => a.UserName == user.UserName))
            {
                throw new ArgumentException("Nome de usuário já cadastrado.");

            }
            else if (!CheckCpf(user.CPF))
            {
                throw new ArgumentException("CPF inválido.");
            }
            else if (await _context.User.AsQueryable().AnyAsync(a => a.CPF == user.CPF))
            {
                throw new ArgumentException("CPF já cadastrado.");

            }
            else if (await _context.User.AsQueryable().AnyAsync(a => a.Email == user.Email))
            {
                throw new ArgumentException("e-mail já cadastrado.");

            }
            else
            {
                await InsertAsync(user, _context);
            }
        }



        private async Task Remove(User user)
        {
            if (!modelState.IsValid)
            {
                throw new ArgumentException();
            }
            _context.Remove(user);
            await Save();
        }


        private async Task Save()
        {
            await _context.SaveChangesAsync();
        }




        public async Task<string> Login(UserDTO request)
        {
            var user = await _context.User.AsQueryable().Where(a => a.UserName == request.UserName).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("Usuário não encontrado.");
            }
            if (!VerifyPassWordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                throw new ArgumentException("Senha incorreta.");
            }

            string token = CreateToken(user);
            // await _sessionControlService.RegisterLogin(user);
            return token;
        }



        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPassWordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }



        private bool CheckCpf(string cpf)
        {
            const int cpfLength = 11;

            var digits = GetDigits(cpf);
            ValidateCpfLength(digits.Length, cpfLength);

            var numbers = GetDigitsArray(digits);
            var (verifier1, verifier2) = CalculateVerificationDigits(numbers);

            return VerifyValidity(digits, verifier1, verifier2);
        }

        private string GetDigits(string digits)
        {
            return Regex.Replace(digits, @"[^\w\d]+", "");
        }

        private void ValidateCpfLength(int currentLength, int expectedLength)
        {
            if (currentLength != expectedLength) throw new ArgumentException("Número inválido de dígitos.");
        }

        private int[] GetDigitsArray(string digits)
        {
            return digits.Select(d => int.Parse(d.ToString())).ToArray();
        }

        private bool VerifyValidity(string digits, int verificationDigit1, int verificationDigit2)
        {
            var asciiNumberDifference = 48;
            return verificationDigit1 == Convert.ToInt32(digits[digits.Length - 2]) - asciiNumberDifference &&
                   verificationDigit2 == Convert.ToInt32(digits[digits.Length - 1]) - asciiNumberDifference;
        }

        public string CreateRandomCpf()
        {
            var cpfDigits = 11;
            int[] digits = GetRandomDigits(cpfDigits);
            return GetCompleteCpf(digits);
        }

        private int[] GetRandomDigits(int cpfDigits)
        {
            var randomDigits = cpfDigits - 2;
            var random = new Random();
            int[] digits = new int[randomDigits];
            for (int i = 0; i < randomDigits; i++)
            {
                digits[i] = random.Next(0, 10);
            }
            return digits;
        }

        private string GetCompleteCpf(int[] digits)
        {
            var (verificationDigit1, verificationDigit2) = CalculateVerificationDigits(digits);
            string document = string.Join("", digits) + verificationDigit1.ToString() + verificationDigit2.ToString();
            return document;
        }

        private (int, int) CalculateVerificationDigits(int[] digits)
        {
            int digitLength = 9;
            var digit1Multiplier = 1;
            var digit2Multiplier = 0;

            int sumDigit1 = digits.Take(digitLength).Select((n, i) => (n * (digit1Multiplier + i > 9 ? digit1Multiplier + i - 8 : digit1Multiplier + i))).Sum();
            int digit1 = GetVerificationDigit(sumDigit1);

            int sumDigit2 = digits.Take(digitLength++).Select((n, i) => (n * (digit2Multiplier + i > 9 ? digit2Multiplier + i - 8 : digit2Multiplier + i))).Sum() + (digit1 * 9);
            int digit2 = GetVerificationDigit(sumDigit2);

            return (digit1, digit2);
        }

        private int GetVerificationDigit(int sum)
        {
            var dvModulo11 = 11;
            var digit = sum % dvModulo11;
            return digit == 10 ? 0 : digit;
        }

    }
}
