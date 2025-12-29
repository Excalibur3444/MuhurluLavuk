using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject currentWorld;
    [SerializeField]
    private GameObject pastWorld;

    public bool IsInPast { get; private set; }


    private void Start()
    {

    }

    public void TimeTravelInput(CallbackContext context)
    {
        if (context.performed)
        {
            MakeTimeTravel();
        }

    }

    private void MakeTimeTravel()
    {
        IsInPast = !IsInPast;

        if (IsInPast)
        {
            currentWorld.SetActive(false);
            pastWorld.SetActive(true);
        }

        if (!IsInPast)
        {
            currentWorld.SetActive(true);
            pastWorld.SetActive(false);           
        }

    }


}
