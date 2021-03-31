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
public class Cell
{
    public Cell()
    {
        points = new List<Marker>();
    }
    public List<Marker> points;
}

public class TraceMap 
{
    public float maxLife;
    List<Cell> cells;
    public Vector2 tl;
    public Vector2 rb;
    float cellSize;
    public int size = 100;

    public TraceMap(Vector2 tl, Vector2 rb ,int size) 
    {
        this.tl = tl;
        this.size = size;
        this.rb = rb;
        cells = new List<Cell>(size*size);
        for ( int i =0; i < size*size; i++)
        {
            cells.Add(new Cell());
        }

        cellSize = (rb.x - tl.x)/size;
    }
    public void AddMark(Vector2 pos)
    {
        pos -= tl;
        pos /=cellSize; 
        int x = (int)(pos.x);
        int y = (int)(pos.y);
        if( x<0 || x >=size ||y<0 ||y >=size )
            return;
        cells[x+y*size].points.Add(new Marker(pos));
    }
    public List<Marker> GetMarks( Vector2 pos,float r)
    {
        var r2= r*r ;
        List<Marker> ret = new List<Marker>();
        pos -= tl;
        pos /=cellSize; 
        int x = (int)(pos.x);
        int y = (int)(pos.y);
        for ( int i = -1; i < 2; i++)
            for ( int j = -1; j < 2; j++)
            {
                if( x+i<0 || x+i >=size ||y+j<0 ||y+j >=size )
                    continue;
                foreach( var p in cells[(x+i)+(y+j)*size].points)
                {
                    if ( (p.position - pos).SqrMagnitude() <= r2)
                    {
                        ret.Add(p);
                    }
                }
            }
        return ret;
    }

    void Clean()
    {
        
    }

    public void FixedUpdate()
    {
        int oldCount = 0;
        int newCount = 0;
        foreach( var c in cells )
        {
            oldCount += c.points.Count;
            c.points.RemoveAll((x) => { return Time.fixedTime - x.createdTime >= maxLife; });
            newCount += c.points.Count;
        }
        Debug.Log(string.Format("{0} {1}",oldCount, newCount));
    }
}
