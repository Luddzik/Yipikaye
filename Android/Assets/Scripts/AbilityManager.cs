using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] private int startAbility = 3;
    [SerializeField] private int curAbility;
    [SerializeField] private Image[] abilityImage;
    [SerializeField] private Sprite[] abilitySprite;

    //[SerializeField] private Image abilityBG;
    //[SerializeField] private Sprite[] abilityBgSprite;

    private int maxAbilityAmount = 8;
    private int abilityPerHeart = 1;

    //private void Start()
    //{
    //    curAbility = startAbility;
    //    UpdateAbility();
    //}

    public void SetAbility(int chakra)
    {
        curAbility = chakra;
        UpdateAbility();
    }

    private void UpdateAbility()
    {
        bool empty = false;
        int i = 0;

        /*if (curAbility == 0)
        {
            abilityBG.sprite = abilityBgSprite[1];
        }
        if (curAbility >= 1)
        {
            abilityBG.sprite = abilityBgSprite[0];
        }*/

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
        UpdateAbility();
    }

    public void AddAbility()
    {
        curAbility++;
        curAbility = Mathf.Clamp(curAbility, 0, maxAbilityAmount);
        UpdateAbility();
    }
}