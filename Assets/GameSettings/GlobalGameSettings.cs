using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalGameSettings", menuName = "Gameplay/Global Game Settings", order = 1)]
public class GlobalGameSettings : ScriptableObject
{
    [Header("Gameplay")]
    public float timePerMatch = 90f;

    [Header("Radar")]
    [Tooltip("The signal of device will be from weak to strong base on the distance of MC position and Treasure Chest position, far to near")]
    public float signal_lvl_1_distance = 20f;
    public float signal_lvl_2_distance = 7f;
    public float signal_lvl_3_distance = 1.5f;
    public float delayEachScan = 1.0f;

    [Header("TreasureChest")]
    public float digSpeed = 1.0f;

    [Header("Player")]
    public float runSpeed = 3.0f;
    public float walkSpeed = 1.0f;
}
