using System;
using System.Runtime.InteropServices;
using Core;
using Dependencies;
using Extensions;
using Saves;
using UnityEngine;
using Weapon;

namespace UI
{
    public class EndGameScreen
    {
        [DllImport("__Internal")]
        private static extern void ShowYandexRewarded(int value);
        
        [DllImport("__Internal")]
        private static extern void ShowYandexInterstitialAdv();

        private int counterForShowingInterstital = 0;

        private class MoneyTracker
        {
            private int currentMoney => DataSaveLoader.SerializableData.money.Value;
            private int startingMoney;
            
            public MoneyTracker()
            {
                GameEvents.GameStarted.Event += () => startingMoney = currentMoney;
            }

            public int GetCollectedMoneyOnRound()
            {
                return currentMoney - startingMoney;
            }
        }

        private Action onScreenInteractionEnd;
            
        private MoneyTracker moneyTracker;
        private UIDependencies dependencies;
        private int collectableMoney;
        
        public EndGameScreen(UIDependencies uiDependencies)
        {
            moneyTracker = new MoneyTracker();
            dependencies = uiDependencies;
            
            uiDependencies.noThanksButton.onClick.AddListener(SkipAd);
            uiDependencies.viewAdButton.onClick.AddListener(ShowAd);
        }

        public void ShowWinScreen()
        {
            collectableMoney = moneyTracker.GetCollectedMoneyOnRound() * 3;

            Show();

            dependencies.endGameIcon.sprite = dependencies.winIconSprite;
            onScreenInteractionEnd = GameEvents.GameEndedByWin.Invoke;

            dependencies.aboveEndGameIcon.enabled = false;
        }

        public void ShowLoseScreen()
        {
            collectableMoney = moneyTracker.GetCollectedMoneyOnRound() * 3;

            Show();
            
            dependencies.endGameIcon.sprite = dependencies.loseIconSprite;
            onScreenInteractionEnd = GameEvents.GameEndedByLose.Invoke;
            
            dependencies.aboveEndGameIcon.enabled = true;
        }

        private void Show()
        {
            GameEvents.EndScreenShowed.Invoke();
            
            dependencies.collectedMoneyOnRoundMultiplyText.text = collectableMoney.ToString();
            dependencies.endGameCanvas.gameObject.SetActive(true);
            dependencies.crosshairCanvas.gameObject.SetActive(false);
            
            //Vadim
            dependencies.progressBarLvl.gameObject.SetActive(false);
            dependencies.progressBar.gameObject.SetActive(false);
            //here
            
            UnityEvents.Update += RotateGlow;
        }

        
        private void Hide()
        {
            dependencies.endGameCanvas.gameObject.SetActive(false);
            
            UnityEvents.Update -= RotateGlow;
        }

        private void RotateGlow()
        {
            dependencies.rotatableGlow.rectTransform.Rotate(Vector3.forward, dependencies.rotateGlowSpeed * Time.deltaTime);
        }

        private void ShowAd()
        {
            //yandex reward
            ShowYandexRewarded(collectableMoney);
            
            // AdController.ShowRewarded(GiveMoneyReward, EndScreenInteraction);
            
        }

        private void SkipAd()
        {
            counterForShowingInterstital++;

            if (counterForShowingInterstital % 5 == 0)
            {
                ShowYandexInterstitialAdv();
            }
            else 
                EndScreenInteraction();
            // AdController.ShowInterstitial(EndScreenInteraction);

        }

        public void EndScreenInteraction()
        {
            Hide();
            onScreenInteractionEnd();
        }

        public void GiveMoneyReward()//int amountCoinForRewarded)
        {
            DataSaveLoader.SerializableData.money.Value += collectableMoney;
            EndScreenInteraction();
        }
    }
}