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
    public float maxMana;
    public float currentMana;
    public float maxStamina;
    public float currentStamina;
    public float maxHealth;
    public float currentHealth;
    public float walkSpeed;
    public float runSpeed;

    public float PlayerPosition_x;
    public float PlayerPosition_y;
    public float PlayerPosition_z;
    public Difficulty Difficulty;
    public string SceneName;
}
