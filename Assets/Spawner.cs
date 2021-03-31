using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Spawner : MonoBehaviour
{
    [SerializeField]
    public GameObject agentTemplate;

    [SerializeField]
    public GameObject searchMark;
    [SerializeField]
    public GameObject foodMark;    
    public int count;
    public float angentPerSecond;
    float startTime = 0.0f;
    int spawned = 0;
    // Start is called before the first frame update
    private void Start() 
    {
        startTime = Time.fixedTime;
    }
    void Update()
    {
        while (spawned < (Time.fixedTime-startTime)*angentPerSecond  && spawned < count)
        {
            var newAgent = Instantiate(agentTemplate, transform.position, Quaternion.identity);
            newAgent.GetComponent<Agent>().spawner = this;
            spawned ++;
        }
    }


}
