using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamelogic
{
    public class Camera : MonoBehaviour
    {
        private PlayerController m_player;

        private void Start()
        {
            m_player = FindAnyObjectByType<PlayerController>();
        }

        private void Update()
        {
            if (m_player == null)
            {
                m_player = FindAnyObjectByType<PlayerController>();
            }
            var pos = m_player.transform.position;
            pos.z = -10;
            transform.position = pos;
        }
    }
}
