using UnityEngine;
using System.Collections.Generic;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class Equipment : MonoBehaviour
    {
        public const string EquippedNotification = "Equipment.EquippedNotification";
        public const string UnEquippedNotification = "Equipment.UnEquippedNotification";

        public IList<Equippable> items { get { return _items.AsReadOnly(); } }
        List<Equippable> _items = new List<Equippable>();

        public void Equip(Equippable item, EnumEquipSlots slots)
        {
            UnEquip(slots);

            _items.Add(item);
            item.transform.SetParent(transform);
            item.slots = slots;
            item.OnEquip();

            this.PostNotification(EquippedNotification, item);
        }

        public void UnEquip(Equippable item)
        {
            item.OnUnEquip();
            item.slots = EnumEquipSlots.None;
            item.transform.SetParent(transform);
            _items.Remove(item);

            this.PostNotification(UnEquippedNotification, item);
        }

        public void UnEquip(EnumEquipSlots slots)
        {
            for (int i = _items.Count - 1; i >= 0; --i)
            {
                Equippable item = _items[i];
                if ((item.slots & slots) != EnumEquipSlots.None)
                    UnEquip(item);
            }
        }

        public Equippable GetItem(EnumEquipSlots slots)
        {
            for (int i = _items.Count - 1; i >= 0; --i)
            {
                Equippable item = _items[i];
                if ((item.slots & slots) != EnumEquipSlots.None)
                    return item;
            }
            return null;
        }
    }
}