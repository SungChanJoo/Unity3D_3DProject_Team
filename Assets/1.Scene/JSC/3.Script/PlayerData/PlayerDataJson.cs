using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty
{
    Easy,
    Normal,
    Hard
}

[System.Serializable]
public class PlayerDataJson
{
    public float maxMana = 100f;
    public float currentMana= 100f;
    public float maxStamina = 100f;
    public float currentStamina = 100f;
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    public float PlayerPosition_x = 20f;
    public float PlayerPosition_y = 0f;
    public float PlayerPosition_z = -164f;

    public Difficulty Difficulty;
    public string SceneName;

    public int Seed;
}
