using System.Security.Cryptography;

namespace TsdDelivery.Application.Utils;

public static class StringUtils
{
    //public static string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    //public static bool Verify(string password, string passwordHash) => BCrypt.Net.BCrypt.Verify(password, passwordHash);
    public static string RandomString()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new char[32];
        var randomNumber = new Random();
    
        for (int i = 0; i < random.Length; i++) 
        {
            random[i] = chars[randomNumber.Next(chars.Length)];
        }
        return new String(random);
    }
}