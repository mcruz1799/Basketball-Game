using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XboxCtrlrInput;

public class GameManager : MonoBehaviour {
  public static GameManager S;

#pragma warning disable 0649
  [SerializeField] private Ball _ball;

  [SerializeField] private SmallPlayer _smallPlayer1;
  [SerializeField] private TallPlayer _tallPlayer1;
  [SerializeField] private SmallPlayer _smallPlayer2;
  [SerializeField] private TallPlayer _tallPlayer2;

  [SerializeField] private GameObject basket1;
  [SerializeField] private GameObject basket2;

  [SerializeField] private int gameLength;

  [SerializeField] private GameObject winningScreen;
  [SerializeField] private GameObject tipoffScreen;
  [SerializeField] private GameObject overtimeScreen;

  //GUI objects
  [SerializeField] private Text winningText;
  [SerializeField] private Text gameTimeText;
  [SerializeField] private Text team1ScoreText;
  [SerializeField] private Text team2ScoreText;
  [SerializeField] private RawImage possessionIndicator1;
  [SerializeField] private RawImage possessionIndicator2;

  [SerializeField] private GameObject sp1_spawn;
  [SerializeField] private GameObject sp2_spawn;
  [SerializeField] private GameObject tp1_spawn;
  [SerializeField] private GameObject tp2_spawn;
#pragma warning restore 0649

  public IBall Ball { get; private set; }
  private Vector3 ballInitialPosition;

  public float Xboundary { get; } = 7.0f;
  public float Zboundary { get; } = 14.0f;

  public IPlayer SmallPlayer1 { get; private set; }
  public IPlayer TallPlayer1 { get; private set; }
  public IPlayer SmallPlayer2 { get; private set; }
  public IPlayer TallPlayer2 { get; private set; }

  private int scoreTeam1;
  private int scoreTeam2;

  private float currentTime;

  public bool tipoff;
  public bool end;
  public bool overtime;
  //this is pointless, only needed to call StopCoroutine

  /********************************************************
                     UI ELEMENTS TO ADD:
   * Main Menu
   * Ending game for tall [with quit or restart button]
   * Ending game for small [with quit or restart button]
   * Overtime display
   * Instructions panel
   * HUD (current scores, current time left)
   ********************************************************/

  private void Awake() {
    S = this;

    if (basket1 == null || basket2 == null) {
      Debug.LogError("Baskets need to be initialized via inspector in GameManager!");
    }

    Ball = _ball;
    ballInitialPosition = _ball.transform.position;

    SmallPlayer1 = _smallPlayer1;
    TallPlayer1 = _tallPlayer1;
    SmallPlayer2 = _smallPlayer2;
    TallPlayer2 = _tallPlayer2;
    // sp1_pos = _smallPlayer1.transform.position;
    // sp2_pos = _smallPlayer2.transform.position;
    // tp1_pos = _tallPlayer1.transform.position;
    // tp2_pos = _tallPlayer2.transform.position;

    StartGame();
  }

  public bool PositionIsOutOfBounds(Vector3 position) {
    return Mathf.Abs(position.x) > S.Xboundary || Mathf.Abs(position.z) > S.Zboundary;
  }

  public void NotifyOfBallOwnership(ScoreComponent.PlayerType? team) {
    if (team == null) {
      possessionIndicator1.gameObject.SetActive(false);
      possessionIndicator2.gameObject.SetActive(false);
    } else {
      switch (team.Value) {
        case ScoreComponent.PlayerType.team1:
          possessionIndicator1.gameObject.SetActive(true);
          possessionIndicator2.gameObject.SetActive(false);
          return;

        case ScoreComponent.PlayerType.team2:
          possessionIndicator1.gameObject.SetActive(false);
          possessionIndicator2.gameObject.SetActive(true);
          return;

        default:
          Debug.LogWarning("Illegal enum value detected");
          break;
      }
    }
  }

  private void StartGame() {
    Debug.Log("Starting Game with " + gameLength + " Seconds");

    winningScreen.SetActive(false);
    overtimeScreen.SetActive(false);

    Ball.SetParent(null);
    Ball.SetPosition(ballInitialPosition);

    scoreTeam1 = 0;
    scoreTeam2 = 0;
    overtime = false;
    currentTime = gameLength;
    SetGameTime(currentTime);
    tipoff = true;
    tipoffScreen.SetActive(true);
    StartCoroutine(UpdateTime()); //start updating the game
    StartCoroutine(GameTime()); //start counting the game
  }

