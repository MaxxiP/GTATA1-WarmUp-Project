using UnityEngine;

public class AsteroidGameManager : MonoBehaviour
{

    [SerializeField] private GameObject winningScreen;
    [SerializeField] private GameObject loosingScreen;
    [SerializeField] private GameObject startScreen;

    void Start()
    {
        winningScreen.SetActive(false);
        loosingScreen.SetActive(false);
        startScreen.SetActive(true);
    }
    
    public void GameStart()
    {
        startScreen.SetActive(false);
        winningScreen.SetActive(false);
        loosingScreen.SetActive(false);
    }
    public void GameOver()
    {
        loosingScreen.SetActive(true);

    }

    public void GameWon()
    {
        winningScreen.SetActive(true);
    }

}
