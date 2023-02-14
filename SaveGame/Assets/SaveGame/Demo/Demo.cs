using System;
using TMPro;
using UnityEngine;

public class Demo : MonoBehaviour
{
    
    [Header("Encrypted")] 
    [SerializeField] private bool encode;
    
    [Header("Imputs")]
    [SerializeField] private TMP_InputField inputName;
    [SerializeField] private TMP_InputField inputHealth;
    [SerializeField] private TMP_InputField inputLevel;

    [Header("Outputs")] 
    [SerializeField] private TextMeshProUGUI tName;
    [SerializeField] private TextMeshProUGUI tHealth;
    [SerializeField] private TextMeshProUGUI tLevel;

    public void Save()
    {
        PlayerStats playerStats = new PlayerStats();

        playerStats.name = "Link";
        playerStats.health = 10;
        playerStats.level = 1;

        SaveGame.SaveData("playerstats", playerStats, encode);
    }

    public void Load()
    {
        PlayerStats playerStats = SaveGame.LoadData<PlayerStats>("playerstats", encode);

        if (playerStats is null) return;
        
        tName.text = playerStats.name;
        tHealth.text = playerStats.health.ToString();
        tLevel.text = playerStats.level.ToString();
    }
}   
