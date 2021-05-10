using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private void OnEnable() {
        Time.timeScale = 0;
    }

    private void OnDisable() {
        Time.timeScale = 1;
    }

    public void OnMainMenuPress() {
        SceneManager.LoadScene(0);
    }
}
