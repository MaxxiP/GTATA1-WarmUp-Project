using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakoutManager : MonoBehaviour
{
    public bool hasStarted;
    public bool gameOver;

    [SerializeField] public GameObject LevelOne;
    [SerializeField] public GameObject LevelTwo;
    private List<GameObject> bricks;
    private string activeLevel;

    private void Start()
    {
        LevelTwo.SetActive(false);
        activeLevel = "LevelOne";

    }

    private void Update()
    {
        // Use the count of active bricks to determine if the level is completed
        //Debug.Log(GameObject.FindGameObjectsWithTag("Brick").Length);
        if (GameObject.FindGameObjectsWithTag("Brick").Length <= 0)
        {
            Win();
        }
    }

    public void GameOver()
    {
        Debug.Log("You lost !");
    }

    void Win()
    {
        //bricks = new List<GameObject>();
        if (activeLevel == "LevelOne")
        {
            LoadLevel(LevelTwo);
        }
        
        
        Debug.Log("Game completed");
        
    }

    void LoadLevel(GameObject Level)
    {
        // here th first level will get set to inactive and swapped out with the second level
        //LevelTwo.SetActive(true);
        //LevelOne.SetActive(false);
        
        Level.SetActive(true);
        GameObject.Find(activeLevel).SetActive(false);
        activeLevel = Level.name;
    }
}
