using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public static class SaveGame
{
    private const string KEY = "5qCnpHTp9+6VcGksBl1yo5E8hxqiYxtsnm6CrBmsNtU="; // You can generate KEY and IV in Menu Save System>Generate KEY and IV
    private const string IV = "6Nv+0YV+PLoVksp589O89g==";
    public static bool SaveData<T>(string relativePath, T data, bool encode = false)
    {
        string path = encode ? Path.Combine(Application.persistentDataPath, relativePath + "_enc") :
            Path.Combine(Application.persistentDataPath, relativePath);

        try
        {
            if (File.Exists(path))
            {
                Debug.Log("Data exists. Deleting old file and writing a new one!");
                File.Delete(path);
            }
            else
            {
                Debug.Log("Writing file for the first time!");
            }
            
            using FileStream stream = File.Create(path);

            if (encode)
            {
                WriteEncryptedData(data, stream);
            }
            else
            {
                stream.Close();
                File.WriteAllText(path, JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }));
            }
            
            return true;
        }
        catch (Exception e)
        {
            Debug.Log($"Unable to save data due to: {e.Message} {e.StackTrace}");
            return false;
        }
    }

    public static T LoadData<T>(string relativePath, bool encode = false)
    {
        string path = encode ? Path.Combine(Application.persistentDataPath, relativePath + "_enc") :
            Path.Combine(Application.persistentDataPath, relativePath);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"Cannot load file at {path}. <color=red>File does not exist!</color>");
            //throw new FileLoadException($"{path} does not exist!");
            return default;
        }

        try
        {
            T data;
            data = encode ? 
                ReadEncryptedData<T>(path) : 
                JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
            throw e;
        }
    }

    public static void DeleteData(string relativePath, bool encrypted = false)
    {
        string path = encrypted ? Path.Combine(Application.persistentDataPath, relativePath, "_enc") :
            Path.Combine(Application.persistentDataPath, relativePath);
        
        try
        {
            if (File.Exists(path))
            {
                Debug.Log($"Deleting {path} file!");
                File.Delete(path);
            }
            else
            {
                Debug.Log($"{path} does not exist");
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Unable to delete data due to: {e.Message} {e.StackTrace}");
        }
    }
    private static void WriteEncryptedData<T>(T data, FileStream stream)
    {
        using Aes aesProvider = Aes.Create();
        aesProvider.Key = Convert.FromBase64String(KEY); // You can generate KEY and IV in Menu Save System>Generate KEY and IV
        aesProvider.IV = Convert.FromBase64String(IV);
     
        using ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor();
        using CryptoStream cryptoStream = new CryptoStream(stream, cryptoTransform, CryptoStreamMode.Write);
        
        cryptoStream.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        })));
    }

    private static T ReadEncryptedData<T>(string path)
    {
        byte[] filBytes = File.ReadAllBytes(path);
        using Aes  aesProvider = Aes.Create();
        aesProvider.Key = Convert.FromBase64String(KEY);
        aesProvider.IV = Convert.FromBase64String(IV);

        using ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor(aesProvider.Key, aesProvider.IV);
        using MemoryStream decryptoStream = new MemoryStream(filBytes);
        using CryptoStream cryptoStream = new CryptoStream(decryptoStream, cryptoTransform, CryptoStreamMode.Read);

        using StreamReader reader = new StreamReader(cryptoStream);

        string result = reader.ReadToEnd();

        return JsonConvert.DeserializeObject<T>(result);
    }
}