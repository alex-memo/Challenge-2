using UnityEngine;
/// <summary>
/// @alex-memo 2023
/// </summary>
public class KenkiDeadState : KenkiStateBase
{
	public override void BuildTransitions()
	{
		AddTransition(KenkiStateTransition.START_DEAD, KenkiStateID.DEAD);
	}

	public override void Enter()
	{
		//anim death
		Debug.Log("Dead");
		Parent.TriggerAnim("Death");
	}

	public override void Update()
	{

	}

	public override void OnTriggerEnter(Collider _coll)
	{

	}

	public override void FixedUpdate()
	{

	}

	public override void Exit()
	{

	}
}
