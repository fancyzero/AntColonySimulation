using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Marker
{
    public Marker(Vector2 p)
    {
        position = p;
        createdTime = Time.fixedTime;
    }     
    public Vector2 position;
    public float createdTime;
    public GameObject gameObject;
}

[System.Serializable]
public class TraceMap
{
    public float decaySpeed=1;
    Color[] values;
    public Vector2 tl;
    public Vector2 rb;
    float cellSize;
    public int size = 256;
    public RenderTexture texture;
    public ComputeShader computeShader;
    public ComputeBuffer buffer;
    public TraceMap( Vector2 tl, Vector2 rb ,int size, ComputeShader computeShader) 
    {
        this.tl = tl;
        this.size = size;
        this.rb = rb;
        values  = new Color[size*size];
        cellSize = (rb.x - tl.x)/size;
        texture = new RenderTexture(size,size,0,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Linear);
        texture.enableRandomWrite = true;
        texture.filterMode= FilterMode.Point;
        texture.Create();     

        buffer = new ComputeBuffer(Colony.instance.ants.Count, 3*4 );
        this.computeShader = computeShader;
        computeShader.SetTexture(0,"map", texture);
        computeShader.SetBuffer(0,"ants", buffer);
        
    }
    public void AddMark(Vector2 pos, float value)
    {
        var gridPos = pos;
        gridPos -= tl;
        gridPos /=cellSize; 
        int x = (int)(gridPos.x);
        int y = (int)(gridPos.y);
        if( x<0 || x >=size ||y<0 ||y >=size )
            return; 
        
        values[x+y*size].r += value;
        if(values[x+y*size].r>4)
            values[x+y*size].r=4;
    }
    public float GetMarks( Vector2 pos,int range)
    {
        var gridPos = pos;
        float result = 0;

        gridPos -= tl;
        gridPos /=cellSize; 
        int x = (int)(gridPos.x);
        int y = (int)(gridPos.y);
        
        for ( int i = -range; i <= range; i++)
            for ( int j = -range; j <= range; j++)
            {
                if( x+i<0 || x+i >=size ||y+j<0 ||y+j >=size )
                    continue;
                result += values[(x+i)+(y+j)*size].r;
            }
        return result;
    }

    void Clean()
    {
        
    }

    public void Update()
    {
        List<Ant> ants = new List<Ant>();
        foreach ( var i in Colony.instance.ants)
        {
            ants.Add( new Ant(i.transform.position,i.hasFood()));
        }
        //update buffer
        buffer.SetData(ants);
        computeShader.Dispatch(0,ants.Count/32,1,1);
    }
}
