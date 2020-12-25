using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cam : MonoBehaviour
{
    public float duration, magnitude;
    // Start is called before the first frame update
    void Start()
    {
        duration = 0.1f;
        magnitude = 0.3f;
    }

    // Update is called once per frame
    void Update()
    {
        
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
