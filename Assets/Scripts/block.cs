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
    public float thr_s = 5f;
    // Start is called before the first frame update
    void Start()
    {
        p = GameObject.Find("pic");
        ani = GetComponent<Animator>();
        rgb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
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
                for(int i = 0; i < 4; i++)
                {
                    a[i] = (int)rgb.rotation - a[i];
                   // Debug.Log(a[i]);
                }
                float l = a.Min();
                
                int ind = Array.IndexOf(a, l);
                var n = new Quaternion();

                n.Set(0f, 0f, (int)((ind + 1) * 90), 0f);
                transform.rotation = n;      
                rgb.rotation = ((ind+1)*90);

            }
            rgb.simulated = false;
        }
    }

    public void thrown()
    {
        follow = false;
        rgb.simulated = true;
        rgb.velocity = new Vector2((50 * p.GetComponent<first>().di), thr_s);
    }

    private void dis()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            ani.Play("explode");
            GetComponent<BoxCollider2D>().enabled = false;
            rgb.simulated = false;
            StartCoroutine(Camera.main.GetComponent<cam>().Shake());

        }
    }
} 
