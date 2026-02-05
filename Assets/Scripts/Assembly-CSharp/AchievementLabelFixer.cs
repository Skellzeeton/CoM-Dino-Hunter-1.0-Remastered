using UnityEngine;

public class AchievementLabelFixer : MonoBehaviour
{
    private static AchievementLabelFixer _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        // Get ONLY AchievementItem(Clone) objects
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go != null && go.name == "AchievementItem(Clone)")
            {
                FixLabelInsideItem(go.transform, "Label_Introduce");
                FixLabelInsideItem(go.transform, "Label_Name");
            }
        }
    }

    private void FixLabelInsideItem(Transform itemRoot, string labelName)
    {
        // ONLY search inside THIS specific item
        Transform label = FindChildInHierarchy(itemRoot, labelName);
        if (label == null)
            return;

        // Double-check the parent is really AchievementItem(Clone)
        if (!IsChildOf(label, itemRoot))
            return;

        MeshRenderer rend = label.GetComponent<MeshRenderer>();
        if (rend == null || rend.sharedMaterial == null)
            return;

        Shader shader = Shader.Find("Unlit/Transparent Colored TwoTexture Vertex Color (HardClip)");
        if (shader == null)
            return;

        rend.sharedMaterial.shader = shader;
    }

    // Recursive search ONLY inside itemRoot
    private Transform FindChildInHierarchy(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform result = FindChildInHierarchy(child, name);
            if (result != null)
                return result;
        }
        return null;
    }

    // Verifies the object truly belongs to the item parent
    private bool IsChildOf(Transform child, Transform potentialParent)
    {
        Transform current = child.parent;
        while (current != null)
        {
            if (current == potentialParent)
                return true;
            current = current.parent;
        }
        return false;
    }
}
