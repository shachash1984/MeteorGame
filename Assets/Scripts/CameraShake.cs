using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour {

    public float shakeDuration = 0.1f;
    public float shakeStrength = 1f;
    public int vibrato = 10;
    public float randomness = 90;


    private void OnEnable()
    {
        PlayerBall.OnAsteroidHit += ShakeCamera;
        PlayerBall.OnLastAsteroidHit += ShakeCamera;
    }

    private void OnDisable()
    {
        PlayerBall.OnAsteroidHit -= ShakeCamera;
        PlayerBall.OnLastAsteroidHit -= ShakeCamera;
    }

    public void ShakeCamera()
    {
        transform.DOShakePosition(shakeDuration, shakeStrength, vibrato, randomness);
#if !UNITY_STANDALONE
        Handheld.Vibrate();
#endif
    }
}
