using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Linq;

namespace Extras
{
    public class Utilites
    {
        public void Meme()
        {
            var h = File.ReadAllLines("Assets/Data/oh.txt");
            var ab = h[0].Split(',').ToArray();

            List<string[]> xy = new List<string[]>();

            xy.Add(ab);
            Debug.Log(xy[0][0]);
        }
    }
}
