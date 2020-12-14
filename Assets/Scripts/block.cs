using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class block : MonoBehaviour
{
    public GameObject p;
    public int sx;
    private bool side;
    private Rigidbody2D rgb;
    // Start is called before the first frame update
    void Start()
    {
        rgb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var obj = collision.gameObject;
     
        if (obj.layer == 8 || obj.layer == 9)
        {
            sx = Check(collision);
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        var obj = collision.gameObject;
        sx = 0;
        if (obj.layer == 9)
        {
            sx = Check(collision.collider);
            if(sx != 0)
            {
               
            }
        }
    }

    private int Check(Collider2D collision)
    {
        if (collision.gameObject.transform.position.x < transform.position.x)
        {
            return -1;
        }

        if (collision.gameObject.transform.position.x > transform.position.x)
        {
            return 1;
        }

        return 0;
    }
       
    private void OnTriggerExit2D(Collider2D collision)
    {
        sx = 0;
    }

}
