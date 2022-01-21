using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Library {
    public class Encryption {
        public static string Encrypt(string text, byte[] key) {
            using (AesManaged aes = new() { Key = key })
            using (MemoryStream memoryStreamm = new()) {
                memoryStreamm.Write(aes.IV);
                using (CryptoStream cryptoStream = new(memoryStreamm, aes.CreateEncryptor(), CryptoStreamMode.Write, true)) {
                    cryptoStream.Write(Encoding.UTF8.GetBytes(text));
                }
                return Convert.ToBase64String(memoryStreamm.ToArray());
            }
        }

        public static string Decrypt(string base64, byte[] key) {
            using (MemoryStream memoryStreamm = new(Convert.FromBase64String(base64))) {
                byte[] iv = new byte[16];
                memoryStreamm.Read(iv);
                using (AesManaged aes = new() { Key = key, IV = iv })
                using (CryptoStream cryptoStream = new(memoryStreamm, aes.CreateDecryptor(), CryptoStreamMode.Read, true))
                using (MemoryStream memoryStreammOutput = new()) {
                    cryptoStream.CopyTo(memoryStreammOutput);
                    return Encoding.UTF8.GetString(memoryStreammOutput.ToArray());
                }
            }
        }
    }
}
