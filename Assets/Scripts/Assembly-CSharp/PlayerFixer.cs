using UnityEngine;

public class PlayerFixer : MonoBehaviour
{
    private void Start()
    {
        GameObject player = GameObject.Find("main_player");
        if (player != null)
        {
            player.transform.position = new Vector3(17.5f, 0f, -120f);
        }
        else
        {
            Debug.LogWarning("main_player not found in the scene.");
        }
    }
}
