using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollectables : MonoBehaviour {

    [SerializeField]
    private GameObject le;
    [SerializeField]
    private GameObject gan;

    public void RandomizedCollectables(float collectableChance)
    {
        if (Random.value > collectableChance/100) le.SetActive(true);
        if (Random.value > collectableChance / 100) gan.SetActive(true);
    }
}
