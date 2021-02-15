using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net.Http;
using System.Threading.Tasks;

public class Boxer : MonoBehaviour
{
    // Start is called before the first frame update

    Rigidbody2D rgb;
    public Animator ani;
    private SpriteRenderer spr;
    public List<string> inps = new List<string>();
    public float cxS = 0, xS, xY = 35, di = 1,
    pwr_dur = 0, pwr_drn; //pwr_drn variable, the higher it is the faster
    // your pwr up runs out.
    private float n_hel = 1f;
    private bool movable = true, can_punch;
    public bool ground = false, good_space, top_hit, hurt_b = false;
    public GameObject h_blk = null, v_blk = null;
    public GameObject ult_bar;
    private GameObject health, com_c;
    public int on = 0, com = 0;
    public float c_c = 0;
    public GameObject colm, n_colm; //n_colm means whatever colm is after the 
    //colm the player is in, it accounts for whether he's turned around or not.
    // Camera camm;
    private string[][] Combos;
    private int frame_C = 0;
    private float drag;
    public bool able;
    Queue<string> colors;
    Action[] moves;
    static readonly HttpClient h= new HttpClient();

    void Start()
    {
        di = 1;
        colors = new Queue<string>();
        pwr_drn = (0.01f) * 0.05f;

        rgb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();

        health = GameObject.Find("health_bar");
        ult_bar = GameObject.Find("ult_bar");

        moves = new Action[3] { Straight, ULTIMATE, Upper };//, Go };
        xS = 15;
        xY = 40;
        drag = 1.2f;

        Combos = new string[][]{
            new string[]{"s","d","s","d","u"}, // 
            new string[]{"s", "s", "s", "o"},
            new string[]{"s", "d", "u"}};

        com_c = GameObject.Find("Com_Cnt");

        StartCoroutine(AddBar());
        StartCoroutine(FindObjectOfType<Online>().GetMeteors());
        Get();
    }

 
    public void ComboCounter()
    {
        
        if(c_c < 0)
        {
            com = 0;
            com_c.SetActive(false);
        }
        c_c -= 0.08f;
        if (c_c > 0 && com >= 2)
        {
            com_c.SetActive(true);

            com_c.GetComponent<Text>().text = com + "x combo"; 
        }
    }

    private static async Task Get()
    {
        /*var values = new Dictionary<string, string>
        {
            { "blocks", "2" }
        };
        var cnt = new FormUrlEncodedContent(values);
        var response = await h.PostAsync("http://localhost:3000/", cnt);
        var responseString = await response.Content.ReadAsStringAsync();
        */

        string user = "noasm";
        string a = $"http://localhost:3000/get/{user}";

        var content = await h.GetStringAsync("http://localhost:3000/get/");

        Debug.Log(a);
    }

    private void Straight()
    {
        if(ult_bar.transform.localScale.x < .4f)
        {
            revertBack();
            return;
        }
        var ori = transform.GetChild(0).transform.position;
        
        /*
         * This "distance" var makes it so that you can't just 
         * throw this move out without being close to the actual
         * blocks.
         */

        var distance = Physics2D.Raycast(new Vector2((ori.x + (0.5f*di)), ori.y),
        Vector2.right * di, .5f, LayerMask.GetMask("blocks"));

        List<RaycastHit2D[]> blocks_for_del = new List<RaycastHit2D[]>
        {
            Physics2D.RaycastAll(new Vector2((ori.x + (0.5f * di)), ori.y),
            Vector2.right * di, 5f, LayerMask.GetMask("blocks")),
            Physics2D.RaycastAll(new Vector2((ori.x + (0.5f * di)), ori.y + 1.5f),
            Vector2.right * di, 5f, LayerMask.GetMask("blocks")),
            Physics2D.RaycastAll(new Vector2((ori.x + (0.5f * di)), ori.y + 3.5f),
            Vector2.right * di, 5f, LayerMask.GetMask("blocks"))
        };


        if(distance.collider != null)
        {
            StartCoroutine(LowerBar(30f, ult_bar));
            for(int i = 0; i < blocks_for_del.ToArray().Length; i++)
            {
                for (int a = 0; a < blocks_for_del[i].Length; a++)
                {
                    QuickEXP(blocks_for_del[i][a].collider.gameObject);                    
                }
            }
        }
        else
        {
            revertBack();
            return;
        }
        ani.Play("upper");
        //return 0;
    }

    private void ULTIMATE()
    {
        Camera.main.GetComponent<cam>().zoom = true;
        Camera.main.GetComponent<cam>().Target = transform.position;
        ani.Play("upper");
    }

