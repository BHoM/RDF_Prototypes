using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Management;
using System.Text;
using Newtonsoft.Json;
using PCLCrypto;
using SymmetricAlgorithm = PCLCrypto.SymmetricAlgorithm;

public class SecureStorage
{
    public void SaveCredentials(string username, string password, string filePath)
    {
        var credentials = new Credentials
        {
            Username = username,
            Password = Encrypt(password)
        };

        var json = JsonConvert.SerializeObject(credentials);
        File.WriteAllText(filePath, json);
    }

    public string GetCredentials(string filePath, string username)
    {
        if (!File.Exists(filePath))
            return null;

        var json = File.ReadAllText(filePath);
        var credentials = JsonConvert.DeserializeObject<Credentials>(json);
        credentials.Password = Decrypt(credentials.Password);
        return credentials.Password;
    }

    private static string Encrypt(string data)
    {
        var provider = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
        var keyMaterial = GetCpuId(); // Use CPU ID to generate a KEY
        var key = provider.CreateSymmetricKey(GenerateKey(keyMaterial));

        var bytes = WinRTCrypto.CryptographicEngine.Encrypt(key, Encoding.UTF8.GetBytes(data));

        return Convert.ToBase64String(bytes);
    }

    private static string Decrypt(string data)
    {
        var provider = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
        var keyMaterial = GetCpuId(); // Use CPU ID to generate a KEY
        var key = provider.CreateSymmetricKey(GenerateKey(keyMaterial));

        var bytes = WinRTCrypto.CryptographicEngine.Decrypt(key, Convert.FromBase64String(data));

        return Encoding.UTF8.GetString(bytes);
    }


    private static string GetCpuId()
    {
        var searcher = new ManagementObjectSearcher("Select * from Win32_Processor");
        foreach (var obj in searcher.Get())
        {
            return obj["ProcessorId"].ToString().Trim();
        }
        return string.Empty;
    }

    static byte[] GenerateKey(string cpuId)
    {
        using (var sha256 = SHA256.Create())
        {
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(cpuId));
        }
    }
}

public class Credentials
{
    public string Username { get; set; }
    public string Password { get; set; }
}


