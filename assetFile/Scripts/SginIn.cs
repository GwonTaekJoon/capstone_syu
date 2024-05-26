using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SginIn : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("SignIn");
    }
}
