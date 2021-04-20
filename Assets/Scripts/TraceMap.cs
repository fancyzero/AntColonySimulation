// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;


// public class TraceMap 
// {
//     public float maxLife=1000;
//     List<float> pheromones;
//     public Vector2 tl;
//     public Vector2 rb;
//     float cellSize;
//     public int size = 100;

//     public TraceMap(Vector2 tl, Vector2 rb ,int size) 
//     {
//         this.tl = tl;
//         this.size = size;
//         this.rb = rb;
//         pheromones = new List<float>(size*size);
//         cellSize = (rb.x - tl.x)/size;
//     }
//     public void AddMark(Vector2 pos, float value)
//     {
//         var gridPos = pos;
//         gridPos -= tl;
//         gridPos /=cellSize; 
//         int x = (int)(gridPos.x);
//         int y = (int)(gridPos.y);
//         if( x<0 || x >=size ||y<0 ||y >=size )
//             return;
        
//         pheromones[x+y*size]+=value;
//     }
//     public float GetMark( Vector2 pos,float r)
//     {
//         float ret = 0;
//         var gridPos = pos;
//         var r2= r*r ;
//         gridPos -= tl;
//         gridPos /=cellSize; 
//         int x = (int)(gridPos.x);
//         int y = (int)(gridPos.y);
        
//         for ( int i = -1; i < 2; i++)
//             for ( int j = -1; j < 2; j++)
//             {
//                 if( x+i<0 || x+i >=size ||y+j<0 ||y+j >=size )
//                     continue;
//                 ret += pheromones[x+y*size];
//             }
//         return ret;
//     }

//     void Clean()
//     {
        
//     }

//     public void FixedUpdate()
//     {
//         for ( int i = 0; i < pheromones.Count;i++)
//         {
//             pheromones[i] -= Time.fixedDeltaTime*Map.instance.decaySPeed;
//         }        
//     }
// }
