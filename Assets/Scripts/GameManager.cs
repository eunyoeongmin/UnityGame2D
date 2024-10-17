using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;
    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject UIRestartBtn;
    public void NextStage()
    {
        //Change Stage
        if(stageIndex < Stages.Length-1) {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE " + (stageIndex + 1);
        }

        else { //Game Clear
            //Player Contol Lock
            Time.timeScale = 0;
            //Result UI
            Debug.Log("클리어");
            //Restart Button UI 
            Text btnText = UIRestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Game Clear!";
            ViewBtn();
        }

        //Calculate Point
            totalPoint += stagePoint;
            stagePoint = 0;
    }

    void Update() {
        UIPoint.text = (totalPoint +  stagePoint).ToString();
    }

    public void HealthDown() {
        if (health > 1) {
            health--;
            UIhealth[health].color = new Color(1,0,0, 0.4f);
        } 
        else {
            //All Health UI Off
            UIhealth[0].color = new Color(1,0,0, 0.4f);
            //Player Die Effect
            player.OnDie();
            //Result UI

            //Retry Button UI
            ViewBtn();
        }
    }
    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            //Player Reposition
            if (health > 1) {
                PlayerReposition();
            }
            //Health Down
            HealthDown();

        }
    }

    void PlayerReposition() {
        player.transform.position = new Vector3(0,0,-1);
        player.VelocityZero();
    }

    void ViewBtn() {
        UIRestartBtn.SetActive(true);
    }

    public void Restart() {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
