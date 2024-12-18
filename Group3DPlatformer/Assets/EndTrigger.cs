using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalPoint3D : MonoBehaviour
{
    public bool goNextLevel;
    public string levelName;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (goNextLevel)
            {
                
                SceneController.instance.NextLevel();
            }
            else
            {
                SceneController.instance.LoadScene(levelName);
            }
        }
    }
}
