using UnityEngine.SceneManagement;
using UnityEngine;

namespace Main.Handler
{
    internal sealed class SceneReload : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}