using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using System;

public class scorer : MonoBehaviour
{
    Text txt;
    float score = 0;
    public void Start()
    {
        txt = gameObject.GetComponent<Text>();
    }

    public void UpdateScore(int x)
    {
        score += x;
        txt.text = score.ToString();
    }
}
