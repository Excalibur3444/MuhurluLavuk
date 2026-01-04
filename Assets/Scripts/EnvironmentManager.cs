using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public GameObject currentWorld;
    public GameObject pastWorld;

    private bool timeState;

    private void Start()
    {
        timeState = GameManager.Instance.IsInPast;
        UpdateWorldVisuals(timeState);

    }

    private void Update()
    {
        if (GameManager.Instance.IsInPast != timeState)
        {
            timeState = GameManager.Instance.IsInPast;

            UpdateWorldVisuals(timeState);

        }


    }

    private void UpdateWorldVisuals(bool isInPast)
    {
        currentWorld.SetActive(!isInPast);
        pastWorld.SetActive(isInPast);

    }



}
