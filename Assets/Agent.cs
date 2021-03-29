using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    [System.Serializable]
public class Agent : MonoBehaviour
{  
    public Spawner spawner;
    public float speed = 1;
    public float wonderingStrength = 0;
    public float foodSearchRadius = 4;

    float nextTimeToAddTrace = 0;
    public float traceInterval = 0.2f;
    Vector2 movingDirection = new Vector3(0,0);

    Food food;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    Vector2 makeRandomDirection(Vector2 startWith)
    {
        return Random.insideUnitCircle ;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (spawner == null)
            return;
        if ( food == null)
        {
            movingDirection = (movingDirection +  Random.insideUnitCircle*wonderingStrength);
            movingDirection.Normalize();

            
            SearchFood();

            var rot = Quaternion.LookRotation(new Vector3(movingDirection.x, 0,movingDirection.y) );
            var p = this.GetComponent<Transform>().position;

            if (Time.fixedTime > nextTimeToAddTrace)
            {
                
                var tp = Instantiate(spawner.traceMark,  p,Quaternion.identity);
                spawner.searchTrace.AddPoint(p,tp);
                nextTimeToAddTrace =traceInterval+ nextTimeToAddTrace;
            }            
            p = p + new Vector3(movingDirection.x, 0,movingDirection.y)*Time.fixedDeltaTime*speed;

            if (p.x > 50 || p.x < - 50)
                movingDirection.x *=-1;
            if (p.z > 50 || p.z < - 50)
                movingDirection.y *= -1;

            this.GetComponent<Transform>().SetPositionAndRotation( p, rot);

            var minFood = GetClosestFood();
            if (minFood != null)
            {
                var minDist = (minFood.GetComponent<Transform>().position -  transform.position).magnitude;

                if ( minDist < 0.01f)
                {
                    food = minFood;
                    Food.allFoods.Remove(food);
                }
            }



        }
            if (food != null)
            {
                food.transform.position = transform.position + transform.forward*1.0f;
            }        
    }

    Food GetClosestFood()
    {
        Food minFood = null;
        float minDist = 1000000;
        foreach ( var f in Food.allFoods)
        {
            if ((f.GetComponent<Transform>().position - transform.position).magnitude < minDist)
            {
                minFood = f;
                minDist = (f.GetComponent<Transform>().position - transform.position).magnitude;
            }
        }
        return minFood;        
    }

    void SearchFood()
    {
        Food minFood = GetClosestFood();
        if (minFood == null)
            return;
        var foodDir = (minFood.GetComponent<Transform>().position -  transform.position);
        if (minFood != null && foodDir.magnitude < foodSearchRadius)
        {
            var v = foodDir.normalized;
            movingDirection =new Vector2 (v.x, v.z);
        }


    }

}
