using System;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SaveGameEditor : Editor
{
    [MenuItem("Save Game/Open save folder")]
    public static void OpenSaveFolder()
    {
        Process.Start(Application.persistentDataPath);
    }

    [MenuItem("Save Game/Generate KEY and IV")]
    public static void GenerateKeyAndIv()
    {
        Aes aes = Aes.Create();
        aes.GenerateIV();
        aes.GenerateKey();
            
        Debug.Log($"IV:<color=blue> {Convert.ToBase64String(aes.IV)}</color>");
        Debug.Log($"KEY: <color=blue>{Convert.ToBase64String(aes.Key)}</color>");
    }
}
