using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlantManager : MonoBehaviour
{
    static GameObject plantPrefab;
    [SerializeField] TMP_Text vineText;
    public static bool GameOver { get; set; }
    private void Awake()
    {
        plantPrefab = Resources.Load<GameObject>("Prefabs/PlantOrigin");
        VineGrowing.ResetVariables();
    }
    void Update()
    {
        if (FoodManager.Nutrients < VineGrowing.VineCount)
        {
            if (VineGrowing.NoNodes)
            {
                Debug.Log("Game Over");
                GameOver = true;
            }
            VineGrowing.KillPlant();
        }
        if (Input.GetMouseButtonDown(0) && !GameOver)
        {
            Vector2Int mousePosition = GridManager.GetGridPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (VineGrowing.ContainsNode(mousePosition))
            {
                FoodManager.HandleDeath();
                Instantiate(plantPrefab, GridManager.GetWorldPosition(mousePosition), Quaternion.identity);
                FoodManager.OccupyFood(mousePosition);
                
                Destroy(gameObject);
            }
                
        }
        if (vineText == null) vineText = GameObject.FindGameObjectWithTag("VineText").GetComponent<TMP_Text>();
        vineText.text = (FoodManager.Nutrients - VineGrowing.VineCount).ToString();
        if (GameOver)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
