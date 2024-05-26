using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GoToSquat : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("SquatScene");
    }
}
