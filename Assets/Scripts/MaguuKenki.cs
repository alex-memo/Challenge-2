using Overtime.FSM;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Worq;
/// <summary>
/// @alex-memo 2023
/// </summary>
public class MaguuKenki : MonoBehaviour
{
    private Animator anim;
    public bool IsInvincible = true;

    private float hp;
    private float maxHp = 100;

    private bool isPlayerIn;

    private AWSPatrol patrol;
    public NavMeshAgent Agent;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        hp = maxHp;
        patrol = GetComponent<AWSPatrol>();
        StartCoroutine(waitUntilPlayerOut());
        Agent = GetComponent<NavMeshAgent>();
    }
    public void TakeDamage()
    {
        FSM.MakeTransition(KenkiStateTransition.STOP_ATTACK);
    }
    private void OnTriggerEnter(Collider _coll)
    {
        if (!_coll.CompareTag("Player")) { return; }
        isPlayerIn = true;
        FSM.OnTriggerEnter(_coll);
        
    }
    private void OnTriggerExit(Collider _coll)
    {
        if (!_coll.CompareTag("Player")) { return; }
        isPlayerIn = false;
    }

    private IEnumerator waitUntilPlayerOut()
    {
        yield return new WaitUntil(() => isPlayerIn);
        patrol.IsPatrolling= false;
        yield return new WaitUntil(() => !isPlayerIn);
        patrol.IsPatrolling = true;
        IsInvincible = true;
        StartCoroutine(waitUntilPlayerOut());
    }
    private void animate(float _value, float _dampTime = .1f)
    {
        if (!hasAnimator()) { return; }
        anim.SetFloat("Speed", _value, _dampTime, Time.deltaTime);
    }
    public void TriggerAnim(string _trigger)
    {
        if (!hasAnimator()) { return; }
        anim.SetTrigger(_trigger);
    }
    private bool hasAnimator()
    {
        if (anim != null) { return true; }
        Debug.LogWarning("There is no Animator Component!");
        return false;
    }
    public StateMachine<MaguuKenki, KenkiStateID, KenkiStateTransition> FSM { get; private set; }

    private const KenkiStateID initialState = KenkiStateID.WANDERING;

    public ScriptableObject[] States;


    private void OnDestroy()
    {
        FSM.Destroy();
    }

    private void Start()
    {
        FSM = new StateMachine<MaguuKenki, KenkiStateID, KenkiStateTransition>(this, States, initialState);
    }

    private void Update()
    {
        FSM.Update();
    }

    private void FixedUpdate()
    {
        FSM.FixedUpdate();
    }
}
