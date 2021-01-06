using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class column : MonoBehaviour
{
    public List<GameObject> blocks;
    public GameObject block, spawn, up_indi;
    
    // Start is called before the first frame update
    void Start()
    {
       // blocks = new List<GameObject>();
        block = Resources.Load<GameObject>("block");
        up_indi = GameObject.Find("up_indi");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator indi()
    {
        up_indi.GetComponent<SpriteRenderer>().enabled = true;
        up_indi.GetComponent<AudioSource>().PlayOneShot(up_indi.GetComponent<AudioSource>().clip);
        yield return new WaitForSeconds(1.25f);
        up_indi.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void drop(string col="")
    {
        var n_b = Instantiate(block, transform.position, transform.rotation);
        n_b.transform.position = new Vector3(transform.position.x, 15.14f, 0);
        n_b.GetComponent<Rigidbody2D>().simulated = true;
        n_b.GetComponent<block>().color = col;
        n_b.GetComponent<block>().colm = gameObject;
        
        up_indi.transform.position = new Vector3(transform.position.x, transform.position.y + 4f,
        transform.position.z);

        StartCoroutine(indi());
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
