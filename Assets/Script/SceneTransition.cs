using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string targetScene;
    public string targetSpawnPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            GameManager.Instance.spawnPointName = targetSpawnPoint;

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(targetScene);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();

        foreach(SpawnPoint sp in spawnPoints)
        {
            if(sp.spawnName == GameManager.Instance.spawnPointName)
            {
                player.transform.position = sp.transform.position;
                break;
            }
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}