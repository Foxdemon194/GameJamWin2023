using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nutrient : MonoBehaviour
{
    [SerializeField] NutrientType nutrientType;
    [SerializeField] int nutrientValue;
    public NutrientType NutrientType => nutrientType;
    public int NutrientValue { get{ return nutrientValue; } set{ nutrientValue = value; } }
}

public enum NutrientType
{
    Food,
    Water,
    Light,
    Fertilizer
}