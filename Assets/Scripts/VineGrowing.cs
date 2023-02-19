using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VineGrowing : MonoBehaviour
{
    [SerializeField] GameObject vinePrefab;
    [SerializeField] GameObject vineLPrefab;
    [SerializeField] GameObject nodePrefab;
    [SerializeField] float growTime = 0.25f;

    public static bool isGrowing;


    GameObject vineGrowerPrefab;
    Vector2Int activeVinePosition;
    Material activeVineMaterial;
    static HashSet<Vector2Int> vinePositions = new HashSet<Vector2Int>();
    HashSet<Vector2Int> localVinePositions = new HashSet<Vector2Int>();
    static HashSet<Vector2Int> nodePositions = new HashSet<Vector2Int>();
    static HashSet<Vector2Int> poisonpositions = new HashSet<Vector2Int>();
    public static bool ContainsNode(Vector2Int node) => nodePositions.Contains(node);
    public static void ResetVariables() { vinePositions.Clear(); nodePositions.Clear(); vineGrid = new GameObject[GridManager.GridSize.x, GridManager.GridSize.y]; isDead = false; totalVines = 0; }
    static GameObject[,] vineGrid;
    static bool isDead = false;
    public static bool NoNodes => nodePositions.Count == 0;
    public static bool IsDead => isDead;
    public static int VineCount => totalVines;
    float elapsedTime = float.MaxValue;
    bool vineStopped = false;
    bool newVineSpawned = false;
    int nextDirection = -1;
    static int totalVines = 0;
    public static bool isGrowing = false;
    public static void KillPlant() => isDead = true;
    void Awake()
    {
        vineGrowerPrefab = Resources.Load<GameObject>("Prefabs/VineGrower");
        activeVinePosition = GridManager.GetGridPosition(transform.position);
        GameObject[] poisons = GameObject.FindGameObjectsWithTag("Poison");
        foreach (GameObject poison in poisons)
        {
            poisonpositions.Add(GridManager.GetGridPosition(poison.transform.position));
        }
    }
    
    void Update()
    {
        if (!vineStopped && !isDead)
        {
            GrowVine();
            isGrowing = true;
        }
        else if (!newVineSpawned && !isDead)
            SpawnNewVine();
    }

    private void LateUpdate()
    {
        isGrowing = false;
    }

    private void GrowVine()
    {
        if (nextDirection == -1)
        {
            FoodManager.GetClosestFoodFromNeighbor(activeVinePosition, out nextDirection);
        }
        if (elapsedTime > growTime && nextDirection != -1)
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
            int currentDirection = nextDirection;

            Vector2Int newVinePosition = activeVinePosition + GridManager.directions[currentDirection];
            FoodManager.GetClosestFoodFromNeighbor(newVinePosition, out nextDirection);

            if (poisonpositions.Contains(newVinePosition))
            {
                vineStopped = true;
                foreach (Vector2Int position in localVinePositions)
                {
                    vineGrid[position.x, position.y].GetComponent<SpriteRenderer>().material.SetFloat("_Burnt", 1);
                    vineGrid[position.x, position.y].GetComponent<SpriteRenderer>().color = Color.red;
                    vinePositions.Remove(position);
                }
                
                vinePositions.Remove(GridManager.GetGridPosition(transform.position));
                return;
            }
            
            GameObject selectedPrefab;
            bool clockwise;
            float rotationOffset = -90;
            VineType vineType;
            if ((currentDirection == 0 && nextDirection == 1) || (currentDirection == 1 && nextDirection == 2) || (currentDirection == 2 && nextDirection == 3) || (currentDirection == 3 && nextDirection == 0))
            {
                selectedPrefab = vineLPrefab;
                clockwise = false;
                rotationOffset = -180;
                vineType = VineType.LCounter;
            }
            else if ((currentDirection == 0 && nextDirection == 3) || (currentDirection == 3 && nextDirection == 2) || (currentDirection == 2 && nextDirection == 1) || (currentDirection == 1 && nextDirection == 0))
            {
                selectedPrefab = vineLPrefab; 
                clockwise = true;
                vineType = VineType.LClock;
            }
            else
            {
                selectedPrefab = vinePrefab;
                clockwise = false;
                vineType = VineType.Straight;
            }
            
            vinePositions.Add(activeVinePosition = newVinePosition);
            localVinePositions.Add(activeVinePosition); 
            Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(GridManager.directions[currentDirection].y, GridManager.directions[currentDirection].x) * Mathf.Rad2Deg + rotationOffset);
            (vineGrid[newVinePosition.x, newVinePosition.y] = Instantiate(selectedPrefab, GridManager.GetWorldPosition(newVinePosition), rotation)).transform.parent = transform;
            activeVineMaterial = vineGrid[activeVinePosition.x, activeVinePosition.y].GetComponent<Renderer>().material;
            activeVineMaterial.SetInt("_Clockwise", clockwise ? 1 : 0);
            VineSegment segment = vineGrid[activeVinePosition.x, activeVinePosition.y].GetComponent<VineSegment>();
            isGrowing = true;

            segment.inputDirection = (currentDirection + 2) % 4;
            segment.vineType = vineType;
            totalVines++;
            
            elapsedTime = 0;
        }
        else if (elapsedTime <= growTime)
        {
            isGrowing = true;
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
        int selectedDirection = -1;
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
                selectedDirection = directionIndex;
            }
        }
        if (!found || selectedDirection == -1) return;
        Instantiate(vineGrowerPrefab, GridManager.GetWorldPosition(selectedSegment), Quaternion.identity).transform.parent = transform.parent;
        vineGrid[selectedSegment.x, selectedSegment.y].GetComponent<VineSegment>().ExtendVine(selectedDirection, out GameObject newSegment);
        if (newSegment != null) vineGrid[selectedSegment.x, selectedSegment.y] = newSegment;
        newVineSpawned = true;
    }
}
