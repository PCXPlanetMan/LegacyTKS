using System.Collections.Generic;

namespace com.tksr.document
{
    // In order to make a object in Unity that's convertable to json it appears it needs to be a regular c# object.
    // Unity does not support property getter and setter. Remove the { get; set; } from all the classes .

    [System.Serializable]
    public class GameItemInfo
    {
        public int ItemId;
        public int Count;
    }

    [System.Serializable]
    public class GameSkillInfo
    {
        public int SkillId;
        public int LvUpPoint;
        public bool Learned;
    }

    [System.Serializable]
    public class GameCharInfo
    {
        public int CharId;
        public int Level;
        public int Weapon;
        public int Armor;
        public int Accessory1;
        public int Accessory2;
        public int HP;
        public int MP;
        public List<GameItemInfo> UsedItems;
        public List<GameSkillInfo> SkillsTree;
    }

    [System.Serializable]
    public class GameTaskCharOwnItem
    {
        public int CharId;
        public List<GameItemInfo> OwnedItems;
    }

    [System.Serializable]
    public class GameScenarioInfo
    {
        public int ScenarioId;
        public bool ScenarioDone;
        public Dictionary<string, int> CurrentTasks;

        public GameScenarioInfo()
        {
            ScenarioDone = false;
        }
    }

    [System.Serializable]
    public class GameEventInfo
    {
        public int EventId;
        public List<GameScenarioInfo> ContainScenarios;
        public bool EventDone;

        public GameEventInfo()
        {
            ContainScenarios = new List<GameScenarioInfo>();
            EventDone = false;
        }
    }

    [System.Serializable]
    public class GameDocument
    {
        public int DocumentId;
        public int SceneId;
        public float PosX;
        public float PosY;
        public string Direction;
        public uint Gold;
        public uint Morality;
        public uint Intelligence;
        public uint Courage;
        public uint MedicalSkill;
        public string FirstName;
        public string LastName;
        public List<GameEventInfo> GameEvents;
        public List<int> FinishedEvents;
        public List<int> DoingTaskEvents;
        public List<int> StoryNotes;
        public List<int> ObtainedChars;
        public long Timestamp;
        public GameCharInfo MainRoleInfo;
        public List<GameCharInfo> Candidates;
        public List<int> Team;
        public List<GameItemInfo> Medics;
        public List<GameItemInfo> Props;
        public List<GameItemInfo> Weapons;
        public List<GameItemInfo> Armors;
        public List<GameItemInfo> Accessories;
        public List<GameItemInfo> Specials;

        public List<GameTaskCharOwnItem> TaskCharOwnItems;
    }

    [System.Serializable]
    public class TKSArchives
    {
        // 理论上共有9份存档,但增加一个作为临时(自动)存档index=0
        public List<GameDocument> Documents;
    }
}
