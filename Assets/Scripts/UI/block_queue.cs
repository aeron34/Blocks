using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using Extras;

public class block_queue : MonoBehaviour
{

    public Transform g, blk_spn;
    float s = 0.5f, drp_tm;
    double time_passed = 0;
    bool moving;
    public float default_col_X = -20.3f;
    Queue<GameObject> blk_queue = new Queue<GameObject>();
    public GameObject colm, null_block, block, meteor;
    GameObject c_b = null;
    float t_m = 0.05f, bts = 0.055f;
    System.Random random = new System.Random();
    public int meteors = 0, default_col_number = 19;
    float[] minutes_passed = { 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f };
    float[] column_positions = new float[19];
    Queue<string> color = new Queue<string>();
    public List<GameObject> colms = new List<GameObject>();
    private Extras.Utilites util;
    public string Game_Mode = "Game", block_arrangment;
    
    // Start is called before the first frame update
    void Start()
    {
        util = new Extras.Utilites();
        util.Meme();


        column_positions[0] = default_col_X;

        for (int i = 1; i < default_col_number; i++)
        {
            double n = (column_positions[i - 1] + 2.2);
            column_positions[i] = (float)(Math.Round(n, 2));
        }

        for (int i = 0; i < default_col_number; i++)
        {
            var new_column = Instantiate(colm,
            new Vector3((column_positions[i]), 5.14f, 0), colm.transform.rotation);
            colms.Add(new_column);
        }

        if (Game_Mode == "Game")
        {
            for (int i = 0; i < 10; i++)
            {
                minutes_passed[i] *= 60f;
            }

            var first_block_hud = Instantiate(g, gameObject.transform);
            blk_queue.Enqueue(first_block_hud.gameObject);
            setColor(first_block_hud.gameObject);

            c_b = first_block_hud.gameObject;
            StartCoroutine(move(c_b));


            float st_y = -11;

            string[] colors = { "green", "blue", "red", "yellow" };

            int col_i = -1;


            int[,] orien = new int[2, 4] { { 0, 1, 2, 3 }, { 2, 3, 0, 1 } };
            int oi = 0;
            for (int i = 0; i < 7; i++)
            {

                col_i++;
                for (int b = 0; b < default_col_number; b++)
                {
                    if (col_i > 3)
                    {
                        oi++;
                        col_i = 0;
                    }
                    if (oi >= 2)
                    {
                        oi = 0;
                    }
                    if (i < 2)
                    {
                        //These are the null gray blocks
                        var blk = Instantiate(null_block, blk_spn.position, blk_spn.rotation);
                        blk.transform.position = new Vector3(column_positions[b], st_y, 0);
                    }
                    else
                    {
                        //These are the actual blocks
                        var blk = Instantiate(block, blk_spn.position, blk_spn.rotation);
                        blk.transform.position = new Vector3(column_positions[b], st_y, 0);
                        blk.GetComponent<block>().color = colors[orien[oi, col_i]];
                        blk.GetComponent<block>().colm = colms[b];
                        //colms[b].GetComponent<column>().
                        col_i++;
                    }
                }

                st_y += 2;
            }
        }

       // StartCoroutine(MeteorTime());
    }

    public IEnumerator MeteorTime()
    {
        Debug.Log("BQ: " + meteors);

        while (meteors > 0) 
        {
            meteors -= 1;
            DropMeteor();
            yield return new WaitForSeconds(1f);
        }

        Debug.Log("BQ: " + meteors);
        /*DropMeteor();
        yield return new WaitForSeconds(1f);
        StartCoroutine(MeteorTime());*/
    }
    public void setColor(GameObject a)
    {
        var color_number = random.Next(4) + 1;

        switch (color_number)
        {
            case 1:
                color.Enqueue("blue");
                a.GetComponent<Image>().color = new Color(0, 65f, 241f, .4f);
                break;
            case 2:
                color.Enqueue("green");
                a.GetComponent<Image>().color = new Color(52f, 241f, 0, .4f);
                break;
            case 3:
                color.Enqueue("red");
                a.GetComponent<Image>().color = new Color(255f, 0, 0, (.4f));
                break;
            case 4:
                color.Enqueue("yellow");
                a.GetComponent<Image>().color = new Color(255f, 255f, 0f, (.4f));
                break;
        }
        //"purple", "white", "yellow"
    }
    // Update is called once per frame
    void Update()
    {
        if (Game_Mode == "Game")
        {
            drop();

            time_passed += Time.deltaTime;
            time_passed = Math.Round(time_passed, 2);
            for (int i = 0; i < 10; i++)
            {

                if (time_passed == minutes_passed[i])
                {
                    bts += 0.025f;
                }
            }
        }
    }


