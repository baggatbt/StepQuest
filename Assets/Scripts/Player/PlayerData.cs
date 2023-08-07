using System;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;

    public int level;
    public int exp;
    public int gold;
    public int attackPower;
    public int defensePower;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Initialization method to setup your data
    public void Initialize(Player player)
    {
        level = player.level;
        exp = player.exp;
        gold = player.gold;
        attackPower = player.attackPower;
        defensePower = player.defensePower;
    }
}
