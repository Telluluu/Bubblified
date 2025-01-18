using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace Gamelogic
{
    public class PlayerController : MonoBehaviour
    {
        [Header("移动")]
        public float moveSpeed = 0.01f;

        public float jumpSpeed = 0.01f;
        public float groundCheckDistance = 0.1f;
        public LayerMask groundCheckLayer;
        public float coyotaTime = 0.5f;

        [Header("发射泡泡")]
        public GameObject bubblePrefab;

        public float bubbleInstiateDistance = 1.5f;
        private GameObject m_bubble;

        [Header("玩家属性")]
        [Range(0, 100)]
        public int health = 100;

        public float invincibilityDuration = 0.5f;
        private float m_lastHitTime;

        private Rigidbody2D m_rb;
        private int lookAt;
        private int faceAt;
        private SpriteRenderer m_sr;

        [SerializeField]
        private bool m_isGround;

        private float m_coyotaTimer;

        private bool m_isJumped = false;

        private bool m_isBouncing;

        private void Start()
        {
            lookAt = 0;
            faceAt = 1;
            m_rb = GetComponent<Rigidbody2D>();
            m_sr = GetComponent<SpriteRenderer>();
            m_isBouncing = false;
            m_isJumped = false;
            EventManager.Instance.onPlayerHealthChanged.Invoke(health);
            m_lastHitTime = Time.time;
        }

        private void FixedUpdate()
        {
            CheckGround();
            Move();
            if (Keyboard.current.jKey.isPressed)
            {
                MakeBubble();
            }
        }

        private void Move()
        {
            if (Keyboard.current.kKey.isPressed && m_isJumped == false && (m_isGround || m_coyotaTimer < coyotaTime))
            {
                m_rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse); // 使用瞬时力量跳跃
                m_isJumped = true;
            }

            // 控制水平移动
            float horizontalInput = 0f;

            if (Keyboard.current.aKey.isPressed)
            {
                horizontalInput = -1f; // 向左移动
                faceAt = -1;
                m_sr.flipX = true;
            }
            else if (Keyboard.current.dKey.isPressed)
            {
                horizontalInput = 1f; // 向右移动
                faceAt = 1;
                m_sr.flipX = false;
            }
            // 平滑地调整水平速度
            if (m_isBouncing == false)
                m_rb.velocity = new Vector2(horizontalInput * moveSpeed, m_rb.velocity.y);

            if (Keyboard.current.wKey.isPressed == true)
            {
                lookAt = 1;
            }
            else if (Keyboard.current.sKey.isPressed == true)
            {
                lookAt = -1;
            }
            else
            {
                lookAt = 0;
            }
        }

        private void CheckGround()
        {
            var hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundCheckLayer);
            m_isGround = hit.collider != null ? true : false;
            if (m_isGround == false)
            {
                m_coyotaTimer += Time.deltaTime;
            }
            else
            {
                m_coyotaTimer = 0;
                m_isJumped = false;
            }
            if (m_rb.velocity.magnitude < 2.0f)
                m_isBouncing = false;
        }

        private void MakeBubble()
        {
            if (m_bubble != null)
            {
                Destroy(m_bubble);
            }
            m_bubble = GameObject.Instantiate(bubblePrefab);
            if (lookAt != 0)
            {
                m_bubble.transform.position = (Vector2)transform.position + lookAt * Vector2.up * bubbleInstiateDistance;
            }
            else
            {
                m_bubble.transform.position = (Vector2)transform.position + faceAt * Vector2.right * bubbleInstiateDistance;
            }
        }

        public void BouncedOff(Vector2 dir, float force)
        {
            Debug.Log("BeBouncedOff:Dir = " + dir + "Force = " + force);
            m_rb.AddForce(dir * force, ForceMode2D.Impulse);
            m_isBouncing = true;
        }

        public void TakeDamage(int damage)
        {
            if (Time.time - m_lastHitTime < invincibilityDuration)
            {
                return;
            }
            else
            {
                m_lastHitTime = Time.time;
            }

            health -= damage;
            if (health <= 0)
            {
                health = 0;
                EventManager.Instance.onPlayerHealthChanged.Invoke(health);
                EventManager.Instance.onPlayerDied.Invoke();
            }
            else
            {
                EventManager.Instance.onPlayerHealthChanged.Invoke(health);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
        }
    }
}
