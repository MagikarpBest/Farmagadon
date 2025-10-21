using System;
using System.Runtime.CompilerServices;
using UnityEngine;



[CreateAssetMenu(fileName = "CropsData", menuName = "Crops/CropsData")]


public class CropsData : ScriptableObject
{
    [Header("Crop Data")]
    [SerializeField] public GameObject cropPrefab;
    public AmmoData ammoData;
    public CROP_NAMES cropName;
    public float cropWeightage = 1.0f;
    public float baseGrowChance;
    public int dropAmount;
    public int growRate;

}
