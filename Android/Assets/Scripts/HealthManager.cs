using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {

    [SerializeField] private int startHealth = 1;
    [SerializeField] private int curHealth;
    [SerializeField] private Image[] healthImage; //location in the canvas
    [SerializeField] private Sprite[] healthSprite; //empty (0) and full (1)

    [SerializeField] private Image healthBG;
    [SerializeField] private Sprite[] healthBgSprite;

    private int maxHealthAmount = 5;
    //private int maxHealth;
    private int healthPerHeart = 1;

	//void Start () 
 //   {
 //       curHealth = startHealth;
 //       //maxHealth = maxHealthAmount;
 //       checkHealthAmount();
	//}
	
    public void SetHealth(int health)
    {
        curHealth = health;
        checkHealthAmount();
    }

    void checkHealthAmount()
    {
        for (int i = 0; i < maxHealthAmount; i++)
        {
            if (curHealth <= i)
            {
                healthImage[i].enabled = false;
            }
            else
            {
                healthImage[i].enabled = true;
            }
        }
        UpdateHealth();
    }

    void UpdateHealth()
    {
        bool empty = false;
        int i = 0;

        if( curHealth <= 1 )
        {
            healthBG.sprite = healthBgSprite[1];
        }
        if (curHealth > 1)
        {
            healthBG.sprite = healthBgSprite[0];
        }

        foreach (Image image in healthImage)
        {
            if(empty)
            {
                image.sprite = healthSprite[0]; //empty sprite
            }
            else
            {
                i++;
                if (curHealth >= i)
                {
                    image.sprite = healthSprite[healthSprite.Length - 1];
                }
                else
                {
                    int currentHealth = (int)(healthPerHeart - (healthPerHeart * i - curHealth));
                    int healthPerImage = healthPerHeart / (healthSprite.Length - 1);
                    int imageIndex = currentHealth / healthPerImage;
                    image.sprite = healthSprite[imageIndex];
                    empty = true;
                }
            }
        }
    }

    public void TakeDamage(int amount)
    {
        curHealth -= amount;
        curHealth = Mathf.Clamp(curHealth, 0, maxHealthAmount);
        if (curHealth <= 0)
        {
            //UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver", UnityEngine.SceneManagement.LoadSceneMode.Single);
            //it is not used here, need to set canvas active/inactive for gamewin and gameover screen
        }
        UpdateHealth();
    }

    public void AddHealth()
    {
        curHealth++;
        curHealth = Mathf.Clamp(curHealth, 0, maxHealthAmount);

        checkHealthAmount();
    }
}
