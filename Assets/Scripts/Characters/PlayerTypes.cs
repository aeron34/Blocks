using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Giz
{
    public class Gizmon
    {
        bool con_active = false;
        int con_X = 0, con_Y = 0, max_dist_X = 3, moved=0;
        GameObject c_colm; //this stands for cursor column 
        public Gizmon()
        {
            
        }

        public void Update()
        {
            
        }
        private int FindColmIndex(GameObject colm)
        {
            var blq = GameObject.FindObjectOfType<block_queue>();
            int indx = blq.colms.FindIndex(a => a == colm);
            c_colm = blq.colms[indx];
            return indx;
        }
        private void MoveHorizontal(GameObject crs, GameObject colm, int mx)
        {
            try
            {
                if (GameObject.FindObjectOfType<block_queue>().colms[con_X + mx] != null)
                {
                    var c = GameObject.FindObjectOfType<block_queue>().colms[con_X + mx];

                    if (c.GetComponent<column>().blocks.ToArray().Length > 0)
                    {
                      
                        if (c.GetComponent<column>().blocks.ToArray().Length >=
                        c_colm.GetComponent<column>().blocks.ToArray().Length)
                        {
                            crs.transform.position =
                                c.GetComponent<column>().blocks[con_Y].transform.position;

                            c_colm = c;
                            con_X += mx;
                            moved += mx;
                            return;
                        }

                        if (mx == 1)
                        {
                            if (c_colm.GetComponent<column>().blocks.ToArray().Length <
                            c.GetComponent<column>().blocks.ToArray().Length)
                            {
                                con_Y = c.GetComponent<column>().blocks.ToArray().Length - 1;
                                Debug.Log("Less: " + con_Y);
                                crs.transform.position =
                                    c.GetComponent<column>().blocks[con_Y].transform.position;

                                c_colm = c;
                                con_X += mx;
                                moved += mx;
                            }
                        }

                        if(mx == -1)
                        {
                            if (c.GetComponent<column>().blocks.ToArray().Length <
                            c_colm.GetComponent<column>().blocks.ToArray().Length)
                            {
                                con_Y = c.GetComponent<column>().blocks.ToArray().Length - 1;
                                Debug.Log("Less: " + con_Y);
                                crs.transform.position =
                                    c.GetComponent<column>().blocks[con_Y].transform.position;

                                c_colm = c;
                                con_X += mx;
                                moved += mx;
                            }
                        }
                    }
                }
            }
            catch (NullReferenceException e)
            {
                Debug.Log("nope");
                return;
            }

            return;
        }
        public void Controller(GameObject crs, GameObject colm)
        {
            if (con_active)
            {
                
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    //GameObject.FindObjectOfType<block_queue>().colms[i]
                }
                if (Input.GetKeyDown(KeyCode.S))
                {

                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if(-max_dist_X >= 10)
                    {
                        return;
                    }
                    MoveHorizontal(crs, colm, -1);
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (max_dist_X <= moved)
                    {
                        return;
                    }
                    MoveHorizontal(crs, colm, 1);
                }
            }
            else {
                int len = colm.GetComponent<column>().blocks.ToArray().Length;
                crs.transform.position = colm.GetComponent<column>().blocks[len - 1].transform.position;
                con_Y = len - 1;
                moved = 0;
                con_X = FindColmIndex(colm);

                /*instead of using an array to 
                 * control how many spaces you
                 * can go, use an int instead. */
               
                con_active = true;
            }
        }
    }
}
