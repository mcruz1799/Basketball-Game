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

  static bool overtime;
  //this is pointless, only needed to call StopCoroutine
  public static GameManager S;
  [SerializeField] GameObject winning_screen;

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
    game_time = 30.0f;
    start_game();
    Debug.Log("Starting Game with " + game_time + " Seconds");
    winning_screen.SetActive(false);
    player1 = S.GetComponent<InputManager>().players[0].transform;
    player2 = S.GetComponent<InputManager>().players2[0].transform;
    player3 = S.GetComponent<InputManager>().players[1].transform;
    player4 = S.GetComponent<InputManager>().players2[1].transform;
  }

  private void start_game() {
    S.StartCoroutine(TipOff());
    score_team1 = 0;
    score_team2 = 0;
    overtime = false;
    game_time = S.game_time_ins;
    winning_score = S.winning_score_ins;
    curr_time = game_time;
    setGameTime(curr_time);
  }

  private void end_game() {
    S.StopCoroutine(GameTime());
    overtime = false;
    if (score_team2 > score_team1) {
      S.winning_screen.GetComponent<Text>().text =
          "Congratulations to Team 2!";
      S.winning_screen.SetActive(true);
    } else if (score_team1 > score_team2) {
      S.winning_screen.GetComponent<Text>().text =
          "Congratulations to Team 1!";
      S.winning_screen.SetActive(true);
    } else {
      overtime = true;
      //TODO: display a screen saying WHOEVER SCORES NEXT WINS
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
    curr_time = 0;
    //somehow reset player positions?
    SceneManager.LoadScene(SceneManager.GetActiveScene().name,
        LoadSceneMode.Single);
  }

  private IEnumerator GameTime() {
    yield return new WaitForSeconds(game_time);
    Debug.Log("Game Time Up");
    end_game();
  }

  private IEnumerator UpdateTime() {
    while (curr_time > 0) {
      curr_time -= 1;
      yield return new WaitForSeconds(1);
      setGameTime(curr_time);
    }
  }

  private IEnumerator TipOff() {
    while (true) {
      if (XCI.GetButtonDown(XboxButton.A)) {
        switch (S.controller) {
          case XboxController.First: //small1
            S.ball.SetParent(S.player1);
            S.ball.SetPosition(player1.transform.position);
            break;
          case XboxController.Second: //tall1
            S.ball.SetParent(S.player2);
            S.ball.SetPosition(player2.transform.position);
            break;
          case XboxController.Third: //small2
            S.ball.SetParent(S.player3);
            S.ball.SetPosition(player3.transform.position);
            break;
          case XboxController.Fourth: //tall2
            S.ball.SetParent(S.player4);
            S.ball.SetPosition(player4.transform.position);
            break;
        }
        break;
      }
    }
    S.StartCoroutine(GameTime()); //start counting the game
    S.StartCoroutine(UpdateTime()); //start updating the game
    yield return new WaitForSeconds(0);
  }

  private void setGameTime(float time) {
    float minutes = Mathf.Floor(curr_time / 60);
    float seconds = curr_time % 60;
    gameTimeText.text = minutes + " : " + seconds;
  }
}