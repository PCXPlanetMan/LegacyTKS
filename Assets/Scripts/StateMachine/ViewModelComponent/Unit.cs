using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.tksr.property;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class Unit : MonoBehaviour
    {
        [HideInInspector]
        public EncapsuleTile Tile { get; protected set; }
        public EnumStateDirections Dir;

        // TODO:状态机应该尽量和业务表现层相分离
        private CharMainController cmc;
        private void Awake()
        {
            cmc = GetComponentInParent<CharMainController>();
        }

        public void Place(EncapsuleTile target)
        {
            // Make sure old tile location is not still pointing to this unit
            if (Tile != null && Tile.content == gameObject)
                Tile.content = null;

            // Link unit and tile references
            Tile = target;

            if (target != null)
                target.content = gameObject;
        }

        public void UnPlace()
        {
            if (Tile != null)
            {
                Tile.content = null;
            }
        }

        public void Match()
        {
            if (cmc)
            {
                cmc.PlaceAttTileByCellPos(Tile.VecPos);
                RefreshDirection();
            }
        }

        public void RefreshDirection()
        {
            switch (Dir)
            {
                case EnumStateDirections.North:
                    {
                        cmc.ForceChangeDirection("NW");
                    }
                    break;
                case EnumStateDirections.South:
                    {
                        cmc.ForceChangeDirection("SE");
                    }
                    break;
                case EnumStateDirections.East:
                    {
                        cmc.ForceChangeDirection("NE");
                    }
                    break;
                case EnumStateDirections.West:
                    {
                        cmc.ForceChangeDirection("SW");
                    }
                    break;
            }
        }

        public string GetCharName()
        {
            if (cmc != null)
            {
                return cmc.name;
            }
            return string.Empty;
        }

        /// <summary>
        /// 由状态机的启动的移动是由协程在每一帧驱动的,然后根据位置的移动方向驱动动画播放;若想停下来,则是start=end即可
        /// </summary>
        /// <param name="start">世界坐标</param>
        /// <param name="end">世界坐标</param>
        public void MoveFormTo(Vector3 start, Vector3 end)
        {
            Vector2 vecDirection = new Vector2(end.x - start.x, end.y - start.y);
            cmc.UpdateAnimDirectionFromSM(vecDirection);
        }

        public float PlayNormalAttackAnim()
        {
            return cmc.DoPlayNormalAttackAnim();
        }

        public void PlayIdleAnim()
        {
            cmc.UpdateAnimDirectionFromSM(Vector2.zero);
        }

        public IEnumerator PlayNormalAttackAnimAsync()
        {
            float fDuration = cmc.DoPlayNormalAttackAnim();
            yield return new WaitForSeconds(fDuration);
        }

        public IEnumerator PlayUnderAttackAnimAsync()
        {
            cmc.CharAnimRender.SetDisplayAlpha(0.75f);
            float fDuration = cmc.DoPlayUnderAttackAnim();
            yield return new WaitForSeconds(fDuration);
            cmc.CharAnimRender.SetDisplayAlpha(1f);
        }

        public IEnumerator PlayDeadAnimAsync()
        {
            cmc.CharAnimRender.SetDisplayAlpha(0.6f);
            float fDuration = cmc.DoPlayDeadAnim();
            yield return new WaitForSeconds(fDuration);
        }
    }
}