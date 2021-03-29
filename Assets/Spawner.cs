using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Spawner : MonoBehaviour
{
    [SerializeField]
    public GameObject agentTemplate;
    public TraceMap searchTrace;
    public TraceMap foodTrace;

    [SerializeField]
    public GameObject traceMark;
    public int count;
    // Start is called before the first frame update
    void Start()
    {
        searchTrace = new TraceMap(100,new Vector2(-50,-50),new Vector2(50,50));
        foodTrace = new TraceMap(100,new Vector2(-50,-50),new Vector2(50,50));
        for ( int i =0; i < count  ; i++)
        {
            var newAgent = Instantiate(agentTemplate);
            newAgent.GetComponent<Agent>().spawner = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
