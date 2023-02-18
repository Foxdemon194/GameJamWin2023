using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FoodManager : MonoBehaviour
{
    [SerializeField] string nextScene;
    [SerializeField] Nutrient foodPrefab;
    [SerializeField] float diminishingFoodValue = 0.9f;
    [SerializeField] int baseNutrients = 10;
    [SerializeField] float attritionFactor = 1;
    [SerializeField] LayerMask obstacles;

    [SerializeField] Vector3Int nutCount;
    [SerializeField] Image canSprite;
    [SerializeField] float canCoolDown = 3f;
    [SerializeField] TMP_Text waterText;
    [SerializeField] TMP_Text lightText;
    [SerializeField] TMP_Text foodText;
    [SerializeField] bool showObstacles;
    
    float canTimer = 99f;
    static int BaseNutrients { get => instance.baseNutrients; set => instance.baseNutrients = value; }
    static float AttritionFactor => instance.attritionFactor;
    static float DiminishingFoodValue => instance.diminishingFoodValue;
    static List<Vector2Int> nutrientPositions = new List<Vector2Int>();
    static Nutrient[,] nutrientGrid;
    static bool[,] nutrientOccupiedGrid;
    static int foodOccupied = 0;
    static int waterOccupied = 0;
    static int lightOccupied = 0;
    static GameObject[] obstaclesList;
    public static void HandleDeath()
    {
        foodOccupied = 0; waterOccupied = 0; lightOccupied = 0; occupiedSpaces = 0;
        BaseNutrients--;
        if (BaseNutrients <= 0)
        {
            PlantManager.GameOver = true;
            Debug.Log("Game Over");
            return;
        }
        foreach (Vector2Int nutrientPosition in nutrientPositions)
        {
            if (nutrientOccupiedGrid[nutrientPosition.x, nutrientPosition.y])
            {
                if (nutrientGrid[nutrientPosition.x, nutrientPosition.y].NutrientValue > 0)
                    nutrientGrid[nutrientPosition.x, nutrientPosition.y].NutrientValue--;
               
            }
            nutrientOccupiedGrid[nutrientPosition.x, nutrientPosition.y] = false;
        }
    }
    public static int Nutrients => (int)(NutrientValue() * AttritionFactor) + instance.baseNutrients;
    static float FoodValue()
    {
        int minResource = Mathf.Min(foodOccupied, waterOccupied, lightOccupied);
        int foodOver = foodOccupied - minResource;

        return minResource + foodOver * Mathf.Pow(DiminishingFoodValue, foodOver);
    }
    static float WaterValue()
    {
        int minResource = Mathf.Min(foodOccupied, waterOccupied, lightOccupied);
        int waterOver = waterOccupied - minResource;

        return minResource + waterOver * Mathf.Pow(DiminishingFoodValue, waterOver);
    }
    static float LightValue()
    {
        int minResource = Mathf.Min(foodOccupied, waterOccupied, lightOccupied);
        int lightOver = lightOccupied - minResource;

        return minResource + lightOver * Mathf.Pow(DiminishingFoodValue, lightOver);
    }
    static float NutrientValue()
    {
        return FoodValue() + WaterValue() + LightValue();
    }
    static int occupiedSpaces = 0;
    static int initialSpaces = 0;

    public static void OccupyFood(Vector2Int foodPosition) 
    { 
        nutrientOccupiedGrid[foodPosition.x, foodPosition.y] = true;
        switch (nutrientGrid[foodPosition.x, foodPosition.y].NutrientType)
        {
            case NutrientType.Food:
                foodOccupied += nutrientGrid[foodPosition.x, foodPosition.y].NutrientValue;
                occupiedSpaces++;
                break;
            case NutrientType.Water:
                waterOccupied += nutrientGrid[foodPosition.x, foodPosition.y].NutrientValue;
                occupiedSpaces++;
                break;
            case NutrientType.Light:
                lightOccupied += nutrientGrid[foodPosition.x, foodPosition.y].NutrientValue;
                occupiedSpaces++;
                break;
            default:
                int amount = nutrientGrid[foodPosition.x, foodPosition.y].NutrientValue;
                foodOccupied += amount;
                waterOccupied += amount;
                lightOccupied += amount;
                break;
        }
        print(occupiedSpaces + " / " + initialSpaces);
    }
    static LayerMask Obstacles => instance.obstacles;

    static FoodManager instance;

    void Awake() 
    {
        obstaclesList = GameObject.FindGameObjectsWithTag("Rock"); 
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        foodOccupied = 0;
        waterOccupied = 0;
        lightOccupied = 0;
        occupiedSpaces = 0;
        nutrientGrid = new Nutrient[GridManager.GridSize.x, GridManager.GridSize.y];
        nutrientOccupiedGrid = new bool[GridManager.GridSize.x, GridManager.GridSize.y];
        nutrientPositions = new List<Vector2Int>();
        InitializeNutrients();
        initialSpaces = nutrientPositions.Count;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canTimer > canCoolDown)
        {
            Vector2Int gridPosition = GridManager.GetGridPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (nutrientGrid[gridPosition.x, gridPosition.y] == null)
            {
                nutrientGrid[gridPosition.x, gridPosition.y] = Instantiate<Nutrient>(foodPrefab, GridManager.GetWorldPosition(gridPosition), Quaternion.identity);
                nutrientPositions.Add(gridPosition);
            }
            canTimer = 0;
        }
        nutCount = new Vector3Int(foodOccupied, waterOccupied, lightOccupied);
        canTimer += Time.deltaTime;
        canSprite.color = Color.Lerp(Color.black, Color.white, canTimer / canCoolDown);
        waterText.text = waterOccupied.ToString();
        lightText.text = lightOccupied.ToString();
        foodText.text = foodOccupied.ToString();

        if (occupiedSpaces >= initialSpaces)
        {
            SceneManager.LoadScene(nextScene);
        }
    }

    public static Vector2Int GetClosestFoodFromNeighbor(Vector2Int gridPosition, out int directionIndex)
    {
        Vector2Int closestFood = gridPosition;
        float cost = float.MaxValue;
        directionIndex = -1;
        for (int i = 0; i < nutrientPositions.Count; i++)
        {
            if (nutrientOccupiedGrid[nutrientPositions[i].x, nutrientPositions[i].y]) continue;
            for (int j = 0; j < GridManager.directions.Length; j++)
            {
                if (Physics2D.Linecast(GridManager.GetWorldPosition(gridPosition + GridManager.directions[j]), GridManager.GetWorldPosition(nutrientPositions[i]), Obstacles))
                    continue;
                if (nutrientGrid[(gridPosition + GridManager.directions[j]).x, (gridPosition + GridManager.directions[j]).y] != null) continue;
                float nutrientValue;
                switch (nutrientGrid[nutrientPositions[i].x, nutrientPositions[i].y].NutrientType)
                {
                    case NutrientType.Food:
                        nutrientValue = foodOccupied;
                        break;
                    case NutrientType.Water:
                        nutrientValue = waterOccupied;
                        break;
                    case NutrientType.Light:
                        nutrientValue = lightOccupied;
                        break;
                    default:
                        nutrientValue = 0;
                        break;
                }
                float newCost = Vector2Int.Distance(gridPosition + GridManager.directions[j], nutrientPositions[i]) + nutrientValue;
                if (cost > newCost)
                {
                    cost = newCost;
                    closestFood = nutrientPositions[i];
                    directionIndex = j;
                }
            }
        }
        return closestFood;
    }

    public static bool FoodIsAdjacent(Vector2Int gridPos, out Vector2Int foodPos)
    {
        foodPos = Vector2Int.zero;
        for (int i = 0; i < GridManager.directions.Length; i++)
        {
            if (nutrientGrid[gridPos.x + GridManager.directions[i].x, gridPos.y + GridManager.directions[i].y] != null && 
                !nutrientOccupiedGrid[gridPos.x + GridManager.directions[i].x, gridPos.y + GridManager.directions[i].y])
            {
                foodPos = gridPos + GridManager.directions[i];
                return true;
            }
        }
        return false;
    }
    void InitializeNutrients()
    {
        Nutrient[] nutrients = FindObjectsOfType<Nutrient>();
        foreach (Nutrient nutrient in nutrients)
        {
            nutrientPositions.Add(GridManager.GetGridPosition(nutrient.transform.position));
            nutrientGrid[GridManager.GetGridPosition(nutrient.transform.position).x, GridManager.GetGridPosition(nutrient.transform.position).y] = nutrient;
        }
    }

    private void OnDrawGizmos()
    {
        if (showObstacles)
        {
            foreach (GameObject go in obstaclesList)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(go.transform.position, new Vector3(1, 1, 1));
            }
        }
    }
}
