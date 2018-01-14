using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

    public float speed = 1;

    public Renderer[] renderers;
    private MaterialPropertyBlock propBlock;
    private bool turnOff = false;
    private bool turnOn = false;
    private float t = 0.0f;
    [SerializeField]
    private float alpha = 1;

    private void Awake()
    {
        propBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if (turnOff)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].GetPropertyBlock(propBlock);
                propBlock.SetFloat("_Transparency", Mathf.Lerp(1, 0, t));
                alpha = propBlock.GetFloat("_Transparency");
                //print(gameObject.name + " " + propBlock.GetFloat("_Transparency"));
                renderers[i].SetPropertyBlock(propBlock);
            }
            t += Time.deltaTime / speed;
            if (t > 1.0f)
            {
                turnOff = false;
            }
        }
        else if (turnOn)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].GetPropertyBlock(propBlock);
                propBlock.SetFloat("_Transparency", Mathf.Lerp(0, 1, t));
                alpha = propBlock.GetFloat("_Transparency");
                renderers[i].SetPropertyBlock(propBlock);
            }
            t += Time.deltaTime / speed;
            if (t > 1.0f)
            {
                turnOn = false;
            }
        }
    }

    public void TurnOffRenderers()
    {
        print(gameObject.name + " TurnOff");
        turnOff = true;
        turnOn = false;
    }

    public void TurnOnRenderers()
    {
        turnOn = true;
        turnOff = false;
    }
}
