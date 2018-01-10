using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarManage : MonoBehaviour {
    [SerializeField] private Image[] stars;
    [SerializeField] private Sprite shiningStar;

    public void SetStar(int starNum)
    {
        if (starNum > 0)
            stars[0].sprite = shiningStar;
        if (starNum > 1)
            stars[1].sprite = shiningStar;
        if (starNum > 2)
            stars[2].sprite = shiningStar;
    }
}
