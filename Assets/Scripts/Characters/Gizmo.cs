using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Gizmo : MonoBehaviour
{
    // Start is called before the first frame update

    Rigidbody2D rgb;
    public Animator ani;
    private SpriteRenderer spr;
    public float cxS = 0, xS, xY = 35, di = 1, hearts = 3, pwr_dur = 0,
        pwr_drn; //pwr_drn variable, the higher it is the faster
    // your pwr up runs out.
    public bool ground = false, top_hit, hurt_b = false;
    GameObject h_blk = null, v_blk = null, dist_blk;
    public GameObject gren;
    Camera camm;
    private float thr_i = 0, drag;
    Queue<string> colors;
    void Start()
    {
        camm = Camera.main;
        Default();
        colors = new Queue<string>();
        pwr_drn = (0.01f)*0.1f;
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

    // Update is called once per frame
    void Update()
    {
        
        Movement();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (h_blk != null)
            {
                h_blk.GetComponent<block>().swap();
            }
        }

        if (Input.GetKeyDown(KeyCode.U) && !(Input.GetKey(KeyCode.M)))
        {
            if(colors.ToArray().Length >= 1)
            {
                PowerUP(colors.Dequeue(), 1);
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
                if (h_blk.GetComponent<block>().Check() == 1)
                {

                    dist_blk = null;
                    transform.GetChild(2).gameObject.SetActive(false);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (v_blk != null)
            {
                if (v_blk.GetComponent<block>().Check() == 1)
                {

                    dist_blk = null;
                    transform.GetChild(2).gameObject.SetActive(false);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            GetDist();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            var g = Instantiate(gren, transform.position, transform.rotation);
            g.GetComponent<grenade>().di = di;

            g.GetComponent<Rigidbody2D>().velocity = new Vector2((32 * di), 10);
        }

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

    private void GetDist()
    {
        if (dist_blk != null)
        {

            dist_blk.GetComponent<block>().v_blk = v_blk;
            dist_blk.GetComponent<block>().swap(1);
            dist_blk = null;
            transform.GetChild(2).gameObject.SetActive(false);
            return;
        }

        if (v_blk != null && dist_blk == null)
        {
            dist_blk = v_blk;
            transform.GetChild(2).gameObject.SetActive(true);
            transform.GetChild(2).GetComponent<SpriteRenderer>().color = dist_blk.GetComponent<SpriteRenderer>().color;
        }
    }

    private Dictionary<string, float> Default(int mode = 0)
    {
        if(mode == 1)
        {
            return new Dictionary<string, float>() {
                {"xS", 10},
                {"xY", 30},
                {"drag", 0.35f}
            };
        }


        xS = 10;
        xY = 30;
        drag = 0.35f;
        return new Dictionary<string, float>() { };
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
        Default();
    }

    public void PowerUP(string color, int mode=0)
    {
        if(Input.GetKey(KeyCode.M))
        {
            colors.Enqueue(color);
            return;
        }

        switch (color)
        {
            case "blue":
                var dict = Default(1);
                xS = dict["xS"] * 2;
                drag = dict["drag"] * 2;
                break;
            case "green":
                var d = Default(1);
                xY = d["xY"] * 1.5f;
                break;
        }
        StartCoroutine(PowerBar());
    }

    private void FixedUpdate()
    {
        h_blk = null;
        ground = false;
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

        RaycastHit2D gc2 = Physics2D.Raycast(new Vector2(np.x + .5f, np.y),
        Vector2.down, 1.25f, LayerMask.GetMask("blocks"));

        if (gc.collider != null || gc2.collider != null)
        {
            ground = true;
        }      
        
        RaycastHit2D uc = Physics2D.Raycast(transform.GetChild(0).transform.position,
        Vector2.up, 2.75f, LayerMask.GetMask("blocks"));

        RaycastHit2D uc2 = Physics2D.Raycast(new Vector2(np.x + .5f, np.y),
        Vector2.up, 2.75f, LayerMask.GetMask("blocks"));

        if (uc.collider != null || uc2.collider != null)
        {
            if (ground && !hurt_b)
            {
                StartCoroutine(hurt());
            }
        }       
        
  
    }

    public IEnumerator hurt()
    {
        if (!hurt_b)
        {
            hurt_b = true;
            rgb.simulated = false;
            GetComponent<BoxCollider2D>().enabled = false;
            yield return new WaitForSeconds(2);
            rgb.simulated = true;
            rgb.velocity = new Vector2(0, 0);
            GetComponent<BoxCollider2D>().enabled = true;
            transform.position = new Vector3(0, 16f, 0);
            yield return new WaitForSeconds(3);
            hurt_b = false;
        }
    }
}