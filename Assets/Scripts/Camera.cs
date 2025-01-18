using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamelogic
{
    public class Camera : MonoBehaviour
    {
        // Update is called once per frame
        private void Update()
        {
            var pos = GameManager.Instance.player.transform.position;
            pos.z = -10;
            transform.position = pos;
        }
    }
}
