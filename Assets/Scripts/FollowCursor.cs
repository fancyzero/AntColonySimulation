using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    public Camera currentCam;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = currentCam.ScreenPointToRay(Input.mousePosition);
        transform.position = new Vector3(ray.origin.x,ray.origin.y, 0);
    }
}
