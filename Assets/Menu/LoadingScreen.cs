using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public Canvas LoadingCanvas;
    public Canvas HUD;
    public void StartLoadingScreen()
    {
        HUD.enabled = false;
        LoadingCanvas.enabled = true;
    }

    public void ExitLoadingScreen()
    {
        HUD.enabled = true;
        LoadingCanvas.enabled = false;
    }
}