  private void EndGame() {
    //TODO: disable player movements
    end = true;
    S.StopAllCoroutines();
    overtimeScreen.SetActive(false);
    overtime = false;
    if (scoreTeam2 > scoreTeam1) {
      S.winningText.text = "Congratulations to \nTeam Blue!";
      S.winningScreen.SetActive(true);
    } else if (scoreTeam1 > scoreTeam2) {
      S.winningText.text = "Congratulations to \nTeam Red!";
      S.winningScreen.SetActive(true);
    } else {
      overtime = true;
      end = false;
      S.StartCoroutine(OvertimeGame());
    }
  }

  public GameObject BasketFromTeam(ScoreComponent.PlayerType team) {
    switch (team) {
      case ScoreComponent.PlayerType.team1:
        return basket1;
      case ScoreComponent.PlayerType.team2:
        return basket2;
      default:
        string errorMessage = "Undefined enum value";
        Debug.LogError(errorMessage);
        throw new System.Exception(errorMessage);
    }
  }

  public GameObject OpponentBasketFromTeam(ScoreComponent.PlayerType team) {
    switch (team) {
      case ScoreComponent.PlayerType.team1:
        return basket2;
      case ScoreComponent.PlayerType.team2:
        return basket1;
      default:
        string errorMessage = "Undefined enum value";
        Debug.LogError(errorMessage);
        throw new System.Exception(errorMessage);
    }
  }

  //scoring, to be called from player script
  public void UpdateScore(ScoreComponent.PlayerType p, int i) {
    Ball.SetParent(null);
    Ball.SetPosition(ballInitialPosition);

    if (p == ScoreComponent.PlayerType.team1) {
      scoreTeam1 += i;
      team1ScoreText.text = scoreTeam1.ToString();
      SmallPlayer1.HoldBall(Ball);
    } else {
      scoreTeam2 += i;
      team2ScoreText.text = scoreTeam2.ToString();
      SmallPlayer2.HoldBall(Ball);
    }
    if (overtime) {
      EndGame();
    }
    ResetAfterScore();
  }

  public void QuitGame() {
    Application.Quit();
  }

  public void RestartGame() {
    //TODO: change to load scene of main menu
    scoreTeam1 = 0;
    scoreTeam2 = 0;
    currentTime = gameLength;
    overtime = false;
    //somehow reset player positions?
    SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
  }

  private IEnumerator GameTime() {
    yield return new WaitForSeconds(gameLength);
    Debug.Log("Game Time Up");
    SetGameTime(0);
    EndGame();
  }

  private IEnumerator UpdateTime() {
    while (currentTime > 0) {
      currentTime -= 1;
      yield return new WaitForSeconds(1);
      SetGameTime(currentTime);
    }
  }

  private IEnumerator OvertimeGame() {
    overtimeScreen.SetActive(true);
    yield return new WaitForSeconds(3);
    overtimeScreen.SetActive(false);
  }

  private void SetGameTime(float time) {
    float minutes = Mathf.Floor(time / 60);
    float seconds = time % 60;
    string min_str = minutes.ToString();
    string sec_str = seconds.ToString();
    if (minutes < 10) min_str = "0" + min_str;
    if (seconds < 10) sec_str = "0" + sec_str;
    gameTimeText.text = min_str + ":" + sec_str;
  }

  public void ResetAfterScore() {
    _tallPlayer1.ThrowSmallPlayer();
    _tallPlayer2.ThrowSmallPlayer();
    _smallPlayer1.transform.position = sp1_spawn.transform.position;
    _smallPlayer2.transform.position = sp2_spawn.transform.position;
    _tallPlayer1.transform.position = tp1_spawn.transform.position;
    _tallPlayer2.transform.position = tp2_spawn.transform.position;
    //S.tipoff = true;
    //S.tipoffScreen.SetActive(true);
  }

  //Players can check for tip-off priority.
  public bool CheckTipOff(XboxController controller) {
    Debug.Log("Checking TipOff...");
    if (S.tipoff) {
      S.tipoff = false;
      switch (controller) {
        case XboxController.First: //small1
          SmallPlayer1.HoldBall(Ball);
          S.tipoffScreen.SetActive(false);
          return true;

        case XboxController.Second: //tall1
          TallPlayer1.HoldBall(Ball);
          S.tipoffScreen.SetActive(false);
          return true;

        case XboxController.Third: //small2
          SmallPlayer2.HoldBall(Ball);
          S.tipoffScreen.SetActive(false);
          return true;

        case XboxController.Fourth: //tall2
          TallPlayer2.HoldBall(Ball);
          S.tipoffScreen.SetActive(false);
          return true;
        default:
          return false;
      }

    } else return false;
  }
}