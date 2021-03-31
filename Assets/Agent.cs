using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    [System.Serializable]
public class Agent : MonoBehaviour
{  
    public SphereCollider centerSensor;
    public SphereCollider leftSensor;
    public SphereCollider rightSensor;
    public Spawner spawner;
    public float speed = 1;
    public float wanderingStrength = 0;
    public float foodSearchRadius = 4;

    float nextTimeToAddTrace = 0;
    public float traceInterval = 0.2f;
    Vector2 movingDirection = new Vector2(0,0);
    public float moveForce = 3;
    Food food;

    float sensorLeft = 0;
    float sensorRight = 0;
    float sensorCenter = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    void Sense()
    {
        var p = transform.position;
        var layerMask = LayerMask.GetMask("foodMark","searchMark");

        TraceMap map = null;
        if (food == null)
        {
            map = Map.instance.foodMarks;
        }
        else
            map = Map.instance.homeMarks;

        List<Marker> leftMarks = map.GetMarks(leftSensor.transform.position,leftSensor.radius);
        List<Marker> rightMarks = map.GetMarks(rightSensor.transform.position,rightSensor.radius);
        List<Marker> centerMarks = map.GetMarks(centerSensor.transform.position,centerSensor.radius);



        sensorLeft = 0;
        sensorRight = 0;
        sensorCenter = 0;

        var p2d = (Vector2)transform.position;
        
        foreach( var m in leftMarks)
        {
            sensorLeft += 1.0f;///(m.position - p2d).sqrMagnitude;
        }
        foreach( var m in rightMarks)
        {
            sensorRight += 1.0f;///(m.position - p2d).sqrMagnitude;
        }
        foreach( var m in centerMarks)
        {
            sensorCenter += 1.0f;///(m.position - p2d).sqrMagnitude;
        }        
        
    }
    Vector2 makeRandomDirection(Vector2 startWith)
    {
        return Random.insideUnitCircle ;
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        Sense();


        var p = this.GetComponent<Transform>().position;

        if (spawner == null)
            return;

        movingDirection = (movingDirection +  Random.insideUnitCircle*wanderingStrength);
        movingDirection.Normalize();      

        if ( food == null)
        {
            Quaternion rotateFix = Quaternion.identity;
            if ( sensorLeft > sensorCenter && sensorLeft > sensorCenter )
            {
                rotateFix = Quaternion.AngleAxis(-50,Vector3.back) ;
            }

            if ( sensorRight > sensorCenter && sensorRight > sensorLeft )
            {
                rotateFix =  Quaternion.AngleAxis(50,Vector3.back) ;
            }            
            movingDirection =  rotateFix *movingDirection ;
            movingDirection.Normalize();                   
            
            SearchFood();
        }

        if (food != null) //return home
        {

            Quaternion rotateFix = Quaternion.identity;
            if ( sensorLeft > sensorCenter && sensorLeft > sensorCenter )
            {
                rotateFix = Quaternion.AngleAxis(-50,Vector3.back) ;
            }

            if ( sensorRight > sensorCenter && sensorRight > sensorLeft )
            {
                rotateFix =  Quaternion.AngleAxis(50,Vector3.back) ;
            }             
            movingDirection =  rotateFix *movingDirection ;
            movingDirection.Normalize();    

            food.transform.position = transform.position + transform.up*1.0f;
            var hives = Physics.OverlapSphere(transform.position, 8,LayerMask.GetMask("hive"));
            if ( hives != null  && hives.Length > 0)
            {
                movingDirection = (hives[0].transform.position - transform.position).normalized;
            }
        }

        if (Time.fixedTime > nextTimeToAddTrace)
        {
            if (food == null)
            {
                Map.instance.homeMarks.AddMark( p);
               // Instantiate(spawner.searchMark, p, Quaternion.identity);
            }
            else
            {
                Map.instance.foodMarks.AddMark( p);
               // Instantiate(spawner.foodMark, p, Quaternion.identity);
            }
            nextTimeToAddTrace = traceInterval + nextTimeToAddTrace;
        }

        var rot = Quaternion.LookRotation(new Vector3(movingDirection.x, movingDirection.y,0).normalized , Vector3.back);
        var fixRot = Quaternion.AngleAxis(90, Vector3.right);
        rot  = rot*fixRot;
        //p = p + new Vector3(movingDirection.x, movingDirection.y,0)*Time.fixedDeltaTime*speed;
        GetComponent<Rigidbody>().AddForce( movingDirection*moveForce, ForceMode.Force );

        this.GetComponent<Transform>().SetPositionAndRotation( p, rot);    

        if (food != null)
        {
            if ((transform.position - Vector3.zero).magnitude < 1)
            {
                Destroy(food);
                food = null;
                movingDirection = Quaternion.AngleAxis(180,Vector3.back) * movingDirection;
            }
        }

    }

    Food GetClosestFood()
    {
        var foods = Physics.OverlapSphere(transform.position,foodSearchRadius, LayerMask.GetMask("food"));
        float minDist = 99999;
        Food minFood = null;
        foreach ( var f in foods)
        {
            if ( f.GetComponent<Food>() == null)
                continue;
            float d = (f.transform.position-transform.position).magnitude;
            if ( d < minDist)
            {
                minDist = d;
                minFood = f.GetComponent<Food>();
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
        foodDir.z = 0;
        if (minFood != null && foodDir.magnitude < foodSearchRadius)
        {
            var v = foodDir.normalized;
            movingDirection =new Vector2 (v.x, v.y);

            if ( foodDir.magnitude < 0.5f)
            {
                food = minFood;
                Food.allFoods.Remove(food);
                food.Taken();
                movingDirection = Quaternion.AngleAxis(180,Vector3.back) * movingDirection;
            }            
        }


    }

    void OnCollisionEnter(Collision other) 
    {
        List<ContactPoint> contactPoints = new List<ContactPoint>();
        other.GetContacts(contactPoints);
        if (contactPoints.Count > 0)
        {
            foreach ( var cp in contactPoints)
            {
                movingDirection = cp.normal;
                break;
            }
        }
    }


}
