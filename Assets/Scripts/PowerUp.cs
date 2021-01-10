using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    bool up = true;
    Vector3[] pos;
    public string color;
    // Start is called before the first frame update
    void Start()
    {
        pos = new Vector3[2]{new Vector3(transform.position.x, transform.position.y + 2, transform.position.z),
        new Vector3(transform.position.x, transform.position.y - 2, transform.position.z)};

        StartCoroutine(ani());
    }
    //public void 
    private IEnumerator ani()
    {
        yield return new WaitForSeconds(2f);
        up = false;
        yield return new WaitForSeconds(2f);
        up = true;
        StartCoroutine(ani());
    }
    // Update is called once per frame
    void Update()
    {
        if(up)
        {
            transform.position = Vector3.Lerp(transform.position, pos[0], 0.005f);    
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, pos[1], 0.005f);
        }
    }

    private void PowerUps()
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != null) 
        { 
            var g = collision.gameObject;
    
            if(g.layer == 9)
            {
                if(g.transform.position.y > transform.position.y)
                {
                    Destroy(gameObject);
                    return;
                }
            }      
        
            if(g.layer == 12)
            {
                g.GetComponent<Gizmo>().PowerUP(color);
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }
    }
}
