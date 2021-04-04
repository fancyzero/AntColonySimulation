using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public float markerLife;
    public TraceMap homeMarks ;
    public TraceMap foodMarks;

    static public Map instance;
    void Awake()
    {
        instance = this;

        homeMarks = new TraceMap(new Vector2(-512,-512), new Vector2(512,512), 20);
        homeMarks.maxLife = markerLife;
        foodMarks = new TraceMap(new Vector2(-512,-512), new Vector2(512,512), 20);        
        foodMarks.maxLife = markerLife;
    }

    // Update is called once per frame
    private void FixedUpdate() {
        homeMarks.FixedUpdate();
        foodMarks.FixedUpdate();
    }

    // private void OnDrawGizmos() {
    //     for ( int i = 0; i < 40;i++)
    //     for ( int j = 0; j < 40;i++)
    //     {
    //         Gizmos.DrawWireCube(new Vector3(i+-512, j+-512), new Vector3(1024.0f/40.0f,1024.0f/40.0f,1024.0f/40.0f));
    //     }
    // }
}
