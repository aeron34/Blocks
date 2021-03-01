using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainController : MonoBehaviour
{

    public bool training;
    private string char_name;
    GameObject char_select, main_menu;
    public GameObject Gizmo, Boxer, weap_box, ultra_bar;
    public int score = 0;
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
            GameObject.Find("score").GetComponent<TextMeshProUGUI>().text = score.ToString();
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

    public void LoadLoss()
    {
        SceneManager.LoadScene("Loss Screen");

    }
    #endregion

    void Update()
    {
        
    }
}
