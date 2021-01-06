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
    public float xS = 0, xY = 35, di = 1, hearts = 3;
    public bool ground = false, top_hit, hurt_b = false;
    GameObject h_blk = null, v_blk = null, dist_blk;
    public GameObject gren;
    Camera camm;
    public float thr_i = 0, drag;
    void Start()
    {
        camm = Camera.main; 
        xY = 30;
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
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x),
                transform.localScale.y, transform.localScale.z);
            //transform.position += m * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            xS = 10;
            di = -1;

            transform.localScale = new Vector3(-Math.Abs(transform.localScale.x),
                transform.localScale.y, transform.localScale.z);
            //transform.position += m * Time.deltaTime;
        }

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
            GetDist();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            var g = Instantiate(gren, transform.position, transform.rotation);
            g.GetComponent<grenade>().di = di;

            g.GetComponent<Rigidbody2D>().velocity = new Vector2((32 * di), 10);
        }

        rgb.velocity = new Vector2(xS * di, rgb.velocity.y);

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
            v_blk = hit2.collider.gameObject;
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9)
        {

        }
    }

}