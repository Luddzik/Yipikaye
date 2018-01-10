using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollectables : MonoBehaviour {

    [SerializeField]
    private GameObject le;
    [SerializeField]
    private GameObject gan;

    public int RandomizedCollectables(float collectableChance)
    {
        int collectableCount = 0;
        if (Random.value < collectableChance / 100) { le.SetActive(true); collectableCount++; }
        if (Random.value < collectableChance / 100) { gan.SetActive(true); collectableCount++; }
        return collectableCount;
    }
}
