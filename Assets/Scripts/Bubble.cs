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

        /// <summary>
        /// 0表示未捕获魔法少女的泡泡 1表示已捕获魔法少女的泡泡
        /// </summary>
        private int m_status = 0;

        private void Start()
        {
            m_rb = GetComponent<Rigidbody2D>();
            m_status = 0;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                if (m_status == 0)
                {
                    Vector2 dir = transform.position - collision.collider.transform.position;
                    if (m_rb == null)
                        m_rb = GetComponent<Rigidbody2D>();
                    m_rb.AddForce(dir * pushForce, ForceMode2D.Impulse);
                }
                else if (m_status == 1)
                {
                    Vector2 dir = collision.collider.transform.position - transform.position;
                    collision.gameObject.GetComponent<PlayerController>().BeBouncedOff(dir, bounceForce);
                }
            }
            else if (collision.collider.CompareTag("MahoShojo"))
            {
                m_status = 1;
            }
        }
    }
}
