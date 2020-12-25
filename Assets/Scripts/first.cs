using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class first : MonoBehaviour
{
    // Start is called before the first frame update

    Rigidbody2D rgb;
    public float c_l, c_h, off;
    public Animator ani;
    private SpriteRenderer spr;
    public float xS = 0, xY = 15, di;
    public bool jump = false, t_e, c_e;
    GameObject h_blk = null, v_blk = null;
    public GameObject dist_blk;
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

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(h_blk != null)
            {
                h_blk.GetComponent<block>().swap();
            }
        }   
        
        if(Input.GetKeyDown(KeyCode.J))
        {
            if(v_blk != null)
            {
                v_blk.GetComponent<block>().swap(1);
            }
        }        
        
        if(Input.GetKeyDown(KeyCode.K))
        {
            GetDist();
        }

        rgb.velocity = new Vector2(xS*di, rgb.velocity.y);

        if (Input.GetKeyDown(KeyCode.W))
        {
            jump = true;
        }

        if (jump)
        {
            rgb.velocity = new Vector2(rgb.velocity.x, xY);
            jump = false;
        }

    }

    private void GetDist()
    { 
        if(dist_blk != null)
        {
       
            dist_blk.GetComponent<block>().v_blk = v_blk;
            dist_blk.GetComponent<block>().swap(1);
            dist_blk = null;
            return;
        }

        if (v_blk != null && dist_blk == null)
        {
            dist_blk = v_blk;
        }
    }
    private void FixedUpdate()
    {
        h_blk = null;
        v_blk = null;

        RaycastHit2D hit = Physics2D.Raycast(transform.GetChild(0).transform.position,
        Vector2.down, 1f, LayerMask.GetMask("blocks"));

        if(hit.collider != null)
        {
            h_blk = hit.collider.gameObject;
        }       
        
        RaycastHit2D hit2 = Physics2D.Raycast(transform.GetChild(0).transform.position,
        Vector2.right * di, 2f, LayerMask.GetMask("blocks"));

        if(hit2.collider != null)
        {
            v_blk = hit2.collider.gameObject;
        }
    }
}