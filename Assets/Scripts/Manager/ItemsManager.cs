using System;
using com.tksr.schema;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class ItemsManager : Singleton<ItemsManager>
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private SchemaItems schemaItems;

    private SpriteAtlas itemIconsAtlas;

    public void LoadItemsSchema(string jsonItems)
    {
        schemaItems = JsonConvert.DeserializeObject<SchemaItems>(jsonItems);
    }

    public void LoadItemIconsAtlas(SpriteAtlas atlas)
    {
        itemIconsAtlas = atlas;
    }

    public Sprite ReadItemIconSpriteById(int Id)
    {
        EnumGameItemType gameItemType;
        string itemName;
        var resItem = GetTKRItemById(Id, out gameItemType, out itemName);
        if (resItem != null && !string.IsNullOrEmpty(resItem.Icon))
        {
            if (itemIconsAtlas != null)
            {
                return itemIconsAtlas.GetSprite(resItem.Icon);
            }
        }

        return null;
    }

    public TKRItem GetTKRItemById(int Id, out EnumGameItemType gameItemType, out string itemName)
    {
        itemName = string.Empty;
        gameItemType = EnumGameItemType.Invalid;
        if (schemaItems != null)
        {
            if (schemaItems.items.ContainsKey(Id.ToString()))
            {
                var itemType = schemaItems.items[Id.ToString()];
                if (Enum.TryParse(itemType.Type, out gameItemType))
                {
                    TKRItem item = null;
                    itemName = itemType.Name;
                    switch (gameItemType)
                    {
                        case EnumGameItemType.Medic:
                        {
                            if (schemaItems.medics.ContainsKey(Id.ToString()))
                            {
                                item = schemaItems.medics[Id.ToString()];
                            }
                        }
                            break;
                        case EnumGameItemType.Prop:
                        {
                            if (schemaItems.props.ContainsKey(Id.ToString()))
                            {
                                item = schemaItems.props[Id.ToString()];
                            }
                        }
                            break;
                        case EnumGameItemType.Weapon:
                        {
                            if (schemaItems.weapons.ContainsKey(Id.ToString()))
                            {
                                item = schemaItems.weapons[Id.ToString()];
                            }
                        }
                            break;
                        case EnumGameItemType.Armor:
                        {
                            if (schemaItems.armors.ContainsKey(Id.ToString()))
                            {
                                item = schemaItems.armors[Id.ToString()];
                            }
                        }
                            break;
                        case EnumGameItemType.Accessory:
                        {
                            if (schemaItems.accessories.ContainsKey(Id.ToString()))
                            {
                                item = schemaItems.accessories[Id.ToString()];
                            }
                        }
                            break;
                        case EnumGameItemType.Special:
                        {
                            if (schemaItems.specials.ContainsKey(Id.ToString()))
                            {
                                item = schemaItems.specials[Id.ToString()];
                            }
                        }
                            break;
                        default:
                            break;
                    }

                    return item;
                }
                return null;
            }
        }
        return null;
    }
}
