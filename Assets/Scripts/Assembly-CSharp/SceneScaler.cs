using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneScaler
{
    private static readonly string[] validScenes = new string[]
    {
        "Yulin",
        "SceneSnow",
        "SceneForest",
        "SceneLava",
        "SceneLava2",
        "SceneGorge"
    };

    public static bool ShouldScale()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        for (int i = 0; i < validScenes.Length; i++)
        {
            if (sceneName == validScenes[i])
                return true;
        }

        if (sceneName.Contains("Yulin"))
            return true;

        return false;
    }
}
