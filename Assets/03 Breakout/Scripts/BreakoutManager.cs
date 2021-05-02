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

    private void Start()
    {
        LevelTwo.SetActive(false);
        Debug.Log(GameObject.FindGameObjectsWithTag("Brick").Length);
        
        
    }

    private void Update()
    {
        Debug.Log(GameObject.FindGameObjectsWithTag("Brick").Length);
        if (GameObject.FindGameObjectsWithTag("Brick").Length <= 0)
        {
            LoadLevel();
        }
    }

    void GameOver()
    {
        
    }

    void Win()
    {
        //bricks = new List<GameObject>();
        LoadLevel();
    }

    void LoadLevel()
    {
        LevelTwo.SetActive(true);
        LevelOne.SetActive(false);
    }
}
