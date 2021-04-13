using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    [System.Serializable]
public class Agent : MonoBehaviour
{  
    public Spawner spawner;
    public float wanderingStrength = 0;
    public float foodSearchRadius = 4;
    float nextTimeToAddTrace = 0;
    public float traceInterval = 0.2f;
    Vector2 targetDirection ;
    Vector2 velocity;
    public float maxSpeed = 3;
    public float steeringStrength=3;
    Food food;
    public float sensorAngle;
    public int sensorDistance;
    public int sensorRange;

    float sensorLeft = 0;
    float sensorRight = 0;
    float sensorCenter = 0;

    // Start is called before the first frame update
    void Start()
    {
        targetDirection = Random.insideUnitCircle.normalized;        
    }

    private void OnDrawGizmosSelected() 
    {

        var sensorRad = (sensorAngle) / 180 * Mathf.PI;
        
        Vector2 leftSensorPos = transform.TransformPoint(sensorDistance*new Vector3(Mathf.Cos(sensorRad), Mathf.Sin(sensorRad),0));
        Vector2 RightSensorPos= transform.TransformPoint(sensorDistance*new Vector3(Mathf.Cos(-sensorRad), Mathf.Sin(-sensorRad),0));
        Vector2 CenterSensorPos= transform.TransformPoint(sensorDistance*new Vector3(Mathf.Cos(0), Mathf.Sin(0),0));

        var leftSensorRect = Map.instance.GetSenseRect(leftSensorPos, sensorRange);
        var rightSensorRect = Map.instance.GetSenseRect(RightSensorPos, sensorRange);
        var forwardSensorRect = Map.instance.GetSenseRect(CenterSensorPos, sensorRange);
        Gizmos.DrawWireCube((Vector2)leftSensorRect.position*Map.instance.gridSize+Map.instance.tl, new Vector3(sensorRange*2,sensorRange*2,sensorRange*2));
        Gizmos.DrawWireCube((Vector2)rightSensorRect.position*Map.instance.gridSize+Map.instance.tl, new Vector3(sensorRange*2,sensorRange*2,sensorRange*2));
        Gizmos.DrawWireCube((Vector2)forwardSensorRect.position*Map.instance.gridSize+Map.instance.tl, new Vector3(sensorRange*2,sensorRange*2,sensorRange*2));
        
        Gizmos.DrawLine(transform.position, (Vector3)targetDirection*5+transform.position );
        Gizmos.DrawLine(transform.position, (Vector3)acceleraton*5+transform.position );
        

    }

    void Sense()
    {
        var p = transform.position;
        //var layerMask = LayerMask.GetMask("foodMark","searchMark");

        Vector4 mask;
        if (food == null)
        {
            mask = new Vector4(0,1,0,0);
        }   
        else
            mask = new Vector4(1,0,0,0);

        var sensorRad = (sensorAngle) / 180 * Mathf.PI;
        
        Vector2 leftSensorPos = transform.TransformPoint(sensorDistance*new Vector3(Mathf.Cos(sensorRad), Mathf.Sin(sensorRad),0));
        Vector2 RightSensorPos= transform.TransformPoint(sensorDistance*new Vector3(Mathf.Cos(-sensorRad), Mathf.Sin(-sensorRad),0));
        Vector2 CenterSensorPos= transform.TransformPoint(sensorDistance*new Vector3(Mathf.Cos(0), Mathf.Sin(0),0));

        sensorLeft = Vector4.Dot(Map.instance.GetPheromone(transform.position, leftSensorPos,sensorRange),mask);
        sensorRight = Vector4.Dot(Map.instance.GetPheromone(transform.position,RightSensorPos,sensorRange),mask);
        sensorCenter = Vector4.Dot(Map.instance.GetPheromone(transform.position,CenterSensorPos,sensorRange),mask);



        var p2d = (Vector2)transform.position;    
    }
    Vector2 makeRandomDirection(Vector2 startWith)
    {
        return Random.insideUnitCircle ;
    }