    private void Upper()
    {
        Debug.Log("Upper");
        if (v_blk != null)
        {
            v_blk.GetComponent<block>().Launch();
        }
        revertBack();
        //return 0;
    }
    public void revertBack()
    {        
        ani.Play("idle");
        movable = true;
        frame_C = 60;
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
        if(Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A))
        {
            cxS = 0;
            return;
        }
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            cxS = xS;
            di = 1;
            spr.flipX = false;
            Combos = new string[][]{
            new string[]{"s","d","s","d","u"}, // 
            new string[]{"s", "s", "s", "o"},
            new string[]{"s", "d", "u"}};
            //transform.position += m * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            cxS = xS;
            di = -1;
            spr.flipX = true;
            Combos = new string[][]{
            new string[]{"s","a","s","a","u"}, // 
            new string[]{"s", "s", "s", "o"},
            new string[]{"s", "a", "u"}};

        }
    }

    private void Punch(int b)
    {
        if (ground)
        {
            if (v_blk != null)
            {
                StartCoroutine(v_blk.GetComponent<block>().slide(di, b));
                ani.Play("punch");
            }
        }
        
    }

    private void process_inp()
    {
        if (Input.inputString != "")
        {
            inps.Add(Input.inputString);
            frame_C = 45;
            return;
        }

        frame_C -= 1;

        if(frame_C == 0)
        {
            inps.Clear();
        }

        string n = string.Join("", inps.ToArray());
        
        if (n == "" || n.Length > 15)
        {
            inps.Clear();
            return;
        }
        
        for (int i = 0; i < moves.Length; i++)
        {
            string a = string.Join("", Combos[i]);
            if (n.Contains(a) && a != "")
            {
                cxS = 0;
                movable = false;
                moves[i]();
                inps.Clear();
                break;
            }
        }
    }
    
    private void QuickEXP(GameObject g)
    {
        FindObjectOfType<scorer>().UpdateScore(50);
        g.GetComponent<block>().colm.GetComponent<column>().casc = false;
        g.GetComponent<block>().explode();
    }

    public void regStrt()
    {
        if(v_blk != null)
        {
            QuickEXP(v_blk);
        }
    }

    public void downStrt()
    {
        ani.Play("d_punch");
        
        transform.position = new Vector2(colm.transform.position.x, 
        transform.position.y);

        Vector2 np = transform.GetChild(0).transform.position;

        var d = Physics2D.RaycastAll(new Vector2(np.x - 0.25f, np.y - 2f),
        Vector2.right * di, .75f, LayerMask.GetMask("blocks"));
        
        for (int i = 0; i < d.Length; i++)
        {
            QuickEXP(d[i].collider.gameObject);
        }
    }

    private IEnumerator AddBar()
    {
        if(ult_bar.transform.localScale.x + 0.01f < 1f)
        {
            ult_bar.transform.localScale = new Vector2(
            ult_bar.transform.localScale.x + 0.01f, 1f);
        }
        else
        {
            ult_bar.transform.localScale = new Vector2(1, 1);
        }
        yield return new WaitForSeconds(.5f);
        StartCoroutine(AddBar());
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

        process_inp();
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


        if (Input.GetKeyDown(KeyCode.K))
        {
            Punch(1);
        }  
        if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)) && Input.GetKeyDown(KeyCode.K))
        {
            Punch(3);
        }

        if (Input.GetKey(KeyCode.S))
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                movable = false;
                downStrt();
                inps.Clear();
                cxS = 0;
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            movable = false;
            regStrt();
            ani.Play("punch");
            cxS = 0;
        }
     }

    // Update is called once per frame
    void Update()
    {
        if (!hurt_b && movable)
        {
            UP_Logic();
            ComboCounter();
        }
    }

    public IEnumerator LowerBar(float by = 0, GameObject g = null)
    {
        if(g == null)
        {
            g = health;
        }

       float n_hel = g.transform.localScale.x - (by * 0.01f);

        //Remember if by < 0 then that means 
        //g(the bar) will go UP, not down.
        //n_hel will be add to a POSITIVE number
        //if by < 0;

        if (by > 0)
        {
            while (g.transform.localScale.x > n_hel + .025f)
            {
                g.transform.localScale = Vector2.Lerp(new Vector2(g.transform.localScale.x, 1),
                new Vector2(n_hel, 1), .1f);

                if (g.transform.localScale.x < 0f)
                {
                    n_hel = 0;
                    break;
                }
                
                yield return 0;
            }
            g.transform.localScale = new Vector2(n_hel, 1);
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
        v_blk = null;
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
        Vector2.down, 1.25f, LayerMask.GetMask("blocks","ground"));

        RaycastHit2D gc2 = Physics2D.Raycast(new Vector2(np.x + .8f, np.y),
        Vector2.down, 1.25f, LayerMask.GetMask("blocks", "ground"));

        if (gc.collider != null || gc2.collider != null)
        {
            ground = true;
            can_punch = true;
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
            StartCoroutine(LowerBar(by));
            pwr_dur = 0;
            GetComponent<BoxCollider2D>().enabled = false;
            yield return new WaitForSeconds(2);
            rgb.simulated = true ;
            on = 0;
            rgb.velocity = new Vector2(0, 0);
            GetComponent<BoxCollider2D>().enabled = true;
            transform.position = new Vector3(0, 16f, 0);
            hurt_b = false; // StartCoroutine(LowerBar(50));
            yield return new WaitForSeconds(3);

        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var g = collision.gameObject;
        if(g.layer == 9 && !ground)
        {
            
            if(Input.GetKey(KeyCode.K) && can_punch && ((transform.position.y - .4f) >
            (g.transform.position.y - 1f)) && g.GetComponent<block>().v_blk == null)
            {
                int f_di = 1;
                if(g.transform.position.x > transform.position.x)
                {
                    f_di = 1;
                }
                else
                {
                    f_di = -1;
                }
                can_punch = false;
                StartCoroutine(collision.gameObject.GetComponent<block>().slide(f_di, 3));                
            }
        }
    }

    public void call_hurt(float by = 0)
    {
        StartCoroutine(hurt(by));
    }
}