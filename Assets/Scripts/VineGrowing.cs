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
    static GameObject[,] vineGrid;

    float elapsedTime = float.MaxValue;
    bool vineStopped = false;
    bool newVineSpawned = false;

    void Awake()
    {
        vineGrid = new GameObject[GridManager.GridSize.x, GridManager.GridSize.y];
        vineGrowerPrefab = Resources.Load<GameObject>("Prefabs/VineGrower");
        activeVinePosition = GridManager.GetGridPosition(transform.position);
    }
    
    void Update()
    {
        if(!vineStopped)
            GrowVine();
        else if (!newVineSpawned)
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

            Vector2Int selectedFood = FoodManager.GetClosestFoodFromNeighbor(activeVinePosition, out int directionIndex);
            
            if (directionIndex == -1) return;
            
            Vector2Int newVinePosition = activeVinePosition + GridManager.directions[directionIndex];
            vinePositions.Add(newVinePosition);

            if (newVinePosition == selectedFood)
            {
                vineStopped = true;
                FoodManager.OccupyFood(selectedFood);
                Instantiate(nodePrefab, GridManager.GetWorldPosition(selectedFood) + new Vector3(0,0,-0.1f), Quaternion.identity);
                return;
            }
            
            
            vinePositions.Add(activeVinePosition = newVinePosition);
            Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(GridManager.directions[directionIndex].y, GridManager.directions[directionIndex].x) * Mathf.Rad2Deg);
            vineGrid[newVinePosition.x, newVinePosition.y] = Instantiate(vinePrefab, GridManager.GetWorldPosition(newVinePosition), rotation);
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
        Instantiate(vineGrowerPrefab, GridManager.GetWorldPosition(selectedSegment), Quaternion.identity);
        newVineSpawned = true;
    }
}
