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
        if (Time.fixedTime - spawnedTime > life)
            Destroy(gameObject);
    }
}
