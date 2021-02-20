using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using System;

public class scorer : MonoBehaviour
{
    Text txt, chain, combo;
    int score = 0;
    int last_chn = 0;
    public int com;
    public float c_c;

    public void Start()
    {
        txt = gameObject.GetComponent<Text>();
        chain = GameObject.Find("Chain_Cnt").GetComponent<Text>();
        combo = GameObject.Find("Com_Cnt").GetComponent<Text>();
    }

    public void UpdateChain(int x)
    {        
        if(last_chn < x)
        {
            chain.text = x.ToString() + "X CHAIN";

        }
    }

    private void Update()
    {
        if (c_c < 0)
        {
            com = 0;
            last_chn = 0;
            chain.text = "";
            combo.gameObject.SetActive(false);
            chain.gameObject.SetActive(false);
        }

        c_c -= 0.08f;

        if (c_c > 0)
        {
            if (com >= 2)
            {
                combo.gameObject.SetActive(true);
                combo.text = com + "x combo";
            }
            chain.gameObject.SetActive(true);
        }
    }

    public void UpdateScore(int x)
    {
        score += x;
        Debug.Log(score);
        txt.text = score.ToString();
    }
}
