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

        homeMarks = new TraceMap(new Vector2(-50,-50), new Vector2(50,50), 100);
        homeMarks.maxLife = markerLife;
        foodMarks = new TraceMap(new Vector2(-50,-50), new Vector2(50,50), 100);        
        foodMarks.maxLife = markerLife;
    }

    // Update is called once per frame
    private void FixedUpdate() {
        homeMarks.FixedUpdate();
        foodMarks.FixedUpdate();
    }
}
