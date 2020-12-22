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
    private bool chk = false;
    private Animator ani;
    public bool follow;
    public List<GameObject> touching;
    public int combo, ni = 0, die = 0;
    public bool t = false;
    public string color;
    public float thr_s = 5f;
    // Start is called before the first frame update
    void Start()
    {
        touching = new List<GameObject>();
        p = GameObject.Find("pic");
        ani = GetComponent<Animator>();
        rgb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
       

    }

    public void setColor()
    {
        switch (color)
        {
            case "blue":
                spr.color = new Color(0, (float)(92 / 255), (float)(236 / 255));
                break;
            case "green":
                spr.color = new Color(0.0f, 1.0f, 0.05f, 1.0f);
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

        Debug.Log("OBJ: " + gameObject.name + " " +touching.ToArray().Length);
        if(touching.ToArray().Length > 4)
        {
            foreach (GameObject n in touching)
            {
                n.GetComponent<block>().die = 1;
            }
        }
        chk = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (die == 0)
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
        if (die == 1)
        {
            explode();
            die = 2;
        }
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position,
        Vector2.down, 1f, LayerMask.GetMask("blocks"));
        if (hit.collider != null && hit.collider.gameObject.layer == 9)
        {
           /*hit.collider.gameObject.GetComponent<Rigidbody2D>().freezeRotation = true;
            hit.collider.gameObject.GetComponent<Rigidbody2D>().SetRotation(0);
            hit.collider.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX
                | RigidbodyConstraints2D.FreezeRotation;
            rgb.MovePosition(transform.position - transform.right * Time.deltaTime); */

          Debug.Log(hit.collider.gameObject.name);
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
        //Check();
        spr.color = new Color(1, 1, 1);
        //StopAllCoroutines();
        ani.Play("explode");
        GetComponent<BoxCollider2D>().enabled = false;
        rgb.simulated = false;
        StartCoroutine(Camera.main.GetComponent<cam>().Shake());
        Destroy(gameObject, 1f);

    }


    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9
        && collision.gameObject.GetComponent<block>().color == color)
        {
            if (!touching.Contains(collision.gameObject))
            {
                touching.Add(collision.gameObject);Check();
            }
           // touching.Add(GameObject.Find("block (11)");
        }   
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9
        && transform.position.y > collision.gameObject.transform.position.y)
        {
          //  Debug.Log(collision.gameObject.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 9
        && collision.gameObject.GetComponent<block>().color == color)
        {
            touching.Remove(collision.gameObject);
        }
    }
}
