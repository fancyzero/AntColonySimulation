using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public Food foodTemplate;
    public float radius;
    public int count;
    void Start()
    {
        for ( int i = 0; i < count; i++)
        {
            var randOffset = Random.insideUnitCircle*radius;
            Instantiate(foodTemplate,transform.position+ (Vector3)(randOffset),Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
