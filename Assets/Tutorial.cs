using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public Sprite[] sprs = new Sprite[10];
    int current_image = 0;
    SpriteRenderer spr_renderer;
    public TextMeshProUGUI tut_text;
    public string character = "Gizmo";
    // Start is called before the first frame update
    void Start()
    {
        spr_renderer = GetComponent<SpriteRenderer>();
        tut_text = GameObject.Find("tutorial").GetComponent<TextMeshProUGUI>();
        tut_text.enabled = false;
        if(character == "Boxer")
        {
            GameObject.FindObjectOfType<block_queue>().max_trials = 6;
            spr_renderer.sprite = sprs[9];
            for (int i = 1; i < 10; i++)
            {
                sprs[i] = null;
            }
            GameObject.Find("Keys").GetComponent<Text>().text = "";

        }
        else
        {
            GameObject.Find("Keys").SetActive(false);
            GameObject.FindObjectOfType<block_queue>().max_trials = 7;
            Destroy(GameObject.Find("next"));
        }
    }

    public void SetText(string text)
    {
        GameObject.Find("tutorial").GetComponent<TextMeshProUGUI>().text = text;
    }

    public void NextImage()
    {
        current_image++;
        spr_renderer.sprite = sprs[current_image];
        spr_renderer.enabled = true;
        if (sprs[current_image] != null)
        {
            tut_text.enabled = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            spr_renderer.enabled = false;
            tut_text.enabled = true;
        }

        if(GameObject.FindObjectOfType<Boxer>() != null)
        {
            var box = GameObject.FindObjectOfType<Boxer>();
            GameObject.Find("Keys").GetComponent<Text>().text = string.Join("", box.inps.ToArray());
        }
    }
    
}
