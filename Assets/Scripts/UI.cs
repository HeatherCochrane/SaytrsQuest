using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    [SerializeField]
    GameObject hearts;

    [SerializeField]
    GameObject fade;

    [SerializeField]
    GameObject end;

    [SerializeField]
    List<Sprite> endScreens = new List<Sprite>();


    int enemiesKilled = 0;

    [SerializeField]
    List<GameObject> endingText = new List<GameObject>();


    [SerializeField]
    List<GameObject> openingPrologue = new List<GameObject>();
    int tracker = -1;

    [SerializeField]
    GameObject opening;

    [SerializeField]
    Player player;

    [SerializeField]
    GameObject next;

    [SerializeField]
    GameObject startGameButton;

    [SerializeField]
    GameObject enemyParent;

    [SerializeField]
    GameObject healthParent;

    [SerializeField]
    GameObject doorParent;

    [SerializeField]
    GameObject boxes;

    [SerializeField]
    AudioHandler audio;
    // Start is called before the first frame update
    void Start()
    {
        end.SetActive(false);
        fadeIn();

        for(int i =0; i < endingText.Count; i++)
        {
            endingText[i].SetActive(false);
        }

        startGameButton.SetActive(false);
        nextPage();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void loseHeart(int l)
    {
        for (int i = 0; i < hearts.transform.childCount; i++)
        {
            hearts.transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i =0; i < l; i++)
        {
            hearts.transform.GetChild(i).gameObject.SetActive(true);
        }

    }

    void fadeIn()
    {
        fade.GetComponent<Animator>().SetBool("fadeOut", false);
        fade.GetComponent<Animator>().SetBool("fadeIn", true);
    }

    void fadeOut()
    {
        fade.GetComponent<Animator>().SetBool("fadeIn", false);
        fade.GetComponent<Animator>().SetBool("fadeOut", true);
    }
    public void gameOver()
    {
        fadeOut();
        audio.endScreenAudio();
        Invoke("endScreen", 2);
    }

    public void enemyKilled()
    {
        enemiesKilled += 1;
    }

    void showEndScreen()
    {
        end.SetActive(true);
    }

    void endScreen()
    {
        if(enemiesKilled == 4)
        {
            //Worst ending
            end.GetComponent<Image>().sprite = endScreens[0];
            endingText[0].SetActive(true);
        }
        else if(enemiesKilled == 0)
        {
            //Best ending
            end.GetComponent<Image>().sprite = endScreens[2];
            endingText[2].SetActive(true);
        }
        else
        {
            //neutral ending
            end.GetComponent<Image>().sprite = endScreens[1];
            endingText[1].SetActive(true);
        }

        showEndScreen();
    }

    public void nextPage()
    {
        tracker += 1;
        if (tracker < openingPrologue.Count)
        {
            for (int i = 0; i < openingPrologue.Count; i++)
            {
                if (i == tracker)
                {
                    openingPrologue[i].SetActive(true);
                }
                else
                {
                    openingPrologue[i].SetActive(false);
                }
            }
        }

        if(tracker == openingPrologue.Count -1)
        {
            next.SetActive(false);
            startGameButton.SetActive(true);
        }

    }

    public void finishPrologue()
    {
        player.setGameStart(false);
        opening.SetActive(false);
        audio.setAudioOn();
    }

    public void restartGame()
    {
        end.SetActive(false);

        for(int i =0; i < enemyParent.transform.childCount; i++)
        {
            enemyParent.transform.GetChild(i).GetComponentInChildren<Enemy>().respawnEnemy();
        }

        for (int i = 0; i < healthParent.transform.childCount; i++)
        {
            healthParent.transform.GetChild(i).gameObject.SetActive(true);
        }

        for(int i =0; i < doorParent.transform.childCount; i++)
        {
            doorParent.transform.GetChild(i).GetComponentInChildren<PressurePlate>().closeDoor();       
        }

        for(int j =0; j < boxes.transform.childCount; j++)
        {
            boxes.transform.GetChild(j).GetComponent<Boxes>().resetBox();
        }
        for(int k =0; k < endingText.Count; k++)
        {
            endingText[k].SetActive(false);
        }

        player.resetPlayer();
        fade.GetComponent<Animator>().SetBool("fadeOut", false);
        Invoke("setAudio", 1);
    }

    void setAudio()
    {
        audio.setAudioBack();
    }
}
