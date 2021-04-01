using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AgentCS
{
    public AgentCS( Vector2 p, float a)
    {
        pos = p;
        angle = a;

    }
    public Vector2 pos;
    public  float angle;
}
public class CSTest : MonoBehaviour
{
    public int numAgents;
    public ComputeShader csAgentRun;
    public ComputeShader csProcessMap;

    public ComputeBuffer buffer;
    public float theMax;
    public RenderTexture traceMap;
    public RenderTexture processedTraceMap;

    public float moveSpeed = 4;
    public float sensorDist = 0.1f;
    public    int sensorRange = 2;
    public float turnSpeed = 0.3f;

    public float decaySpeed=0.2f;
    public float diffuseSpeed = 1;

    public int size = 256;
    // Start is called before the first frame update
    void Start()
    {
        traceMap = new RenderTexture(size,size,0,RenderTextureFormat.RFloat,RenderTextureReadWrite.Linear);
        traceMap.enableRandomWrite = true;
        // traceMap.filterMode= FilterMode.Point;
        traceMap.Create(); 

        processedTraceMap = new RenderTexture(size,size,0,RenderTextureFormat.RFloat,RenderTextureReadWrite.Linear);
        processedTraceMap.enableRandomWrite = true;
        // processedTraceMap.filterMode= FilterMode.Point;
        processedTraceMap.Create(); 

        
        List<AgentCS> agents=new List<AgentCS>();
        for ( int i = 0; i < numAgents ; i++)
        {
            
            var initialDir = Random.insideUnitCircle;
            var initialAngle = Mathf.Atan2(-initialDir.y, -initialDir.x);
            agents.Add(new AgentCS(initialDir*(size*0.4f)+new Vector2(size/2,size/2), initialAngle ));
        }
        buffer = new ComputeBuffer(agents.Count,3*4);
        buffer.SetData<AgentCS>(agents);
        csAgentRun.SetTexture(0,"res",traceMap,0);
        csAgentRun.SetBuffer(0, "agents",buffer);
        csAgentRun.SetInt("width",size);
        csAgentRun.SetInt("height",size);
        csAgentRun.SetFloat("speed",moveSpeed);        
        csAgentRun.SetInt("numAgents",agents.Count);
        csAgentRun.SetFloat("sensorDist",sensorDist);
        csAgentRun.SetInt("sensorRange",sensorRange);
        csAgentRun.SetFloat("turnSpeed",turnSpeed);

        csProcessMap.SetInt("width",size);
        csProcessMap.SetInt("height",size);
        csProcessMap.SetFloat("decaySpeed",decaySpeed);
        csProcessMap.SetFloat("diffuseSpeed",diffuseSpeed);
        csProcessMap.SetTexture(0,"map",traceMap,0);
        csProcessMap.SetTexture(0,"newMap",processedTraceMap,0);

        GetComponent<MeshRenderer>().material.SetTexture("_MainTex", traceMap);


    }

    // Update is called once per frame
    void Update()
    {
        csAgentRun.SetTexture(0,"res",traceMap,0);
        csProcessMap.SetTexture(0,"map",traceMap,0);
        csProcessMap.SetTexture(0,"newMap",processedTraceMap,0);

        float deltaTime = 0.005f;
        csAgentRun.SetFloat("deltaTime", deltaTime);
        csProcessMap.SetFloat("deltaTime", deltaTime);

        csProcessMap.SetFloat("decaySpeed",decaySpeed);
        csProcessMap.SetFloat("diffuseSpeed",diffuseSpeed);

        csAgentRun.SetFloat("sensorDist",sensorDist);
        csAgentRun.SetInt("sensorRange",sensorRange);
        csAgentRun.SetFloat("turnSpeed",turnSpeed);
        csAgentRun.SetFloat("speed",moveSpeed);        

        csAgentRun.Dispatch(0,size,size,1);
        csProcessMap.Dispatch(0,size/8,size/8,1);

        var tmp = traceMap;
        traceMap = processedTraceMap;
        processedTraceMap = tmp;

        
    }
}
