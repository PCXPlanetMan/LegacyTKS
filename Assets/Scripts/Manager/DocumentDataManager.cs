using com.tksr.document;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using com.tksr.data;
using com.tksr.schema;
using UnityEngine;

public class DocumentDataManager : Singleton<DocumentDataManager>
{
    private readonly int MAX_DOCUMENT_INDEX = 10;
    private readonly int MIN_DOCUMENT_INDEX = 1;
    private readonly int MAX_TEAM_NUMBER = 5;
    private readonly int MAX_ITEM_COUNT = 99;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private TKSArchives _archives;
    void Start()
    {
        
    }

    public TKSArchives LoadTKSArchives()
    {
        _archives = null;

        var dataPath = Path.Combine(Application.persistentDataPath, ResourceUtils.DOCUMENT_ARCHIVES_FILE_NAME);
        if (File.Exists(dataPath))
        {
            string dataAsJson = File.ReadAllText(dataPath);
            _archives = JsonConvert.DeserializeObject<TKSArchives>(dataAsJson);
        }
        return _archives;
    }

    public void SaveTKSArchives()
    {
        if (_archives != null)
        {
            var dataPath = Path.Combine(Application.persistentDataPath, ResourceUtils.DOCUMENT_ARCHIVES_FILE_NAME);
            if (File.Exists(dataPath))
            {
                File.Delete(dataPath);
            }

            string dataAsJson = JsonConvert.SerializeObject(_archives, Formatting.Indented);
            File.WriteAllText(dataPath, dataAsJson);
        }
    }

    public bool LoadGameDocument(int docIndex)
    {
        var archives = DocumentDataManager.Instance.LoadTKSArchives();
        if (archives == null || archives.Documents == null || archives.Documents.Count == 0)
        {
            Debug.LogWarning("No document file existed");
            return false;
        }
        else
        {
            GameDocument currentDocument = null;
            if (docIndex >= MIN_DOCUMENT_INDEX && docIndex < archives.Documents.Count)
            {
                currentDocument = archives.Documents[docIndex];
            }

            if (currentDocument == null)
            {
                Debug.LogWarningFormat("There is no saved file in current archives by index = {0}", docIndex);
                return false;
            }

            _gameDocument = null;

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, currentDocument);
                stream.Seek(0, SeekOrigin.Begin);
                _gameDocument = (GameDocument)formatter.Deserialize(stream);
            }

