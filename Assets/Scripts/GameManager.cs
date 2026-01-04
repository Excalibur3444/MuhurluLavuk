using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float currentHealth;
    public float maxHealth = 20f; // this is also max berserk time

    public bool IsInPast { get; private set; }
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            currentHealth = maxHealth;

        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;


    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        IsInPast = false;

    }

    public void MakeTimeTravel()
    {
        IsInPast = !IsInPast;

    }


}
