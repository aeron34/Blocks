using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Boxer : MonoBehaviour
{
    // Start is called before the first frame update

    Rigidbody2D rgb;
    public Animator ani;
    private SpriteRenderer spr;
    public float cxS = 0, xS, xY = 35, di = 1,
    pwr_dur = 0, pwr_drn; //pwr_drn variable, the higher it is the faster
    // your pwr up runs out.
    private float n_hel = 1f;
    private bool movable = true;
    public bool ground = false, good_space, top_hit, hurt_b = false;
    public GameObject h_blk = null, v_blk = null;
    public int on = 0;
    public GameObject colm, health, n_colm; //n_colm means whatever colm is after the 
    //colm the player is in, it accounts for whether he's turned around or not.

    // Camera camm;

    private float drag;
    Queue<string> colors;

    void Start()
    {

        di = 1;
        colors = new Queue<string>();
        pwr_drn = (0.01f) * 0.05f;

        rgb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        health = GameObject.Find("health_bar");

        xS = 15;
        xY = 40;
        drag = 0.6f;

    }


    public void none()
    {
        ani.Play("idle");
    }

    private void Movement()
    {
        if (cxS > 0)
        {
            cxS -= drag;
            if (cxS <= 0)
            {
                cxS = 0;
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            cxS = xS;
            di = 1;
            spr.flipX = false;

            //transform.position += m * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            cxS = xS;
            di = -1;
            spr.flipX = true;
        }
    }

    private void Punch()
    {
        if(v_blk != null)
        {
            StartCoroutine(v_blk.GetComponent<block>().slide(di));
        }
    }
    private void UP_Logic()
    {

        Movement();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (h_blk != null)
            {
                h_blk.GetComponent<block>().swap();
            }
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            if (v_blk != null)
            {
                v_blk.GetComponent<block>().swap(1);
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (h_blk != null)
            {
                h_blk.GetComponent<block>().Check();
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (v_blk != null)
            {
                v_blk.GetComponent<block>().Check();
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Punch();
        }

        ComboKeys();

        rgb.velocity = new Vector2(cxS * di, rgb.velocity.y);

        if (ground)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                rgb.velocity = new Vector2(rgb.velocity.x, xY);
                ground = false;
            }
        }
    }

    private void ComboKeys()
    {
        if(Input.GetKey(KeyCode.O) && Input.GetKey(KeyCode.S))
        {
            Debug.Log("DOWN");
            movable = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!hurt_b && movable)
        {
            UP_Logic();
        }
    }

    public IEnumerator LowerHealth(float by = 0)
    {
        n_hel -= (by * 0.01f);

        while (health.transform.localScale.x > n_hel)
        {
            health.transform.localScale = Vector2.Lerp(new Vector2(health.transform.localScale.x, 1),
            new Vector2(n_hel, 1), .1f);
            yield return 0;
        }
    }

    private IEnumerator PowerBar()
    {
        transform.Find("pwr_bar").gameObject.SetActive(true);
        pwr_dur = 1f;
        while (pwr_dur > 0)
        {
            pwr_dur -= pwr_drn;
            transform.Find("pwr_bar").transform.localScale = new Vector3(pwr_dur, 0.25f, 1f);
            yield return 0;
        }

        transform.Find("pwr_bar").gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        
        h_blk = null;
        ground = false;
        good_space = false;

        RaycastHit2D hit = Physics2D.Raycast(transform.GetChild(0).transform.position,
        Vector2.down, 1f, LayerMask.GetMask("blocks"));

        if (hit.collider != null)
        {
            h_blk = hit.collider.gameObject;
        }

        RaycastHit2D hit2 = Physics2D.Raycast(transform.GetChild(0).transform.position,
        Vector2.right * di, 2f, LayerMask.GetMask("blocks"));


        if (hit2.collider != null)
        {
            /*if(hit2.collider.gameObject != v_blk)
            {
                hit2.collider.gameObject.GetComponent<block>().Check(1);
            }*/
            v_blk = hit2.collider.gameObject;
        }
        else
        {
            v_blk = null;
        }

        var np = transform.GetChild(0).transform.position;

        RaycastHit2D gc = Physics2D.Raycast(transform.GetChild(0).transform.position,
        Vector2.down, 1.25f, LayerMask.GetMask("blocks"));

        RaycastHit2D gc2 = Physics2D.Raycast(new Vector2(np.x + .8f, np.y),
        Vector2.down, 1.25f, LayerMask.GetMask("blocks"));

        if (gc.collider != null || gc2.collider != null)
        {
            ground = true;
        }


        RaycastHit2D uc = Physics2D.Raycast(transform.GetChild(0).transform.position,
        Vector2.up, 2.75f, LayerMask.GetMask("blocks"));

        RaycastHit2D uc2 = Physics2D.Raycast(new Vector2(np.x + .65f, np.y),
        Vector2.up, 2.75f, LayerMask.GetMask("blocks"));

        if (uc.collider != null || uc2.collider != null)
        {
            if (ground && !hurt_b)
            {
                StartCoroutine(hurt(20f));
            }
        }

        RaycastHit2D gd_spc = Physics2D.Raycast(transform.GetChild(0).transform.position,
        Vector2.right * di, 3f, LayerMask.GetMask("blocks"));

        if (gd_spc.collider == null)
        {
            good_space = true;
        }


        float least = 100;
        var n = FindObjectOfType<block_queue>();
        
        n_colm = null;

        for (int i = 0; i < n.colms.ToArray().Length; i++)
        {
            if (Math.Abs(n.colms[i].transform.position.x - transform.position.x) < least)
            {
                least = Math.Abs(n.colms[i].transform.position.x - transform.position.x);
                colm = n.colms[i];
                if (i != 0 && i != 18)
                {
                    n_colm = n.colms[(i + (1 * (int)di))];
                }
            }
        }
    }

    public IEnumerator hurt(float by = 0)
    {

        if (!hurt_b)
        {
            hurt_b = true;
            rgb.simulated = false;
            StartCoroutine(LowerHealth(by));
            pwr_dur = 0;
            GetComponent<BoxCollider2D>().enabled = false;
            yield return new WaitForSeconds(2);
            rgb.simulated = true ;
            on = 0;
            rgb.velocity = new Vector2(0, 0);
            GetComponent<BoxCollider2D>().enabled = true;
            transform.position = new Vector3(0, 16f, 0);
            hurt_b = false; // StartCoroutine(LowerHealth(50));
            yield return new WaitForSeconds(3);

        }
    }

    public void call_hurt(float by = 0)
    {
        StartCoroutine(hurt(by));
    }
}