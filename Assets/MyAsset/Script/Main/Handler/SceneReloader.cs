using UnityEngine.SceneManagement;
using UnityEngine;

namespace Main.Handler
{
    internal sealed class SceneReloader : MonoBehaviour
    {
        private void Update()
        {
            if (GameManager.Instance.State == GameState.Over) return;

            if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}