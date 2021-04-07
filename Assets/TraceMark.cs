using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MarkType
{
    mtFood,
    mtSearch
}
public class TraceMark : MonoBehaviour
{
    public float maxLife;
    float spawnedTime;
    float life;
    public MarkType type;
    
    // Start is called before the first frame update
    void Awake()
    {
        spawnedTime = Time.fixedTime;
        life = maxLife;
    }

    // Update is called once per frame
    private void FixedUpdate() {
        var c = GetComponentInChildren<SpriteRenderer>().color;
        GetComponentInChildren<SpriteRenderer>().color =new Color(c.r,c.g,c.b,1.0f-(Time.fixedTime - spawnedTime)/life) ;
        if (Time.fixedTime - spawnedTime > life)
            Destroy(gameObject);
    }
}
