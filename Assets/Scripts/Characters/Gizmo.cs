using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Gizmo : MonoBehaviour
{
    // Start is called before the first frame update

    Rigidbody2D rgb;
    public Animator ani;
    private SpriteRenderer spr;
    public float cxS = 0, xS, xY = 35, di = 1, 
    pwr_dur = 0, pwr_drn; //pwr_drn variable, the higher it is the faster
    // your pwr up runs out.
    private float n_hel = 1f;
    public bool ground = false, good_space, top_hit, hurt_b = false;
    public GameObject h_blk = null, v_blk = null;
    public GameObject gren, spider;
    public int[] weapons;
    public int on = 0, wep_i=0, com = 0;
    public float c_c = 0;
    public GameObject colm, health, n_colm, dist_blk, block_controller; //n_colm means whatever colm is after the 
    //colm the player is in, it accounts for whether he's turned around or not.

    // Camera camm;

    private float drag;
    Queue<string> colors;
    void Start()
    {
   
        di = 1;
        colors = new Queue<string>();
        pwr_drn = (0.01f) * 0.05f;

        block_controller = FindObjectOfType<block_queue>().gameObject;
        rgb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();

        weapons = new int[3] { 5, 5, 5 };
        health = GameObject.Find("health_bar");
        GameObject.Find("weap_box").transform.Find("1").
        GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1);
        
        xS = 12;
        xY = 30;
        drag = 0.35f;
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

    private void UP_Logic()
    {
        for (int i = 0; i < 3; i++)
        {
            if (weapons[i] > 5)
            {
                weapons[i] = 5;
            }
            GameObject.Find("weap_box").transform.GetChild(i).
                transform.GetChild(0).GetComponent<Text>().text = weapons[i].ToString();
        }
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

        if (Input.GetKeyDown(KeyCode.H) && dist_blk != null
        && good_space && n_colm != null)
        {
            DropBlock();
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

        if (on == 0)
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                wep_i++;
                if (wep_i > 2)
                {
                    wep_i = 0;
                }
                for (int i = 0; i < 3; i++)
                {
                    GameObject.Find("weap_box").transform.GetChild(i).
                    GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, .5f);

                }
                GameObject.Find("weap_box").transform.GetChild(wep_i).
                    GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1);

            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Weapon();
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

    // Update is called once per frame
    void Update()
    {
        if(!hurt_b)
        { 
            UP_Logic();
        }
    }

    private void DropBlock()
    {
        var blk_col = dist_blk.GetComponent<block>().colm;
        dist_blk.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;

        dist_blk.transform.position = new Vector2(n_colm.transform.position.x,
            transform.position.y - .25f);

        transform.position = new Vector2(colm.transform.position.x, transform.position.y);

        blk_col.GetComponent<column>().blocks.Remove(dist_blk);

        n_colm.GetComponent<column>().blocks.Add(dist_blk);
        dist_blk.GetComponent<block>().colm = n_colm;

        dist_blk.GetComponent<block>().v_blk = null;
        dist_blk.GetComponent<block>().h_blk = null;

        dist_blk.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX |
        RigidbodyConstraints2D.FreezeRotation;

        dist_blk = null;

        transform.GetChild(2).gameObject.SetActive(false);
    }

    public void GetDist()
    {
        if (dist_blk != null)
        {
            GameObject pc = dist_blk.GetComponent<block>().colm;
            //pc in this context stands for previous block
            //this is a distance swap operation btw.

            pc.GetComponent<column>().blocks.Remove(dist_blk);
            pc.GetComponent<column>().blocks.Add(v_blk);

            colm = v_blk.GetComponent<block>().colm;

            colm.GetComponent<column>().blocks.Remove(v_blk);
            colm.GetComponent<column>().blocks.Add(dist_blk);

            dist_blk.GetComponent<block>().colm = colm;
            v_blk.GetComponent<block>().colm = pc;

            dist_blk.GetComponent<block>().v_blk = v_blk;
            dist_blk.GetComponent<block>().swap(1);
            dist_blk.GetComponent<block>().v_blk = null;
            dist_blk.GetComponent<block>().h_blk = null;

            dist_blk = null;
            v_blk = null;
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

    private void Weapon()
    {
        if (wep_i == 2 && on == 0)
        {
            on = 1;
            block_controller.GetComponent<block_queue>().enabled = false;
            StartCoroutine(PowerBar());
            weapons[wep_i] -= 1;
        }

        if (on == 0)
        {
            if (weapons[wep_i] != 0)
            {                    
                GameObject g = null;

                switch (wep_i)
                {
                    case 0:
                        g = Instantiate(gren, transform.position, transform.rotation);
                        g.GetComponent<grenade>().di = 1;
                        g.GetComponent<Rigidbody2D>().velocity = new Vector2((32 * di), 10);
                        weapons[wep_i] -= 1;
                        break;

                    case 1:
                        int len = colm.GetComponent<column>().blocks.ToArray().Length;
                        g = Instantiate(spider,
                            colm.GetComponent<column>().blocks[len-1].transform.position, 
                            transform.rotation);
                        weapons[wep_i] -= 1;
                        break;
                }
            }
        }

        return;
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
        on = 0;
        transform.Find("pwr_bar").gameObject.SetActive(false);
        block_controller.GetComponent<block_queue>().enabled = true;

    }
    private void FixedUpdate()
    {
        if (on == 0)
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
                v_blk = hit2.collider.gameObject;
            }
            else
            {
                v_blk = null;
            }
        }
        var np = transform.GetChild(0).transform.position;
      
        RaycastHit2D gc = Physics2D.Raycast(transform.GetChild(0).transform.position,
        Vector2.down, 1.25f, LayerMask.GetMask("blocks", "ground"));

        RaycastHit2D gc2 = Physics2D.Raycast(new Vector2(np.x + .8f, np.y),
        Vector2.down, 1.25f, LayerMask.GetMask("blocks", "ground"));

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
            rgb.simulated = true;
            on = 0;
            rgb.velocity = new Vector2(0, 0);
            GetComponent<BoxCollider2D>().enabled = true;
            transform.position = new Vector3(0, 16f, 0);
            hurt_b = false; 
            yield return new WaitForSeconds(3);
           
        }
    }

    public void call_hurt(float by=0)
    {
        StartCoroutine(hurt(by));
    }
}