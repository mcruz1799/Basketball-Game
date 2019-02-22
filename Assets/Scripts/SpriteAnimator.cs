using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpriteAnimator {
  bool IsVisible { get; set; }
  bool IsLooping { get; set; }
  bool IsPaused { get; set; }
  bool IsDone { get; }

  bool FlipX { get; set; }
  bool FlipY { get; set; }

  //How many frames to wait in between each sprite of the animation
  int FramesPerSprite { get; set; }

  //Set the animation to run from its first frame, even if mid-animation
  void StartFromFirstFrame();
}

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour, ISpriteAnimator {
#pragma warning disable 0649
  [SerializeField] private List<Sprite> sprites;
  [SerializeField] private bool initialIsVisible = true;
  [SerializeField] private bool initialIsLooping = false;
#pragma warning restore 0649

  public SpriteRenderer spriteRenderer;

  private bool _isVisible;
  public bool IsVisible {
    get {
      return _isVisible;
    }
    set {
      _isVisible = value;
      spriteRenderer.enabled = IsVisible;
    }
  }
  public bool IsLooping { get; set; }
  public bool IsPaused { get; set; }
  public bool IsDone { get; private set; }

  public bool FlipX { get => spriteRenderer.flipX; set => spriteRenderer.flipX = value; }
  public bool FlipY { get => spriteRenderer.flipY; set => spriteRenderer.flipY = value; }

  [SerializeField] private int _framesPerSprite;
  public int FramesPerSprite { get { return _framesPerSprite; } set { _framesPerSprite = value; } }

  private bool animateFromStartFlag;

  private void Awake() {
    spriteRenderer = GetComponent<SpriteRenderer>();

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
        spriteRenderer.sprite = sprite;
      }
      IsDone = true;
    }
  }
}