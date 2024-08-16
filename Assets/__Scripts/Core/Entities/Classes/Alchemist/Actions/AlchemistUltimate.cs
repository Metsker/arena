using System;
using DG.Tweening;
using Tower.Core.Entities.Classes.Alchemist.Components;
using Tower.Core.Entities.Classes.Alchemist.Data;
using Tower.Core.Entities.Classes.Common.Components.InputActions;
using Tower.Core.Entities.Classes.Common.Components.UI;
using Tower.Core.Entities.Classes.Common.Stats.DataContainers;
using UnityEngine;
using VContainer;

namespace Tower.Core.Entities.Classes.Alchemist.Actions
{
    public class AlchemistUltimate : ClassUltimateBase
    {
        [SerializeField] private OverheatSystem overheatSystem;

        private AlchemistDataContainer _dataContainer;
        private PlayerLocalCanvas _localCanvas;

        [Inject]
        private void Construct(AlchemistDataContainer dataContainer,PlayerLocalCanvas localCanvas)
        {
            _dataContainer = dataContainer;
            _localCanvas = localCanvas;
        }
        
        protected override Sequence PreformUltimate()
        {
            return DOTween.Sequence()
                .AppendCallback(() =>
                {
                    _dataContainer.AddAttackSpeedRpc(_dataContainer.AlchemistStats.ultASBuff);
                    overheatSystem.Disable();
                    overheatSystem.ResetOverheat();

                    DOTween.Sequence()
                        .AppendCallback(() => _localCanvas.ProgressBar.SetColor(Color.blue))
                        .Append(_localCanvas.ProgressBar.Fill(1, 0))
                        .Join(_localCanvas.ProgressBar.PunchScale())
                        .Append(_localCanvas.ProgressBar.Fill(0, Duration));

                })
                .AppendInterval(Duration)
                .AppendCallback(() =>
                {
                    _dataContainer.AddAttackSpeedRpc(-_dataContainer.AlchemistStats.ultASBuff);
                    overheatSystem.Enable();
                });
        }

        protected override IClassDataContainer ClassDataContainer() =>
            _dataContainer;
    }
}
