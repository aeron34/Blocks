using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class column : MonoBehaviour
{
    public List<GameObject> blocks;
    bool run = false;
    float t = 0;
    // Start is called before the first frame update
    void Start()
    {
        blocks = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Takeoff(GameObject a)
    {
        foreach (GameObject n in blocks)
        {
            if (n != null)
            {
                n.GetComponent<block>().locked = false;
                n.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX |
                    RigidbodyConstraints2D.FreezeRotation;
            }
        }
        blocks.Clear();
    }
}
