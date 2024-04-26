using Riptide;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public enum AbilityState
{
    ready, active, cooldown
}


public class AbilityController : MonoBehaviour
{
    [SerializeField] private float cooldownTime;

    public ushort clientId;

    private bool abilityInput;

    public AbilityState abilityState = AbilityState.ready;

    [SerializeField] public UnityEvent ready, active, cooldown;

    public void GetInputs(bool input, ushort clientId)
    {
        abilityInput = input;
        this.clientId = clientId;
    }

    private void FixedUpdate()
    {
        switch(abilityState)
        {
            case AbilityState.ready:
                if (abilityInput)
                {
                    StartCoroutine(ChangeStateInSeconds(0, AbilityState.active, active));
                }
                break;
            case AbilityState.active:
                break;
            case AbilityState.cooldown:
                cooldown?.Invoke();
                StartCoroutine(ChangeStateInSeconds(cooldownTime ,AbilityState.ready, ready));
                break;
        }
    }

    public IEnumerator ChangeStateInSeconds(float time, AbilityState newState, UnityEvent nextEvent)
    {
        yield return new WaitForSeconds(time);
        nextEvent?.Invoke();
        abilityState = newState;
    }
}
