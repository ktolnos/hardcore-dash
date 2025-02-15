using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float awareDistance;
    public float attackDistance;
    public GameObject gunObject;
    public NavMeshAgent agent;
    private GameObject player;
    private Gun gun;
    void Start()
    {
        player = FindAnyObjectByType<PlayerMovement>().GameObject();
        agent = GetComponent<NavMeshAgent>();
        gun = gunObject.GetComponent<Gun>();
    }


    void Update()
    {
        float currentDistance = Vector3.Distance(player.transform.position, transform.position);
        var ray = new Ray(transform.position, player.transform.position - transform.position);
        var did_hit = Physics.Raycast(ray, out RaycastHit hit, awareDistance, LayerMask.GetMask("Player"));
        if (currentDistance < awareDistance)
        {
            if (currentDistance < attackDistance)
            {
                if(did_hit)
                {
                    gun.Shoot(); 
                    transform.LookAt(player.transform.position);
                    agent.SetDestination(transform.position);
                }
                else
                {
                    float randomX = Random.Range(-0.5f, 0.5f);
                    float randomZ = Random.Range(-0.5f, 0.5f);
                    agent.SetDestination(transform.position + new Vector3(randomX, 0, randomZ));
                }
                
            }
            else
            {
                agent.SetDestination(player.transform.position);
            }
        }
    }
}
