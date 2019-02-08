using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XboxCtrlrInput;

public class GameManager : MonoBehaviour {
  public XboxController controller;
  public Ball ball;

  private Transform player1;
  private Transform player2;
  private Transform player3;
  private Transform player4;
  private static int score_team1;
  private static int score_team2;
  [SerializeField] private int game_time_ins;
  [SerializeField] private int winning_score_ins;
  private float game_time;
  private float curr_time;
  private int winning_score;
  private Text gameTimeText;
  private Text team1ScoreText;
  private Text team2ScoreText;
  private bool tipoff;

  static bool overtime;
  //this is pointless, only needed to call StopCoroutine
  public static GameManager S;
  [SerializeField] private GameObject winning_screen;
  [SerializeField] private Text winning_text;
  [SerializeField] private GameObject tipoff_screen;
  [SerializeField] private GameObject overtime_screen;

  /********************************************************
                     UI ELEMENTS TO ADD:
   * Main Menu
   * Ending game for tall [with quit or restart button]
   * Ending game for small [with quit or restart button]
   * Overtime display
   * Instructions panel
   * HUD (current scores, current time left)
   ********************************************************/

  void Awake() {
    S = this;
    team1ScoreText = GameObject.Find("HUDCanvas/Team1/team1_pts/Pts").GetComponent<Text>();
    team2ScoreText = GameObject.Find("HUDCanvas/Team2/team2_pts/Pts").GetComponent<Text>();
    gameTimeText = GameObject.Find("HUDCanvas/game_time/time").GetComponent<Text>();
    start_game();
    Debug.Log("Starting Game with " + game_time + " Seconds");
    winning_screen.SetActive(false);
    overtime_screen.SetActive(false);
    player1 = S.GetComponent<InputManager>().players[0].transform;
    player2 = S.GetComponent<InputManager>().players2[0].transform;
    player3 = S.GetComponent<InputManager>().players[1].transform;
    player4 = S.GetComponent<InputManager>().players2[1].transform;
  }

  private void start_game() {
    game_time = S.game_time_ins;
    winning_score = S.winning_score_ins;
    score_team1 = 0;
    score_team2 = 0;
    overtime = false;
    curr_time = game_time;
    setGameTime(curr_time);
    S.tipoff = true;
    S.tipoff_screen.SetActive(true);
    S.StartCoroutine(UpdateTime()); //start updating the game
    S.StartCoroutine(GameTime()); //start counting the game
  }

  private void end_game() {
    //TODO: disable player movements
    S.StopAllCoroutines();
    overtime_screen.SetActive(false);
    overtime = false;
    if (score_team2 > score_team1) {
      S.winning_text.text = "Congratulations to \nTeam 2!";
      S.winning_screen.SetActive(true);
    } else if (score_team1 > score_team2) {
      S.winning_text.text = "Congratulations to \nTeam 1!";
      S.winning_screen.SetActive(true);
    } else {
      overtime = true;
      S.StartCoroutine(OvertimeGame());
    }
  }
  //scoring, to be called from player script
  public void update_score(ScoreComponent.PlayerType p, int i) {
    if (p == ScoreComponent.PlayerType.team1) {
      score_team1 += i;
      team1ScoreText.text = score_team1.ToString();
    } else {
      score_team2 += i;
      team2ScoreText.text = score_team2.ToString();
    }
    if (score_team1 >= winning_score || score_team2 >= winning_score
        || overtime) {
      end_game();
    }
  }

  public void QuitGame() {
    quit_game();
  }

  private void quit_game() {
    Application.Quit();
  }

  public void RestartGame() {
    restart_game();
  }

  private void restart_game() {
    //TODO: change to load scene of main menu
    score_team1 = 0;
    score_team2 = 0;
    curr_time = game_time;
    overtime = false;
    //somehow reset player positions?
    SceneManager.LoadScene(SceneManager.GetActiveScene().name,
        LoadSceneMode.Single);
  }

  private IEnumerator GameTime() {
    yield return new WaitForSeconds(game_time);
    Debug.Log("Game Time Up");
    setGameTime(0);
    end_game();
  }

  private IEnumerator UpdateTime() {
    while (curr_time > 0) {
      curr_time -= 1;
      yield return new WaitForSeconds(1);
      setGameTime(curr_time);
    }
  }

  private IEnumerator OvertimeGame(){
    overtime_screen.SetActive(true);
    yield return new WaitForSeconds(3);
    overtime_screen.SetActive(false);
  }

  private void setGameTime(float time) {
    float minutes = Mathf.Floor(time / 60);
    float seconds = time % 60;
    string min_str = minutes.ToString();
    string sec_str = seconds.ToString();
    if (minutes < 10) min_str = "0" + min_str;
    if (seconds < 10) sec_str = "0" + sec_str;
    gameTimeText.text = min_str + ":" + sec_str;
  }
  
  //Players can check for tip-off priority.
  public void CheckTipOff(XboxController controller)
  {
        Debug.Log("Checking TipOff...");
    if (S.tipoff)
    {
            S.tipoff = false;
            switch (controller)
            {
                case XboxController.First: //small1
                    S.ball.SetPosition(S.player1.transform.position);
                    S.ball.SetParent(S.player1);
                    S.ball.transform.localPosition += new Vector3(0.5f, 0.5f, 0.5f);
                    S.ball.SetOwner((IPlayer)S.player1.gameObject.GetComponent<SmallPlayer>());
                    
                    break;
                case XboxController.Second: //tall1
                    S.ball.SetPosition(S.player2.transform.position);
                    S.ball.SetParent(S.player2);
                    S.ball.transform.localPosition += new Vector3(0.5f, 0.5f, 0.5f);
                    S.ball.SetOwner((IPlayer)S.player2.gameObject.GetComponent<TallPlayer>());
                    break;
                case XboxController.Third: //small2
                    S.ball.SetPosition(S.player3.transform.position);
                    S.ball.SetParent(S.player3);
                    S.ball.transform.localPosition += new Vector3(0.5f, 0.5f, 0.5f);
                    S.ball.SetOwner((IPlayer)S.player3.gameObject.GetComponent<SmallPlayer>());
                    break;
                case XboxController.Fourth: //tall2
                    S.ball.SetPosition(S.player4.transform.position);
                    S.ball.SetParent(S.player4);
                    S.ball.transform.localPosition += new Vector3(0.5f, 0.5f, 0.5f);
                    S.ball.SetOwner((IPlayer)S.player4.gameObject.GetComponent<TallPlayer>());
                    break;
            }
            S.tipoff_screen.SetActive(false);
    }
  }
}