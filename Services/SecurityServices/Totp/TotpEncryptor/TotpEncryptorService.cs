using System.Security.Cryptography;

namespace MakeItWork.Services.SecurityServices.Totp.TotpEncryptor
{
    public sealed class TotpEncryptorService : IDisposable
    {
        private readonly byte[] _masterKey;

        public TotpEncryptorService(string base64MasterKey)
        {
            if (string.IsNullOrWhiteSpace(base64MasterKey))
                throw new ArgumentException("Master key required", nameof(base64MasterKey));

            _masterKey = Convert.FromBase64String(base64MasterKey);
            if (_masterKey.Length != 32)
                throw new ArgumentException("Master key have to be 32 bytes", nameof(base64MasterKey));
        }

        // return nonce || ciphertext || tag
        public byte[] Encrypt(byte[] plaintext, byte[]? associatedData = null)
        {
            var iv = RandomNumberGenerator.GetBytes(12); // 96-bit nonce
            var ciphertext = new byte[plaintext.Length];
            var tag = new byte[16];

            using var aes = new AesGcm(_masterKey, tag.Length);
            aes.Encrypt(iv, plaintext, ciphertext, tag, associatedData);

            var outBuf = new byte[iv.Length + ciphertext.Length + tag.Length];
            Buffer.BlockCopy(iv, 0, outBuf, 0, iv.Length);
            Buffer.BlockCopy(ciphertext, 0, outBuf, iv.Length, ciphertext.Length);
            Buffer.BlockCopy(tag, 0, outBuf, iv.Length + ciphertext.Length, tag.Length);

            return outBuf;
        }

        // expects nonce || ciphertext || tag
        public byte[] Decrypt(byte[] stored, byte[]? associatedData = null)
        {
            if (stored.Length < 12 + 16) throw new ArgumentException("Invalid payload", nameof(stored));

            var iv = new byte[12];
            Buffer.BlockCopy(stored, 0, iv, 0, 12);

            var ciphertextLen = stored.Length - 12 - 16;
            var ciphertext = new byte[ciphertextLen];

            Buffer.BlockCopy(stored, 12, ciphertext, 0, ciphertextLen);

            var tag = new byte[16];
            Buffer.BlockCopy(stored, stored.Length - 16, tag, 0, 16);

            var plaintext = new byte[ciphertextLen];

            using var aes = new AesGcm(_masterKey, tag.Length);
            aes.Decrypt(iv, ciphertext, tag, plaintext, associatedData);

            return plaintext;
        }

        public void Dispose()
        {
            if (_masterKey != null)
                CryptographicOperations.ZeroMemory(_masterKey);
        }
    }
}
