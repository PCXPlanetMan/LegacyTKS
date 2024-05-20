using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class StatPanel : MonoBehaviour
    {
        public Panel panel;
        public Image avatar;
        public Text nameLabel;
        public Text hpLabel;
        public Text mpLabel;
        public Text lvLabel;
        public Image hpImage;
        public Image mpImage;

        private float fHpMaxLength;
        private float fMpMaxLength;
        void Start()
        {
            fHpMaxLength = hpImage.rectTransform.rect.width;
            fMpMaxLength = mpImage.rectTransform.rect.width;
        }
    
        public void Display(GameObject obj)
        {
            Alliance alliance = obj.GetComponent<Alliance>();
            // 动态获取头像
            CharMainController cmc = obj.GetComponentInParent<CharMainController>();
            if (cmc)
            {
                avatar.sprite = CharactersManager.Instance.ReadFullPortraitById(cmc.CharID);
            }

            Unit unitGo = obj.GetComponent<Unit>();
            nameLabel.text = unitGo.GetCharName();
            Stats stats = obj.GetComponent<Stats>();
            if (stats)
            {
                hpLabel.text = string.Format(ResourceUtils.FORMAT_STATS_VALUE, stats[EnumStatTypes.HP], stats[EnumStatTypes.MHP]);
                mpLabel.text = string.Format(ResourceUtils.FORMAT_STATS_VALUE, stats[EnumStatTypes.MP], stats[EnumStatTypes.MMP]);
                lvLabel.text = string.Format("{0}", stats[EnumStatTypes.LVL]);
                float fHpRatio = stats[EnumStatTypes.HP] * 1f / stats[EnumStatTypes.MHP];
                hpImage.rectTransform.sizeDelta = new Vector2(fHpRatio * fHpMaxLength, hpImage.rectTransform.sizeDelta.y);
                float fMpRatio = stats[EnumStatTypes.MP] * 1f / stats[EnumStatTypes.MMP];
                mpImage.rectTransform.sizeDelta = new Vector2(fMpRatio * fMpMaxLength, mpImage.rectTransform.sizeDelta.y);
            }
        }
    }
}