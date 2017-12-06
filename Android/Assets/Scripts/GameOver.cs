using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameOver : MonoBehaviour {

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainScreen", LoadSceneMode.Single);
    }

    public void Restart()
    {
        SceneManager.LoadScene("RandomMap", LoadSceneMode.Single);
    }
}
