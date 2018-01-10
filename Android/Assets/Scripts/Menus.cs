using UnityEngine;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{

    [SerializeField] private GameObject mainMenu;

    [SerializeField] private GameObject options;
    [SerializeField] private GameObject manual;

    [SerializeField] private GameObject volume;
    [SerializeField] private GameObject screenBrightness;

    [SerializeField] private GameObject manualMode1;
    [SerializeField] private GameObject manualMode2;

    private void Awake()
    {
        mainMenu.SetActive(true);

        options.SetActive(false);
        manual.SetActive(false);
        volume.SetActive(false);
        screenBrightness.SetActive(false);
    }

    public void StoryMode()
    {
        SceneManager.LoadScene("RandomMap", LoadSceneMode.Single);
    }

    public void Options()
    {
        mainMenu.SetActive(false);

        options.SetActive(true);
    }

    public void Volume()
    {
        options.SetActive(false);

        volume.SetActive(true);
    }

    public void ScreenBrightness()
    {
        options.SetActive(false);

        screenBrightness.SetActive(true);
    }

    public void Manual()
    {
        mainMenu.SetActive(false);

        manual.SetActive(true);
    }

    public void NextPage()
    {
        manualMode1.SetActive(false);
        manualMode2.SetActive(true);
    }

    public void BackPage()
    {
        if ( manualMode2.activeSelf )
        {
            manualMode2.SetActive(false);
            manualMode1.SetActive(true);
        }
        else
        {
            options.SetActive(false);
            manual.SetActive(false);
            volume.SetActive(false);
            screenBrightness.SetActive(false);

            mainMenu.SetActive(true);
        }

    }

    public void Back()
    {
        options.SetActive(false);
        manual.SetActive(false);
        volume.SetActive(false);
        screenBrightness.SetActive(false);

        mainMenu.SetActive(true);
    }
}