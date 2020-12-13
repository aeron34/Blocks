using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    // Start is called before the first frame update
    public void Load()
    {
        int CSI = SceneManager.GetActiveScene().buildIndex;
        if (CSI == 2)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(CSI + 1);
        }
    }
}
