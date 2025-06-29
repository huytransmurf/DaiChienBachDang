using Assets.Scripts.Menu;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.EnvironmentObjects
{
    public class ExitTrigger : MonoBehaviour
    {
        public string nextSceneName; // hoặc gán qua Inspector

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                LoadingData.targetScene = nextSceneName;
                SceneManager.LoadScene("LoadingScene");
            }
        }
    }
}
