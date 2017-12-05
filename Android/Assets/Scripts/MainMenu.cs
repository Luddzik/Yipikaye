using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public void NewGame()
    {
        SceneManager.LoadScene("RandomMap", LoadSceneMode.Single);
    }

    public void Options()
    {
        SceneManager.LoadScene("Options", LoadSceneMode.Single);
    }

}
