using System;
using System.Runtime.CompilerServices;
using UnityEngine;



[CreateAssetMenu(fileName = "CropsData", menuName = "Crops/CropsData")]


public class CropsData : ScriptableObject
{
    [Header("Crop Data")]
    [SerializeField] public GameObject cropPrefab;
    public CROP_NAMES cropNames;
    public float cropWeightage = 1.0f;
    public float baseGrowChance;
    public int dropRate;
    public int growRate;

}
