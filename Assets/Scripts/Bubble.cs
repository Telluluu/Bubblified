using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamelogic
{
    public class Bubble : MonoBehaviour
    {
        private Rigidbody2D m_rb;
        public float pushForce = 30.0f;
        public float bounceForce = 30.0f;
        private SpriteRenderer m_sr;
        private MahoShojo m_capturedMahoShojo = null;

        // 是否捕获到魔法少女
        private bool m_isCaptured = false;

        /// <summary>
        /// 动画状态，0表示创建，1表示idle，2表示破裂
        /// </summary>
        private int m_status = 0;

        private Animator m_animator;

        private void OnEnable()
        {
            m_rb = GetComponent<Rigidbody2D>();
            m_status = -1;
            m_isCaptured = false;
            m_animator = GetComponent<Animator>();
            m_sr = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (m_isCaptured)
            {
                if (m_capturedMahoShojo != null)
                {
                    m_capturedMahoShojo.transform.position = transform.position;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                InteractWithPlayer(collision);
            }
            else if (collision.collider.CompareTag("MahoShojo"))
            {
                InteractWithMahoShojo(collision);
            }
        }

        public void Create()
        {
            m_status = 0;
            m_animator.SetInteger("status", m_status);
        }

        public void Idle()
        {
            m_status = 1;
            m_animator.SetInteger("status", m_status);
        }

        public void BubbleBurst()
        {
            Audio.AudioManager.Instance.PlayFX("burst");
            m_status = 2;
            m_animator.SetInteger("status", m_status);
        }

        public void DestroyThis()
        {
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (m_isCaptured)
            {
                m_capturedMahoShojo?.BreakAway();
            }

            m_isCaptured = false;
        }

        private void InteractWithPlayer(Collision2D collision)
        {
            if (m_isCaptured == false)
            {
                Vector2 dir = transform.position - collision.collider.transform.position;
                if (m_rb == null)
                    m_rb = GetComponent<Rigidbody2D>();
                m_rb.AddForce(dir * pushForce, ForceMode2D.Impulse);
            }
            else if (m_isCaptured == true)
            {
                Destroy(m_capturedMahoShojo.gameObject);

                Vector2 dir = collision.collider.transform.position - transform.position;
                collision.gameObject.GetComponent<PlayerController>().BouncedOff(dir, bounceForce);
                BubbleBurst();
                m_isCaptured = false;
            }
        }

        private void InteractWithMahoShojo(Collision2D collision)
        {
            m_isCaptured = true;
            m_rb.velocity = Vector2.zero;
            m_capturedMahoShojo = collision.gameObject.GetComponent<MahoShojo>();
            if (m_capturedMahoShojo.isCaptured != true)
            {
                var color = m_sr.color;
                color.a = 120.0f / 255.0f;
                m_sr.color = color;
                m_capturedMahoShojo.Captured();
                m_capturedMahoShojo.transform.position = transform.position;
                m_rb.velocity = Vector2.zero;
            }
        }
    }
}
