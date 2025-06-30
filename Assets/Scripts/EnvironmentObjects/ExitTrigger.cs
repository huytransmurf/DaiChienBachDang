using Assets.Scripts.Menu;
using Assets.Scripts.Player;
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
                string currentScene = SceneManager.GetActiveScene().name;

                if (currentScene == "SecondScene")
                {
                    LoadingData.targetScene = nextSceneName;
                    SceneManager.LoadScene("LoadingScene");
                    return;
                }

                if (GameManager.Instance.bossDefeated)
                {
                    LoadingData.targetScene = nextSceneName;
                    SceneManager.LoadScene("LoadingScene");
                }
                else
                {
                    Debug.Log("Chưa hoàn thành nhiệm vụ hoặc chưa nói chuyện với NPC.");
                }
            }
        }
    }
}
