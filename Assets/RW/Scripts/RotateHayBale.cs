using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateHayBale : MonoBehaviour
{
    public float xRotationSpeed;
    public float yRotationSpeed;   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, yRotationSpeed * Time.deltaTime, 0);
    }
}
