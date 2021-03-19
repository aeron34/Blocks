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
    Text chain, combo, met_text;
    GameObject met_box;
    int score = 0, mets;
    int last_chn = 0;
    public int com;
    public float c_c;

    public void Start()
    {
        txt = gameObject.GetComponent<TextMeshProUGUI>();
        chain = GameObject.Find("Chain_Cnt").GetComponent<Text>();
        combo = GameObject.Find("Com_Cnt").GetComponent<Text>();
        met_box = GameObject.Find("meteors_box");
        met_box.SetActive(false);
    }

    public void UpdateChain(int x)
    {
        met_box.SetActive(true);
        met_box.transform.GetChild(1).GetComponent<Text>().text = $"{x / 4} METEORS SENT";
        if (last_chn < x)
        {
            chain.text = x.ToString() + "X LINK";
            last_chn = x;
        }
    }
    public void SetScore(int x)
    {
        txt.text = x.ToString();
    }   
    public void Reset(int x)
    {
        score = 0;
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
            met_box.SetActive(false);

        }

        c_c -= 0.08f;

        if (c_c > 0)
        {
            if (com >= 2)
            {
                combo.gameObject.SetActive(true);
                combo.text = com + "X COMBO";
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
