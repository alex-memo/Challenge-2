using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Worq
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AWSEntityIdentifier))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class AWSPatrol : MonoBehaviour
    {
        //group settings
        [Header("Group")] 
        [Space(10)]
        public WaypointRoute group;
        [HideInInspector] public int groupID = 0;

        //patrol settings
        [Header("Patrol")] [Space(10)] [Tooltip("Minimum amount of time to wait before moving to next patrol point")]
        public float minPatrolWaitTime = 1f;

        [Tooltip("Maximum amount of time to wait before moving to next patrol point")]
        public float maxPatrolWaitTime = 3f;

        [Tooltip("If or not all entities patrol waypoints at random or in sequence")]
        public bool randomPatroler = false;

//		[Tooltip("When you drag in any active gameObject into this slot the patrol entity will abandon current patrol" +
//		         "and go to this position. Can also be set from script by calling the static method " +
//		         "AWSPatrol.GoTo(position); The patrol entity will stop upon arriving this position. " +
//		         "Please tick the resetPatrol checkbox in the inspector, or call ")] 
//		public Transform goTo;

        //NavMesh Agent settings
        [Space(10)] [Header("Agent")] [Space(10)] [Tooltip("Speed by which the NavMesh agent moves")]
        public float moveSpeed = 3f;

        [Tooltip("The distance from destination the Navmesh agent stops")]
        public float stoppingDistance = 1f;

        [Tooltip("Turning speed of the NavMesh  agent")]
        public float angularSpeed = 500f;

        [Tooltip("Defines how high up the entity is. This is useful for creating flying entities")]
        public float distanceFromGround = 0.5f;

        //Debug
        [Space(10)] [Header("Debug")] [Space(10)]
        public bool resetPatrol;

        public bool interruptPatrol;
        public static bool reset;

        //private variables
        private AWSManager mAWSManager;
        private NavMeshAgent agent;
        private Animator anim;
        private AudioSource src;
        private Transform[] patrolPoints;
        private bool isWaiting;
        private bool hasPlayedDetectSound;
        private bool hasReachedGoTo;
        private int waypointCount;
        private int destPoint;

        public bool IsPatrolling = true;


        void Awake()
        {
            mAWSManager = FindObjectOfType<AWSManager>();

            agent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
            src = GetComponent<AudioSource>();

            try
            {
                waypointCount = 0;
                Transform groupTransform = group.transform;
                int childrenCount = groupTransform.childCount;

                for (int i = 0; i < childrenCount; i++)
                {
                    if (groupTransform.GetChild(i).GetComponent<WaypointIdentifier>())
                    {
                        waypointCount += 1;
                    }
                }

                patrolPoints = new Transform[waypointCount];
                int curIndex = 0;
                for (int i = 0; i < childrenCount; i++)
                {
                    if (groupTransform.GetChild(i).GetComponent<WaypointIdentifier>())
                    {
                        patrolPoints[curIndex] = groupTransform.GetChild(i);
                        if (patrolPoints[curIndex].gameObject.GetComponent<MeshRenderer>())
                            patrolPoints[curIndex].gameObject.GetComponent<MeshRenderer>().enabled = false;
                        if (patrolPoints[curIndex].gameObject.GetComponent<Collider>())
                            patrolPoints[curIndex].gameObject.GetComponent<Collider>().enabled = false;
                        curIndex++;
                    }
                }
            }
            catch 
            {
                Debug.LogWarning("Group not assigned for " + gameObject.name);
            }
        }

        void Start()
        {
            if (!TryGetComponent<Animator>(out anim))
                anim = gameObject.AddComponent<Animator>();

            agent.autoBraking = false;
            agent.stoppingDistance = stoppingDistance;
            agent.speed = moveSpeed;
            agent.angularSpeed = angularSpeed;
            agent.baseOffset = distanceFromGround;

            try
            {
                GotoNextPoint();
            }
            catch { }
        }

        void Update()
        {

            playAnimation(agent.velocity.magnitude>.1f?1:0);
            if(!IsPatrolling) { return; }
            if (resetPatrol || reset)
            {
                agent.isStopped = false;
                goToNextPointDirect();
                interruptPatrol = false;
                resetPatrol = false;
                reset = false;
            }

            if (interruptPatrol)
            {
                agent.isStopped = true;               
            }

            if (!interruptPatrol && !isWaiting && agent.remainingDistance <= stoppingDistance && null != group)
            {
                GotoNextPoint();
            }

            //updating variables

            agent.stoppingDistance = stoppingDistance;
            agent.speed = this.moveSpeed;
            agent.angularSpeed = angularSpeed;
            agent.baseOffset = distanceFromGround;
        }

        private void GotoNextPoint()
        {
            if (patrolPoints.Length == 0)
                return;
//			Debug.Log ("Going to next point...");
            StartCoroutine(pauseAndContinuePatrol());
        }

        IEnumerator pauseAndContinuePatrol()
        {
            isWaiting = true;

            float waitTime = Random.Range(minPatrolWaitTime, maxPatrolWaitTime);
            if (waitTime < 0f)
                waitTime = 1f;

            yield return new WaitForSeconds(waitTime);

            if (randomPatroler)
            {
                agent.destination = patrolPoints[destPoint].position;
                int nextPos;
                do
                {
                    nextPos = Random.Range(0, patrolPoints.Length);
                } while (nextPos == destPoint);

                destPoint = nextPos;
            }
            else
            {
                agent.destination = patrolPoints[destPoint].position;
                destPoint = (destPoint + 1) % patrolPoints.Length;
            }

            isWaiting = false;
        }

        void goToNextPointDirect()
        {
            if (randomPatroler)
            {
                agent.destination = patrolPoints[destPoint].position;
                int nextPos;
                do
                {
                    nextPos = Random.Range(0, patrolPoints.Length);
                } while (nextPos == destPoint);

                destPoint = nextPos;
            }
            else
            {
                agent.destination = patrolPoints[destPoint].position;
                destPoint = (destPoint + 1) % patrolPoints.Length;
            }
        }

        void RestartPatrol()
        {
            hasPlayedDetectSound = false;
            resetPatrol = false;
            agent.speed = moveSpeed;

            agent.stoppingDistance = 1f;
            goToNextPointDirect();
        }

        private void playAnimation(float _value, float _dampTime = .1f)
        {
            if (!hasAnimator()) { return; }
            anim.SetFloat("Speed", _value, _dampTime, Time.deltaTime);
        }
        private bool hasAnimator()
        {
            if (anim != null) { return true; }
            Debug.LogWarning("There is no Animator Component!");
            return false;
        }

        public void ResetPatrol()
        {
            resetPatrol = true;
        }

        public void InterruptPatrol()
        {
            interruptPatrol = true;
        }

        public void SetDeatination(Transform t)
        {
            agent.destination = t.position;
            isWaiting = false;
        }
    }
}