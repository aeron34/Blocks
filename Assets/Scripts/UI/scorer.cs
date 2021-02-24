using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class scorer : MonoBehaviour
{
    TextMeshProUGUI txt;
    Text chain, combo;
    int score = 0;
    int last_chn = 0;
    public int com;
    public float c_c;

    public void Start()
    {
        txt = gameObject.GetComponent<TextMeshProUGUI>();
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
    public void SetScore(int x)
    {
        txt.text = x.ToString();
    }   
    public void Reset(int x)
    {
        txt.text = x.ToString();
        com = 0;
        combo.text = com.ToString();
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

        c_c -= 0.12f;

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

    public int GetScore()
    {
        return score;
    }
    public void UpdateScore(int x)
    {
        score += x;

        txt.text = score.ToString();
    }
}
