using Konscious.Security.Cryptography;
using MakeItWork.ServiceOptions;
using Microsoft.Extensions.Options;

using System.Security.Cryptography;
using System.Text;

namespace MakeItWork.Services.SecurityServices.PasswordHasher
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly byte _saltSize;
        private readonly byte _hashSize;
        private readonly byte _parallelism;
        private readonly int _memorySizeKb;
        private readonly byte _iterations;
        private readonly int _version;

        public PasswordHasherService(IOptions<PasswordHasherOptions> options)
            : this(options?.Value ?? throw new ArgumentNullException(nameof(options))) { }

        public PasswordHasherService(PasswordHasherOptions options)
        {
            _saltSize = options.SaltSize;
            _hashSize = options.HashSize;
            _parallelism = options.Parallelism;
            _memorySizeKb = options.MemorySizeKb;
            _iterations = options.Iterations;
            _version = options.Version;
        }

        public async Task<string> HashValidatedPasswordAsync(char[] password)
        {
            byte[] salt = new byte[_saltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var hash = await ComputeArgon2id(Encoding.UTF8.GetBytes(password), salt, _memorySizeKb, _iterations, _parallelism, _hashSize);
            Array.Clear(password);

            string Base64Salt = Convert.ToBase64String(salt);
            string Base64Hash = Convert.ToBase64String(hash);

            return $"argon2id$v={_version}$m={_memorySizeKb},t={_iterations},p={_parallelism}${Base64Salt}${Base64Hash}";
        }

        public async Task<bool> VerifyPasswordAsync(char[] password, string encodedHash)
        {
            if (password == null) return false;
            if (password.Length == 0 || string.IsNullOrWhiteSpace(encodedHash))
            {
                Array.Clear(password);
                return false;
            }
            if (!TryParseEncodedHash(encodedHash, out Argon2Data argonData))
            {
                Array.Clear(password);
                return false;
            }

            var hash = await ComputeArgon2id(Encoding.UTF8.GetBytes(password), argonData.Salt, argonData.MemoryKb, argonData.Iterations, argonData.Parallelism, argonData.Hash.Length);
            Array.Clear(password);

            return CryptographicOperations.FixedTimeEquals(hash, argonData.Hash);
        }

        private static async Task<byte[]> ComputeArgon2id(byte[] passwordBytes, byte[] salt, int memoryKb, int iterations, int parallelism, int outputLength)
        {
            var argon2 = new Argon2id(passwordBytes)
            {
                Salt = salt,
                DegreeOfParallelism = parallelism,
                MemorySize = memoryKb,
                Iterations = iterations
            };

            Array.Clear(passwordBytes);
            return await argon2.GetBytesAsync(outputLength);
        }

        private static bool TryParseEncodedHash(string encodedHash, out Argon2Data argonData)
        {
            argonData = Argon2Data.GetEmtpyData();
            //Expected: argon2id$v=<VERSION>$m=<SIZE_KB>,t=<ITERATIONS>,p=<PARALLELISM>$<BASE64SALT>$<BASE64HASH>
            var parts = encodedHash.Split("$");

            if (parts.Length != 5) return false;

            var parameters = parts[2];
            var salt64 = parts[3];
            var hash64 = parts[4];

            if (!TryDecodingBase64(salt64, out byte[] salt)) return false;
            if (!TryDecodingBase64(hash64, out byte[] hash)) return false;
            if (!TryParseParams(parameters, out int memoryKb, out int iterations, out int parallelism)) return false;

            argonData = new Argon2Data()
            {
                Salt = salt,
                Hash = hash,
                MemoryKb = memoryKb,
                Iterations = iterations,
                Parallelism = parallelism
            };
            return true;
        }

        private static bool TryParseParams(string parameters, out int memoryKb, out int iterations, out int parallelism)
        {
            var splitParameters = parameters.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim());

            memoryKb = iterations = parallelism = 0;
            foreach (var parameter in splitParameters)
            {
                var kv = parameter.Split("=", 2);
                switch (kv[0])
                {
                    case "m":
                        if (!int.TryParse(kv[1], out memoryKb)) return false; break;
                    case "t":
                        if (!int.TryParse(kv[1], out iterations)) return false; break;
                    case "p":
                        if (!int.TryParse(kv[1], out parallelism)) return false; break;
                    default:
                        return false;
                }
            }

            return memoryKb > 0 && iterations > 0 && parallelism > 0;
        }

        private static bool TryDecodingBase64(string s, out byte[] bytes)
        {
            bytes = Array.Empty<byte>();
            try
            {
                bytes = Convert.FromBase64String(s);
                return true;
            }
            catch { return false; }
        }

        private record Argon2Data
        {
            public required byte[] Salt { get; init; }
            public required byte[] Hash { get; init; }
            public required int MemoryKb { get; init; }
            public required int Iterations { get; init; }
            public required int Parallelism { get; init; }

            public static Argon2Data GetEmtpyData()
            {
                return new()
                {
                    Salt = [],
                    Hash = [],
                    MemoryKb = 0,
                    Iterations = 0,
                    Parallelism = 0,
                };
            }
        }
    }
}
