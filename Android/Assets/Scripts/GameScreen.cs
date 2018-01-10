using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScreen : MonoBehaviour {

    [SerializeField] private GameObject gameScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject gameOverScreen;

    [SerializeField] private GameObject miniMap;
    [SerializeField] private AbilityManager abilityManager;
    [SerializeField] private HealthManager healthManager;
    [SerializeField] private Image[] stars;
    [SerializeField] private Sprite shiningStar;

    public void PauseScreen()
    {
        Time.timeScale = 0;
        gameScreen.SetActive(false);
        //miniMap.SetActive(false);
        pauseScreen.SetActive(true);
    }

    public void Resume()
    {
        gameScreen.SetActive(true);
        pauseScreen.SetActive(false);
        //miniMap.SetActive(false);
        Time.timeScale = 1;
    }

    public void RestartGame()
    {
        pauseScreen.SetActive(false);
        //miniMap.SetActive(false);
        gameScreen.SetActive(true);
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void MiniMap()
    {
        gameScreen.SetActive(false);
        miniMap.SetActive(true);
    }

    public void Victory(int starNum)
    {
        gameScreen.SetActive(false);
        victoryScreen.SetActive(true);
        if (starNum > 0)
            stars[0].sprite = shiningStar;
        if (starNum > 1)
            stars[1].sprite = shiningStar;
        if (starNum > 2)
            stars[2].sprite = shiningStar;
    }

    public void GameOver()
    {
        gameScreen.SetActive(false);
        gameOverScreen.SetActive(true);
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("Menus", LoadSceneMode.Single);
    }

    public void InitializePlayerUI(int health, int chakra)
    {
        healthManager.SetHealth(health);
        abilityManager.SetAbility(chakra);
    }

    public void AddChakra()
    {
        abilityManager.AddAbility();
    }

    public void ReduceChakra(int amount)
    {
        abilityManager.UsedAbility(amount);
    }

    public void AddHealth()
    {
        healthManager.AddHealth();
    }

    public void ReduceHealth(int amount)
    {
        healthManager.TakeDamage(amount);
    }
}
