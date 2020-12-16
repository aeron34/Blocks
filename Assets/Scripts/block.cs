using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class block : MonoBehaviour
{
    private GameObject p;
    private SpriteRenderer spr;
    private Rigidbody2D rgb;
    private Animator ani;
    public bool follow;

    public int combo, ni = 0, die = 0;
    public bool t = false;
    public string color;
    public float thr_s = 5f;
    // Start is called before the first frame update
    void Start()
    {
        p = GameObject.Find("pic");
        ani = GetComponent<Animator>();
        rgb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
        switch(color)
        {
            case "blue":
                spr.color = new Color(0, (float)(92/255), (float)(236/255));
                break;
            case "green":
                Debug.Log("j");
                spr.color = new Color(0.0f, 1.0f, 0.05f, 1.0f);
                break;
        }

    }

    public void Check()
    {
        var c = Physics2D.OverlapCircleAll(transform.position, 4f);
        var b = c.Where(a => a.gameObject.layer == 9 && 
        a.gameObject.GetComponent<block>().color == color
        ).ToArray(); 
        var d = b.GroupBy(a => a.gameObject.name).Select(a => a.First()).ToArray();

        /*coll_objs is the object that counts how many objects are
        touching inside the OverlapCirlce (defined above).
        if they all 5 blocks are touching, in which the only
        thing they CAN touch is a block of the same color (thanks
        to the filter above), then
        trigger a chain explosion.
        */
        var coll_objs = 0;

        if (d.Length >= 5)
        {
            Debug.Log("sioaj");
            foreach (Collider2D h in d)
            {
                Debug.Log(h.gameObject.name);
                if (h.GetComponent<block>().t)
                {
                    coll_objs++;
                }
            }
        }

        if (coll_objs >= 5)
        {
            die = 1;
            foreach (Collider2D h in d)
            {
                h.gameObject.GetComponent<block>().die = 1;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (die ==0)
        {
            if (follow)
            {
                transform.position = new Vector3(
                    p.transform.position.x,
                    p.transform.position.y + 1.5f,
                    0f);

                spr.flipX = p.GetComponent<SpriteRenderer>().flipX;


                if (rgb.simulated)
                {
                    float[] a = { 90, 180, 270, 360 };
                    for (int i = 0; i < 4; i++)
                    {
                        a[i] = (int)rgb.rotation - a[i];
                        // Debug.Log(a[i]);
                    }
                    float l = a.Min();

                    int ind = Array.IndexOf(a, l);
                    var n = new Quaternion();

                    n.Set(0f, 0f, (int)((ind + 1) * 90), 0f);
                    transform.rotation = n;
                    rgb.rotation = ((ind + 1) * 90);

                }
                rgb.simulated = false;
            }
        }
        if(die == 1)
        {
            explode();
            die = 2;
        }
    }

    public void thrown()
    {
        follow = false;
        rgb.simulated = true;
        rgb.velocity = new Vector2((50 * p.GetComponent<first>().di), thr_s);
    }

    private void explode()
    {
        Check();
        spr.color = new Color(1, 1, 1);

        ani.Play("explode");
        GetComponent<BoxCollider2D>().enabled = false;
        rgb.simulated = false;
        StartCoroutine(Camera.main.GetComponent<cam>().Shake());

     
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            t = true;
            Check();
        }
          
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var obj = collision.gameObject;

        if (obj.layer == 9)
        {
//            combo += 1;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        var obj = collision.gameObject;

    }
} 
