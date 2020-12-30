using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class column : MonoBehaviour
{
    public List<GameObject> blocks;
    public GameObject block, spawn;
    bool run = false;
    float t = 0;
    // Start is called before the first frame update
    void Start()
    {
       // blocks = new List<GameObject>();
        block = Resources.Load<GameObject>("block");
  


    }

    // Update is called once per frame
    void Update()
    {
    }

    public void drop(string col="")
    {
        var n_b = Instantiate(block, transform.position, transform.rotation);
       
        n_b.transform.position = new Vector3(transform.position.x, 10.14f, 0);
        n_b.GetComponent<Rigidbody2D>().simulated = true;
        n_b.GetComponent<block>().color = col;
        n_b.GetComponent<block>().colm = gameObject; 
       // blocks.Add(n_b);
    }
    public void Takeoff(GameObject a)
    {
        foreach (GameObject n in blocks)
        {
            if (n != null)
            {
                n.GetComponent<block>().sChk = true;
            }
        }
        blocks.Remove(a);
        //blocks.Clear();
    }
}
