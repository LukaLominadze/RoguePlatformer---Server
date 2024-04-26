using UnityEngine;
using UnityEngine.Events;

public class RangedWeapons : MonoBehaviour
{
    [SerializeField] private float activeTime;
    [SerializeField] private float cooldownTime;

    private float originalTime;

    private bool attackInput;

    [SerializeField] UnityEvent active, cooldown;

    enum AttackState
    {
        ready, active, cooldown
    }

    AttackState attackState = AttackState.ready;

    public void GetInputs(bool attackInput)
    {
        this.attackInput = attackInput;
    }

    private void Start()
    {
        originalTime = activeTime;
    }

    private void FixedUpdate()
    {
        switch(attackState)
        {
            case AttackState.ready:
                if (attackInput)
                {
                    attackState = AttackState.active;
                }
                break;
            case AttackState.active:
                active?.Invoke();
                ChangeStateInSeconds(AttackState.cooldown, cooldownTime);
                break;
            case AttackState.cooldown:
                cooldown?.Invoke();
                ChangeStateInSeconds(AttackState.ready, activeTime);
                break;
        }
    }

    private void ChangeStateInSeconds(AttackState newState, float timeInNextState)
    {
        originalTime -= Time.fixedDeltaTime;
        if (originalTime <= 0)
        {
            originalTime = timeInNextState;
            attackState = newState;
        }
    }
}
