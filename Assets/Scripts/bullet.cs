using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;

    void Start()
    {
        GetComponent<Rigidbody2D>().AddForce(new Vector2(speed, 1f), ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
