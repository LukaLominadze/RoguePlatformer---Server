using UnityEngine;

public class DeactivateOnClick : MonoBehaviour
{
    [SerializeField] GameObject console;

    enum ConsoleState
    {
        on, off
    }

    ConsoleState consoleState = ConsoleState.off;

    void Update()
    {
        switch (consoleState)
        {
            case ConsoleState.off:
                console.SetActive(false);
                if (Input.GetKeyDown(KeyCode.BackQuote))
                {
                    consoleState = ConsoleState.on;
                }
                break;
            case ConsoleState.on:
                console.SetActive(true);
                if (Input.GetKeyDown(KeyCode.BackQuote))
                {
                    consoleState = ConsoleState.off;
                }
                break;
        }
    }
}
