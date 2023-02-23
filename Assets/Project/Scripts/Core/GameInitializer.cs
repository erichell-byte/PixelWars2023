using System;
using System.Collections;
using Controller;
using Environments;
using Extensions;
using Saves;
using Services;
using TMPro;
using Tutorial;
using UI;
using Unity.Collections;
using UnityEngine;
using Upgrades;
using Wall;
using Weapon;

namespace Core
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField]
        private DependenciesData dependencies;
        [SerializeField]
        private SerializableDataSave save;

        private DataSaveLoader saveLoader;

        private int loadedWallIndex;
        private WallManager wallManager;
        private Crosshair crosshair;
        private UpgradeSystem upgradeSystem;
        private CameraCornerFollower cornerFollower;
        private WallDestroyingObserver wallDestroyingObserver;
        private WeaponManager weaponManager;
        private GameScreen gameScreen;
        private EndGameScreen endGameScreen;
        private TutorialManager tutorialManager;
        private AdController adController;

        public static int sWallIndex;

        private void Awake()
        {
            InitializeSaves();
        }

        private void Start()
        {
            InitializeEnvironment();
            
            InitializeCubes();
            InitializeCrosshair();
            InitializeWeapon();
            InitializeUpgrades();
            InitializeCamera();

            InitializeUI();
            InitializeTutorial();
            
            InitializeAnalyticsEvents();
            InitializeAds();
            InitializeGameEvents();
        }

        private void InitializeEnvironment()
        {
            new CloudsManager(dependencies.environmentDependencies);
        }

        private void InitializeSaves()
        {
            saveLoader = new DataSaveLoader();
            save = saveLoader.LoadData();
            sWallIndex = save.wallIndex.Value;
        }

        public void SetPlayerInfo(string value)
        {
            DataSaveLoader.SetPlayerInfo(value);
           
        }
        public void AddCoinForYandexRewarded(int coinAmount)
        {
            endGameScreen.GiveMoneyReward();
        }

        public void InterstitialShowed()
        {
            endGameScreen.EndScreenInteraction();
        }

        private void InitializeCrosshair()
        {
            crosshair = new Crosshair(dependencies.crosshairReferences);
        }

        private void InitializeWeapon()
        {
            weaponManager = new WeaponManager(dependencies.weaponReferences, crosshair);
        }

        private void InitializeUpgrades()
        {
            upgradeSystem = new UpgradeSystem(dependencies.upgradesData, weaponManager, save.upgrades);
        }

        private void InitializeCamera()
        {
            cornerFollower = new CameraCornerFollower(dependencies.cameraDependencies);
            cornerFollower.SaveCurrentPositionAsDefault();
            cornerFollower.SetNewCorner(wallManager.farCameraCorner);
        }

        private void InitializeCubes()
        {
            wallManager = new  WallManager(dependencies.wallConfig);
            wallDestroyingObserver = new WallDestroyingObserver(wallManager);

            LoadWallFromSave();

            new CubeMoneyConvertArea(save.money, dependencies.converterDependencies);
        }

        private void InitializeUI()
        {
            gameScreen = new GameScreen(dependencies.uiDependencies, upgradeSystem);

            weaponManager.ammo.DataChanged += gameScreen.SetAmmo;
            save.money.DataChanged += gameScreen.SetMoney;
            save.wallIndex.DataChanged += gameScreen.SetNewLevelOnProgressBar;
            
            gameScreen.SetMoney(save.money.Value);
            gameScreen.SetAmmo(weaponManager.ammo.Value);
            gameScreen.SetNewLevelOnProgressBar(save.wallIndex.Value);

            endGameScreen = new EndGameScreen(dependencies.uiDependencies);
        }

        private void InitializeTutorial()
        {
            tutorialManager = new TutorialManager(save.tutorialIndex);
            tutorialManager.AddTutorialPart(new ButtonTutorial(dependencies.tutorialDependencies, dependencies.tutorialDependencies.playButton));
            tutorialManager.AddTutorialPart(new ShootTutorial(dependencies.tutorialDependencies, crosshair));
            tutorialManager.AddTutorialPart(new UpgradeTutorial(dependencies.tutorialDependencies, save.money));
            
            if (!tutorialManager.IsFinished())
                tutorialManager.StartTutorial();
        }

        private void InitializeAnalyticsEvents()
        {
            GameEvents.GameEndedByLose.Event += AnalyticsController.SendLevelFailEvent;
            GameEvents.GameEndedByWin.Event += AnalyticsController.SendLevelCompletedEvent;

            CoroutinesHolder.StartCoroutine(SendAnalyticsTimeEvents());
            
        }
        
        private void InitializeAds()
        {
            adController = new AdController(dependencies.servicesDependencies);
            adController.InitializeSdk();
        }
 
        private void InitializeGameEvents()
        {
            // GameEvents.CubeFalled.Event += cube => cornerFollower.TrySetNewCorner(cube.GetPosition());
            
            GameEvents.CubeFalled.Event += wallDestroyingObserver.RegisterCubeFall;
            // GameEvents.GameEndedByLose.Event += cornerFollower.ResetToDefaultPosition;
            GameEvents.GameEndedByLose.Event += wallManager.ResetCubes;
            GameEvents.GameEndedByLose.Event += wallDestroyingObserver.Reset;

            GameEvents.GameEndedByWin.Event += () => save.wallIndex.Value++;
            GameEvents.GameEndedByWin.Event += () => sWallIndex++;
            GameEvents.GameEndedByWin.Event += wallDestroyingObserver.Reset;
            // GameEvents.GameEndedByWin.Event += cornerFollower.ResetToDefaultPosition;

            save.wallIndex.DataChanged += newWallIndex =>
            {
                if (newWallIndex != loadedWallIndex)
                    LoadWallFromSave();
                
            };
            
            AmmoTracker ammoTracker = new AmmoTracker(weaponManager.playingBullets);
            ammoTracker.AmmoEnded += () => this.InvokeDelay(endGameScreen.ShowLoseScreen, 0.4f);
            ammoTracker.StartTracking();
            GameEvents.EndScreenShowed.Event += ammoTracker.StopTracking;
            GameEvents.GameStarted.Event += ammoTracker.StartTracking;

            wallDestroyingObserver.WallDestroyed += endGameScreen.ShowWinScreen;
            wallDestroyingObserver.WallDestroyPercentUpdated += gameScreen.SetNewProgressBarValue;
            
            
            // TEMPLATE FOR VIDEO
            // UnityEvents.Update += delegate
            // {
            //     if (Input.GetKeyDown(KeyCode.A))
            //     {
            //         weaponManager.UntrackedMouseMovement();
            //         // crosshair.UntrackedCrossHair();
            //     }
            //
            //     if (Input.GetKeyDown(KeyCode.S))
            //     {
            //         weaponManager.TrackedMouseMovement();
            //         // crosshair.TrackedCrossHair();
            //         
            //     }
            //         
            // };
        }
        
        // by Vadim
        private void LoadWallFromSave()
        {
            wallManager.DestroyWall();
            wallManager.farCameraCorner = Vector2.zero;
            Sprite sprite = dependencies.wallConfig.sprites[save.wallIndex.Value % dependencies.wallConfig.sprites.Count];
            wallManager.SpawnCubes(sprite);
            wallManager.IncreaseHealthCubeAtPool();

            if (cornerFollower != null)
                cornerFollower.SetNewCorner(wallManager.farCameraCorner);
        }

        private IEnumerator SendAnalyticsTimeEvents()
        {
            AnalyticsController.SendSessionStartPlay(save.timeIndex.Value);
            WaitForSeconds interval = new WaitForSeconds(dependencies.servicesDependencies.analyticsSecondsTimeInterval);

            int sessionTimeIndex = 0;

            while (true)
            {
                yield return interval;
                save.timeIndex.Value++;
                sessionTimeIndex++;

                AnalyticsController.SendPlayedSessionTime(sessionTimeIndex);
                AnalyticsController.SendPlayedTime(save.timeIndex.Value);
            }
        }
    }
}