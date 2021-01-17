using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class block : MonoBehaviour
{
    [SerializeField] private GameObject pwr_up;
    private SpriteRenderer spr;
    private Rigidbody2D rgb;
    private Animator ani;
    public bool follow, runable=true;
    public List<GameObject> touching;
    public GameObject colm, v_blk, h_blk;
    public int die = 0;
    public float nt = 0;
    public string color;
    private bool ex;
    public bool grounded, sChk, dud;
    // Start is called before the first frame update
    void Start()
    {
        runable = true;
        touching = new List<GameObject>();
        //p = GameObject.Find("pic");
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
        colm.GetComponent<column>().blocks.Add(gameObject);
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
     
    public int Check()
    {
      
        if(color == "none")
        {
            return -1;
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
            if(i >= touching.ToArray().Length)
            {
                break;
            }
           
           // yield return null;
        }

        /*
         Fix the Check function with this:

            o Keep the checking routine the exact same but 
            use raycasts instead

         */


        if(touching.ToArray().Length >= 3)
        {
            foreach (GameObject n in touching)
            {
                n.GetComponent<block>().die = 1;
            }
            if (touching.ToArray().Length >= 4)
            {
                FindObjectOfType<scorer>().UpdateScore((int)Math.Pow(touching.ToArray().Length * .75, 5));
                return 1;
            }
            
            FindObjectOfType<scorer>().UpdateScore(touching.ToArray().Length * 50);
            
            return 1;
        }

        return 0;
    }

    public IEnumerator flash()
    {
            
        float b = 1, by = 0.01f;
        runable = false;
        while (b > 0)
        {
            b -= by;
            if(b < 0)
            {
                b = 0f;
            }
            spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, b);
            yield return null;
        }
        while (b < 1)
        {
            b += by;
            if (b > 1)
            {
                b = 1f;
            }
            spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, b);
            yield return 0;
        }

        spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 1f);
        yield return new WaitForSeconds(.25f);
        runable = true;
    }

    public void swap(int mode=0)
    {
        
        if (mode == 0 && h_blk != null)
        {
            Vector2 a = gameObject.transform.position;
            Vector2 b = h_blk.GetComponent<Rigidbody2D>().position;
            GameObject pc = colm;

            rgb.constraints = RigidbodyConstraints2D.None;
            h_blk.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None; 
                
            transform.position = new Vector3(b.x, b.y, 0);
            h_blk.transform.position = new Vector3(a.x, a.y, 0);
            
            pc.GetComponent<column>().blocks.Remove(gameObject);
            pc.GetComponent<column>().blocks.Add(h_blk);
            
            colm = h_blk.GetComponent<block>().colm;

            colm.GetComponent<column>().blocks.Remove(h_blk);
            colm.GetComponent<column>().blocks.Add(gameObject);
            
            h_blk.GetComponent<block>().colm = pc;

            h_blk.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX |
            RigidbodyConstraints2D.FreezeRotation;

            h_blk.GetComponent<block>().v_blk = null;
            h_blk.GetComponent<block>().h_blk = null;
 
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


            try
            {
                v_blk.GetComponent<block>().v_blk = null;
                v_blk.GetComponent<block>().h_blk = null;
            }catch(NullReferenceException e)
            {

            }
          
        }
         
        v_blk = null;
        h_blk = null;

        touching.Clear();
        rgb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        return;        
    }
    private void FixedUpdate()
    {
        // Debug.Log();
        touching.Clear();

        var hit = Physics2D.Raycast(transform.GetChild(0).transform.position,
        Vector2.right * -1, 2f, LayerMask.GetMask("blocks"));

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

        //DIRECTIONAL RAYCASTS
        RaycastHit2D[] rays = { Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1.025f),
            Vector2.up, 1f, LayerMask.GetMask("blocks")),
            Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 1.025f),
            Vector2.down, 1f, LayerMask.GetMask("blocks")), 
            Physics2D.Raycast(new Vector2(transform.position.x - 1.025f, transform.position.y),
            Vector2.left, 1f, LayerMask.GetMask("blocks")),
            Physics2D.Raycast(new Vector2(transform.position.x + 1.025f, transform.position.y),
            Vector2.right, 1f, LayerMask.GetMask("blocks"))
        };

        for (int i = 0; i < 4; i++)
        {
            if (rays[i].collider != null)
            {
                if (!touching.Contains(rays[i].collider.gameObject)
                    && rays[i].collider.gameObject.GetComponent<block>().color == color)
                {
                    touching.Add(rays[i].collider.gameObject);
                }
            }
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

    // Update is called once per frame
    void Update()
    {
        //StartCoroutine(move());


        if (die == 0)
        {
            /*if (follow)
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
            }*/
            if (sChk)
            {
                if (nt <= 120.0)
                {
                    nt += 2f;
                }
                if(nt % 10 == 0)
                {
                    Check();
                }
                if (sChk && nt > 120)
                {
                    sChk = false;
                    nt = 0;
                }
            }

        }
        if (die == 1 && !ex)
        {
            explode();
            ex = true;
            die = 2;
        }
    }


    public void explode()
    {
        //Check();
        //pickup();
        if (die < 2)
        {

            if (FindObjectOfType<Gizmo>().dist_blk == gameObject)
            {
                FindObjectOfType<Gizmo>().dist_blk = null;
                FindObjectOfType<Gizmo>().transform.GetChild(2).gameObject.SetActive(false);
            }

            if (dud)
            {
                FindObjectOfType<scorer>().UpdateScore(-100);
            }
            else
            {
                System.Random random = new System.Random();
                GetComponent<BoxCollider2D>().enabled = false;
                int rn = random.Next(1, 14);


                if (rn == 2)
                {
                    var g = Instantiate(pwr_up, transform.position, transform.rotation);
                    g.GetComponent<SpriteRenderer>().color = spr.color;
                    g.GetComponent<PowerUp>().color = color;
                }
            }
            
            

            foreach (GameObject n in touching)
            {
                n.GetComponent<block>().die = 1;
            }
            colm.GetComponent<column>().Takeoff(gameObject);

            spr.color = new Color(1, 1, 1);
            //StopAllCoroutines();
            ani.Play("explode");
            GetComponent<BoxCollider2D>().enabled = false;
            rgb.simulated = false;
            StartCoroutine(Camera.main.GetComponent<cam>().Shake());
            Destroy(gameObject, 1f);
        }

    }

    /* Make a raycast on the player so when an
     * object is detected too close to him
     * his throw speed will slow down to
     * somethign that won't break the physics.
     */
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9
         && collision.gameObject.GetComponent<block>().color == color)
        {
            if (!touching.Contains(collision.gameObject))
            {
                touching.Add(collision.gameObject);
                if (sChk)
                {
                    Check();
                }
            }
        }
    }

    private void OnMouseEnter()
    {
        spr.color = new Color(135f,0,150f, 1);
    }

    private void OnMouseOver()
    {
        var ply = GameObject.Find("pic");
        
        if (Input.GetMouseButtonDown(0) && ply.GetComponent<Gizmo>().on == 2
        && ply.GetComponent<Gizmo>().v_blk != null)
        {
            Debug.Log("Ogh");
            ply.GetComponent<Gizmo>().dist_blk = gameObject;
            ply.GetComponent<Gizmo>().on = 1;
            ply.GetComponent<Gizmo>().GetDist();
            return;
        }

        if (Input.GetMouseButtonDown(0) && ply.GetComponent<Gizmo>().on == 1)
        {
            ply.GetComponent<Gizmo>().on = 2;
            ply.GetComponent<Gizmo>().v_blk = gameObject;
            return;
        }



    }

    private void OnMouseExit()
    {
        MyColor();
    }
}
