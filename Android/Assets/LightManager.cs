using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour {

    [SerializeField]
    private List<Light> lights = new List<Light>();

    public void AddLight(Light light)
    {
        lights.Add(light);
    }

    public void Resize(float change)
    {
        for (int i = 0; i < lights.Count; i++)
        {
            lights[i].range *= change;
        }
    }

    public void ResizeNSetIntensity(float change, float intensity)
    {
        for(int i = 0; i < lights.Count; i++)
        {
            lights[i].range *= change;
            lights[i].intensity *= (1 + intensity);
        }
    }


}
