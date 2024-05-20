剧情场景元素
1.Map
Tag:GOGameMap
Set Colliders GameObject's Layer to "GameBorder"
Set EntryTriggers tag to "GOContainEntries"
Disable all Entries gameobjects.
2.Timeline
所有的Timeline对象必须处于Disabled的状态





游戏逻辑顺序:
1.GameMainManager是游戏逻辑的RootController,必须优先于其他逻辑Controller创建.
2.由于其他Controller都或多或少的包含了数据,所以可以优先调用GameAssetBundlesManager,完成资源和配置数据的加载

地图=UnityScene

每个剧情场景中的Map对象的ScenarioMap均要正确设置当前场景的SceneId


ScenarioMap
场景/战斗地图 Layer=Background Order=0

人物 Layer=Default Order=5
场景中的某些装饰物(从背景地图中提取)(在OverObjects中)的层级需要与人物保持一致,这样人物在移动的过程中才能与其保持正确的遮挡关系;

OverObjects中某些装饰物一定会在人物层级之上 Layer=Foreground Order=0