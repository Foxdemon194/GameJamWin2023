using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    [SerializeField] GameObject foodPrefab;
    [SerializeField] LayerMask obstacles;
    static List<Vector2Int> foodPositions = new List<Vector2Int>();
    static GameObject[,] foodGrid;
    static bool[,] foodOccupiedGrid;

    public static void OccupyFood(Vector2Int foodPosition) => foodOccupiedGrid[foodPosition.x, foodPosition.y] = true;
    static LayerMask Obstacles => instance.obstacles;

    static FoodManager instance;

    void Awake() 
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        foodGrid = new GameObject[GridManager.GridSize.x, GridManager.GridSize.y];
        foodOccupiedGrid = new bool[GridManager.GridSize.x, GridManager.GridSize.y];
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2Int gridPosition = GridManager.GetGridPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (foodGrid[gridPosition.x, gridPosition.y] == null)
            {
                foodGrid[gridPosition.x, gridPosition.y] = Instantiate(foodPrefab, GridManager.GetWorldPosition(gridPosition), Quaternion.identity);
                foodPositions.Add(gridPosition);
            }
        }
    }

    public static Vector2Int GetClosestFoodFromNeighbor(Vector2Int gridPosition, out int directionIndex)
    {
        Vector2Int closestFood = gridPosition;
        float distance = float.MaxValue;
        directionIndex = -1;
        for (int i = 0; i < foodPositions.Count; i++)
        {
            if (foodOccupiedGrid[foodPositions[i].x, foodPositions[i].y]) continue;
            for (int j = 0; j < GridManager.directions.Length; j++)
            {
                if (Physics2D.Linecast(GridManager.GetWorldPosition(gridPosition + GridManager.directions[j]), GridManager.GetWorldPosition(foodPositions[i]), Obstacles))
                    continue;
                float newDistance = Vector2Int.Distance(gridPosition + GridManager.directions[j], foodPositions[i]);
                if (distance > newDistance)
                {
                    distance = newDistance;
                    closestFood = foodPositions[i];
                    directionIndex = j;
                }
            }
        }
        return closestFood;
    }
}
