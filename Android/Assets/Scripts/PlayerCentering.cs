using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCentering : MonoBehaviour {

    public float camHeight;

	public void CenterToThePlayer()
    {
        Transform charTF = transform.GetChild(0);
        transform.localPosition = new Vector3(-charTF.localPosition.x * transform.localScale.x,
            (charTF.localPosition.z * 0.7f) * transform.localScale.y - 0.27f,
            -(charTF.localPosition.z + 0.007f) * transform.localScale.z);
        transform.localRotation = Quaternion.Euler(transform.parent.localEulerAngles.x + 77, transform.localRotation.y, transform.localRotation.z);
    }
}
