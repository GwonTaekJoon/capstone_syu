using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BackToWorkoutPage : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("WorkoutPage");
    }
}
