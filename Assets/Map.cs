using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    
    static public Map instance;
    public int grids = 1024;
    public float mapSize;
    public float gridSize = 0;
    public float decaySpeed;    
    float[] pheromones; 
    Texture2D pheromoneTexture;
    public Vector2 tl;
    public Vector2 rb;
    void Awake()
    {
        instance = this;
        pheromones = new float[grids*grids*4];
        tl = new Vector2(-mapSize/2,-mapSize/2);
        rb = new Vector2(mapSize/2,mapSize/2);
        gridSize = mapSize/grids;
        pheromoneTexture = new Texture2D(grids, grids, TextureFormat.RGBAFloat, false,true);
        GetComponent<Renderer>().material.SetTexture("_MainTex", pheromoneTexture);
    }

    
    private void FixedUpdate() {
        var decay =  Time.fixedDeltaTime*decaySpeed;
        for ( int i = 0; i < pheromones.Length/4;i++)
        {
            pheromones[i*4] -= decay;
            if (pheromones[i*4] < 0 )
                pheromones[i*4] = 0;
            pheromones[i*4+1] -= decay;
            if (pheromones[i*4+1] < 0 )
                pheromones[i*4+1] = 0;                
        }     


        pheromoneTexture.SetPixelData(pheromones,0);
        pheromoneTexture.Apply();
    }
    public void AddPheromone(Vector2 agnetPos, Vector4 value)
    { 
        var pos = agnetPos;
        pos -= tl;
        pos /=gridSize; 
        int x = (int)(pos.x); 
        int y = (int)(pos.y);
        if( x<0 || x>=grids ||y<0 ||y >=grids )
            return;

        pheromones[(x+y*grids)*4] += value.x;
        pheromones[(x+y*grids)*4+1] += value.y;
    }

    public RectInt GetSenseRect(Vector2 center, int range)
    {
        center -= tl;
        center /=gridSize;

        return new RectInt( (int)center.x, (int)center.y,range,range );
    }
    public Vector4 GetPheromone( Vector2 agentPos, Vector2 center, int range)
    {
        float pheromone1 = 0;
        float pheromone2 = 0;
        var rect = GetSenseRect(center, range);
        agentPos -= tl;
        agentPos /= gridSize;
        
        var senseCenter = rect.position + new Vector2(0.5f,0.5f);
        senseCenter *= gridSize;
                
        
        for ( int x = rect.xMin ; x <= rect.xMax ; x++)
        {
            for ( int y = rect.yMin; y <= rect.yMax; y++)
            {
                if( x<0 || x >=grids || y<0 || y >=grids )
                    continue;
                float fade =1;// ( new Vector2(x+0.5f,y+0.5f) -agentPos).sqrMagnitude;
                pheromone1 += pheromones[(x+y*grids)*4] /  fade;
                pheromone2 += pheromones[(x+y*grids)*4+1] / fade;
            }
        }
        return new Vector4(pheromone1,pheromone2,0,0);        ;
    }

}
