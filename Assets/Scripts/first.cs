using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class first : MonoBehaviour
{
    // Start is called before the first frame update

    Rigidbody2D rgb;
    public Animator ani;
    private SpriteRenderer spr;
    public float xS = 0, xY = 15, di;
    public bool jump = false, t_e, c_e;
    public int pickup, thr_v = 25;
    GameObject block = null;
    Camera camm;
    public float thr_i = 0, sh_dur = 0, sh_mag = 0, drag;
    void Start()
    {
        camm = Camera.main;
        //Instantiate(n, transform.position, transform.rotation);
        //m_c = transform.Find("spawn_pos").gameObject.GetComponent<BoxCollider2D>();
        rgb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        // transform.position = cam.ViewportToWorldPoint(new Vector3(2 / (cam.orthographicSize * 4 * cam.aspect), 0.5f, 1));
        // transform.position = cam.ViewportToWorldPoint(new Vector3(2 / (cam.orthographicSize * 4 * cam.aspect), 0.5f, 1));
    }


    public void none()
    {
        ani.Play("idle");
    }
    // Update is called once per frame
    void Update()
    {
        if (xS > 0)
        {
            xS -= drag;
            if (xS <= 0)
            {
                xS = 0;
            }
        }
        thr_i += 0.05f;

        if (Input.GetKey(KeyCode.D))
        {
            xS = 10;
            di = 1;
            spr.flipX = false;
            //transform.position += m * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            xS = 10;
            di = -1;
            spr.flipX = true;
            //transform.position += m * Time.deltaTime;
        }

        
        rgb.velocity = new Vector2(xS*di, rgb.velocity.y);

        if (Input.GetKeyDown(KeyCode.W))
        {
            jump = true;

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Instantiate(n, t.position, t.rotation);
            if (pickup == 1)
            {
                block.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                block.GetComponent<block>().follow = true;
                block.GetComponent<block>().pickup();
                GetComponent<BoxCollider2D>().offset = new Vector2(-0.0015f, 0.08f);
                GetComponent<BoxCollider2D>().size = new Vector2(0.4f, 0.96f);
                pickup = 2;
                thr_i = 0;
            }
            else if (pickup == 2 && thr_i >= 1.0) 
            {
                GetComponent<BoxCollider2D>().offset = new Vector2(-0.004f, -0.12f);
                GetComponent<BoxCollider2D>().size = new Vector2(0.33f, 0.56f);
                block.GetComponent<block>().thrown(thr_v);
                pickup = 0;
                thr_i = 0;
            }
        }

        if (jump)
        {
            rgb.velocity = new Vector2(rgb.velocity.x, xY);
            jump = false;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        /*if (collision.gameObject.layer == 9)
        {
            c_e = true;
            if (pickup < 2)
            {
                pickup = 1;
                block = collision.gameObject;
            }
        }*/
    }
    
    private void FixedUpdate()
    {
        if(pickup != 2)
                {
            pickup = 0;
            block = null;

        }
        RaycastHit2D hit = Physics2D.Raycast(transform.GetChild(0).transform.position,
            Vector2.right*di, 3f, LayerMask.GetMask("blocks")); 

        if (hit.collider != null)
        {
            Debug.Log(hit.collider.name);
                
            if (pickup < 2)
            {
                pickup = 1;
                block = hit.collider.gameObject;
                //Debug.Log(block.name);
            }
        }

        RaycastHit2D hit2 = Physics2D.Raycast(transform.position,
          Vector2.right * di, 6f, LayerMask.GetMask("blocks"));
        if (hit2.collider != null)
        {
            thr_v = 25/2;
        }
        else
        {
            thr_v =25;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        /*if (collision.gameObject.layer == 9)
        {
            c_e = false;
        }*/
    }
}