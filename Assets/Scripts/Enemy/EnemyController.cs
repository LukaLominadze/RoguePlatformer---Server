using Riptide;
using UnityEngine;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
    [SerializeField] Enemy enemy;
    [Space(5)]
    [SerializeField] Transform leftDestination;
    [SerializeField] Transform rightDestination;
    [SerializeField] LayerMask playerLayer;

    [SerializeField] private float enemySpeed;
    [SerializeField] private float minimumPatrolTime;
    [SerializeField] private float maximumPatrolTime;
    [SerializeField] private float attackTime;
    [SerializeField] private float stopTime;
    [SerializeField] private float linecastLenght;

    private float timeInCurrentState;

    private Vector2 _leftDestination;
    private Vector2 _rightDestination;

    private bool enemyDetected;

    [SerializeField] UnityEvent patrol, attack, stop;

    enum EnemyState
    {
        patrol, attack, stop
    }

    EnemyState enemyState = EnemyState.patrol;

    private void Start()
    {
        _leftDestination = leftDestination.position;
        _rightDestination = rightDestination.position;
        Destroy(leftDestination.gameObject);
        Destroy(rightDestination.gameObject);

        timeInCurrentState = Random.Range(minimumPatrolTime, maximumPatrolTime);
    }

    private void FixedUpdate()
    {
        enemyDetected = Physics2D.Linecast(transform.position, transform.position + Vector3.right * linecastLenght, playerLayer);

        Debug.DrawLine(transform.position, transform.position + Vector3.right * linecastLenght);

        if(transform.position.x != Mathf.Clamp(transform.position.x, _leftDestination.x, _rightDestination.x))
        {
            //flip player
            linecastLenght *= -1;
            enemySpeed *= -1;
            transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
        }

        if (enemyDetected)
        {
            timeInCurrentState = attackTime;
            enemyState = EnemyState.attack;
        }

        switch (enemyState)
        {
            case EnemyState.patrol:
                patrol?.Invoke();
                transform.position += Vector3.right * enemySpeed;
                SendPosition();
                ChangeStateInSeconds(stopTime, EnemyState.stop);
                break;
            case EnemyState.attack:
                attack?.Invoke();
                SendPosition();
                ChangeStateInSeconds(stopTime, EnemyState.stop);
                break;
            case EnemyState.stop:
                stop?.Invoke();
                ChangeStateInSeconds(Random.Range(minimumPatrolTime, maximumPatrolTime), EnemyState.patrol);
                break;
        }
    }

    private void SendPosition()
    { 
        Message message = Message.Create(MessageSendMode.Unreliable, ServerToClientID.enemyMovement);
        message.AddUShort(enemy.Id);
        message.AddUShort(NetworkManager.Singleton.CurrentTick);
        message.AddVector2(transform.position);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    private void ChangeStateInSeconds(float timeInNextState, EnemyState newState)
    {
        timeInCurrentState -= Time.fixedDeltaTime;
        if(timeInCurrentState <= 0)
        {
            timeInCurrentState = timeInNextState;
            enemyState = newState;
        }
    }
}

