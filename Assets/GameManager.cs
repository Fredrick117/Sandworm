using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public int score = 0;

    public Transform scoreUI;

    public Transform sandworm;

    private void Update()
    {
        scoreUI.GetComponent<TMP_Text>().text = score.ToString();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
    }

    public void GameOver()
    {
        sandworm.GetComponent<Sandworm>().isDead = true;
        StartCoroutine(ResetGameAfterTime());
    }

    private IEnumerator ResetGameAfterTime()
    {
        yield return new WaitForSeconds(3);
        score = 0;
        ResumeGame();
        SceneManager.LoadScene("SampleScene");
    }
}
