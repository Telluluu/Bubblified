using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Gamelogic
{
    public class EventManager : Singleton<EventManager>
    {
        public UnityEvent<int> onPlayerHealthChanged = new UnityEvent<int>();
        public UnityEvent onPlayerDied = new UnityEvent();
    }
}
