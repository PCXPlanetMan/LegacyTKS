using AssetBundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class GameAssetBundlesManager : Singleton<GameAssetBundlesManager>
{
    public bool SimulationMode = true;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
#if UNITY_EDITOR
        AssetBundleManager.SimulateAssetBundleInEditor = SimulationMode;
#endif
    }
    
    [HideInInspector]
    public bool InitFinished = false;

    private int coroutineLoaded = 0;
    private readonly int MAX_COROUTINE_LOADED = 13;

    // Use this for initialization
    IEnumerator Start()
    {
        yield return StartCoroutine(Initialize());

        // Load assets.
        yield return StartCoroutine(LoadUIRootAsync());
        yield return StartCoroutine(LoadStorySchemaAsync());
        yield return StartCoroutine(LoadTextsSchemaAsync());
        yield return StartCoroutine(LoadScenarioSchemaAsync());
        yield return StartCoroutine(LoadCharacterSchemaAsync());
        yield return StartCoroutine(LoadTKRItemsSchemaAsync());
        yield return StartCoroutine(LoadSkillsSchemaAsync());
        yield return StartCoroutine(LoadStateMachineSchemaAsync());
        yield return StartCoroutine(LoadTKRSoundSchemaAsync());
        yield return StartCoroutine(LoadCharPrefabAsync());

        string assetBundleName = ResourceUtils.AB_RESOURCE_ICONS_CHAR;
        string assetName = ResourceUtils.ASSET_ICON_CHAR_PORTRAITS;
        yield return StartCoroutine(LoadAtlasAsync(assetBundleName, assetName));
        assetBundleName = ResourceUtils.AB_RESOURCE_ICONS_ITEM;
        assetName = ResourceUtils.ASSET_ICON_ITEM;
        yield return StartCoroutine(LoadAtlasAsync(assetBundleName, assetName));
    }

    void Update()
    {
        if (!InitFinished && coroutineLoaded >= MAX_COROUTINE_LOADED) // TODO:Coroutine数目
        {
            InitFinished = true;
            GameMainManager.Instance.UIRoot = GameObject.FindObjectOfType<UIGameRootCanvas>();
        }
    }

    // Initialize the downloading URL.
    // eg. Development server / iOS ODR / web URL
    void InitializeSourceURL()
    {
        // If ODR is available and enabled, then use it and let Xcode handle download requests.
#if ENABLE_IOS_ON_DEMAND_RESOURCES
        if (UnityEngine.iOS.OnDemandResources.enabled)
        {
            AssetBundleManager.SetSourceAssetBundleURL("odr://");
            return;
        }
#endif
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        // With this code, when in-editor or using a development builds: Always use the AssetBundle Server
        // (This is very dependent on the production workflow of the project.
        //      Another approach would be to make this configurable in the standalone player.)
        AssetBundleManager.SetDevelopmentAssetBundleServer();
        return;
#else
        // Use the following code if AssetBundles are embedded in the project for example via StreamingAssets folder etc:
        AssetBundleManager.SetSourceAssetBundleURL(Application.streamingAssetsPath + "/");
        // Or customize the URL based on your deployment or configuration
        //AssetBundleManager.SetSourceAssetBundleURL("http://www.MyWebsite/MyAssetBundles");
        return;
#endif
    }

    // Initialize the downloading url and AssetBundleManifest object.
    protected IEnumerator Initialize()
    {
        // Don't destroy this gameObject as we depend on it to run the loading script.
        DontDestroyOnLoad(gameObject);

        InitializeSourceURL();

        // Initialize AssetBundleManifest which loads the AssetBundleManifest object.
        var request = AssetBundleManager.Initialize();
        if (request != null)
            yield return StartCoroutine(request);

        coroutineLoaded++;
    }

    protected IEnumerator InstantiateGameObjectAsync(string assetBundleName, string assetName)
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(TextAsset));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Get the asset.
        GameObject prefab = request.GetAsset<GameObject>();

        if (prefab != null)
            GameObject.Instantiate(prefab);

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log(assetName + (prefab == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
    }

    protected IEnumerator InitializeLevelAsync(string sceneAssetBundle, string levelName, bool isAdditive)
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        // Load level from assetBundle.
        AssetBundleLoadOperation request = AssetBundleManager.LoadLevelAsync(sceneAssetBundle, levelName, isAdditive);
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log("Finished loading scene " + levelName + " in " + elapsedTime + " seconds");
    }

    public void LoadSceneAsync(string strSceneAssetBundle, string strSceneAssetName)
    {
        StartCoroutine(InitializeLevelAsync(strSceneAssetBundle, strSceneAssetName, false));
    }

    protected IEnumerator LoadUIRootAsync()
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        string assetBundleName = ResourceUtils.AB_UI_ROOT;
        string assetName = ResourceUtils.ASSET_UI_CANVAS;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(GameObject));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Get the asset.
        GameObject prefab = request.GetAsset<GameObject>();
        UIGameRootCanvas uiRoot = GameObject.Instantiate(prefab).GetComponent<UIGameRootCanvas>();
        uiRoot.name = prefab.name;

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log(assetName + (uiRoot == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
        coroutineLoaded++;
    }

    protected IEnumerator LoadStorySchemaAsync()
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        string assetBundleName = ResourceUtils.AB_CFG_DATA;
        string assetName = ResourceUtils.ASSET_SCHEMA_STORYLINE;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(TextAsset));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Get the asset.
        TextAsset taStory = request.GetAsset<TextAsset>();
        string json = taStory.text;
        ScenarioManager.Instance.LoadStorySchema(json);

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log(assetName + (taStory == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
        coroutineLoaded++;
    }

    protected IEnumerator LoadTextsSchemaAsync()
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        string assetBundleName = ResourceUtils.AB_CFG_DATA;
        string assetName = ResourceUtils.ASSET_SCHEMA_TEXTS;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(TextAsset));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Get the asset.
        TextAsset taEvents = request.GetAsset<TextAsset>();
        string json = taEvents.text;
        TextsManager.Instance.LoadTextsSchema(json);

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log(assetName + (taEvents == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
        coroutineLoaded++;
    }

    protected IEnumerator LoadScenarioSchemaAsync()
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        string assetBundleName = ResourceUtils.AB_CFG_DATA;
        string assetName = ResourceUtils.ASSET_SCHEMA_SCENEMAP;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(TextAsset));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Get the asset.
        TextAsset taScenario = request.GetAsset<TextAsset>();
        string json = taScenario.text;
        SceneMapManager.Instance.LoadScenarioSchema(json);

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log(assetName + (taScenario == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
        coroutineLoaded++;
    }

    protected IEnumerator LoadCharacterSchemaAsync()
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        string assetBundleName = ResourceUtils.AB_CFG_DATA;
        string assetName = ResourceUtils.ASSET_SCHEMA_CHARACTER;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(TextAsset));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Get the asset.
        TextAsset taCharacter = request.GetAsset<TextAsset>();
        string json = taCharacter.text;
        CharactersManager.Instance.LoadCharacterSchema(json);

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log(assetName + (taCharacter == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
        coroutineLoaded++;
    }

    public void LoadCharacterSchemaSync()
    {
        string assetBundleName = ResourceUtils.AB_CFG_DATA;
        string assetName = ResourceUtils.ASSET_SCHEMA_CHARACTER;
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetSync(assetBundleName, assetName, typeof(TextAsset));
        TextAsset taCharacter = request.GetAsset<TextAsset>();
        string json = taCharacter.text;
        CharactersManager.Instance.LoadCharacterSchema(json);
    }

    protected IEnumerator LoadTKRItemsSchemaAsync()
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        string assetBundleName = ResourceUtils.AB_CFG_DATA;
        string assetName = ResourceUtils.ASSET_SCHEMA_TKRITEM;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(TextAsset));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Get the asset.
        TextAsset taItems = request.GetAsset<TextAsset>();
        string json = taItems.text;
        ItemsManager.Instance.LoadItemsSchema(json);

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log(assetName + (taItems == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
        coroutineLoaded++;
    }

    protected IEnumerator LoadSkillsSchemaAsync()
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        string assetBundleName = ResourceUtils.AB_CFG_DATA;
        string assetName = ResourceUtils.ASSET_SCHEMA_SKILLS;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(TextAsset));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Get the asset.
        TextAsset taSkills = request.GetAsset<TextAsset>();
        string json = taSkills.text;
        SkillsManager.Instance.LoadSkillsSchema(json);

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log(assetName + (taSkills == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
        coroutineLoaded++;
    }

    protected IEnumerator LoadStateMachineSchemaAsync()
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        string assetBundleName = ResourceUtils.AB_CFG_DATA;
        string assetName = ResourceUtils.ASSET_SCHEMA_STATEMACHINE;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(TextAsset));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Get the asset.
        TextAsset taStateMachine = request.GetAsset<TextAsset>();
        string json = taStateMachine.text;
        CharactersManager.Instance.LoadStateMachineSchema(json);

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log(assetName + (taStateMachine == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
        coroutineLoaded++;
    }

    protected IEnumerator LoadCharPrefabAsync()
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        string assetBundleName = ResourceUtils.AB_PREFABS;
        string assetName = ResourceUtils.ASSET_CHAR_TEMPLATE;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(GameObject));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Get the asset.
        GameObject prefab = request.GetAsset<GameObject>();
        CharactersManager.Instance.LoadCharTemplatePrefab(prefab);

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log(assetName + (prefab == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
        coroutineLoaded++;
    }

    public RuntimeAnimatorController LoadAnimCtrlSync(string strABName, string strAssetName)
    {
        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetSync(strABName, strAssetName, typeof(RuntimeAnimatorController));
        if (request != null && request.IsDone())
        {
            var charAnimator = request.GetAsset<RuntimeAnimatorController>();
            return charAnimator;
        }
        return null;
    }

    public GameObject LoadPrefabSync(string strABName, string strAssetName)
    {
        // Load asset from assetBundle.
        var request = AssetBundleManager.LoadAssetSync(strABName, strAssetName, typeof(GameObject));
        if (request != null && request.IsDone())
        {
            var prefab = request.GetAsset<GameObject>();
            Debug.LogFormat("LoadPrefabSync, strABName = {0}, strAssetName = {1}", strABName, strAssetName);
            return prefab;
        }
        return null;
    }

    public Sprite LoadSpriteSync(string strABName, string strAssetName)
    {
        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetSync(strABName, strAssetName, typeof(Texture2D));
        if (request != null && request.IsDone())
        {
            var spriteTex = request.GetAsset<Texture2D>();
            Sprite resultRes = Sprite.Create(spriteTex, new Rect(0, 0, spriteTex.width, spriteTex.height), new Vector2(0.5f, 0.5f), 64);
            return resultRes;
        }
        return null;
    }

    protected IEnumerator LoadAtlasAsync(string assetBundleName, string assetName)
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(SpriteAtlas));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Get the asset.
        if (assetBundleName.CompareTo(ResourceUtils.AB_RESOURCE_ICONS_CHAR) == 0 &&
            assetName.CompareTo(ResourceUtils.ASSET_ICON_CHAR_PORTRAITS) == 0)
        {
            SpriteAtlas charPortraits = request.GetAsset<SpriteAtlas>();
            CharactersManager.Instance.LoadAllCharsPortraitAtlas(charPortraits);
            // Calculate and display the elapsed time.
            float elapsedTime = Time.realtimeSinceStartup - startTime;
            Debug.Log(assetName + (charPortraits == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
        }
        else if (assetBundleName.CompareTo(ResourceUtils.AB_RESOURCE_ICONS_ITEM) == 0 &&
                 assetName.CompareTo(ResourceUtils.ASSET_ICON_ITEM) == 0)
        {
            SpriteAtlas itemIcons = request.GetAsset<SpriteAtlas>();
            ItemsManager.Instance.LoadItemIconsAtlas(itemIcons);
            // Calculate and display the elapsed time.
            float elapsedTime = Time.realtimeSinceStartup - startTime;
            Debug.Log(assetName + (itemIcons == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
        }


        coroutineLoaded++;
    }

    protected IEnumerator LoadTKRSoundSchemaAsync()
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        string assetBundleName = ResourceUtils.AB_CFG_DATA;
        string assetName = ResourceUtils.ASSET_SCHEMA_SOUND;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(TextAsset));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Get the asset.
        TextAsset taItems = request.GetAsset<TextAsset>();
        string json = taItems.text;
        AudioManager.Instance.LoadSoundSchema(json);

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log(assetName + (taItems == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
        coroutineLoaded++;
    }

    public AudioClip LoadAudioSync(string strABName, string strAssetName)
    {
        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetSync(strABName, strAssetName, typeof(AudioClip));
        if (request != null && request.IsDone())
        {
            var audio = request.GetAsset<AudioClip>();
            return audio;
        }
        return null;
    }
}
