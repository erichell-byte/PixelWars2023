using System;
using System.IO;
using System.Runtime.InteropServices;
using Extensions;
using Upgrades;

namespace Saves
{
    [Serializable]
    public class PlayerInfoForYandex
    {
        public int wallIndex;
        public int money;
        public int timeIndex;
        public int tutorialIndex;
        public int[] upgrades;
    }
    
    [Serializable]
    public class SerializableDataSave
    {
        public PlayerInfoForYandex playerInfo = new PlayerInfoForYandex();
        
        public ObservableSerializedObject<int> wallIndex = new ObservableSerializedObject<int>();
        public ObservableSerializedObject<int> money = new ObservableSerializedObject<int>();
        public ObservableSerializedObject<int> timeIndex = new ObservableSerializedObject<int>();
        public ObservableSerializedObject<int> tutorialIndex = new ObservableSerializedObject<int>();
        public SerializableDictionary<UpgradeType, int> upgrades = new SerializableDictionary<UpgradeType, int>();
        public SerializableDataSave()
        {
            upgrades[UpgradeType.Weapon] = 0;
            upgrades[UpgradeType.Ammo] = 0;
            upgrades[UpgradeType.FireRate] = 0;
            upgrades[UpgradeType.Radius] = 0;
        }
        
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(wallIndex.Value);
            writer.Write(money.Value);
            writer.Write(timeIndex.Value);
            writer.Write(tutorialIndex.Value);
            
            writer.Write(upgrades.Count);
            foreach (var keyValue in upgrades)
            {
                writer.Write((int)keyValue.Key);
                writer.Write(keyValue.Value);
            }
        }

        public void Deserialize(BinaryReader reader)
        {
            wallIndex.Value = reader.ReadInt32();
            money.Value = reader.ReadInt32();
            // money.Value = 10000000;
            timeIndex.Value = reader.ReadInt32();
            tutorialIndex.Value = reader.ReadInt32();

            upgrades.Clear();
            int upgradesLength = reader.ReadInt32();
            for (int i = 0; i < upgradesLength; i++)
            {
                UpgradeType type = (UpgradeType)reader.ReadInt32();
                int lvl = reader.ReadInt32();
                upgrades.Add(type, lvl);
            }
        }

        public void ConvertFromYandexToSerializedObject()
        {
            money.Value = playerInfo.money;
            wallIndex.Value = playerInfo.wallIndex;
            timeIndex.Value = playerInfo.timeIndex;
            tutorialIndex.Value = playerInfo.tutorialIndex;
            upgrades.Clear();
            upgrades.Add(UpgradeType.Weapon, playerInfo.upgrades[0]);
            upgrades.Add(UpgradeType.Ammo, playerInfo.upgrades[1]);
            upgrades.Add(UpgradeType.FireRate, playerInfo.upgrades[2]);
            upgrades.Add(UpgradeType.Radius, playerInfo.upgrades[3]);
        }

        public void ConvertFromSerializedObjectToYandex()
        {
            playerInfo.money = money.Value;
            playerInfo.wallIndex = wallIndex.Value;
            playerInfo.timeIndex = timeIndex.Value;
            playerInfo.tutorialIndex = tutorialIndex.Value;
            playerInfo.upgrades = new int[5];
            playerInfo.upgrades[0] = upgrades[UpgradeType.Weapon];
            playerInfo.upgrades[1] = upgrades[UpgradeType.Ammo];
            playerInfo.upgrades[2] = upgrades[UpgradeType.FireRate];
            playerInfo.upgrades[3] = upgrades[UpgradeType.Radius];

            // playerInfo.money = 0;
            // playerInfo.wallIndex = 0;
            // playerInfo.timeIndex = 0;
            // playerInfo.tutorialIndex = 0;
            // playerInfo.upgrades = new int[5];
            // playerInfo.upgrades[0] = 0;
            // playerInfo.upgrades[1] = 0;
            // playerInfo.upgrades[2] = 0;
            // playerInfo.upgrades[3] = 0;
        }
    }
}