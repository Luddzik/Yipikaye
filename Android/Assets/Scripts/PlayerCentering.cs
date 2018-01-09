using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCentering : MonoBehaviour {

    public float camHeight;
    public Transform lookAtCamera;
    public Transform imageTarget;
    public Transform charTF;
    public LayerMask obstacleLayerMask;
    public LayerMask playerLayerMask;
    public float speed;
    
    private RaycastHit hitted;
    private bool centered = false;
    private int playerLayer;
    private int allButPlayerLayer;

    public void CenterToThePlayer()
    {
        transform.position = new Vector3(-charTF.position.x, -(charTF.position.y + camHeight), -charTF.position.z);
        centered = true;

        int layerIndex = LayerMask.NameToLayer("Target");
        if (layerIndex == -1)
        {
            Debug.LogError("Layer does not exist");
        }
        else
        {
            playerLayer = (1 << layerIndex);
            allButPlayerLayer = ~playerLayer;
        }

        //transform.rotation = Camera.main.transform.rotation;
        //print(UnwrapAngle(imageTarget.localEulerAngles.x));
        //print(UnwrapAngle(imageTarget.localEulerAngles.y));
        //print(UnwrapAngle(imageTarget.localEulerAngles.z));
        //transform.localRotation = Quaternion.Euler(-UnwrapAngle(imageTarget.localEulerAngles.x) * 0.7f, // + UnwrapAngle(transform.localEulerAngles.x), 
        //    -UnwrapAngle(imageTarget.localEulerAngles.y) * 0.7f, // + UnwrapAngle(transform.localEulerAngles.y), 
        //    -UnwrapAngle(imageTarget.localEulerAngles.z) * 0.7f); // + UnwrapAngle(transform.localEulerAngles.z));
    }

    

    private void LateUpdate()
    {
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, Camera.main.transform.rotation, speed * Time.deltaTime);
        //if (charTF.position.magnitude > 1)
        //    transform.position = Vector3.MoveTowards(transform.position, -charTF.position, speed * Time.deltaTime);
        lookAtCamera.LookAt(Camera.main.transform);

        lookAtCamera.rotation = Quaternion.Euler(lookAtCamera.rotation.eulerAngles.x, 180, 0);
        //if (centered)
        //{
        //    if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitted, 100, obstacleLayerMask, QueryTriggerInteraction.Collide))
        //    {
        //        print("hitted" + hitted.transform.name);
        //        print(UnwrapAngle(transform.localEulerAngles.x) + ", " + UnwrapAngle(transform.localEulerAngles.y) + ", " +
        //            UnwrapAngle(transform.localEulerAngles.z));
        //        if (UnwrapAngle(transform.localEulerAngles.x) > UnwrapAngle(Camera.main.transform.localEulerAngles.x) - 90)
        //        {
        //            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x - speed * Time.deltaTime, -180, 0);

        //            //transform.position = (new Vector3(-charTF.position.x - transform.position.x, -(charTF.position.y + camHeight) - transform.position.y, -charTF.position.z - transform.position.z)).normalized 
        //             //   * Time.deltaTime * speed + transform.position;
        //        }
        //    }
        //    else if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitted, 100, playerLayer, QueryTriggerInteraction.Collide)){
        //        print(hitted.transform.name);
        //        print(UnwrapAngle(transform.localEulerAngles.x));
        //        if (UnwrapAngle(transform.localEulerAngles.x) < -20)
        //        {
        //            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x + speed * Time.deltaTime, -180, 0);
        //            print(UnwrapAngle(transform.localEulerAngles.x) + ", " + UnwrapAngle(transform.localEulerAngles.y) + ", " +
        //                UnwrapAngle(transform.localEulerAngles.z));
        //        }
                
        //    }

        //}


        //print(lookAtCamera.rotation.eulerAngles);
        //print(charTF.position);
        //transform.rotation = Quaternion.FromToRotation(charTF.forward, Camera.main.transform.forward);
    }

    private float UnwrapAngle(float angle)
    {
        if (angle >= 180)
            return angle - 360;
        else return angle;
    }
}
