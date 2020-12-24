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
    private bool chk = false, strt_al = true;
    private Animator ani;
    public bool follow;
    public List<GameObject> touching;
    public List<GameObject> columns;
    public int combo, ni = 0, die = 0;
    public bool t = false;
    public string color;
    public bool locked = false, spcl_grnd, grounded, ran = false;
    float last_pos;
    public float thr_s = 5f;
    private float[] dists = new float[4];
    // Start is called before the first frame update
    void Start()
    {
        touching = new List<GameObject>();
        columns = new List<GameObject>();
        p = GameObject.Find("pic");
        ani = GetComponent<Animator>();
        rgb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
        MyColor();

    }

    public void MyColor()
    {
        switch (color)
        {
            case "blue":
                spr.color = new Color(0.09f, 0.44f, 1f, 1f);
                break;
            case "green":
                spr.color = new Color(0.0f, 1.0f, 0.05f, 1.0f);
                break; 
            case "red":
                spr.color = new Color(1.0f, 0.0f, 0f, 1.0f);
                break;
        }
     }
    public void Check()
    {

        int coll_objs = 1, last_len = 0, i = 0;
        touching.Add(gameObject);
        touching = touching.GroupBy(a => a.gameObject).Select(a => a.First()).ToList();
        
        while (true)
        {
            last_len = touching.ToArray().Length;
            touching.AddRange(touching.ElementAt(i).GetComponent<block>().touching);
            touching = touching.GroupBy(a => a.gameObject).Select(a => a.First()).ToList();
            i++;
            if(i >= touching.ToArray().Length || touching.ToArray().Length > 4)
            {
                break;
            }
           
           // yield return null;
        }

        if(touching.ToArray().Length > 4)
        {
            foreach (GameObject n in touching)
            {
                n.GetComponent<block>().die = 1;
            }
        }
        chk = false;
    }

    public IEnumerator move()
    {
        if(locked)
        {
            StopCoroutine("move");
        }
        if(!ran && !locked)
        {
            ran = true;
            last_pos = transform.position.x;

            yield return new WaitForSeconds(2);

            if ((Math.Round(last_pos, 2)) == (Math.Round(transform.position.x, 2)))
            {
                
                int ind = 0;

                locked = false;
                var i = 0;
                dists = new float[4] { 2, 2, 9, 2 };
                foreach (GameObject n in columns)
                {
                    dists[i] = Math.Abs(Math.Abs(n.transform.position.x) - Math.Abs(transform.position.x));
                    i++;
                }

                float min = dists.Min();
                ind = Array.IndexOf(dists, min);

                if (grounded && columns.ToArray().Length != 0)
                {
                    Lock(ind);
                    locked = true;
                }
            }

            ran = false;
           // spcl_grnd = false;
            StopAllCoroutines();
        }                     
    }
    public void Lock(int ind, GameObject col=null)
    {
        Vector3 pos = new Vector3(0,0,0);
        if (ind >= 0)
        {
            if (ind < columns.ToArray().Length)
            {
                if (!columns.ElementAt(ind).GetComponent<column>().blocks.Contains(gameObject))
                {
                    columns.ElementAt(ind).GetComponent<column>().blocks.Add(gameObject);
                }
                pos = columns.ElementAt(ind).transform.position;
            }
            else
            {
                return;
            }
            
        }
        if(ind < 0)
        {
            if (!col.GetComponent<column>().blocks.Contains(gameObject))
            {
                col.GetComponent<column>().blocks.Add(gameObject);
            }

            pos = col.transform.position;
        }
        rgb.position = new Vector2(pos.x, rgb.position.y);
        rgb.constraints = RigidbodyConstraints2D.FreezeAll;
        rgb.SetRotation(0);
    }
    // Update is called once per frame
    void Update()
    {
       
        StartCoroutine(move());

        if (die == 0)
        {
            if (follow)
            {
                transform.position = new Vector3(
                    p.transform.position.x,
                    p.transform.position.y + 1.5f,
                    0f);

                grounded = false;
                locked = false;
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
                    rgb.SetRotation((ind + 1) * 90);

                }
                rgb.simulated = false;
            }
        }
        if (die == 1)
        {
            explode();
            die = 2;
        }
    }


    public void thrown(int vel)
    {
        follow = false;
        rgb.simulated = true;
        rgb.velocity = new Vector2((vel * p.GetComponent<first>().di), thr_s);
    }

    private void explode()
    {
        //Check();
        spr.color = new Color(1, 1, 1);
        //StopAllCoroutines();
        ani.Play("explode");
        GetComponent<BoxCollider2D>().enabled = false;
        rgb.simulated = false;
        StartCoroutine(Camera.main.GetComponent<cam>().Shake());
        Destroy(gameObject, 1f);

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.layer == 9
          && collision.gameObject.GetComponent<block>().color == color)
        {
            if (!touching.Contains(collision.gameObject))
            {
                touching.Add(collision.gameObject);
                Check();
            }
            // touching.Add(GameObject.Find("block (11)");
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {


        if (collision.gameObject.layer == 13)
        {
            if (!columns.Contains(collision.gameObject))
            {
                columns.Add(collision.gameObject);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        
        if(collision.gameObject.layer == 8 || collision.gameObject.layer == 9)
        {
            grounded = true;
         
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8||collision.gameObject.layer == 9)
        {
            grounded = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 8 ||
        collision.gameObject.layer == 9)
        {
            grounded = false;
        }
        if (collision.gameObject.layer == 9)
        {
            touching.Remove(collision.gameObject);
        }
    }
    /* Make a raycast on the player so when an
     * object is detected too close to him
     * his throw speed will slow down to
     * somethign that won't break the physics.
     */

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            touching.Remove(collision.gameObject);
        }
        if (collision.gameObject.layer == 13)
        {
            columns.Remove(collision.gameObject);
        }
    }
}
