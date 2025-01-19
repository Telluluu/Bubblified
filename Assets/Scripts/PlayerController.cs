using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gamelogic
{
    public class PlayerController : MonoBehaviour
    {
        [Header("移动")]
        public float moveSpeed = 0.01f;

        public float jumpSpeed = 0.01f;
        public float groundCheckDistance = 0.5f;
        public float inteval = 0.5f;
        public LayerMask groundCheckLayer;
        public LayerMask groundLayer;

        [Header("跳跃手感优化")]
        public float coyotaTime = 0.5f;

        public float jumpBufferTime = 0.5f;
        private float m_jumpBufferTimer = 0.0f;
        public float gravityScaleHalved = 0.5f;
        public float gravityScaleNormal = 1.0f;

        [Header("发射泡泡")]
        public GameObject bubblePrefab;

        public Vector2 bubbleInstiateDistance = new Vector2(2.0f, 0.8f);
        private Bubble m_bubble;
        private bool m_isCreating = false;

        [Header("玩家属性")]
        [Range(0, 100)]
        public int health = 100;

        public float invincibilityDuration = 0.5f;
        private float m_lastHitTime;
        public float injuryForce = 1.0f;

        public float injuryTime = 1.0f;
        private float m_injuryTimer = 0.0f;
        private Rigidbody2D m_rb;

        private int lookAt;
        private int faceAt;
        private SpriteRenderer m_sr;

        [Header("动画")]
        private Animator m_animator;

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
            m_animator = GetComponent<Animator>();
        }

        private void Update()
        {
            CheckGround();
            m_jumpBufferTimer -= Time.deltaTime;
            if (m_injuryTimer > 0.0f)
            {
                m_injuryTimer -= Time.deltaTime;
            }
            else
            {
                m_injuryTimer = 0.0f;
            }
            if (Keyboard.current.jKey.wasPressedThisFrame && m_isCreating == false)
            {
                if (m_bubble == null)
                {
                    m_isCreating = true;
                    m_animator.SetTrigger("Bubble");
                }
                else
                {
                    m_bubble.GetComponent<Bubble>().BubbleBurst();
                }
            }
        }

        private void FixedUpdate()
        {
            if (m_injuryTimer <= 0.0f)
            {
                Move();
            }
            AdjustGravity();
            SetAnimation();
        }

        #region 移动

        private void Move()
        {
            if (Keyboard.current.kKey.isPressed)
            {
                if (m_isJumped == false && (m_isGround || (m_coyotaTimer < coyotaTime)))
                {
                    m_rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse); // 使用瞬时力量跳跃
                    m_isJumped = true;
                    m_jumpBufferTimer = 0;
                }
                else
                {
                    m_jumpBufferTimer = jumpBufferTime;
                }
            }
            else if (m_jumpBufferTimer > 0)
            {
                if (m_isJumped == false && (m_isGround || (m_coyotaTimer < coyotaTime)))
                {
                    m_rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse); // 使用瞬时力量跳跃
                    m_isJumped = true;
                    m_jumpBufferTimer = 0;
                }
            }

            // 控制水平移动
            float horizontalInput = 0f;

            if (Keyboard.current.aKey.isPressed)
            {
                var hit1 = Physics2D.Raycast((Vector2)transform.position + Vector2.up * inteval, Vector2.left,
                    groundCheckDistance, groundCheckLayer);
                var hit2 = Physics2D.Raycast((Vector2)transform.position + Vector2.down * inteval, Vector2.left,
                    groundCheckDistance, groundCheckLayer);
                var hit3 = Physics2D.Raycast(transform.position, Vector2.left, horizontalInput, groundLayer);
                bool hit = hit1.collider != null || hit2.collider != null || hit3.collider != null;
                if (hit == false)
                {
                    horizontalInput = -1f; // 向左移动
                }
                else
                {
                    Debug.Log("撞到墙");
                    horizontalInput = 0f;
                }
                faceAt = -1;
                m_sr.flipX = true;
            }
            else if (Keyboard.current.dKey.isPressed)
            {
                var hit1 = Physics2D.Raycast((Vector2)transform.position + Vector2.up * inteval, Vector2.right,
                    groundCheckDistance, groundCheckLayer);
                var hit2 = Physics2D.Raycast((Vector2)transform.position + Vector2.down * inteval, Vector2.right,
                    groundCheckDistance, groundCheckLayer);
                var hit3 = Physics2D.Raycast(transform.position, Vector2.right, horizontalInput, groundLayer);
                bool hit = hit1.collider != null || hit2.collider != null || hit3.collider != null;
                if (hit == false)
                {
                    horizontalInput = 1f; // 向右移动
                }
                else
                {
                    Debug.Log("撞到墙");
                    horizontalInput = 0f;
                }
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

        private void AdjustGravity()
        {
            if (m_isGround == true)
                return;
            if (m_rb.velocity.y < 0.0f)
            {
                m_rb.gravityScale = gravityScaleHalved;
            }
            else
            {
                m_rb.gravityScale = gravityScaleNormal;
            }
        }

        private void CheckGround()
        {
            if (m_rb.velocity.y > 0f)
            {
                m_isGround = false;
            }
            else
            {
                var leftHit = Physics2D.Raycast((Vector2)transform.position - new Vector2(inteval, 0),
                    Vector2.down, groundCheckDistance, groundCheckLayer);
                var midHit = Physics2D.Raycast((Vector2)transform.position, Vector2.down, groundCheckDistance, groundCheckLayer);
                var rightHit = Physics2D.Raycast((Vector2)transform.position + new Vector2(inteval, 0),
                    Vector2.down, groundCheckDistance, groundCheckLayer);
                var hit = leftHit.collider != null || rightHit.collider != null || midHit.collider != null;
                m_isGround = hit ? true : false;
            }
            if (m_isGround == false)
            {
                m_coyotaTimer += Time.deltaTime;
            }
            else
            {
                m_coyotaTimer = 0;
                m_isJumped = false;
            }
            if (m_rb.velocity.magnitude < 4.0f)
                m_isBouncing = false;
        }

        #endregion 移动

        #region Bubble

        private void MakeBubble()
        {
            m_bubble = GameObject.Instantiate(bubblePrefab).GetComponent<Bubble>();

            if (lookAt != 0)
            {
                m_bubble.transform.position = (Vector2)transform.position + lookAt * Vector2.up * bubbleInstiateDistance.y;
            }
            else
            {
                m_bubble.transform.position = (Vector2)transform.position + faceAt * Vector2.right * bubbleInstiateDistance.x;
            }
            m_bubble.Create();
            m_isCreating = false;
        }

        public void BouncedOff(Vector2 dir, float force)
        {
            m_rb.AddForce(dir * force, ForceMode2D.Impulse);
            m_isBouncing = true;
        }

        #endregion Bubble

        #region 动画

        private void SetAnimation()
        {
            m_animator.SetInteger("velocityY", (int)m_rb.velocity.y);
            m_animator.SetBool("isGround", m_isGround);
        }

        #endregion 动画

        #region 受击

        public void TakeDamage(int damage, Vector2 pos)
        {
            if (Time.time - m_lastHitTime < invincibilityDuration)
            {
                return;
            }
            else
            {
                m_lastHitTime = Time.time;
            }
            Vector2 dir = (Vector2)transform.position - pos;
            m_rb.AddForce(dir * injuryForce, ForceMode2D.Impulse);
            m_injuryTimer = injuryTime;
            m_animator.SetTrigger("Injury");

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

        public void TriggerFlash()
        {
        }

        //private IEnumerator FlashCorotine()
        //{
        //    float timer = 0.0f;
        //    while(timer < flash)
        //}

        #endregion 受击

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            #region 地面检测

            Vector2 leftOrigin = (Vector2)transform.position + Vector2.left * inteval;
            Vector2 rightOrigin = (Vector2)transform.position + Vector2.right * inteval;
            Gizmos.DrawLine(leftOrigin, leftOrigin + Vector2.down * groundCheckDistance);
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.down * groundCheckDistance);
            Gizmos.DrawLine(rightOrigin, rightOrigin + Vector2.down * groundCheckDistance);

            #endregion 地面检测

            #region 水平检测

            Gizmos.DrawLine((Vector2)transform.position + Vector2.up * inteval,
                (Vector2)transform.position + Vector2.left * groundCheckDistance + Vector2.up * inteval);
            Gizmos.DrawLine((Vector2)transform.position + Vector2.down * inteval,
                (Vector2)transform.position + Vector2.left * groundCheckDistance + Vector2.down * inteval);
            Gizmos.DrawLine((Vector2)transform.position + Vector2.up * inteval,
                (Vector2)transform.position + Vector2.right * groundCheckDistance + Vector2.up * inteval);
            Gizmos.DrawLine((Vector2)transform.position + Vector2.down * inteval,
                (Vector2)transform.position + Vector2.right * groundCheckDistance + Vector2.down * inteval);

            #endregion 水平检测
        }
    }
}
