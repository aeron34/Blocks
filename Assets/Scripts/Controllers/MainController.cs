using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainController : MonoBehaviour
{

    public bool training;
    private string char_name;
    //public int[] stats = new int[5] {-1,-1,-1,-1,-1 }; //[win,loss,prev_score,curr_score,place];
    public TextMeshProUGUI[] texts;// = new TextMeshProUGUI[5];
    GameObject char_select, main_menu;
    public GameObject Gizmo, Boxer, weap_box, ultra_bar, new_record;
    public int score = 0, place;
    void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        texts = new TextMeshProUGUI[5] { null, null, null, null, null };
        main_menu = GameObject.Find("Main Menu");
        char_select = GameObject.Find("Char Select");
        main_menu.SetActive(true);
        char_select.SetActive(false);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += LoadedScene;
    }
    public void LoadGizmo()
    {
        if (!training)
        {
            LobbyMenu();
           
        }
        else
        {
            LoadTrainingRoom();
        }
        char_name = "Gizmo";
        
    }   
    public void LoadBoxer()
    {
        if (!training)
        {
            LobbyMenu();
        }
        else
        {
            LoadTrainingRoom();
        }
        char_name = "Boxer";
    }

    private void LoadedScene(Scene scene, LoadSceneMode mode)
    {
        var canvas = GameObject.Find("Canvas");

        if (scene.name == "Game")
        {

            if (char_name == "Gizmo")
            {
                var g = Instantiate(Gizmo);
                g.name = "pic";
                Instantiate(weap_box, canvas.transform).name = "weap_box";
            }
            if (char_name == "Boxer")
            {
                var g = Instantiate(Boxer);
                g.name = "pic";
                Instantiate(ultra_bar, canvas.transform).name = "ultra_bar";
            }
            StartCoroutine(FindObjectOfType<Online>().GetMeteors());

        }
        if (scene.name == "Training")
        {
            if (char_name == "Gizmo")
            {
                var g = Instantiate(Gizmo);
                g.name = "pic";
                Instantiate(weap_box, canvas.transform).name = "weap_box";
                weap_box.transform.position = new Vector3(-725f, -275f, 0);
            }
            if (char_name == "Boxer")
            {
                var g = Instantiate(Boxer);
                g.name = "pic";

                var bar = Instantiate(ultra_bar, canvas.transform);
                bar.name = "ultra_bar";
                bar.transform.localPosition = new Vector3(-738f, -430f, 0);

            }

            var tut = GameObject.Find("tut");
            tut.GetComponent<Tutorial>().character = char_name;

        }

        if (scene.name == "Loss Screen")
        {
            new_record = GameObject.Find("new record");// (false);
            new_record.SetActive(false);
            GameObject.Find("score").GetComponent<TextMeshProUGUI>().text = score.ToString();
            texts[0] = GameObject.Find("winloss").GetComponent<TextMeshProUGUI>();
            texts[1] = null;
            texts[2] = GameObject.Find("previous highscore").GetComponent<TextMeshProUGUI>();
            texts[3] = GameObject.Find("score").GetComponent<TextMeshProUGUI>();
            texts[4] = GameObject.Find("your place").GetComponent<TextMeshProUGUI>();
            texts[0].text = "WINS: ... 		LOSSES: ...";
            for (int i = 2; i < 5; i++)
            {
                texts[i].text = $"{texts[i].text} ...";
            }
            string suffix = "th";
            switch(place)
            {
                case 1:
                    suffix = "st";
                    break;
                case 2:
                    suffix = "nd";
                    break;
                case 3:
                    suffix = "rd";
                    break;
                default:
                    break;
            }
            texts[3].text = $"FINAL SCORE: {score}";
            texts[4].text = $"YOU PLACED {place}{suffix}";
        }
    }

    #region View and Scene Loaders

    public void LobbyMenu()
    {
        SceneManager.LoadScene("Lobby");
    }
    public void LoadTrainingRoom()
    {
        SceneManager.LoadScene("Training");
    }
    public void MainMenu()
    {
        main_menu.SetActive(true);
        char_select.SetActive(false);
    }
    public void CharSelect()
    {
        main_menu.SetActive(false);
        char_select.SetActive(true);
    }
    
    public void TrainingToCharSelect()
    {
        training = true;
        CharSelect();
    }

    public void LoadLoss(int scr, int plc)
    {
        score = scr;
        place = plc;
        SceneManager.LoadScene("Loss Screen");

    }
    #endregion

    void Update()
    {
        
    }
}
