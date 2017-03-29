using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_BombDisplay : MonoBehaviour
{
    [SerializeField]
    AudioClip countDownSound;
    [SerializeField]
    Animator flashLight;
    [SerializeField]
    Text scoreText;
    [SerializeField]
    Text timeText;
    [SerializeField]
    Slider scoreSlider;
    [SerializeField]
    Slider timeSlider;

    float currentCountDown = 0;
    float countDownInterval = 5f;

	void Update ()
    {
        scoreText.text = GameController.Instance.CurrentScore.ToString() + "/" + GameController.Instance.Level.TargetScore;
        timeText.text = Mathf.RoundToInt(GameController.Instance.CurrentTime).ToString();
        scoreSlider.value = (float)GameController.Instance.CurrentScore / (float)GameController.Instance.Level.TargetScore;
        timeSlider.value = GameController.Instance.CurrentTime / GameController.Instance.Level.Time;

        currentCountDown += Time.deltaTime;
        if (currentCountDown >= countDownInterval * (GameController.Instance.CurrentTime / GameController.Instance.Level.Time) + 0.25f)
        {
            currentCountDown = 0;
            GetComponent<AudioSource>().PlayOneShot(countDownSound, 0.1f);
            flashLight.SetTrigger("Flash");
        }
    }
}
