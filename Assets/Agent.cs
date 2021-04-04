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
    public float speed = 3;
    public float rotateSpeed=3;
    Food food;
    public float sensorAngle;
    public float sensorDistance;
    public float sensorRange;

    public UnityEngine.UI.Text dbgText;
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

        Gizmos.DrawWireSphere(leftSensorPos, sensorRange);
        Gizmos.DrawWireSphere(RightSensorPos, sensorRange);
        Gizmos.DrawWireSphere(CenterSensorPos, sensorRange);

    }

    void Sense()
    {
        var p = transform.position;
        // var layerMask = LayerMask.GetMask("foodMark","searchMark");

        TraceMap map = null;
        if (food == null)
        {
            map = Map.instance.foodMarks;
        }   
        else
            map = Map.instance.homeMarks;

        var sensorRad = (sensorAngle) / 180 * Mathf.PI;
        
        Vector2 leftSensorPos = transform.TransformPoint(sensorDistance*new Vector3(Mathf.Cos(sensorRad), Mathf.Sin(sensorRad),0));
        Vector2 RightSensorPos= transform.TransformPoint(sensorDistance*new Vector3(Mathf.Cos(-sensorRad), Mathf.Sin(-sensorRad),0));
        Vector2 CenterSensorPos= transform.TransformPoint(sensorDistance*new Vector3(Mathf.Cos(0), Mathf.Sin(0),0));

        List<Marker> leftMarks = map.GetMarks(leftSensorPos,sensorRange);
        List<Marker> rightMarks = map.GetMarks(RightSensorPos,sensorRange);
        List<Marker> centerMarks = map.GetMarks(CenterSensorPos,sensorRange);



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

    Quaternion MakeRotation(float angle)
    {
        return Quaternion.Euler(0,0,angle);
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        
        Sense();
        // if (food != null)
        //     dbgText.text = string.Format("L:{0} C:{1} R:{2}",sensorLeft,sensorCenter,sensorRight);
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
                food.transform.position = transform.position + transform.right*3.0f;
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
            {
                Map.instance.homeMarks.AddMark( transform.position);
                //Instantiate(spawner.searchMark,transform.position, Quaternion.identity);
            }
            else
            {
                Map.instance.foodMarks.AddMark( transform.position);
                //Instantiate(spawner.foodMark, transform.position, Quaternion.identity);
            }
            nextTimeToAddTrace = traceInterval + nextTimeToAddTrace;
        }



        if ( food == null)
            SearchFood();
        

        if (food != null)
        {
            if ((transform.position - Vector3.zero).magnitude < 50)
            {
                Destroy(food.gameObject);
                food = null;
            }
        }

        var newPos = transform.position + (Vector3)(velocity * Time.fixedDeltaTime);
        if (newPos.x <-512 || newPos.x > 512 ||
            newPos.y <-512 || newPos.y > 512 )
            {
                newPos = transform.position;
                targetDirection = (-transform.position).normalized;
                velocity = targetDirection*speed;
            }        


        Vector2 targetVelocity = targetDirection * speed;
        Vector2 targetSteeringForce = (targetVelocity - velocity)*rotateSpeed;
        Vector2 acceleraton = Vector2.ClampMagnitude(targetSteeringForce,rotateSpeed);

        velocity = Vector2.ClampMagnitude(velocity+acceleraton*Time.fixedDeltaTime,speed);


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
            velocity = -transform.right *speed;
        }
    }


}
