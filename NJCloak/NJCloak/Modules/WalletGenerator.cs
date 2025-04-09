using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NJCloak.Modules {
    public class WalletGenerator {

        private static Random rnd = new Random();

        public static string GenerateRandomAddress(string type) {
            switch (type) {
                case "ETH":
                    return "0x" + GenerateRandomHexString(40);
                case "TRC20":
                    return GenerateRandomBase58String(34);
                case "BTC":
                    return GenerateRandomBech32String(42);
                case "LTC":
                    return GenerateRandomBech32String(42);
                case "TON":
                    return GenerateRandomBase58String(48);
                default:
                    return "Invalid Type";
            }
        }

        private static string GenerateRandomHexString(int length) {
            byte[] randomBytes = new byte[length / 2];
            rnd.NextBytes(randomBytes);
            return BitConverter.ToString(randomBytes).Replace("-", "").ToLower();
        }

        private static string GenerateRandomBase58String(int length) {
            const string base58Chars = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
            return new string(Enumerable.Range(0, length).Select(_ => base58Chars[rnd.Next(base58Chars.Length)]).ToArray());
        }

        private static string GenerateRandomBech32String(int length) {
            const string bech32Chars = "qpzry9x8gf2tvdw0s3jn54khce6mua7l";
            return new string(Enumerable.Range(0, length).Select(_ => bech32Chars[rnd.Next(bech32Chars.Length)]).ToArray());
        }
    }
}
