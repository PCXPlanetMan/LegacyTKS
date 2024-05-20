using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using com.tksr.statemachine.defines;
using System;

namespace com.tksr.statemachine
{
    public class CommandFixPanel : MonoBehaviour
    {
        public Panel panel;
        public Button BtnMove;      // 固定移动
        public Button BtnAttack;    // 固定攻击
        public Button BtnSlot1;     // 可变技能槽1
        public Button BtnSlot2;     // 可变技能槽2
        public Button BtnDefence;   // 固定移动
        public Button BtnWait;      // 固定休息

        [HideInInspector]
        public AbilityMenuPanelUIController parentController;

        void Start()
        {
            BtnMove.onClick.AddListener(OnClickBtnMove);
            BtnAttack.onClick.AddListener(OnClickBtnAttack);
            BtnSlot1.onClick.AddListener(OnClickBtnSlot1);
            BtnSlot2.onClick.AddListener(OnClickBtnSlot2);
            BtnDefence.onClick.AddListener(OnClickBtnDefense);
            BtnWait.onClick.AddListener(OnClickBtnWait);
        }

        public void Display(GameObject obj)
        {
            string slot1 = "技     能";
            string slot2 = "技     能";
            BtnSlot1.GetComponentInChildren<Text>().text = slot1;
            BtnSlot2.GetComponentInChildren<Text>().text = slot2;
            /*
            Alliance alliance = obj.GetComponent<Alliance>();
            // avatar.sprite = null; Need a component which provides this data
            Unit unitGo = obj.GetComponent<Unit>();
            nameLabel.text = unitGo.GetCharName();
            Stats stats = obj.GetComponent<Stats>();
            if (stats)
            {
                hpLabel.text = string.Format("{0} / {1}", stats[EnumStatTypes.HP], stats[EnumStatTypes.MHP]);
                mpLabel.text = string.Format("{0} / {1}", stats[EnumStatTypes.MP], stats[EnumStatTypes.MMP]);
                lvLabel.text = string.Format("{0}", stats[EnumStatTypes.LVL]);
            }
            */
        }

        private void OnClickBtnMove()
        {
            Debug.Log("OnClickBtnMove");
            if (parentController != null)
                parentController.DoCommand(new InfoEventArgs<EnumActionCommand>(EnumActionCommand.Move));
            LockButton(BtnMove);
        }

        private void OnClickBtnAttack()
        {
            Debug.Log("OnClickBtnAttack");
            if (parentController != null)
                parentController.DoCommand(new InfoEventArgs<EnumActionCommand>(EnumActionCommand.Attack));
            LockButton(BtnAttack);
        }

        private void OnClickBtnSlot1()
        {
            Debug.Log("OnClickBtnSlot1");
            if (parentController != null)
                parentController.DoCommand(new InfoEventArgs<EnumActionCommand>(EnumActionCommand.Slot1));
            LockButton(BtnSlot1);
        }

        private void OnClickBtnSlot2()
        {
            Debug.Log("OnClickBtnSlot2");
            if (parentController != null)
                parentController.DoCommand(new InfoEventArgs<EnumActionCommand>(EnumActionCommand.Slot2));
            LockButton(BtnSlot2);
        }

        private void OnClickBtnDefense()
        {
            Debug.Log("OnClickBtnDefense");
            if (parentController != null)
                parentController.DoCommand(new InfoEventArgs<EnumActionCommand>(EnumActionCommand.Defense));
            LockButton(BtnDefence);
        }

        private void OnClickBtnWait()
        {
            Debug.Log("OnClickBtnWait");
            if (parentController != null)
                parentController.DoCommand(new InfoEventArgs<EnumActionCommand>(EnumActionCommand.Wait));
            LockButton(BtnWait);
        }

        public void ResetButtons()
        {
            UnLockButton(BtnMove);
            UnLockButton(BtnAttack);
            UnLockButton(BtnSlot1);
            UnLockButton(BtnSlot2);
            UnLockButton(BtnDefence);
            UnLockButton(BtnWait);
        }

        public void LockButtonByIndex(int index, bool locked)
        {
            switch (index)
            {
                case 0:
                {
                    if (locked)
                    {
                        LockButton(BtnMove);
                    }
                    else
                    {
                        UnLockButton(BtnMove);
                    }
                }
                    break;
                case 1:
                {
                    if (locked)
                    {
                        LockButton(BtnAttack);
                    }
                    else
                    {
                        UnLockButton(BtnAttack);
                    }
                }
                    break;
                case 2:
                {
                    if (locked)
                    {
                        LockButton(BtnSlot1);
                    }
                    else
                    {
                        UnLockButton(BtnSlot1);
                    }
                }
                    break;
                case 3:
                {
                    if (locked)
                    {
                        LockButton(BtnSlot2);
                    }
                    else
                    {
                        UnLockButton(BtnSlot2);
                    }
                }
                    break;
                case 4:
                {
                    if (locked)
                    {
                        LockButton(BtnDefence);
                    }
                    else
                    {
                        UnLockButton(BtnDefence);
                    }
                }
                    break;
                case 5:
                {
                    if (locked)
                    {
                        LockButton(BtnWait);
                    }
                    else
                    {
                        UnLockButton(BtnWait);
                    }
                }
                    break;
            }
        }

        private void LockButton(Button btn)
        {
            btn.enabled = false;
            btn.GetComponentInChildren<Text>().color = Color.gray;
        }

        private void UnLockButton(Button btn)
        {
            btn.enabled = true;
            btn.GetComponentInChildren<Text>().color = Color.white;
        }

        private Button GetButtonByIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return BtnMove;
                case 1:
                    return BtnAttack;
                case 2:
                    return BtnSlot1;
                case 3:
                    return BtnSlot2;
                case 4:
                    return BtnDefence;
                case 5:
                    return BtnWait;
            }
            return null;
        }
    }
}