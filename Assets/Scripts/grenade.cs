using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenade : MonoBehaviour
{
    public float di;
    private Animator ani;
    private Rigidbody2D rgb;
    int radius = 0;
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
            if (radius == 0) 
            { 
                var b = collision.gameObject;
                b.GetComponent<block>().Check();
                if (b.GetComponent<block>().touching.ToArray().Length < 3)
                {
                    b.GetComponent<block>().die = 1;
                }
            }
            else
            {
                var r = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask("blocks"));
                for(int i = 0; i < r.Length; i++)
                {
                    var b = r[i].gameObject;
                    b.GetComponent<block>().Check();
                    if (b.GetComponent<block>().touching.ToArray().Length < 3)
                    {
                        b.GetComponent<block>().die = 1;
                    }
                }
            }
            Destroy(gameObject, 2f);
            GetComponent<BoxCollider2D>().enabled = false;
          
        }
    }

    // Update is called once per frame
    void Update()
    {


    }
}