    float PickRotation()
    {
            if ( sensorCenter > sensorLeft && sensorCenter > sensorRight )
            {
                return 0;
            }

            else if ( sensorRight > sensorCenter && sensorLeft > sensorCenter )
            {
                var ret= Random.Range(0,1);
                if (ret == 0)
                    ret = -1;
                return ret;
            }       
            else if (sensorLeft > sensorRight)
            {
                return 1;
            }
            else if (sensorRight > sensorLeft)
            {
                return -1;
            }
            return 0;
    }
Vector2 acceleraton;
    Quaternion MakeRotation(float angle)
    {
        return Quaternion.Euler(0,0,angle);
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        
        Sense();
        var steering = PickRotation();        

        if (steering > 0)
            targetDirection = Quaternion.Euler(0,0,sensorAngle)* transform.right;
        else if (steering < 0 )
            targetDirection =Quaternion.Euler(0,0,-sensorAngle)* transform.right;
        else
            targetDirection = transform.right;

        // // add noise to the direction
        targetDirection = (targetDirection+Random.insideUnitCircle* wanderingStrength).normalized ;
        
        
        if (food != null) //return to colony
        {
                food.transform.position = transform.position + transform.right*4.0f;
                var colony = Physics2D.OverlapCircle(transform.position, 80,LayerMask.GetMask("colony"));
                if ( colony != null)
                {
                    var diff = colony.transform.position - transform.position;
                    targetDirection = diff.normalized;
                }
        }

        if (Time.fixedTime > nextTimeToAddTrace)
        {
            if (food == null)
                Map.instance.AddPheromone( transform.position, new Vector4(1,0,0,0));
            else
                Map.instance.AddPheromone( transform.position, new Vector4(0,1,0,0));
            nextTimeToAddTrace = traceInterval + nextTimeToAddTrace;
        }



        if ( food == null)
            SearchFood();
        

        if (food != null)
        {
            if ((transform.position - Vector3.zero).magnitude < 50)
            {
                Destroy(food.gameObject);

            targetDirection = -transform.right ;
            velocity = -transform.right *maxSpeed;                
                food = null;
            }
        }

        var newPos = transform.position + (Vector3)(velocity * Time.fixedDeltaTime);
        if (newPos.x <-512 || newPos.x > 512 ||
            newPos.y <-512 || newPos.y > 512 )
            {
                newPos = transform.position;
                targetDirection = (-transform.position).normalized;
                velocity = targetDirection*maxSpeed;
            }        


        Vector2 targetVelocity = targetDirection * maxSpeed;
        Vector2 targetSteeringForce = (targetVelocity - velocity)*steeringStrength;
        acceleraton = Vector2.ClampMagnitude(targetSteeringForce,steeringStrength);

        velocity = Vector2.ClampMagnitude(velocity+acceleraton*Time.fixedDeltaTime,maxSpeed);


        transform.SetPositionAndRotation(newPos,Quaternion.Euler(0,0,Mathf.Atan2(velocity.y,velocity.x)*Mathf.Rad2Deg));

        
    }
    public float slowDownAngle = 90;
    Food GetClosestFood()
    {
        var food = Physics2D.OverlapCircle(transform.position,foodSearchRadius, LayerMask.GetMask("food"));
        if (food != null)
            return food.GetComponent<Food>();
        else
        return null;
    }

    void SearchFood()
    {
        Food newFood = GetClosestFood();
        if (newFood == null)
            return;
        var foodDir = (newFood.GetComponent<Transform>().position -  transform.position);
        targetDirection = foodDir.normalized;
        if (( newFood.transform.position - transform.position).magnitude < 10)
        {
            food = newFood;
            food.Taken();
            targetDirection = -transform.right ;
            velocity = -transform.right *maxSpeed;
        }
    }


}
