using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PointsHud : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI pointsValue;
    [SerializeField] TextMeshProUGUI personalBestPointsValue;
    
    private void OnEnable()
    {
        GameManager.OnScoreAdded += UpdateScore;
    }
    private void OnDisable()
    {
        GameManager.OnScoreAdded += UpdateScore;
    }

    private void Start()
    {
        if(pointsValue != null)
            pointsValue.text = 0.ToString();
        if(personalBestPointsValue != null)
            personalBestPointsValue.text = PlayerPrefs.GetInt(Constants.PlayerPrefsKeys.HighScore,0).ToString();
    }

    private void UpdateScore(int score)
    {
        pointsValue.text = score.ToString();
    }
}
