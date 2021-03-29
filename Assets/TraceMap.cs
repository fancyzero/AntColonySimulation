using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TracePoint
{
    public TracePoint(Vector2 p, float l,GameObject go)
    {
        position = p;
        life=l;
        gameObject = go;
    }
    public Vector2 position;
    public float life;
    public GameObject gameObject;
}
public class Cell
{
    public Cell()
    {
        points = new List<TracePoint>();
    }
    public List<TracePoint> points;
}

public class TraceMap 
{
    List<Cell> cells;
    Vector2 tl;
    Vector2 rb;

    int hCount;
    int vCount;

    int size;
    public TraceMap(int size,Vector2 tl, Vector2 rb)
    {
        this.tl = tl;
        this.rb = rb;
        this.size = size;
        cells = new List<Cell>();
        for (int i =0; i < size*size; i++) 
        {
            cells.Add( new Cell());
        }
    }
    public void AddPoint(Vector2 pos, GameObject go)
    {
        var cellSize = (rb.x - tl.x)/size;
        pos -= tl;
        pos /=cellSize; 
        int x = (int)(pos.x);
        int y = (int)(pos.y);
        cells[x+y*size].points.Add(new TracePoint(pos, 1,go));
    }
    public List<Vector2> GetPoints( Vector2 pos)
    {
        return null;
    }

    void Clean()
    {
        
    }

    void FixedUpdate()
    {
        foreach( var c in cells )
        {
            c.points.ForEach( x => x.life -= Time.fixedDeltaTime);
            c.points.ForEach( x => if () {Destroy(x.gameObject)});
            c.points.RemoveAll((x) => { return x.life <= 0; });
        }
    }
}
