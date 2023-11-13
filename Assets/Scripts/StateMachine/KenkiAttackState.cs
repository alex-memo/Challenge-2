using System.Collections;
using UnityEngine;
/// <summary>
/// @alex-memo 2023
/// </summary>
public class KenkiAttackState : KenkiStateBase
{
	public override void BuildTransitions()
	{
		AddTransition(KenkiStateTransition.STOP_ATTACK, KenkiStateID.DEAD);
	}
	public override void Enter()
	{
		StartCoroutine(StartBehaviour());
	}
	public IEnumerator StartBehaviour()
	{
		Parent.Agent.ResetPath();
		Parent.IsInvincible = false;
		
		while(Parent.Hp>1)
		{
			yield return sendMaskToPlayer();	
			yield return walkToPlayer();
			yield return circularAttack();
			yield return walkToPlayer();
			yield return threeHitCombo();
		}		
	}

	private IEnumerator walkToPlayer()
	{
		Parent.TargetPlayer = true;
		float circularAttackRange=3;
		while(Vector3.Distance(transform.position,PlayerController.Instance.transform.position)>circularAttackRange)
		{
			yield return null;
		}
	}
	private IEnumerator sendMaskToPlayer()
	{
		Parent.Agent.ResetPath();
		transform.GetChild(0).LookAt(PlayerController.Instance.transform);
		Parent.TriggerAnim("MaskForward");
		
		yield return new WaitForSeconds(6);
	}
	private IEnumerator circularAttack()
	{
		Parent.TargetPlayer=false;
		Parent.Agent.ResetPath();
		Parent.TriggerAnim("ChargeSlash");
		//charge sword
		//do circular attack
		yield return new WaitForSeconds(11);
		Parent.TargetPlayer=true;
	}
	private IEnumerator threeHitCombo()
	{
		Parent.TargetPlayer=false;
		Parent.Agent.ResetPath();
		Parent.RotateToPlayer();
		Parent.TriggerAnim("Attack1");
		yield return new WaitForSeconds(3);
		Parent.RotateToPlayer();
		Parent.TriggerAnim("Attack2");
		yield return new WaitForSeconds(4);
		Parent.RotateToPlayer();
		Parent.TriggerAnim("Attack3");
		yield return new WaitForSeconds(5);
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
		Parent.Agent.isStopped = true;
		StopAllCoroutines();
		Debug.Log("Exit Attack State");
	}
}
