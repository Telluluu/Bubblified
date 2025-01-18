using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamelogic
{
    public class GameManager : Singleton<GameManager>
    {
        public PlayerController player;

        // Start is called before the first frame update
        private void Start()
        {
            player = GameObject.FindAnyObjectByType<PlayerController>();
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}
