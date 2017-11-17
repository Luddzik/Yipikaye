using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {

	public void ChangeScene(string scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }
}
