using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class UIImageAnimator : MonoBehaviour, ISpriteAnimator {
#pragma warning disable 0649
  [SerializeField] private List<Sprite> sprites;
  [SerializeField] private bool initialIsVisible = true;
  [SerializeField] private bool initialIsLooping = false;
#pragma warning restore 0649

  private UnityEngine.UI.Image image;

  private bool _isVisible;
  public bool IsVisible {
    get {
      return _isVisible;
    }
    set {
      _isVisible = value;
      image.enabled = IsVisible;
    }
  }
  public bool IsLooping { get; set; }
  public bool IsPaused { get; set; }
  public bool IsDone { get; private set; }

  public bool FlipX { get => false; set => Debug.LogError("Unsupported operation"); }
  public bool FlipY { get => false; set => Debug.LogError("Unsupported operation"); }

  [SerializeField] private int _framesPerSprite;
  public int FramesPerSprite { get { return _framesPerSprite; } set { _framesPerSprite = value; } }

  private bool animateFromStartFlag;

  private void Awake() {
    image = GetComponent<UnityEngine.UI.Image>();

    IsVisible = initialIsVisible;
    IsLooping = initialIsLooping;

    IsPaused = false;
    IsDone = true;

    StartCoroutine(AnimationRoutine());
  }

  public void StartFromFirstFrame() {
    animateFromStartFlag = true;
  }

  private IEnumerator WaitForFrames(int numFrames) {
    if (numFrames < 1) {
      Debug.LogError("Attempting to wait for less than 1 frame");
      yield break;
    }

    for (int i = 0; i < numFrames; i++) {
      yield return null;
    }
  }

  private IEnumerator AnimationRoutine() {
    while (true) {

      //If we're not looping and nobody has called StartFromFirstFrame, then wait.
      if (!(IsLooping || animateFromStartFlag)) {
        yield return new WaitUntil(() => IsLooping || animateFromStartFlag);
      }
      animateFromStartFlag = false;

      IsDone = false;
      foreach (Sprite sprite in sprites) {
        //Wait until we aren't paused.  Note that even if not paused, this still waits for one frame.
        yield return new WaitUntil(() => !IsPaused);

        //Wait for the specified number of frames
        if (FramesPerSprite - 1 > 0) {
          yield return WaitForFrames(FramesPerSprite - 1);
        }

        //If restarting, cancel this loop
        if (animateFromStartFlag) {
          break;
        }

        //Display the next sprite in sprites
        image.sprite = sprite;
      }
      IsDone = true;
    }
  }
}