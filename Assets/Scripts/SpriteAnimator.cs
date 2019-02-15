using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour {
#pragma warning disable 0649
  [SerializeField] private List<Sprite> sprites;
#pragma warning restore 0649

  private SpriteRenderer spriteRenderer;

  private void Awake() {
    spriteRenderer = GetComponent<SpriteRenderer>();
    StartCoroutine(AnimationRoutine());
  }

  private IEnumerator AnimationRoutine() {
    while (true) {
      foreach (Sprite sprite in sprites) {
        spriteRenderer.sprite = sprite;
        yield return null;
      }
    }
  }
}
