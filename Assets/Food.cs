using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public static List<Food> allFoods = new List<Food>();
    // Start is called before the first frame update
    void Start()
    {
        Food.allFoods.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
