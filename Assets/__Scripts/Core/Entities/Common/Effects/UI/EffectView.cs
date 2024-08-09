using System.Globalization;
using Assemblies.Utilities.Timers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tower.Core.Entities.Common.Effects.UI
{
    public class EffectView : MonoBehaviour
    {
        [SerializeField] private Image circle;
        [SerializeField] private TextMeshProUGUI stacksText;

        public readonly CountdownTimer Timer = new (0);

        private void Update()
        {
            if (Timer.IsRunning)
            {
                Timer.Tick(Time.deltaTime);
                SetAmount(Timer.Progress);
            }
            
            ConsoleProDebug.Watch("ui_timer", Timer.GetTime().ToString(CultureInfo.InvariantCulture));
        }
        
        private void SetAmount(float amount) =>
            circle.fillAmount = amount;

        public void SetSprite(Sprite sprite) =>
            circle.sprite = sprite;

        public void SetStacks(int stacks)
        {
            stacksText.gameObject.SetActive(true);
            stacksText.SetText(stacks.ToString());
        }
        
        public void RemoveStacks() =>
            stacksText.gameObject.SetActive(false);
    }
}
