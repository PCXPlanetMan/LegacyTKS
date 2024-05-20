using UnityEngine;
using System.Collections.Generic;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class Stats : MonoBehaviour
    {
        static Dictionary<EnumStatTypes, string> willChangeNotifications = new Dictionary<EnumStatTypes, string>();
        static Dictionary<EnumStatTypes, string> didChangeNotifications = new Dictionary<EnumStatTypes, string>();

        public static string WillChangeNotification(EnumStatTypes type)
        {
            if (!willChangeNotifications.ContainsKey(type))
                willChangeNotifications.Add(type, string.Format("Stats.{0}WillChange", type.ToString()));
            return willChangeNotifications[type];
        }

        public static string DidChangeNotification(EnumStatTypes type)
        {
            if (!didChangeNotifications.ContainsKey(type))
                didChangeNotifications.Add(type, string.Format("Stats.{0}DidChange", type.ToString()));
            return didChangeNotifications[type];
        }

        public int this[EnumStatTypes s]
        {
            get { return data[(int)s]; }
            set { SetValue(s, value, true); }
        }
        int[] data = new int[(int)EnumStatTypes.Count];


        public void SetValue(EnumStatTypes type, int value, bool allowExceptions)
        {
            int oldValue = this[type];
            if (oldValue == value)
                return;

            if (allowExceptions)
            {
                // Allow exceptions to the rule here
                ValueChangeException exc = new ValueChangeException(oldValue, value);

                // The notification is unique per stat type
                this.PostNotification(WillChangeNotification(type), exc);

                // Did anything modify the value?
                value = Mathf.FloorToInt(exc.GetModifiedValue());

                // Did something nullify the change?
                if (exc.toggle == false || value == oldValue)
                    return;
            }

            data[(int)type] = value;
            this.PostNotification(DidChangeNotification(type), oldValue);
        }
    }
}