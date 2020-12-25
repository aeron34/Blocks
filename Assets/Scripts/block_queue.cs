using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class block_queue : MonoBehaviour
{

    public Transform g, blk_spn ;
    float s = 0.5f, drp_tm;
    bool end_of_list;
    Queue<GameObject> q = new Queue<GameObject>();
    public GameObject colm, n_block,block;
    GameObject c_b = null;
    float t_m = 0.05f, bts = 0.05f;
    System.Random random = new System.Random();
    int c_n = 0;

    float[] pos = new float[19];
    Queue<string> color = new Queue<string>();
    List<GameObject> colms = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        var a = Instantiate(g, gameObject.transform);
        q.Enqueue(a.gameObject);
        setColor(a.gameObject);

        c_b = a.gameObject;
        StartCoroutine(move(c_b));
        pos[0] = -20.3f;

        for(int i = 1; i < 19; i++)
        {
            double n = (pos[i - 1] + 2.2);
            pos[i] = (float)(Math.Round(n, 2));
        }

        for(int i = 0; i < 19; i++)
        {
            var blk = Instantiate(colm, 
            new Vector3((pos[i]),5.14f,0), blk_spn.rotation);
            //blk.GetComponent<column>().block = block;
            colms.Add(blk);
        }

        float st_y = -9;
        
        string[] cols = { "green", "blue", "red" };
        int col_i = 0;

        for(int i = 0; i < 4; i++)
        {
            for (int b = 0; b < 19; b++)
            {
                if (col_i > 2)
                {
                    col_i = 0;
                }
                if (i < 2)
                {
                    var blk = Instantiate(n_block, blk_spn.position, blk_spn.rotation);
                    blk.transform.position = new Vector3(pos[b], st_y, 0);
                    col_i++;
                }
                else
                {
                    var blk = Instantiate(block, blk_spn.position, blk_spn.rotation);
                    blk.transform.position = new Vector3(pos[b], st_y, 0);
                    blk.GetComponent<block>().color = cols[col_i];
                    blk.GetComponent<block>().colm = colms[b];
                    col_i++;
                }
            } 

            st_y += 2;
        }
        //rect.anchoredPosition = new Vector2(-41f);

    }

    public void setColor(GameObject a)
    {
        c_n = random.Next(3) + 1;

        switch (c_n)
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
        }

    }
    // Update is called once per frame
    void Update()
    {
        drop();

        if (!end_of_list)
        {
            if (t_m <= 20)
            {
                t_m += 0.065f;
            }
            else
            {
                t_m = 0.05f;
                UP_List();
            }
        }
        else
        {

        }
    }

    public void drop()
    {
        var bar = c_b.transform.Find("bar").gameObject;

        if(true)
        { 
            if(drp_tm < 1)
            {
                drp_tm += bts*.025f;
            }else
            {
                drp_tm = 0;
                System.Random r = new System.Random();
                int ax = r.Next(0, 19);
                StartCoroutine(kill(q.Dequeue()));
                colms.ElementAt(ax).GetComponent<column>().drop(color.Peek());
                color.Dequeue();
                c_b = q.Peek();
            }

           bar.GetComponent<RectTransform>().localScale = new Vector2(drp_tm, .1f);
        }
    }

    public void UP_List()
    {  
        var a = Instantiate(g, gameObject.transform);
        q.Enqueue(a.gameObject);
        setColor(a.gameObject);
        var qn = q.ToArray();
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
        float dist = a_x + (100f * b_d);
        float act_dist = x - (100f * b_d);

        while (x > act_dist)
        {
           
            float sx = Math.Abs(((dist - a_x) * 0.05f));
            a_x += sx;
            x -= sx;
            if (x <= act_dist + 2)
            {
                x = act_dist;
                a_x = dist;
                rect.anchoredPosition = new Vector2(x, -50f);
                
                if(act_dist == -2350)
                {
                    end_of_list = true;
                    bts = 0.15f;
                }
                yield return 0;
                break;
            }
            rect.anchoredPosition = new Vector2(x, -50f);
            x = rect.anchoredPosition.x;
            yield return 0;
           // break;
        }
    }
    public IEnumerator kill(GameObject obj)
    {
        float al = 119;
        var s_r = obj.GetComponent<Image>();
        var rect = obj.GetComponent<RectTransform>();
        while (al > 0)
        {
            al -= 3f;
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - 3);
            Color c = obj.GetComponent<Image>().color;
            
            obj.GetComponent<Image>().color = new Color(c.r, c.g, c.b, al/255);
            yield return 0;
        }
        if(end_of_list)
        {
            UP_List();
        }
        Destroy(obj);  
    }

}
