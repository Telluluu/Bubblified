using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamelogic
{
    public class MahoShojo : MonoBehaviour
    {
        [Header("怪物属性")]
        public int damage = 20;

        public float patrolSpeed = 1.0f;
        public float chaseSpeed = 1.5f;
        public float detectionRange = 5.0f;
        public float patrolRange = 4.0f;

        private Rigidbody2D m_rb;
        private SpriteRenderer m_sr;
        private Vector2 m_patrolStartPosition;
        private bool m_isChasing = false;
        private bool m_isMovingRight = false;

        private Animator m_animator;
        public bool isCaptured = false;

        private void Start()
        {
            m_rb = GetComponent<Rigidbody2D>();
            m_sr = GetComponent<SpriteRenderer>();
            m_animator = GetComponent<Animator>();
            m_patrolStartPosition = transform.position;
        }

        private void Update()
        {
            if (isCaptured == false)
            {
                DetectAndChase();
            }
        }

        public void Captured()
        {
            m_rb.simulated = false;
            isCaptured = true;
            m_animator.SetBool("isCaptured", isCaptured);
        }

        public void BreakAway()
        {
            if (m_rb != null)
                m_rb.simulated = true;

            isCaptured = false;
            m_animator.SetBool("isCaptured", isCaptured);
        }

        #region 巡逻逻辑

        private void DetectAndChase()
        {
            if (IsPlayerInRange())
            {
                m_isChasing = true;
            }
            else
            {
                m_isChasing = false;
            }

            if (m_isChasing)
            {
                ChasePlayer();
            }
            else
            {
                Patrol();
            }
            if (m_rb.velocity.x >= 0.0f)
            {
                m_sr.flipX = false;
            }
            else
            {
                m_sr.flipX = true;
            }
        }

        private bool IsPlayerInRange()
        {
            var player = GameManager.Instance.player;
            if (player != null)
            {
                return Vector2.Distance(transform.position, GameManager.Instance.player.transform.position) < detectionRange;
            }
            else
            {
                return false;
            }
        }

        private void Patrol()
        {
            if (((Vector2)transform.position - m_patrolStartPosition).magnitude > patrolRange + 2.0f)
            {
                Vector2 dir = m_patrolStartPosition - (Vector2)transform.position;
                dir.Normalize();
                m_rb.velocity = new Vector2(dir.x * patrolSpeed, m_rb.velocity.y);
            }
            else
            {
                // 获取当前位置和起始巡逻位置的距离
                float distanceFromStart = ((Vector2)transform.position - m_patrolStartPosition).magnitude;

                // 检查是否超出巡逻范围
                if (distanceFromStart >= patrolRange)
                {
                    // 如果超出巡逻范围，反转移动方向
                    m_isMovingRight = !m_isMovingRight;
                }

                // 根据当前移动方向设置速度
                float moveDirection = m_isMovingRight ? 1.0f : -1.0f;
                m_rb.velocity = new Vector2(moveDirection * patrolSpeed, m_rb.velocity.y);
            }
        }

        private void ChasePlayer()
        {
            var player = GameManager.Instance.player;

            if (player != null)
            {
                Vector2 dir2Player = player.transform.position - transform.position;
                dir2Player.Normalize();
                m_rb.velocity = new Vector2(dir2Player.x * chaseSpeed, m_rb.velocity.y);
            }
        }

        #endregion 巡逻逻辑

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (isCaptured == true)
                return;
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<PlayerController>().TakeDamage(damage, transform.position);
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
