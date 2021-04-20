using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    [System.Serializable]
public class Agent : MonoBehaviour
{  
    public Spawner spawner;
    [Range(0,1)]
    public float wanderingStrength = 0;
    public float foodSearchRadius = 4;
    float nextTimeToAddTrace = 0;
    public float traceInterval = 0.2f;
    Vector2 targetDirection ;
    Vector2 velocity;
    public float maxSpeed = 3;
    public float steeringStrength=3;
    Food food;
    
    public float  pheromoneDensity;
    public float sensorAngle;
    public int sensorDistance;
    public int sensorRange;

    float sensorLeft = 0;
    float sensorRight = 0;
    float sensorCenter = 0;

    public LineRenderer lineMoving;
    public LineRenderer lineNudge;

    public LineRenderer lineTarget;

    public LineRenderer lineVelocity;

    public LineRenderer lineTargetVelocity;

    public LineRenderer lineTargetSteeringForce;
    public LineRenderer lineAcceleraton;

    public LineRenderer lineNewVelocity;

    public ParticleSystem homeMarkPareticles;
    public ParticleSystem foodMarkPareticles;

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

        Gizmos.DrawCube((Vector2)leftSensorRect.position*Map.instance.gridSize+Map.instance.tl, new Vector3(sensorRange*2,sensorRange*2,sensorRange*2));
        Gizmos.DrawCube((Vector2)rightSensorRect.position*Map.instance.gridSize+Map.instance.tl, new Vector3(sensorRange*2,sensorRange*2,sensorRange*2));
        Gizmos.DrawCube((Vector2)forwardSensorRect.position*Map.instance.gridSize+Map.instance.tl, new Vector3(sensorRange*2,sensorRange*2,sensorRange*2));

        

    }

    void Sense()
    {
        Vector4 mask;
        if (food == null)
        {
            mask = new Vector4(1,0,0,0);
        }   
        else
            mask = new Vector4(0,1,0,0);

        var sensorRad = (sensorAngle) / 180 * Mathf.PI;
        
        Vector2 leftSensorPos = transform.TransformPoint(sensorDistance*new Vector3(Mathf.Cos(sensorRad), Mathf.Sin(sensorRad),0));
        Vector2 RightSensorPos= transform.TransformPoint(sensorDistance*new Vector3(Mathf.Cos(-sensorRad), Mathf.Sin(-sensorRad),0));
        Vector2 CenterSensorPos= transform.TransformPoint(sensorDistance*new Vector3(Mathf.Cos(0), Mathf.Sin(0),0));

        sensorLeft = Vector4.Dot(Map.instance.GetPheromone(transform.position, leftSensorPos,sensorRange),mask);
        sensorRight = Vector4.Dot(Map.instance.GetPheromone(transform.position,RightSensorPos,sensorRange),mask);
        sensorCenter = Vector4.Dot(Map.instance.GetPheromone(transform.position,CenterSensorPos,sensorRange),mask);
    }
    Vector2 makeRandomDirection(Vector2 startWith)
    {
        return Random.insideUnitCircle ;
    }

    float PickRotation()
    { 
        if ( sensorCenter > sensorLeft && sensorCenter > sensorRight )
            return 0;
        if ( sensorRight > sensorCenter && sensorLeft > sensorCenter )
            return sensorLeft > sensorRight? 1: -1;
        else if (sensorLeft > sensorRight)
            return 1;
        else if (sensorRight > sensorLeft)
            return -1;
        return 0;            
    }
Vector2 acceleration;
    Quaternion MakeRotation(float angle)
    {
        return Quaternion.Euler(0,0,angle);
    }
    
    // Update is called once per frame

    void UpdateDebugLine( LineRenderer lineRenderer,Vector2 o,Vector2 p)
    {
        float lineLength = p.magnitude;
        float arrowLength = Mathf.Min(lineLength*0.1f, 1.0f);
        lineRenderer.SetPosition(0,o);
        lineRenderer.SetPosition(1,o+p-p.normalized*arrowLength);

        if (lineRenderer.transform.childCount > 0 )
        {
        var head = lineRenderer.transform.GetChild(0).GetComponentInChildren<LineRenderer>() ;
        if (head )
        {
            head.SetPosition(0,o+p-p.normalized*arrowLength);
            head.SetPosition(1,o+p);
            head.colorGradient = lineRenderer.colorGradient;
            head.sortingOrder=lineRenderer.sortingOrder;
        }
        }
    }
    void FixedUpdate()
    {
        Sense();
        var steering = PickRotation();        

        if (steering > 0)
            targetDirection = Quaternion.Euler(0,0,sensorAngle)* transform.right;
        else if (steering < 0 )
            targetDirection =Quaternion.Euler(0,0,-sensorAngle)* transform.right;
        // else
        //     targetDirection = transform.right;

        // add a tiny nudge to the moving direction every frame 
        // var nudge = Random.insideUnitCircle * wanderingStrength;
        // UpdateDebugLine(lineNudge,transform.position, nudge);
        // UpdateDebugLine(lineVelocity,transform.position, velocity);
        // UpdateDebugLine(lineMoving,transform.position, targetDirection);
        // targetDirection = (targetDirection + nudge).normalized;
        // UpdateDebugLine(lineTarget,transform.position, targetDirection);
        
        var nudge = Random.insideUnitCircle * wanderingStrength;
        targetDirection = (targetDirection + nudge).normalized; 

        if (food == null)
            SearchFood();

        if (food != null) //return to colony
        {
                food.transform.position = transform.position + transform.right*12.0f;
                var colony = Physics2D.OverlapCircle(transform.position, 80,LayerMask.GetMask("colony"));
                if ( colony != null)
                {
                    var diff = colony.transform.position - transform.position;
                    targetDirection = diff.normalized;
                }
        }

        // if (food == null )
        // {
        //     if (!foodMarkPareticles.isStopped)
        //         foodMarkPareticles.Stop();
        //     if (!homeMarkPareticles.isPlaying)
        //         homeMarkPareticles.Play();
        // }
        // else
        // {
        //     if (!homeMarkPareticles.isStopped)
        //         homeMarkPareticles.Stop();
        //     if (!foodMarkPareticles.isPlaying)
        //         foodMarkPareticles.Play();

        // }


        if (Time.fixedTime > nextTimeToAddTrace)
        {
            if (food == null)
                Map.instance.AddPheromone( transform.position, new Vector4(0,Time.fixedDeltaTime*pheromoneDensity,0,0));
            else
                Map.instance.AddPheromone( transform.position, new Vector4(Time.fixedDeltaTime*pheromoneDensity,0,0,0));
            nextTimeToAddTrace = traceInterval + nextTimeToAddTrace;
        }     

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
        transform.SetPositionAndRotation(newPos, Quaternion.Euler(0, 0, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg));


        Vector2 targetSteeringForce = (targetVelocity - velocity);
        targetSteeringForce *= steeringStrength;
        acceleration = Vector2.ClampMagnitude(targetSteeringForce, steeringStrength);
        velocity = Vector2.ClampMagnitude(velocity + acceleration * Time.fixedDeltaTime, maxSpeed);

        transform.SetPositionAndRotation(newPos, Quaternion.Euler(0, 0, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg));

        // UpdateDebugLine(lineTargetVelocity,transform.position, targetVelocity);
        // UpdateDebugLine(lineTargetSteeringForce,transform.position + (Vector3)velocity, targetVelocity-velocity);
        // UpdateDebugLine(lineAcceleraton, transform.position, acceleraton);
        // UpdateDebugLine(lineNewVelocity, transform.position, velocity);


        
    }
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
        }
    }


}
