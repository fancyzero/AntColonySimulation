using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lookad2dTest : MonoBehaviour
{

    public float  x;
    public float y;
    public Vector3 up;

    public float fixAngle=90;
    public Vector3 fixAxis = Vector3.right;


    public float fixAngle2=90;
    public Vector3 fixAxis2 = Vector3.up;    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion fixRot = Quaternion.AngleAxis(fixAngle, fixAxis);
        Quaternion fixRot2 = Quaternion.AngleAxis(fixAngle2, fixAxis2);
        transform.rotation = Quaternion.LookRotation(new Vector3(x,y,0).normalized,up)*fixRot2*fixRot;
    }
}
