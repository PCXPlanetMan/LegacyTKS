using UnityEngine;
using System;
using System.Collections;

namespace com.tksr.statemachine
{
    class Repeater
    {
        const float threshold = 0.5f;
        const float rate = 0.25f;
        float _next;
        bool _hold;
        string _axis;

        public Repeater(string axisName)
        {
            _axis = axisName;
        }

        public int Update()
        {
            int retValue = 0;
            int value = Mathf.RoundToInt(Input.GetAxisRaw(_axis));

            if (value != 0)
            {
                if (Time.time > _next)
                {
                    retValue = value;
                    _next = Time.time + (_hold ? rate : threshold);
                    _hold = true;
                }
            }
            else
            {
                _hold = false;
                _next = 0;
            }

            return retValue;
        }
    }

    public class InputController : MonoBehaviour
    {
        public static event EventHandler<InfoEventArgs<Vector2Int>> moveEvent;
        public static event EventHandler<InfoEventArgs<int>> fireEvent;
        public static event EventHandler<InfoEventArgs<Vector3>> mouseEvent;

        Repeater _hor = new Repeater("Horizontal");
        Repeater _ver = new Repeater("Vertical");
        string[] _buttons = new string[] { "Fire1", "Fire2", "Fire3" };

        void Update()
        {
            // TKS在PC上通过键盘移动地图 
            int x = _hor.Update();
            int y = _ver.Update();
            if (x != 0 || y != 0)
            {
                if (moveEvent != null)
                    moveEvent(this, new InfoEventArgs<Vector2Int>(new Vector2Int(x, y)));
            }

            for (int i = 0; i < 3; ++i)
            {
                if (Input.GetButtonUp(_buttons[i]))
                {
                    if (fireEvent != null)
                        fireEvent(this, new InfoEventArgs<int>(i));
                }
            }

            if (mouseEvent != null)
            {
                mouseEvent(this, new InfoEventArgs<Vector3>(Input.mousePosition));
            }
        }
    }
}