using Gamelogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public Slider healthSlider;

        private void OnEnable()
        {
            EventManager.Instance.onPlayerHealthChanged.AddListener(SetHealth);
        }

        private void OnDisable()
        {
            EventManager.Instance?.onPlayerHealthChanged.RemoveListener(SetHealth);
        }

        public void SetHealth(int health)
        {
            var nowHealthPercent = health / 100.0f;
            healthSlider.value = nowHealthPercent;
        }
    }
}
