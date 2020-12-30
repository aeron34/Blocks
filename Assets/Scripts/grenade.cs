using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenade : MonoBehaviour
{
    public float di;
    private Animator ani;
    private Rigidbody2D rgb;
    public GameObject bl;
    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
        rgb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 9)
        {

            ani.Play("explode");
            rgb.constraints = RigidbodyConstraints2D.FreezeAll;
             var b = collision.gameObject;
            bl = collision.gameObject;
            b.GetComponent<block>().Check();
            if (b.GetComponent<block>().touching.ToArray().Length < 3)
            {
                b.GetComponent<block>().die = 2;
                b.GetComponent<block>().explode();
            }
            Destroy(gameObject, 2f);
            GetComponent<BoxCollider2D>().enabled = false;
          
        }
    }
    private void FixedUpdate()
    {   
        
        
    }
    // Update is called once per frame
    void Update()
    {


    }
}
