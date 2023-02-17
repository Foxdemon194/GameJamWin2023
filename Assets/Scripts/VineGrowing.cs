using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VineGrowing : MonoBehaviour
{
    [SerializeField] GameObject vinePrefab;
    [SerializeField] GameObject nodePrefab;
    [SerializeField] float growTime = 0.25f;
    
    GameObject vineGrowerPrefab;
    Vector2Int activeVinePosition;
    Material activeVineMaterial;
    static HashSet<Vector2Int> vinePositions = new HashSet<Vector2Int>();
    static HashSet<Vector2Int> nodePositions = new HashSet<Vector2Int>();
    public static bool ContainsNode(Vector2Int node) => nodePositions.Contains(node);
    public static void ResetVariables() { vinePositions.Clear(); nodePositions.Clear(); vineGrid = new GameObject[GridManager.GridSize.x, GridManager.GridSize.y]; isDead = false; }
    static GameObject[,] vineGrid;
    static bool isDead = false;
    public static bool NoNodes => nodePositions.Count == 0;
    public static bool IsDead => isDead;
    public static int VineCount => vinePositions.Count;
    float elapsedTime = float.MaxValue;
    bool vineStopped = false;
    bool newVineSpawned = false;

    public static void KillPlant() => isDead = true;
    void Awake()
    {
        vineGrowerPrefab = Resources.Load<GameObject>("Prefabs/VineGrower");
        activeVinePosition = GridManager.GetGridPosition(transform.position);
    }
    
    void Update()
    {
        if(!vineStopped && !isDead)
            GrowVine();
        else if (!newVineSpawned && !isDead)
            SpawnNewVine();
    }

    private void GrowVine()
    {
        if (elapsedTime > growTime)
        {
            if (activeVineMaterial != null)
            {
                activeVineMaterial.SetFloat("_Fill", 1);
            }

            while (FoodManager.FoodIsAdjacent(activeVinePosition, out Vector2Int foodPos))
            {
                vineStopped = true;
                FoodManager.OccupyFood(foodPos);
                nodePositions.Add(foodPos);
                Instantiate(nodePrefab, GridManager.GetWorldPosition(foodPos) + new Vector3(0, 0, -0.1f), Quaternion.identity).transform.parent = transform;
            }

            if (vineStopped) return;
            
            Vector2Int selectedFood = FoodManager.GetClosestFoodFromNeighbor(activeVinePosition, out int directionIndex);
            
            if (directionIndex == -1) return;
            
            Vector2Int newVinePosition = activeVinePosition + GridManager.directions[directionIndex];
            vinePositions.Add(newVinePosition);
            
            vinePositions.Add(activeVinePosition = newVinePosition);
            Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(GridManager.directions[directionIndex].y, GridManager.directions[directionIndex].x) * Mathf.Rad2Deg -90);
            (vineGrid[newVinePosition.x, newVinePosition.y] = Instantiate(vinePrefab, GridManager.GetWorldPosition(newVinePosition), rotation)).transform.parent = transform;
            activeVineMaterial = vineGrid[activeVinePosition.x, activeVinePosition.y].GetComponent<Renderer>().material;
            

            elapsedTime = 0;
        }
        else
        {
            elapsedTime += Time.deltaTime;
            if (activeVineMaterial != null)
            {
                activeVineMaterial.SetFloat("_Fill", elapsedTime / growTime);
            }
        }
    }
    
    void SpawnNewVine()
    {
        float distance = float.MaxValue;
        Vector2Int selectedSegment = Vector2Int.zero;
        bool found = false;
        foreach (Vector2Int position in vinePositions)
        {
            Vector2Int foodPos = FoodManager.GetClosestFoodFromNeighbor(position, out int directionIndex);
            if (directionIndex == -1) continue;
            found = true;
            float newDistance = Vector2Int.Distance(position + GridManager.directions[directionIndex], foodPos);
            if (distance > newDistance)
            {
                distance = newDistance;
                selectedSegment = position;
            }
        }
        if (!found) return;
        Instantiate(vineGrowerPrefab, GridManager.GetWorldPosition(selectedSegment), Quaternion.identity).transform.parent = transform.parent;
        newVineSpawned = true;
    }
}