            return true;
        }
    }

    public void SaveGameDocument(int docIndex)
    {
        LoadTKSArchives();
        if (_archives == null)
        {
            _archives = new TKSArchives();
        }

        GameDocument currentDocument = GetCurrentDocument();
        if (docIndex >= MIN_DOCUMENT_INDEX && docIndex < MAX_DOCUMENT_INDEX)
        {
            if (_archives.Documents == null || _archives.Documents.Count == 0)
            {
                _archives.Documents = new List<GameDocument>();
                for (int i = 0; i < MAX_DOCUMENT_INDEX; i++)
                {
                    var document = new GameDocument();
                    document.DocumentId = -1;
                    _archives.Documents.Add(document);
                }
            }

            _archives.Documents.RemoveAt(0);
            currentDocument.Timestamp = DateTime.Now.Ticks;
            if (CharactersManager.Instance.MainRoleController != null)
            {
                currentDocument.PosX = CharactersManager.Instance.MainRoleController.transform.position.x;
                currentDocument.PosY = CharactersManager.Instance.MainRoleController.transform.position.y;
                currentDocument.Direction = CharactersManager.Instance.MainRoleController.CharAnimRender
                    .GetLastDirection().ToString();
            }

            currentDocument.SceneId = GameMainManager.Instance.SceneId;
            _archives.Documents.Insert(0, currentDocument);

            _archives.Documents.RemoveAt(docIndex);
            GameDocument newDocument = null;
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, currentDocument);
                stream.Seek(0, SeekOrigin.Begin);
                newDocument = (GameDocument)formatter.Deserialize(stream);
            }

            newDocument.DocumentId = docIndex;
            _archives.Documents.Insert(docIndex, newDocument);

            SaveTKSArchives();
        }
        else
        {
            Debug.LogErrorFormat("Save Document in error index : {0}", docIndex);
            _archives = null;
        }
    }

    private GameDocument _gameDocument;
    /// <summary>
    /// 新建游戏存档(初始化),一般是开启新游戏时创建
    /// </summary>
    /// <returns></returns>
    public GameDocument NewDefaultDocument()
    {
        if (_gameDocument != null)
            _gameDocument = null;

        _gameDocument = new GameDocument()
        {
            DocumentId = 0,
            SceneId = 0,
            PosX = 0f,
            PosY = 0f,
            Gold = 0,
            Morality = 0,
            Intelligence = 0,
            Courage = 0,
            MedicalSkill = 0,
            GameEvents = new List<GameEventInfo>(),
            FinishedEvents = new List<int>(),
            DoingTaskEvents = new List<int>(),
            StoryNotes = new List<int>(),
            Timestamp = DateTime.Now.Ticks,
            MainRoleInfo = GenGameCharInfoFromSchema(ResourceUtils.MAINROLE_ID, 1),
            TaskCharOwnItems = new List<GameTaskCharOwnItem>()
        };
        return _gameDocument;
    }

    private GameCharInfo GenGameCharInfoFromSchema(int charId, int level)
    {
        GameCharInfo info = new GameCharInfo();
        info.CharId = charId;
        info.Level = level;
        info.Weapon = 0;
        info.Armor = 0;
        info.Accessory1 = 0;
        info.Accessory2 = 0;
        info.HP = 0;
        info.MP = 0;
        info.UsedItems = new List<GameItemInfo>();
        info.SkillsTree = new List<GameSkillInfo>();

        var dataInfo = CharactersManager.Instance.ParseDataCharStatsInfo(info);
        if (dataInfo != null)
        {
            info.HP = dataInfo.MaxHP;
            info.MP = dataInfo.MaxMP;
        }
        else
        {
            return null;
        }

        var instanceItem = CharactersManager.Instance.GetCharacterInstanceById(charId);

        var skillsTree = CharactersManager.Instance.ParseSkillsTreeFromSchema(instanceItem.AttachResID);
        if (skillsTree != null)
            info.SkillsTree.AddRange(skillsTree);

        return info;
    }

    public GameDocument GetCurrentDocument()
    {
        if (_gameDocument == null)    
        {
            return NewDefaultDocument();
        }
        return _gameDocument;
    }

    public void UpdateMainRoleName(string strFirstName, string strLastName)
    {
        if (_gameDocument != null)
        {
            _gameDocument.FirstName = strFirstName;
            _gameDocument.LastName = strLastName;
        }
    }

    public string GetMainRoleFirstName()
    {
        if (_gameDocument == null)
        {
            return ResourceUtils.DEFAULT_MAINROLE_FIRST_NAME;
        }

        if (string.IsNullOrEmpty(_gameDocument.FirstName))
        {
            return ResourceUtils.DEFAULT_MAINROLE_FIRST_NAME;
        }

        return _gameDocument.FirstName;
    }

    public string GetMainRoleLastName()
    {
        if (_gameDocument == null)
        {
            return ResourceUtils.DEFAULT_MAINROLE_LAST_NAME;
        }

        if (string.IsNullOrEmpty(_gameDocument.LastName))
        {
            return ResourceUtils.DEFAULT_MAINROLE_LAST_NAME;
        }

        return _gameDocument.LastName;
    }

    public int GetCountOfTKRItemInPackage(int id)
    {
        int countOfItem = 0;

        EnumGameItemType itemType = EnumGameItemType.Invalid;
        string itemName = string.Empty;
        var item = ItemsManager.Instance.GetTKRItemById(id, out itemType, out itemName);
        if (item != null)
        {
            var gameDocument = GetCurrentDocument();
            if (gameDocument != null)
            {
                switch (itemType)
                {
                    case EnumGameItemType.Medic:
                    {
                        if (gameDocument.Medics != null)
                        {
                            var foundMedic = gameDocument.Medics.Find(x => x.ItemId == id);
                            if (foundMedic != null)
                            {
                                countOfItem = foundMedic.Count;
                            }
                        }
                    }
                        break;
                    case EnumGameItemType.Prop:
                    {
                        if (gameDocument.Props != null)
                        {
                            var foundProp = gameDocument.Props.Find(x => x.ItemId == id);
                            if (foundProp != null)
                            {
                                countOfItem = foundProp.Count;
                            }
                        }
                    }
                        break;
                    case EnumGameItemType.Weapon:
                    {
                        if (gameDocument.Weapons != null)
                        {
                            var foundWeapon = gameDocument.Weapons.Find(x => x.ItemId == id);
                            if (foundWeapon != null)
                            {
                                countOfItem = foundWeapon.Count;
                            }
                        }
                    }
                        break;
                    case EnumGameItemType.Armor:
                    {
                        if (gameDocument.Armors != null)
                        {
                            var foundArmor = gameDocument.Armors.Find(x => x.ItemId == id);
                            if (foundArmor != null)
                            {
                                countOfItem = foundArmor.Count;
                            }
                        }
                    }
                        break;
                    case EnumGameItemType.Accessory:
                    {
                        if (gameDocument.Accessories != null)
                        {
                            var foundAccessory = gameDocument.Accessories.Find(x => x.ItemId == id);
                            if (foundAccessory != null)
                            {
                                countOfItem = foundAccessory.Count;
                            }
                        }
                    }
                        break;
                    case EnumGameItemType.Special:
                    {
                        if (gameDocument.Specials != null)
                        {
                            var foundSpecial = gameDocument.Specials.Find(x => x.ItemId == id);
                            if (foundSpecial != null)
                            {
                                countOfItem = foundSpecial.Count;
                            }
                        }
                    }
                        break;
                    default:
                        break;
                }
            }
        }

        return countOfItem;
    }

    public void AddTKRItemToPackage(int id, int count = 1)
    {
        var gameDocument = GetCurrentDocument();
        EnumGameItemType itemType = EnumGameItemType.Invalid;
        string itemName = string.Empty;
        var item = ItemsManager.Instance.GetTKRItemById(id, out itemType, out itemName);
        if (item != null && gameDocument != null)
        {
            switch (itemType)
            {
                case EnumGameItemType.Medic:
                {
                    if (gameDocument.Medics == null)
                    {
                        gameDocument.Medics = new List<GameItemInfo>();
                    }

                    var foundItem = gameDocument.Medics.Find(x => x.ItemId == id);
                    int newCount = count;
                    if (foundItem != null)
                    {
                        newCount += foundItem.Count;
                        newCount = Mathf.Min(newCount, MAX_ITEM_COUNT);
                        gameDocument.Medics.Remove(foundItem);
                    }
                    gameDocument.Medics.Add(new GameItemInfo() { ItemId = id, Count = newCount });
                    
                }
                    break;
                case EnumGameItemType.Prop:
                {
                    if (gameDocument.Props == null)
                    {
                        gameDocument.Props = new List<GameItemInfo>();
                    }

                    var foundItem = gameDocument.Props.Find(x => x.ItemId == id);
                    int newCount = count;
                    if (foundItem != null)
                    {
                        newCount += foundItem.Count;
                        newCount = Mathf.Min(newCount, MAX_ITEM_COUNT);
                        gameDocument.Props.Remove(foundItem);
                    }
                    gameDocument.Props.Add(new GameItemInfo() { ItemId = id, Count = newCount });
                }
                    break;
                case EnumGameItemType.Weapon:
                {
                    if (gameDocument.Weapons == null)
                    {
                        gameDocument.Weapons = new List<GameItemInfo>();
                    }

                    var foundItem = gameDocument.Weapons.Find(x => x.ItemId == id);
                    int newCount = count;
                    if (foundItem != null)
                    {
                        newCount += foundItem.Count;
                        newCount = Mathf.Min(newCount, MAX_ITEM_COUNT);
                        gameDocument.Weapons.Remove(foundItem);
                    }
                    gameDocument.Weapons.Add(new GameItemInfo() { ItemId = id, Count = newCount });
                }
                    break;
                case EnumGameItemType.Armor:
                {
                    if (gameDocument.Armors == null)
                    {
                        gameDocument.Armors = new List<GameItemInfo>();
                    }

                    var foundItem = gameDocument.Armors.Find(x => x.ItemId == id);
                    int newCount = count;
                    if (foundItem != null)
                    {
                        newCount += foundItem.Count;
                        newCount = Mathf.Min(newCount, MAX_ITEM_COUNT);
                        gameDocument.Armors.Remove(foundItem);
                    }
                    gameDocument.Armors.Add(new GameItemInfo() { ItemId = id, Count = newCount });
                }
                    break;
                case EnumGameItemType.Accessory:
                {
                    if (gameDocument.Accessories == null)
                    {
                        gameDocument.Accessories = new List<GameItemInfo>();
                    }

                    var foundItem = gameDocument.Accessories.Find(x => x.ItemId == id);
                    int newCount = count;
                    if (foundItem != null)
                    {
                        newCount += foundItem.Count;
                        newCount = Mathf.Min(newCount, MAX_ITEM_COUNT);
                        gameDocument.Accessories.Remove(foundItem);
                    }
                    gameDocument.Accessories.Add(new GameItemInfo() { ItemId = id, Count = newCount });
                }
                    break;
                case EnumGameItemType.Special:
                {
                    if (gameDocument.Specials == null)
                    {
                        gameDocument.Specials = new List<GameItemInfo>();
                    }

                    var foundItem = gameDocument.Specials.Find(x => x.ItemId == id);
                    int newCount = count;
                    if (foundItem != null)
                    {
                        newCount += foundItem.Count;
                        newCount = Mathf.Min(newCount, MAX_ITEM_COUNT);
                        gameDocument.Specials.Remove(foundItem);
                    }
                    gameDocument.Specials.Add(new GameItemInfo() { ItemId = id, Count = newCount });
                }
                    break;
            }
        }
        else
        {
            Debug.LogErrorFormat("No item found : {0}", id.ToString());
        }
    }

    public void DelTKRItemFromPackage(int id, int count = 1)
    {
        var gameDocument = GetCurrentDocument();
        EnumGameItemType itemType = EnumGameItemType.Invalid;
        string itemName = string.Empty;
        var item = ItemsManager.Instance.GetTKRItemById(id, out itemType, out itemName);
        if (item != null && gameDocument != null)
        {
            switch (itemType)
            {
                case EnumGameItemType.Medic:
                    {
                        if (gameDocument.Medics != null)
                        {
                            var foundItem = gameDocument.Medics.Find(x => x.ItemId == id);
                            if (foundItem != null)
                            {
                                int newCount = foundItem.Count;
                                newCount -= count;
                                newCount = Math.Max(0, newCount);
                                gameDocument.Medics.Remove(foundItem);
                                if (newCount != 0)
                                    gameDocument.Medics.Add(new GameItemInfo() { ItemId = id, Count = newCount });
                            }
                        }
                    }
                    break;
                case EnumGameItemType.Prop:
                    {
                        if (gameDocument.Props != null)
                        {
                            var foundItem = gameDocument.Props.Find(x => x.ItemId == id);
                            if (foundItem != null)
                            {
                                int newCount = foundItem.Count;
                                newCount -= count;
                                newCount = Math.Max(0, newCount);
                                gameDocument.Props.Remove(foundItem);
                                if (newCount != 0)
                                    gameDocument.Props.Add(new GameItemInfo() { ItemId = id, Count = newCount });
                            }
                        }
                    }
                    break;
                case EnumGameItemType.Weapon:
                    {
                        if (gameDocument.Weapons != null)
                        {
                            var foundItem = gameDocument.Weapons.Find(x => x.ItemId == id);
                            if (foundItem != null)
                            {
                                int newCount = foundItem.Count;
                                newCount -= count;
                                newCount = Math.Max(0, newCount);
                                gameDocument.Weapons.Remove(foundItem);
                                if (newCount != 0)
                                    gameDocument.Weapons.Add(new GameItemInfo() { ItemId = id, Count = newCount });
                            }
                        }
                    }
                    break;
                case EnumGameItemType.Armor:
                    {
                        if (gameDocument.Armors != null)
                        {
                            var foundItem = gameDocument.Armors.Find(x => x.ItemId == id);
                            if (foundItem != null)
                            {
                                int newCount = foundItem.Count;
                                newCount -= count;
                                newCount = Math.Max(0, newCount);
                                gameDocument.Armors.Remove(foundItem);
                                if (newCount != 0)
                                    gameDocument.Armors.Add(new GameItemInfo() { ItemId = id, Count = newCount });
                            }
                        }
                    }
                    break;
                case EnumGameItemType.Accessory:
                    {
                        if (gameDocument.Accessories != null)
                        {
                            var foundItem = gameDocument.Accessories.Find(x => x.ItemId == id);
                            if (foundItem != null)
                            {
                                int newCount = foundItem.Count;
                                newCount -= count;
                                newCount = Math.Max(0, newCount);
                                gameDocument.Accessories.Remove(foundItem);
                                if (newCount != 0)
                                    gameDocument.Accessories.Add(new GameItemInfo() { ItemId = id, Count = newCount });
                            }
                        }
                    }
                    break;
                case EnumGameItemType.Special:
                    {
                        if (gameDocument.Specials != null)
                        {
                            var foundItem = gameDocument.Specials.Find(x => x.ItemId == id);
                            if (foundItem != null)
                            {
                                int newCount = foundItem.Count;
                                newCount -= count;
                                newCount = Math.Max(0, newCount);
                                gameDocument.Specials.Remove(foundItem);
                                if (newCount != 0)
                                    gameDocument.Specials.Add(new GameItemInfo() { ItemId = id, Count = newCount });
                            }
                        }
                    }
                    break;
            }
        }
        else
        {
            Debug.LogErrorFormat("No item found : {0}", id.ToString());
        }
    }

    public List<GameItemInfo> LoadItemsFormPackageByType(EnumGameItemType type)
    {
        if (_gameDocument == null)
        {
            return null;
        }

        switch (type)
        {
            case EnumGameItemType.Medic:
            {
                return _gameDocument.Medics;
            }
            case EnumGameItemType.Prop:
            {
                return _gameDocument.Props;
            }
            case EnumGameItemType.Weapon:
            {
                return _gameDocument.Weapons;
            }
            case EnumGameItemType.Armor:
            {
                return _gameDocument.Armors;
            }
            case EnumGameItemType.Accessory:
            {
                return _gameDocument.Accessories;
            }
            case EnumGameItemType.Special:
            {
                return _gameDocument.Specials;
            }
        }

        return null;
    }

    public void UpdateGoldByValue(int offfset)
    {
        var gameDocument = GetCurrentDocument();
        if (offfset > 0)
        {
            gameDocument.Gold += (uint)offfset;
        }
        else
        {
            if (Mathf.Abs(offfset) > Mathf.Abs(gameDocument.Gold))
            {
                Debug.LogErrorFormat("Gold is not enough, current = {0}, offset = {1}", gameDocument.Gold, offfset);
                offfset = (int)gameDocument.Gold;
            }
            gameDocument.Gold -= (uint) offfset;
        }
    }

    public void EquipTKRItemToChar(int itemId, int charId, int accessoryIndex = 0)
    {
        var gameDocument = GetCurrentDocument();
        EnumGameItemType itemType = EnumGameItemType.Invalid;
        string itemName = string.Empty;
        var item = ItemsManager.Instance.GetTKRItemById(itemId, out itemType, out itemName);
        GameCharInfo foundCharInfo = FindCharInfoFromCandidates(charId);
        if (item != null && gameDocument != null && foundCharInfo != null)
        {
            switch (itemType)
            {
                case EnumGameItemType.Weapon:
                {
                    var foundItem = gameDocument.Weapons.Find(x => x.ItemId == itemId);
                    if (foundItem.ItemId != foundCharInfo.Weapon)
                    {
                        int oldWeaponId = foundCharInfo.Weapon;
                        foundCharInfo.Weapon = foundItem.ItemId;
                        if (oldWeaponId != 0)
                        {
                            AddTKRItemToPackage(oldWeaponId);
                        }
                    }
                }
                    break;
                case EnumGameItemType.Armor:
                {
                    var foundItem = gameDocument.Armors.Find(x => x.ItemId == itemId);
                    if (foundItem.ItemId != foundCharInfo.Armor)
                    {
                        int oldArmorId = foundCharInfo.Armor;
                        foundCharInfo.Armor = foundItem.ItemId;
                        if (oldArmorId != 0)
                        {
                            AddTKRItemToPackage(oldArmorId);
                        }
                    }
                }
                    break;
                case EnumGameItemType.Accessory:
                {
                    var foundItem = gameDocument.Accessories.Find(x => x.ItemId == itemId);
                    if (accessoryIndex == 0)
                    {
                        if (foundItem.ItemId != foundCharInfo.Accessory1)
                        {
                            int oldAccessoryId = foundCharInfo.Accessory1;
                            foundCharInfo.Accessory1 = foundItem.ItemId;
                            if (oldAccessoryId != 0)
                            {
                                AddTKRItemToPackage(oldAccessoryId);
                            }
                        }
                    }
                    else if (accessoryIndex == 1)
                    {
                        if (foundItem.ItemId != foundCharInfo.Accessory2)
                        {
                            int oldAccessoryId = foundCharInfo.Accessory2;
                            foundCharInfo.Accessory2 = foundItem.ItemId;
                            if (oldAccessoryId != 0)
                            {
                                AddTKRItemToPackage(oldAccessoryId);
                            }
                        }
                    }
                }
                    break;
            }
        }
    }

    public GameCharInfo FindCharInfoFromCandidates(int charId)
    {
        var gameDocument = GetCurrentDocument();
        if (charId == ResourceUtils.MAINROLE_ID)
        {
            return gameDocument.MainRoleInfo;
        }
        else
        {
            if (gameDocument.Candidates != null)
                return gameDocument.Candidates.Find(x => x.CharId == charId);
        }

        return null;  
    }

    public GameCharInfo[] FindCharsInfoInTeam()
    {
        var gameDocument = GetCurrentDocument();
        List<GameCharInfo> chars = new List<GameCharInfo>();
        chars.Add(gameDocument.MainRoleInfo);
        if (gameDocument.Team != null)
        {
            for (int i = 0; i < gameDocument.Team.Count; i++)
            {
                int id = gameDocument.Team[i];
                GameCharInfo charInfo = FindCharInfoFromCandidates(id);
                chars.Add(charInfo);
            }
        }
        return chars.ToArray();
    }

    public GameCharInfo FindCharInfoInTeam(int charId)
    {
        var gameDocument = GetCurrentDocument();
        if (charId == ResourceUtils.MAINROLE_ID)
        {
            return gameDocument.MainRoleInfo;
        }
        else
        {
            for (int i = 0; i < gameDocument.Team.Count; i++)
            {
                if (gameDocument.Team[i] == charId)
                {
                    GameCharInfo charInfo = FindCharInfoFromCandidates(charId);
                    return charInfo;
                }
            }
        }

        return null;
    }

    #region Events/Scenarios
    /// <summary>
    /// 开启剧情,如果其所属的事件没有开启,则同时开启事件
    /// Attention:若剧情(或者事件已经完成),则可能会重启该剧情(或者事件)
    /// </summary>
    /// <param name="scenarioId"></param>
    public GameScenarioInfo OpenScenarioById(int scenarioId, bool forceReload = false)
    {
        var scenarioItem = ScenarioManager.Instance.GetScenarioItemById(scenarioId);
        if (scenarioItem == null)
            return null;


        List<GameScenarioInfo> toInitTasksOfScenarios = new List<GameScenarioInfo>();

        var gameDocument = GetCurrentDocument();
        int eventId = scenarioItem.ParentEventId;
        var foundGameEvent = gameDocument.GameEvents.Find(x => x.EventId == eventId);
        if (foundGameEvent == null)
        {
            foundGameEvent = new GameEventInfo
            {
                EventId = eventId
            };
            gameDocument.GameEvents.Add(foundGameEvent);

            // 每当有事件被开启则同时开启其默认的剧情
            var eventItem = ScenarioManager.Instance.GetEventItemById(eventId);
            if (eventItem != null && !string.IsNullOrEmpty(eventItem.DefaultToOpenScenarios))
            {
                var toOpenScenarios = SchemaParser.ParseParamToInts(eventItem.DefaultToOpenScenarios);
                for (int i = 0; i < toOpenScenarios.Count; i++)
                {
                    int toOpenScenarioId = toOpenScenarios[i];
                    if (toOpenScenarioId != scenarioId)
                    {
                        var scenario = foundGameEvent.ContainScenarios.Find(x => x.ScenarioId == toOpenScenarioId);
                        if (scenario == null)
                        {
                            scenario = new GameScenarioInfo
                            {
                                ScenarioId = toOpenScenarioId
                            };
                            foundGameEvent.ContainScenarios.Add(scenario);
                        }
                        else
                        {
                            scenario.ScenarioDone = false;
                        }
                        toInitTasksOfScenarios.Add(scenario);
                    }
                }
            }
        }
        else
        {
            if (forceReload)
                foundGameEvent.EventDone = false;
        }

        var foundGameScenario = foundGameEvent.ContainScenarios.Find(x => x.ScenarioId == scenarioId);
        if (foundGameScenario == null)
        {
            foundGameScenario = new GameScenarioInfo
            {
                ScenarioId = scenarioId
            };
            foundGameEvent.ContainScenarios.Add(foundGameScenario);
        }
        else
        {
            if (forceReload)
                foundGameScenario.ScenarioDone = false;
        }

        if (!toInitTasksOfScenarios.Contains(foundGameScenario))
        {
            toInitTasksOfScenarios.Add(foundGameScenario);
        }


        // 开启Scenario后,还需要对其包含的Tasks进行初始化开启
        foreach (var scenario in toInitTasksOfScenarios)
        {
            GameScenarioInfo openedScenario = scenario;
            var scenarioItemData = ScenarioManager.Instance.GetScenarioItemById(openedScenario.ScenarioId);
            if (openedScenario != null && scenarioItemData != null)
            {
                if (openedScenario.CurrentTasks == null)
                {
                    openedScenario.CurrentTasks = new Dictionary<string, int>();
                }

                var tasksId = SchemaParser.ParseParamToInts(scenarioItemData.InitAssignedTasks);
                foreach (var taskId in tasksId)
                {
                    var taskItem = UpdateScenarioTaskById(taskId);
                }
            }
        }

        return foundGameScenario;
    }

    /// <summary>
    /// 当剧情完成后,关闭该剧情
    /// PS:同时会检测该剧情所属事件是否完成了,若完成,则同时关闭该事件
    /// </summary>
    /// <param name="scenarioId"></param>
    public void CloseScenarioById(int scenarioId)
    {
        var scenarioItem = ScenarioManager.Instance.GetScenarioItemById(scenarioId);
        if (scenarioItem == null)
            return;

        var gameDocument = GetCurrentDocument();
        int eventId = scenarioItem.ParentEventId;
        var foundGameEvent = gameDocument.GameEvents.Find(x => x.EventId == eventId);
        if (foundGameEvent == null)
        {
            Debug.LogWarningFormat("Not found scenario {0} and event {1}", scenarioId, eventId);
            return;
        }

        var foundGameScenario = foundGameEvent.ContainScenarios.Find(x => x.ScenarioId == scenarioId);
        if (foundGameScenario == null)
        {
            foundGameScenario = new GameScenarioInfo
            {
                ScenarioId = scenarioId
            };
            foundGameEvent.ContainScenarios.Add(foundGameScenario);
        }
        foundGameScenario.ScenarioDone = true;

        bool eventFinished = ScenarioManager.Instance.IsGameEventFinished(eventId);
        if (eventFinished)
        {
            foundGameEvent.EventDone = true;
        }
    }

    public EnumEventStatus GetGameEventStatus(int eventId)
    {
        GameEventInfo eventInfo;
        bool eventOpened = IsGameEventOpened(eventId, out eventInfo);
        if (!eventOpened)
        {
            return EnumEventStatus.NotHappened;
        }
        else
        {
            bool eventClosed = IsGameEventClosed(eventId, out eventInfo);
            if (eventClosed)
            {
                return EnumEventStatus.Finished;
            }
            else
            {
                return EnumEventStatus.Running;
            }
        }
    }

    public EnumScenarioStatus GetGameScenarioStatus(int scenarioId)
    {
        GameScenarioInfo scenarioInfo;
        bool scenarioOpened = IsGameScenarioOpened(scenarioId, out scenarioInfo);
        if (!scenarioOpened)
        {
            return EnumScenarioStatus.NotHappened;
        }
        else
        {
            bool scenarioClosed = IsGameScenarioClosed(scenarioId, out scenarioInfo);
            if (scenarioClosed)
            {
                return EnumScenarioStatus.Finished;
            }
            else
            {
                return EnumScenarioStatus.Running;
            }
        }
    }

    public GameScenarioInfo GetGameScenarioById(int scenarioId)
    {
        GameScenarioInfo scenarioInfo;
        bool scenarioOpened = IsGameScenarioOpened(scenarioId, out scenarioInfo);
        if (scenarioOpened)
        {
            return scenarioInfo;
        }

        return null;
    }

    /// <summary>
    /// 测试事件是否已经开启(不论事件是否完成)
    /// </summary>
    /// <param name="eventId"></param>
    /// <param name="foundGameEvent"></param>
    /// <returns></returns>
    private bool IsGameEventOpened(int eventId, out GameEventInfo foundGameEvent)
    {
        foundGameEvent = null;
        var gameDocument = GetCurrentDocument();
        foundGameEvent = gameDocument.GameEvents.Find(x => x.EventId == eventId);
        return foundGameEvent != null ? true : false;
    }

    /// <summary>
    /// 测试剧情是否已经开启(不论剧情是否完成)
    /// PS:首先先测试剧情所属的事件是否开启,若事件未开启,则剧情必定未开启
    /// </summary>
    /// <param name="scenarioId"></param>
    /// <param name="foundScenarioInfo"></param>
    /// <returns></returns>
    private bool IsGameScenarioOpened(int scenarioId, out GameScenarioInfo foundScenarioInfo)
    {
        foundScenarioInfo = null;
        var scenarioItem = ScenarioManager.Instance.GetScenarioItemById(scenarioId);
        if (scenarioItem == null)
            return false;

        int eventId = scenarioItem.ParentEventId;
        GameEventInfo eventInfo;
        bool eventOpened = IsGameEventOpened(eventId, out eventInfo);
        if (eventOpened && eventInfo != null)
        {
            foundScenarioInfo = eventInfo.ContainScenarios.Find(x => x.ScenarioId == scenarioId);
            return foundScenarioInfo != null ? true : false;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 测试事件是否已经完成关闭,若事件未开启,则必定没有完成
    /// </summary>
    /// <param name="eventId"></param>
    /// <param name="foundGameEvent"></param>
    /// <returns></returns>
    private bool IsGameEventClosed(int eventId, out GameEventInfo foundGameEvent)
    {
        foundGameEvent = null;
        bool eventOpened = IsGameEventOpened(eventId, out foundGameEvent);
        if (eventOpened && foundGameEvent != null && foundGameEvent.EventDone)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 测试剧情是否完成关闭,若事件未开启则说明必定未完成;
    /// 事件完成关闭后,剧情仍然可以未关闭而正常执行
    /// </summary>
    /// <param name="scenarioId"></param>
    /// <param name="foundScenarioInfo"></param>
    /// <returns></returns>
    private bool IsGameScenarioClosed(int scenarioId, out GameScenarioInfo foundScenarioInfo)
    {
        foundScenarioInfo = null;
        bool scenarioOpened = IsGameScenarioOpened(scenarioId, out foundScenarioInfo);
        if (scenarioOpened && foundScenarioInfo != null && foundScenarioInfo.ScenarioDone)
        {
            return true;
        }

        return false;
    }

    public bool ChkTaskRunningById(int taskId)
    {
        var taskItem = ScenarioManager.Instance.GetTaskItemParamById(taskId);
        if (taskItem != null)
        {
            var foundScenario = DocumentDataManager.Instance.GetGameScenarioById(taskItem.BelongScenario);
            if (foundScenario != null && foundScenario.ScenarioDone == false && foundScenario.CurrentTasks != null)
            {
                string strKey = string.Format("{0}:{1}", taskItem.BelongObjType.ToString(), taskItem.BelongObjId);
                if (foundScenario.CurrentTasks.ContainsKey(strKey))
                {
                    if (foundScenario.CurrentTasks[strKey] == taskId)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 更新当前存档中Scenario中包含的Task
    /// </summary>
    /// <param name="taskItemId"></param>
    public StoryTaskItem UpdateScenarioTaskById(int taskItemId)
    {
        var taskItem = ScenarioManager.Instance.GetTaskItemParamById(taskItemId);
        if (taskItem != null)
        {
            var foundScenario = DocumentDataManager.Instance.GetGameScenarioById(taskItem.BelongScenario);
            if (foundScenario != null && foundScenario.ScenarioDone == false && foundScenario.CurrentTasks != null)
            {
                string strKey = string.Format("{0}:{1}", taskItem.BelongObjType.ToString(), taskItem.BelongObjId);
                if (foundScenario.CurrentTasks.ContainsKey(strKey))
                {
                    foundScenario.CurrentTasks[strKey] = taskItem.Id;
                }
                else
                {
                    foundScenario.CurrentTasks.Add(strKey, taskItemId);
                }
            }
            else
            {
                Debug.LogWarningFormat("Update, Not found scenario id = {0} with task id = {1}", taskItem.BelongScenario, taskItemId);
            }
        }
        else
        {
            Debug.LogWarningFormat("Update, Not found task id = {0}", taskItemId);
        }

        return taskItem;
    }

    /// <summary>
    /// 清除存档中的Task,便于记录当前游戏剧情进度
    /// </summary>
    /// <param name="taskItemId"></param>
    public void ClearScenarioTaskById(int taskItemId)
    {
        var taskItem = ScenarioManager.Instance.GetTaskItemParamById(taskItemId);
        if (taskItem != null)
        {
            var foundScenario = DocumentDataManager.Instance.GetGameScenarioById(taskItem.BelongScenario);
            if (foundScenario != null && foundScenario.ScenarioDone == false && foundScenario.CurrentTasks != null)
            {
                string strKey = string.Format("{0}:{1}", taskItem.BelongObjType.ToString(), taskItem.BelongObjId);
                if (foundScenario.CurrentTasks.ContainsKey(strKey))
                {
                    foundScenario.CurrentTasks.Remove(strKey);
                }
            }
            else
            {
                Debug.LogWarningFormat("Clear Not found scenario id = {0} with task id = {1}", taskItem.BelongScenario, taskItemId);
            }
        }
        else
        {
            Debug.LogWarningFormat("Clear Not found task id = {0}", taskItemId);
        }
    }

    public void ClearScenarioTasks(int taskItemId)
    {
        var taskItem = ScenarioManager.Instance.GetTaskItemParamById(taskItemId);
        if (taskItem != null)
        {
            var foundScenario = DocumentDataManager.Instance.GetGameScenarioById(taskItem.BelongScenario);
            if (foundScenario != null && foundScenario.ScenarioDone == false && foundScenario.CurrentTasks != null)
            {
                foundScenario.CurrentTasks.Clear();
            }
            else
            {
                Debug.LogWarningFormat("Clear Not found scenario id = {0} with task id = {1}", taskItem.BelongScenario, taskItemId);
            }
        }
        else
        {
            Debug.LogWarningFormat("Clear Not found task id = {0}", taskItemId);
        }
    }

    public void RecordNotesOfScenario(List<int> notesId)
    {
        var gameDocument = GetCurrentDocument();
        List<int> newNotes = new List<int>(notesId);
        gameDocument.StoryNotes.AddRange(newNotes.Distinct());
        gameDocument.StoryNotes = gameDocument.StoryNotes.Distinct().ToList();
    }

    public void ScanEventsToOpen(EnumEventType eventType)
    {
        var foundEventItems = ScenarioManager.Instance.GetEventItemsByType(eventType);
        for (int i = 0; i < foundEventItems.Count; i++)
        {
            var eventItem = foundEventItems[i];
            if (eventItem != null && !string.IsNullOrEmpty(eventItem.DefaultToOpenScenarios))
            {
                var toOpenScenarios = SchemaParser.ParseParamToInts(eventItem.DefaultToOpenScenarios);
                for (int j = 0; j < toOpenScenarios.Count; j++)
                {
                    int scenarioId = toOpenScenarios[j];
                    OpenScenarioById(scenarioId);
                }
            }
        }
    }
    #endregion

    /// <summary>
    /// 直接在队伍中创建一个等级为charLevel的角色(一般用于调试)
    /// </summary>
    /// <param name="charID"></param>
    /// <param name="charLevel"></param>
    public void CreateCharToTeam(int charID, int charLevel)
    {
        var gameDocument = GetCurrentDocument();
        GameCharInfo foundCharInfo = null;
        if (charID == ResourceUtils.MAINROLE_ID)
        {
            Debug.LogWarning("MainRole can't be created by function");
            gameDocument.MainRoleInfo = GenGameCharInfoFromSchema(charID, charLevel);
            foundCharInfo = gameDocument.MainRoleInfo;
        }
        else
        {
            var roleInfo = GenGameCharInfoFromSchema(charID, charLevel);
            if (roleInfo == null)
                return;

            if (gameDocument.Team == null)
                gameDocument.Team = new List<int>(MAX_TEAM_NUMBER);
            if (gameDocument.Candidates == null)
                gameDocument.Candidates = new List<GameCharInfo>();

            if (gameDocument.Candidates != null)
            {
                foundCharInfo = gameDocument.Candidates.Find(x => x.CharId == charID);
                if (foundCharInfo != null)
                {
                    foundCharInfo.HP = roleInfo.HP;
                    foundCharInfo.MP = roleInfo.MP;
                    foundCharInfo.Level = roleInfo.Level;
                    foundCharInfo.Weapon = roleInfo.Weapon;
                    foundCharInfo.Armor = roleInfo.Armor;
                    foundCharInfo.Accessory1 = roleInfo.Accessory1;
                    foundCharInfo.Accessory2 = roleInfo.Accessory2;
                    foundCharInfo.UsedItems = new List<GameItemInfo>();
                    foundCharInfo.UsedItems.AddRange(roleInfo.UsedItems);
                }
                else
                {
                    foundCharInfo = roleInfo;
                    gameDocument.Candidates.Add(foundCharInfo);
                }

                if (gameDocument.Team.Count < MAX_TEAM_NUMBER)
                {
                    gameDocument.Team.Add(roleInfo.CharId);
                }
            }
        }
    }

    public void DismissCharFromTeam(int charID)
    {
        var gameDocument = GetCurrentDocument();
        if (charID == ResourceUtils.MAINROLE_ID)
        {
            Debug.LogWarning("MainRole can't be dismissed");
            return;
        }

        if (gameDocument.Team != null)
        {
            int removed = gameDocument.Team.RemoveAll(x => x == charID);
            // TODO:某队员离队后,需要重新更新游戏中某些场景的布局或者剧情数据
        }
    }

    /// <summary>
    /// 判断某角色是否在队伍中
    /// </summary>
    /// <param name="charID"></param>
    /// <returns></returns>
    public bool IsCharInTeam(int charID)
    {
        bool inTeam = false;
        if (_gameDocument == null)
        {
            return false;
        }

        if (_gameDocument.Team != null)
        {
            inTeam = _gameDocument.Team.Exists(x => x == charID);
        }

        return inTeam;
    }

    #region Special Task Codes

    public bool TaskIsCharOwnItemCount(int charId, int itemId, int itemCount)
    {
        // TODO:如何区分Task(主角)和主角包裹内的物品???
        if (charId == ResourceUtils.MAINROLE_ID)
        {
            int curItemCount = GetCountOfTKRItemInPackage(itemId);
            if (curItemCount >= itemCount)
            {
                return true;
            }
        }
        else
        {
            var foundTaskCharInfo = _gameDocument.TaskCharOwnItems.Find(x => x.CharId == charId);
            if (foundTaskCharInfo != null)
            {
                var ownedItem = foundTaskCharInfo.OwnedItems.Find(x => x.ItemId == itemId);
                if (ownedItem != null)
                {
                    if (ownedItem.Count >= itemCount)
                    {
                        return true;
                    }
                }
            }
        }
        

        return false;
    }

    public void TaskOwnItemToChar(int charId, int itemId, int itemCount)
    {
        var foundTaskCharInfo = _gameDocument.TaskCharOwnItems.Find(x => x.CharId == charId);
        if (foundTaskCharInfo == null)
        {
            foundTaskCharInfo = new GameTaskCharOwnItem()
            {
                CharId = charId,
                OwnedItems = new List<GameItemInfo>()
            };
            _gameDocument.TaskCharOwnItems.Add(foundTaskCharInfo);
        }

        var ownedItem = foundTaskCharInfo.OwnedItems.Find(x => x.ItemId == itemId);
        if (ownedItem == null)
        {
            ownedItem = new GameItemInfo()
            {
                ItemId = itemId,
                Count = 0
            };
            foundTaskCharInfo.OwnedItems.Add(ownedItem);
        }

        ownedItem.Count += itemCount;
    }

    #endregion
}
