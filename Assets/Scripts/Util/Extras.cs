﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Linq;
using TMPro;
using System.Threading.Tasks;

namespace Extras
{
    public class Utilites
    {
        public bool opp_info_init = false;
        //private List<GameObject> opp_infos;
        public string[] LoadTutText(string charc="")
        {
            var textData = File.ReadAllText($"Assets/Data/{charc} Text.txt");
            List<string[]> xy = new List<string[]>();
            return textData.Split('|').ToArray();

            //return xy;
        }
        public List<string[]> Load(int number, string charc="")
        {
            var trialDataText = File.ReadAllLines($"Assets/Data/{charc}{number}.txt");
            List<string[]> xy = new List<string[]>();

            for (int i = 0; i < trialDataText.Length; i++)
            {
                var arr = trialDataText[i].Split(',').ToArray();
                xy.Add(arr);
            }

            return xy;//[0][0]);
        }

        public bool CheckCondition(GameObject this_obj, string[] condition)
        {
            bool satisfied = false;
            switch(condition[0])
            {
                case "COMBO":
                    satisfied = CheckCombo(this_obj, Int32.Parse(condition[1]));
                    break;
                case "SCORE":
                    satisfied = CheckScore(this_obj, Int32.Parse(condition[1]));
                    break;
                default:
                    break;
            }
            return satisfied;
        }

        public bool CheckCombo(GameObject obj, int expected)
        {
            
            int combo = obj.GetComponent<block_queue>().scorer.GetComponent<scorer>().com;
            if (combo >= expected)
            {
                return true;
            }
            return false;
        }
        public bool CheckScore(GameObject obj, int expected)
        {

            int score = obj.GetComponent<block_queue>().scorer.GetComponent<scorer>().GetScore();
            if (score >= expected)
            {
                return true;
            }
            return false;
        }

       
    }
}
