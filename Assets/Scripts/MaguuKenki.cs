using Overtime.FSM;
using UnityEngine;
using UnityEngine.AI;
using Worq;
/// <summary>
/// @alex-memo 2023
/// </summary>
public class MaguuKenki : MonoBehaviour
{
	[SerializeField] private GameObject maskPrefab;
	[SerializeField] private Transform maskSpawnPoint;
	public bool TargetPlayer;
	private Animator anim;
	public bool IsInvincible = true;
	private bool isDead;

	public float Hp {get; private set;}
	public float MaxHp {get; private set;}=50;

	private bool isPlayerIn;

	private AWSPatrol patrol;
	public NavMeshAgent Agent{get; private set;}
	
	
	private const float rotationSpeed = 5f;
	private void Awake()
	{
		anim = GetComponent<Animator>();
		Hp = MaxHp;
		patrol = GetComponent<AWSPatrol>();
		Agent = GetComponent<NavMeshAgent>();
	}
	public void TakeDamage(float _damage)
	{
		if(isDead){return;}
		Hp-=_damage;
		if(Hp<1)
		{
			isDead=true;
			FSM.MakeTransition(KenkiStateTransition.STOP_ATTACK);
		}
	}
	private void OnTriggerEnter(Collider _coll)
	{
		if(isDead){return;}
		if (!_coll.CompareTag("Player")) { return; }
		patrol.enabled=false;
		Agent.ResetPath();
		FSM.OnTriggerEnter(_coll);
		
	}
	private void OnTriggerExit(Collider _coll)
	{
		if(isDead){return;}
		if (!_coll.CompareTag("Player")) { return; }
		patrol.enabled=true;
		IsInvincible = true;
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
		if(isDead){return;}
		animate(Agent.velocity.magnitude>.1f?1:0);
		if(TargetPlayer)
		{
			Agent.SetDestination(PlayerController.Instance.transform.position);
		}
		if (Agent.hasPath)
		{
			RotateToPlayer();
		}
		FSM.Update();
	}
	public void RotateToPlayer()
	{
		Vector3 targetDirection = (Agent.destination - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(targetDirection);
		transform.GetChild(0).rotation = Quaternion.Slerp(transform.GetChild(0).rotation, lookRotation, Time.deltaTime * rotationSpeed);
	}
	private void FixedUpdate()
	{
		FSM.FixedUpdate();
	}
	public void InstantiateMask()
	{
		print("mask");
		if(maskPrefab==null){return;}
		GameObject mask = Instantiate(maskPrefab, maskSpawnPoint.position, transform.rotation,null);
		//mask.GetComponent<Mask>().SetTarget(PlayerController.Instance.transform);
	}
}
