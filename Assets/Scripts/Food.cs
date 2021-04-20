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

    public void Taken()
    {
        var cs = GetComponents<Collider2D>();
        foreach( var c in cs )
        {
            c.enabled = false;
        
        }
        if (GetComponent<FollowCursor>())
            GetComponent<FollowCursor>().enabled = false;
        GetComponentInChildren<SpriteRenderer>().sortingOrder = 20;
    }

}
