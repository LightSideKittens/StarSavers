using UnityEngine;
using System.Collections;

public class ExampleClass : MonoBehaviour
{
    public Transform cam;
    public Vector3 cameraRelative;

    void Update()
    {
        cam = Camera.main.transform;
        Vector3 cameraRelative = cam.InverseTransformPoint(transform.position);

        if (cameraRelative.z > 0)
            print("The object is in front of the camera");
        else
            print("The object is behind the camera");
    }
}