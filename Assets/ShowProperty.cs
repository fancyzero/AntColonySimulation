using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowProperty : MonoBehaviour
{
    // Start is called before the first frame update
    public Agent agent;
    public string contentStr;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       GetComponent<UnityEngine.UI.Text>().text=string.Format(contentStr,agent.wanderingStrength);
    }
}