    private void DropMeteor()
    {
        //var ply = GameObject.Find("pic");

        System.Random r = new System.Random();
        int rn = r.Next(0, colms.ToArray().Length);
        StartCoroutine(colms[rn].GetComponent<column>().Meteor(meteor));
    }

    public void drop()
    {

        if(true)
        { 
            if(drp_tm < 1)
            {
                drp_tm += bts*.025f;
            }else{

                /*
                    This is the code that manages a block drop.
                    
                    1st, there's a var called drp_tm, which stands
                    for drop timer. It increments by (bts*.25f) (see above)
                    and bts is a variable that ranges from 0 - 1, if it's
                    1 then the drop timer will increment extremely fast, 
                    if it's almost zero then it will be slow. Back to drp_tm,
                    we set it back to zero because we're about to drop a block,
                    basically we reset it.
                
                    2nd, we create a random number from 0 - 19, because there's
                    19 columns. the int 'ax' is what we assigned the result of the RNG to.

                    3rd, We start a Coroutine called kill, it takes the block that we
                    drop from the UI.

                    4th we call the drop function of the column object which creates
                    an entirely new block, and we pass the same color of the UI block 
                    we just called 'kill' on.
                    
                    
                 */
                drp_tm = 0;
                System.Random r = new System.Random();
                int ax = r.Next(0, default_col_number);
                StartCoroutine(kill(blk_queue.Dequeue()));
                colms.ElementAt(ax).GetComponent<column>().drop(color.Peek());
                color.Dequeue();
                 UP_List();
                c_b = blk_queue.Peek();
               
            }
            var bar = c_b.transform.Find("bar").gameObject;

            bar.GetComponent<RectTransform>().localScale = new Vector2(drp_tm, .1f);
        }
    }

    public void UP_List()
    {  
        var a = Instantiate(g, gameObject.transform);
        blk_queue.Enqueue(a.gameObject);
        setColor(a.gameObject);
        var qn = blk_queue.ToArray();
        for (int i = 0; i < qn.Length; i++)
        {
            StartCoroutine(move(qn[i], 1));
        }
      
    }

    /* Create a list of how many blocks are currently 
     in the column. Reason being that when one is 
    remove (picked up by player) it will update and 
    turn on the Y axis of the blocks above the taken
    out block.*/

    public IEnumerator move(GameObject obj, float b_d=1)
    {
        
        var rect = obj.GetComponent<RectTransform>();
        var x = rect.anchoredPosition.x;
        var a_x = Math.Abs(rect.anchoredPosition.x);
        float dist = a_x + (-1280 * b_d);
        float act_dist = -1280;

        while (rect.anchoredPosition.x > act_dist + 1)
        {
            rect.anchoredPosition = Vector3.Lerp(rect.anchoredPosition,
            new Vector3(-1280f, -50f, obj.transform.position.z),
            0.1f);

            yield return 0;
           // break;
        }
    }
    public IEnumerator kill(GameObject obj)
    {
        float al = 119;
        var s_r = obj.GetComponent<Image>();
        var rect = obj.GetComponent<RectTransform>();
        obj.transform.Find("bar").gameObject.SetActive(false);
        while (al > 0)
        {
            al -= 3f;
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - 3);
            Color c = obj.GetComponent<Image>().color;
            
            obj.GetComponent<Image>().color = new Color(c.r, c.g, c.b, al/255);
            yield return 0;
        }
        Destroy(obj, .1f);  
    }

}
