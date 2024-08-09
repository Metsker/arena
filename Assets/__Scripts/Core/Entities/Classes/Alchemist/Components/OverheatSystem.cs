using System;
using System.Collections;
using DG.Tweening;
using Tower.Core.Entities.Classes.Alchemist.Actions.Potions;
using Tower.Core.Entities.Classes.Alchemist.Data;
using Tower.Core.Entities.Classes.Alchemist.Enums;
using Tower.Core.Entities.Classes.Common.Components.InputActions;
using Tower.Core.Entities.Classes.Common.Components.UI;
using Tower.Core.Entities.Classes.Common.UI;
using Tower.Core.Entities.Common.Data;
using Tower.Core.Entities.Common.Interfaces.Toggleables;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using Observable = UniRx.Observable;

namespace Tower.Core.Entities.Classes.Alchemist.Components
{
    public class OverheatSystem : NetworkBehaviour
    {
        public bool IsOverheated => _overheat.Value == _alchemistDataContainer.MaxOverheat;
        public bool IsCold => _overheat.Value == 0;
        private AlchemistStaticData AlchemistStaticData => _classStaticData.alchemistStaticData;
        private ProgressBar ProgressBar => _playerLocalCanvas.ProgressBar;

        private readonly CompositeDisposable _disposable = new ();

        private readonly NetworkVariable<int> _overheat = new (writePerm: NetworkVariableWritePermission.Owner);

        private PotionLauncher _potionLauncher;
        private AlchemistDataContainer _alchemistDataContainer;
        private ClassStaticData _classStaticData;
        private ActionToggler _actionToggler;
        private PlayerLocalCanvas _playerLocalCanvas;
        
        private bool _speedChanged;

        [Inject]
        private void Construct(
            PotionLauncher potionLauncher,
            AlchemistDataContainer alchemistDataContainer,
            ClassStaticData classStaticData,
            ActionToggler actionToggler,
            PlayerLocalCanvas playerLocalCanvas)
        {
            _playerLocalCanvas = playerLocalCanvas;
            _potionLauncher = potionLauncher;
            _alchemistDataContainer = alchemistDataContainer;
            _classStaticData = classStaticData;
            _actionToggler = actionToggler;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                return;

            ProgressBar.Show(true);
            Enable();
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner)
                return;
            
            ProgressBar.Hide();
            Disable();
            _disposable.Dispose();
        }

        public void Enable() =>
            _potionLauncher.PotionFired += OnPotionFired;

        public void Disable()
        {
            _disposable.Clear();
            _potionLauncher.PotionFired -= OnPotionFired;
        }

        public void ResetOverheat()
        {
            _overheat.Value = 0;
            if (_speedChanged)
            {
                _alchemistDataContainer.AddSpeed(-AlchemistStaticData.OverheatSpeedBuff);
                _speedChanged = false;
            }
            _actionToggler.Enable<IToggleableAttack>();
        }

        private void OnPotionFired(PotionType type)
        {
            if (IsOverheated)
                return;

            AddOverheat(type);

            ProgressBar.SetColor(new Color(1f, 0.41f, 0f));
            ProgressBar.Fill((float)_overheat.Value / _alchemistDataContainer.MaxOverheat)
                .OnComplete(() =>
                {
                    _disposable.Clear();
                   
                    if (IsOverheated)
                        Observable.FromCoroutine(OverheatCoroutine).Subscribe().AddTo(_disposable);
                    else
                        Observable.FromCoroutine(ColdOutCoroutine).Subscribe().AddTo(_disposable);
                });
        }

        private void AddOverheat(PotionType type)
        {
            _overheat.Value += _alchemistDataContainer.PotionsStats.GetOverheatValue(type);
            Clamp();
        }

        private IEnumerator OverheatCoroutine()
        {
            //Play fx
            _actionToggler.Disable<IToggleableAttack>();
            _alchemistDataContainer.AddSpeed(AlchemistStaticData.OverheatSpeedBuff);
            _speedChanged = true;

            ProgressBar.PunchScale();
            ProgressBar.SetColor(Color.red);

            yield return new WaitForSeconds(AlchemistStaticData.OverheatSec);

            ProgressBar.Fill(0, AlchemistStaticData.ResetSec).OnComplete(ResetOverheat);
        }

        private IEnumerator ColdOutCoroutine()
        {
            yield return new WaitForSeconds(AlchemistStaticData.ColdOutDelay);

            _overheat.Value = 0;
            ProgressBar.Fill(0, AlchemistStaticData.ResetSec);
        }

        private void Clamp() =>
            _overheat.Value = Mathf.Clamp(_overheat.Value, 0, _alchemistDataContainer.MaxOverheat);
    }
}
