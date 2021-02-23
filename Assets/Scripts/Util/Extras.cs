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
        public List<string[]> Meme()
        {
            var trialDataText = File.ReadAllLines("Assets/Data/oh.txt");
            List<string[]> xy = new List<string[]>();

            for (int i = 0; i < trialDataText.Length; i++)
            {
                var arr = trialDataText[i].Split(',').ToArray();
                xy.Add(arr);
            }

            return xy;//[0][0]);
        }
    }
}
