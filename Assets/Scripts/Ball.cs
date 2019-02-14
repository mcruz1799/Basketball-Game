using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour, IBall {
#pragma warning disable 0649
  [SerializeField] private DigitalRuby.LightningBolt.LightningBoltScript lightningEffectPrefab;
  [SerializeField] private float effectTime;
#pragma warning restore 0649

  public float Radius { get { return transform.lossyScale.x; } }

  //For use only in BallUserComponent.HoldBall
  public void SetPosition(Vector3 newPosition) {
    DigitalRuby.LightningBolt.LightningBoltScript gfx = Instantiate(lightningEffectPrefab);
    gfx.StartPosition = transform.position;
    transform.localPosition = newPosition;
    gfx.EndPosition = transform.position;
    StartCoroutine(GfxRoutine(gfx.gameObject));
  }

  //For use only in BallUserComponent.HoldBall
  public void SetParent(Transform newParent) {
    transform.SetParent(newParent, true);
  }

  private IEnumerator GfxRoutine(GameObject effect) {
    yield return new WaitForSeconds(effectTime);
    Destroy(effect);
  }
}