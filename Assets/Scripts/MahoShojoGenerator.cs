using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

namespace Gamelogic
{
    public class MahoShojoGenerator : MonoBehaviour
    {
        public GameObject prefab;
        private MahoShojo m_mahoShojo;

        public int damage = 20;

        public float patrolSpeed = 1.0f;
        public float chaseSpeed = 1.5f;
        public float detectionRange = 5.0f;
        public float patrolRange = 4.0f;

        public float spawnTime = 5.0f;
        private float timer = 0.0f;

        private void Start()
        {
            if (prefab == null)
            {
                throw new System.Exception("Prefab is null");
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (m_mahoShojo == null)
            {
                if (timer > spawnTime)
                {
                    m_mahoShojo = Instantiate(prefab, transform.position, Quaternion.identity).GetComponent<MahoShojo>();
                    m_mahoShojo.patrolSpeed = patrolSpeed;
                    m_mahoShojo.chaseSpeed = chaseSpeed;
                    m_mahoShojo.detectionRange = detectionRange;
                    m_mahoShojo.patrolRange = patrolRange;
                    m_mahoShojo.damage = damage;
                    timer = 0.0f;
                }
                else
                {
                    timer += Time.deltaTime;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine((Vector2)transform.position - Vector2.right * detectionRange, (Vector2)transform.position + Vector2.right * detectionRange);
            Gizmos.color = Color.red;
            Gizmos.DrawLine((Vector2)transform.position - Vector2.right * patrolRange, (Vector2)transform.position + Vector2.right * patrolRange);
        }
    }
}
