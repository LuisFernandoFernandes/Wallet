using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web.Http.ModelBinding;
using Wallet.Tools.database;
using Wallet.Tools.generic_module;
using Wallet.Tools.scheduler;
using Wallet.Tools.validation_dictionary;

namespace Wallet.Modules.user_module
{
    public class UserService : GenericService<User>, IUserService
    {
        #region Variáveis
        private IValidationDictionary _validatonDictionary;
        private Context _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        ModelStateDictionary modelState = new ModelStateDictionary();
        #endregion

        #region Construtor
        public UserService(Context context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion


        public async Task<User> Create(UserDTO userDto)
        {

            CreatePasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User()
            {
                Name = userDto.Name,
                UserName = userDto.UserName,
                Email = userDto.Email,
                CPF = userDto.CPF,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = userDto.Role
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

            if (!CheckCpf(user.CPF))
            {
                throw new ArgumentException("CPF inválido.");
            }

            if (!IsValidEmail(user.Email))
            {
                throw new ArgumentException("E-mail inválido.");
            }

            var existingUser = await _context.User.FirstOrDefaultAsync(a => a.Id == user.Id);

            if (existingUser != null)
            {
                await UpdateAsync(user, _context);
            }
            else
            {
                var existingData = await _context.User.FirstOrDefaultAsync(a => a.UserName == user.UserName || a.CPF == user.CPF || a.Email == user.Email);

                if (existingData != null)
                {
                    if (existingData.UserName == user.UserName)
                    {
                        throw new ArgumentException("Nome de usuário já cadastrado.");
                    }

                    if (existingData.CPF == user.CPF)
                    {
                        throw new ArgumentException("CPF já cadastrado.");
                    }

                    if (existingData.Email == user.Email)
                    {
                        throw new ArgumentException("E-mail já cadastrado.");
                    }
                }

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
            if (request.Password == null) { throw new ArgumentException("Insira uma senha."); }
            if (request.UserName == null && request.Email == null && request.CPF == null) { throw new ArgumentException("Insira um dado para login."); }

            var user = await _context.User.AsQueryable().Where(a => a.UserName == request.UserName || a.Email == request.Email || a.CPF == request.CPF).FirstOrDefaultAsync();

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

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            bool isMatch = Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
            if (!isMatch)
                return false;

            string domain = email.Split('@')[1];

            string confirmationCode = GenerateConfirmationCode(email);

            bool emailSent = SendConfirmationEmail(email, confirmationCode);

            return emailSent;
        }

        private string GenerateConfirmationCode(string email)
        {
            string confirmationCode = $"{email}#{DateTime.Now.Ticks}-{Guid.NewGuid().ToString()}";
            return confirmationCode;
        }


        private bool SendConfirmationEmail(string email, string confirmationCode)
        {
            try
            {
                using (var client = new SmtpClient())
                {

                    client.Timeout = 10000;
                    client.Host = "sandbox.smtp.mailtrap.io";
                    client.Port = 587;
                    client.EnableSsl = true;


                    client.Credentials = new NetworkCredential("6d834633092e98", "8316473f971f84");

                    var confirmationEmail = new MailMessage
                    {
                        From = new MailAddress("your_email@example.com")
                    };
                    confirmationEmail.To.Add(email);
                    confirmationEmail.Subject = "Confirmação de E-mail";
                    confirmationEmail.Body = $"Por favor, clique no link abaixo para confirmar o seu e-mail:\n\n" +
                                             $"https://localhost:7184/user/confirm?code={confirmationCode}";

                    client.Send(confirmationEmail);

                    return true;
                }
            }
            catch (SmtpException)
            {
                return false;
            }
        }


        public async Task ConfirmEmail(string code)
        {
            var email = ExtractEmailFromConfirmationCode(code);

            if (email == null) throw new ArgumentException("Código inválido. e-mail inexistente.");

            var user = await _context.User.AsQueryable().Where(a => a.Email == email).FirstOrDefaultAsync();

            if (user == null) throw new ArgumentException("Código inválido. Usuário inexistente.");

            user.IsEmailConfirmed = true;

            await UpdateAsync(user, _context);
        }

        private string ExtractEmailFromConfirmationCode(string confirmationCode)
        {
            if (confirmationCode.Contains("#"))
            {
                string[] parts = confirmationCode.Split('#');

                string email = parts[0];

                return email;
            }

            return string.Empty;
        }

        public string GetLoggedInUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId;
        }
    }
}
