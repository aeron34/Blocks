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
    public List<GameObject> touching;
    public GameObject colm, v_blk, h_blk;
    public int die = 0;
    public float nt = 0;
    public string color;
    public bool grounded;
    // Start is called before the first frame update
    void Start()
    {
        touching = new List<GameObject>();
        p = GameObject.Find("pic");
        ani = GetComponent<Animator>();
        rgb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
        MyColor();
        if (color == "none")
        {
            Destroy(GetComponent<block>());
            gameObject.layer = 8;
            rgb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
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
            case "purple":
                spr.color = new Color(133/255f, 0, 221/255, (1f));
                break;
            case "white":
                spr.color = new Color(255/255f, 255/255f, 255/255f, (1f));
                break;
            case "yellow":
                spr.color = new Color(255f, 255f, 0f, 1f);
                break;
        }
    }
     
    public void Check()
    {
        if(color == "none")
        {
            return;
        }
        int coll_objs = 1, last_len = 0, i = 0;
        touching.Add(gameObject);
        touching = touching.GroupBy(a => a.gameObject).Select(a => a.First()).ToList();
        
        while (true)
        {
            last_len = touching.ToArray().Length;
            touching.AddRange(touching.ElementAt(i).GetComponent<block>().touching);
            touching = touching.GroupBy(a => a.gameObject).Select(a => a.First()).ToList();
            i++;
            if(i >= touching.ToArray().Length || touching.ToArray().Length >= 3)
            {
                break;
            }
           
           // yield return null;
        }

        //Debug.Log(touching.ToArray().Length);

        if(touching.ToArray().Length >= 3)
        {
            foreach (GameObject n in touching)
            {
                n.GetComponent<block>().die = 1;
            }
        }
        //        touching.Clear();

    }

    public void swap(int mode=0)
    {
        Debug.Log("js");
 
        if (mode == 0 && h_blk != null)
        {
            Vector2 a = gameObject.transform.position;
            Vector2 b = h_blk.GetComponent<Rigidbody2D>().position;
            GameObject pc = colm;

            rgb.constraints = RigidbodyConstraints2D.None;
            h_blk.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None; 
                
            transform.position = new Vector3(b.x, b.y, 0);
            h_blk.transform.position = new Vector3(a.x, a.y, 0);
            colm = h_blk.GetComponent<block>().colm;
            h_blk.GetComponent<block>().colm = pc;

            h_blk.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX |
            RigidbodyConstraints2D.FreezeRotation;
        }

        if(mode==1 && v_blk != null)
        {
            Vector2 a = gameObject.transform.position;
            Vector2 b = v_blk.GetComponent<Rigidbody2D>().position;

            rgb.constraints = RigidbodyConstraints2D.None;
            v_blk.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;

            transform.position = new Vector3(b.x, b.y, 0);
            v_blk.transform.position = new Vector3(a.x, a.y, 0);
                
            v_blk.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX |
            RigidbodyConstraints2D.FreezeRotation;
        }

        touching.Clear();
        rgb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        return;        
    }
    private void FixedUpdate()
    {
        // Debug.Log();

        var hit = Physics2D.Raycast(transform.GetChild(0).transform.position,
        Vector2.right * -1, 2f, LayerMask.GetMask("blocks")) ;

        if (hit.collider != null)
        {
            h_blk = hit.collider.gameObject;
        }     
        
        var hit2 = Physics2D.Raycast(transform.GetChild(1).transform.position,
        Vector2.up, 2f, LayerMask.GetMask("blocks"));

        if (hit2.collider != null)
        {
            v_blk = hit2.collider.gameObject;
        }
    }

    public void pickup()
    {
       
        if (colm != null)
        {
             colm.GetComponent<column>().Takeoff(gameObject);
        }

        //columns.Clear();
        touching.Clear();

    }
    /*public void Lock(int ind, GameObject col=null)
    {
        Vector3 pos = new Vector3(0,0,0);
        colm = null;
        if (ind >= 0)
        {
            if (ind < columns.ToArray().Length)
            {
                if (!columns.ElementAt(ind).GetComponent<column>().blocks.Contains(gameObject))
                {
                    columns.ElementAt(ind).GetComponent<column>().blocks.Add(gameObject);
                }
                pos = columns.ElementAt(ind).transform.position;
                colm = columns.ElementAt(ind).gameObject;
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
            colm = col;
            pos = col.transform.position;
        }
        rgb.position = new Vector2(pos.x, rgb.position.y);
        rgb.constraints =  RigidbodyConstraints2D.FreezeRotation;
        rgb.SetRotation(0);
    }*/
    // Update is called once per frame
    void Update()
    {

        //StartCoroutine(move());

        if (die == 0)
        {
            if (follow)
            {
                transform.position = new Vector3(
                    p.transform.position.x,
                    p.transform.position.y + 1.5f,
                    0f);

                grounded = false;
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


    private void explode()
    {
        //Check();
        //pickup();
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
                touching.Add(collision.gameObject);
                Check();
            }
            // touching.Add(GameObject.Find("block (11)");
        }   
    }

    private void OnCollisionStay2D(Collision2D collision)
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

        if (collision.gameObject.layer == 8 || collision.gameObject.layer == 9)
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
            touching.Clear();
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

            touching.Clear();
        }
    }
}
