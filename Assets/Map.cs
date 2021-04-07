using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Ant
{
    Vector2 pos;
    float hasFood;
    public Ant( Vector2 iPos, float iHasFOod)
    {
        pos = iPos;
        hasFood = iHasFOod ;
    }
}
public class Map : MonoBehaviour
{
    
    public float markerDecaySpeed;
    public TraceMap homeMarks ;
    public TraceMap foodMarks;

    public Vector2 mapLeftTop;
    public Vector2 mapRightBottom;
    static public Map instance;

    public ComputeShader computeShader;
    void Awake()
    {
        instance = this;

        homeMarks = new TraceMap(new Vector2(-512,-512), new Vector2(512,512), 256,computeShader);
        foodMarks = new TraceMap(new Vector2(-512,-512), new Vector2(512,512), 256,computeShader);        
        homeMarks.decaySpeed = markerDecaySpeed;
        foodMarks.decaySpeed = markerDecaySpeed;
        GetComponent<Renderer>().material.SetTexture("FoodMark", foodMarks.texture);
        GetComponent<Renderer>().material.SetTexture("HomeMark", homeMarks.texture);
    }

    // Update is called once per frame
    private void FixedUpdate() {
        homeMarks.decaySpeed = markerDecaySpeed;
        foodMarks.decaySpeed = markerDecaySpeed;        
        homeMarks.Update();
        foodMarks.Update();
    }

    // private void OnDrawGizmos() {
    //     for ( int i = 0; i < 40;i++)
    //     for ( int j = 0; j < 40;i++)
    //     {
    //         Gizmos.DrawWireCube(new Vector3(i+-512, j+-512), new Vector3(1024.0f/40.0f,1024.0f/40.0f,1024.0f/40.0f));
    //     }
    // }
}
