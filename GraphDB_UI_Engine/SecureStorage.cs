using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
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
        var credentialsList = JsonConvert.DeserializeObject<List<Credentials>>(json);

        foreach (var credentials in credentialsList)
        {
            if (credentials.Username == username)
            {
                credentials.Password = Decrypt(credentials.Password);
                return credentials.Password;
            }
        }

        return null; 
    }

    private static string Encrypt(string data)
    {
        var provider = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
        var key = provider.CreateSymmetricKey(Encoding.UTF8.GetBytes("sJ1sr3/BgVMz0I4+Hy2jvQ==")); // Key? how to deal with

        var bytes = WinRTCrypto.CryptographicEngine.Encrypt(key, Encoding.UTF8.GetBytes(data));

        return Convert.ToBase64String(bytes);
    }

    private static string Decrypt(string data)
    {
        var provider = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
        var key = provider.CreateSymmetricKey(Encoding.UTF8.GetBytes("sJ1sr3/BgVMz0I4+Hy2jvQ==")); // Key? how to deal with

        var bytes = WinRTCrypto.CryptographicEngine.Decrypt(key, Convert.FromBase64String(data));

        return Encoding.UTF8.GetString(bytes);
    }
}

public class Credentials
{
    public string Username { get; set; }
    public string Password { get; set; }
}
