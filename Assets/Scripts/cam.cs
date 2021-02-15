using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class cam : MonoBehaviour
{
    public float duration, magnitude;
    public Vector3 Target;
    Vector3 velocity = Vector3.zero;
    Camera camm;
    public bool zoom;
    // Start is called before the first frame update
    void Start()
    {
        camm = Camera.main;
        duration = 0.1f;
        magnitude = 0.3f;
    }

    private void Update()
    {


    }
    // Update is called once per frame
 
    private void FixedUpdate()
    {
        if(zoom)
        {
            Target = GameObject.Find("pic").transform.position;
            //Time.timeScale = Mathf.Lerp(1.0f, 0.25f, 0.8f);
            camm.orthographicSize = Mathf.Lerp(camm.orthographicSize, 3, 0.2f);
            transform.position = Vector3.Lerp(transform.position,
            new Vector3(Target.x, Target.y, -5),
            0.2f);
        }

    }
    public IEnumerator Shake()
    {
        Vector3 orignalPosition = new Vector3(-0.5f, 0, -10);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(x, y, -10f);
            elapsed += Time.deltaTime;
            yield return 0;
        }
        transform.position = orignalPosition;
        duration = 0.1f;
        magnitude = 0.3f;
    }

}
