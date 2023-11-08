using Overtime.FSM.Example;
using UnityEngine;
/// <summary>
/// @alex-memo 2023
/// </summary>
public class KenkiWanderState : KenkiStateBase
{
    public override void BuildTransitions()
    {
        AddTransition(KenkiStateTransition.STOP_WANDER, KenkiStateID.ATTACK);
    }

    public override void Enter()
    {
        
    }

    public override void Update()
    {

    }

    public override void OnTriggerEnter(Collider _coll)
    {
        if (!_coll.CompareTag("Player")) { return; }
        MakeTransition(KenkiStateTransition.STOP_WANDER);
    }

    public override void FixedUpdate()
    {

    }

    public override void Exit()
    {

    }
}
