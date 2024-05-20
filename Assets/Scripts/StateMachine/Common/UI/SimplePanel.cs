using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class SimplePanel : MonoBehaviour
    {
        public Panel panel;
        public Image avatar;
        public Text command;
    
        public void Display(GameObject obj, string strCommand)
        {
            command.text = strCommand;
            // 动态获取头像
            CharMainController cmc = obj.GetComponentInParent<CharMainController>();
            if (cmc)
            {
                avatar.sprite = CharactersManager.Instance.ReadFullPortraitById(cmc.CharID);
            }
        }
    }
}