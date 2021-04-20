using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AgentCS
{
    public AgentCS( Vector2 p, float a, Vector4 sp)
    {
        pos = p;
        angle = a;
        specie = sp;
    }
    public Vector2 pos;
    public  float angle;
    public Vector4 specie;
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
    public float trailWeight = 1.0f;
    public float decaySpeed=0.2f;
    public float diffuseSpeed = 1;

    public float sensorAngle = 60;
    public float sensorBaseAngle = 0;

    public int species = 1;
    public float simulationDeltaTime = 0.005f;
    public Texture2D uibg;
    public int size = 256;
    // Start is called before the first frame update
    void Reset()
    {

        traceMap = new RenderTexture(size,size,0,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Linear);
        traceMap.enableRandomWrite = true;
        traceMap.filterMode= FilterMode.Point;
        traceMap.Create(); 

        processedTraceMap = new RenderTexture(size,size,0,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Linear);
        processedTraceMap.enableRandomWrite = true;
        processedTraceMap.filterMode= FilterMode.Point;
        processedTraceMap.Create();     

        List<AgentCS> agents=new List<AgentCS>();
        for ( int i = 0; i < numAgents ; i++)
        {
            
            var initialDir = Random.insideUnitCircle;
            var initialAngle = Mathf.Atan2(initialDir.x, -initialDir.y);
            var specieId = Random.Range(0,species);
            Vector4 specie = new Vector4(0,0,0,0);
            if (specieId == 0)
                specie = new Vector4(1,-1,-1,-1);
            if (specieId == 1)
                specie = new Vector4(-1,1,-1,-1);
            if (specieId == 2)
                specie = new Vector4(-1,-1,1,-1);
            if (specieId == 3)
                specie = new Vector4(-1,-1,-1,1);                                                
            
            agents.Add(new AgentCS(initialDir*(size*0.4f)+new Vector2(size/2,size/2), initialAngle ,specie));
        
        }
        if (buffer != null )
            buffer.Dispose();
        buffer = new ComputeBuffer(agents.Count,7*4);
        buffer.SetData<AgentCS>(agents);
        csAgentRun.SetTexture(0,"res",traceMap,0);
        csAgentRun.SetBuffer(0, "agents",buffer);
        csAgentRun.SetInt("width",size);
        csAgentRun.SetInt("height",size);
        csAgentRun.SetFloat("speed",moveSpeed);        
        csAgentRun.SetInt("numAgents",agents.Count);
        csAgentRun.SetFloat("sensorDist",sensorDist);
        csAgentRun.SetInt("sensorRange",sensorRange);
        csAgentRun.SetFloat("sensorAngle",sensorAngle);
        csAgentRun.SetFloat("sensorBaseAngle",sensorBaseAngle);
        csAgentRun.SetFloat("turnSpeed",turnSpeed);

        csProcessMap.SetInt("width",size);
        csProcessMap.SetInt("height",size);
        csProcessMap.SetFloat("decaySpeed",decaySpeed);
        csProcessMap.SetFloat("diffuseSpeed",diffuseSpeed);
        csProcessMap.SetTexture(0,"map",traceMap,0);
        csProcessMap.SetTexture(0,"newMap",processedTraceMap,0);

        GetComponent<MeshRenderer>().material.SetTexture("_MainTex", traceMap);    
    }
    void Start()
    {
        Reset();
    }

    float SliderUI( float value, float left, float right,string labelName)
    {
        UnityEngine.GUILayout.BeginHorizontal(GUILayout.MinWidth(200));
        UnityEngine.GUILayout.Label(labelName);
        float ret = UnityEngine.GUILayout.HorizontalSlider(value, left,right);
        UnityEngine.GUILayout.EndHorizontal();
        return ret;
    }
    private void OnGUI() {
        
        GUIStyle style = new GUIStyle();
        style.normal.background = uibg;
        GUILayout.BeginArea(new Rect(10, 10, 250, 200),style);
        if (UnityEngine.GUILayout.Button("Reset"))
        {
            Reset();
        }
        moveSpeed = SliderUI(moveSpeed,0,100,"Move:");
        turnSpeed = SliderUI(turnSpeed,0,100,"Turn:");
        decaySpeed = SliderUI(decaySpeed,0,10,"Decay:");
        diffuseSpeed = SliderUI(diffuseSpeed,0,200,"Diffuse:");
        trailWeight = SliderUI(trailWeight,0,400,"Weight");
        GUILayout.EndArea();
    }
    // Update is called once per frame
    void Update()
    {
        csAgentRun.SetTexture(0,"res",traceMap,0);

        float deltaTime = simulationDeltaTime;
        csAgentRun.SetFloat("deltaTime", deltaTime);
        csAgentRun.SetFloat("sensorDist",sensorDist);
        csAgentRun.SetInt("sensorRange",sensorRange);
        csAgentRun.SetFloat("trailWeight",trailWeight);
        csAgentRun.SetFloat("turnSpeed",turnSpeed);
        csAgentRun.SetFloat("speed",moveSpeed);        
        csAgentRun.SetFloat("sensorAngle",sensorAngle);
        csAgentRun.SetFloat("sensorBaseAngle",sensorBaseAngle);


        csProcessMap.SetTexture(0,"map",traceMap,0);
        csProcessMap.SetTexture(0,"newMap",processedTraceMap,0);
        csProcessMap.SetFloat("deltaTime", deltaTime);
        csProcessMap.SetFloat("decaySpeed",decaySpeed);
        csProcessMap.SetFloat("diffuseSpeed",diffuseSpeed);

        csAgentRun.Dispatch(0,numAgents/32,1,1);
        csProcessMap.Dispatch(0,size/8,size/8,1);

        var tmp = traceMap;
        traceMap = processedTraceMap;
        processedTraceMap = tmp;


        GetComponent<MeshRenderer>().material.SetTexture("_MainTex", traceMap);
        
    }
    private void OnDestroy() {
        buffer.Dispose();
    }
}
