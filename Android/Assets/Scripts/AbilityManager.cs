using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour
{

    [SerializeField] private int startAbility = 3;
    [SerializeField] private int curAbility;
    [SerializeField] private Image[] abilityImage; //location in the canvas
    [SerializeField] private Sprite[] abilitySprite; //empty (0) and full (1)

    private int maxAbilityAmount = 10;
    //private int maxAbility;
    private int abilityPerHeart = 1;

    void Start()
    {
        //curAbility = startAbility;
        //maxAbility = maxAbilityAmount;
        //checkAbilityAmount();
    }

    public void SetAbility(int amount)
    {
        curAbility = amount;
        checkAbilityAmount();
    }

    void checkAbilityAmount()
    {
        for (int i = 0; i < maxAbilityAmount; i++)
        {
            if (curAbility <= i)
            {
                abilityImage[i].enabled = false;
            }
            else
            {
                abilityImage[i].enabled = true;
            }
        }
        UpdateAbility();
    }

    void UpdateAbility()
    {
        bool empty = false;
        int i = 0;

        foreach (Image image in abilityImage)
        {
            if (empty)
            {
                image.sprite = abilitySprite[0]; //empty sprite
            }
            else
            {
                i++;
                if (curAbility >= i)
                {
                    image.sprite = abilitySprite[abilitySprite.Length - 1];
                }
                else
                {
                    int currentAbility = (int)(abilityPerHeart - (abilityPerHeart * i - curAbility));
                    int abilityPerImage = abilityPerHeart / (abilitySprite.Length - 1);
                    int imageIndex = currentAbility / abilityPerImage;
                    image.sprite = abilitySprite[imageIndex];
                    empty = true;
                }
            }
        }
    }

    public void UsedAbility(int amount)
    {
        curAbility -= amount;
        curAbility = Mathf.Clamp(curAbility, 0, maxAbilityAmount);
        checkAbilityAmount();
    }

    public void AddAbility()
    {
        curAbility++;
        curAbility = Mathf.Clamp(curAbility, 0, maxAbilityAmount);

        checkAbilityAmount();
    }
}
