using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitConfirmationManager : MonoBehaviour
{
    // Method to handle the Yes button click
    public void OnYesButtonClick()
    {
#if UNITY_STANDALONE
        // Quit the application
        Application.Quit();
#endif

        // If we are running in the editor
#if UNITY_EDITOR
        // Stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Method to handle the No button click
    public void OnNoButtonClick()
    {
        // Load the Menu scene
        SceneManager.LoadScene("Menu");

    }
}