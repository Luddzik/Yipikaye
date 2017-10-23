using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class TUIController : MonoBehaviour
{

    public Text outputText;
    public Transform inputTransform;
    public Transform outputTransform;

    [SerializeField]
    private float sensitivity;

    private Vector3 mInitialInputPosition;
    private Vector3 mInitialOutputPosition;
    private Quaternion mInitialInputRotation;
    private Quaternion mInitialOutputRotation;

    private Quaternion mRelativeInputRotation;

    private bool mInputFound;
    private bool mOutputFound;

    // Use this for initialization
    void Start()
    {
        //mInitialInputPosition = Vector3.zero;
        //mInitialOutputPosition = Vector3.zero;
        //mInitialInputRotation = Quaternion.identity;
        //mInitialOutputRotation = Quaternion.identity;
        mInputFound = false;
        mOutputFound = false;
    }

    // Update is called once per frame
    void Update()
    {
        Control();
        MonitorPosition();
    }

    void Control()
    {
        if(mInputFound && mOutputFound)
        {
            mRelativeInputRotation = inputTransform.rotation * Quaternion.Inverse(mInitialInputRotation);
            mRelativeInputRotation.eulerAngles = new Vector3(mRelativeInputRotation.x - 360, mRelativeInputRotation.y - 360, mRelativeInputRotation.z - 360);

            if (mRelativeInputRotation.eulerAngles.magnitude < 0.1)
            {
                mRelativeInputRotation = Quaternion.identity;
            }

            outputTransform.Translate(-(mRelativeInputRotation.z) * sensitivity * Time.deltaTime, 
                -(mRelativeInputRotation.x) * sensitivity * Time.deltaTime, 0);
        }
    }

    void MonitorPosition()
    {
        outputText.text = "In Pos = " + inputTransform.position + ", In rota. = " + inputTransform.rotation
            + "\nOu Pos = " + outputTransform.position + ", Ou rota. = " + outputTransform.rotation;
    }

    public void UpdateInitialInputTransform(Vector3 position, Quaternion rotation)
    {
        mInitialInputPosition = position;
        mInitialInputRotation = rotation;

        mInputFound = true;
    }

    public void UpdateInitialOutputTransform(Vector3 position, Quaternion rotation)
    {
        mInitialOutputPosition = position;
        mInitialOutputRotation = rotation;

        mOutputFound = true;
    }
}
