using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class block_queue : MonoBehaviour
{

    public Transform g, block, blk_spn;
    float s = 0.5f, drp_tm;
    bool end_of_list;
    Queue<GameObject> q = new Queue<GameObject>();
    GameObject c_b = null;
    float t_m = 0.05f, bts = 0.05f;
    System.Random random = new System.Random();
    int c_n = 0;
    Queue<string> color = new Queue<string>();
    // Start is called before the first frame update
    void Start()
    {
        var a = Instantiate(g, gameObject.transform);
        q.Enqueue(a.gameObject);
        setColor(a.gameObject);

        c_b = a.gameObject;
        StartCoroutine(move(c_b));

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
                var n_b = Instantiate(block, blk_spn.position, blk_spn.rotation);
                StartCoroutine(kill(q.Dequeue()));
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
        float al = 255;
        var s_r = obj.GetComponent<Image>();
        var rect = obj.GetComponent<RectTransform>();
        while (al > 0)
        {
            al -= 8f;
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
