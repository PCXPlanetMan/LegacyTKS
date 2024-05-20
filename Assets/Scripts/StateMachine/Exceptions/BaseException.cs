using UnityEngine;
using System.Collections;

namespace com.tksr.statemachine
{
    public class BaseException
    {
        public bool toggle { get; private set; }
        public readonly bool defaultToggle;

        public BaseException(bool defaultToggle)
        {
            this.defaultToggle = defaultToggle;
            toggle = defaultToggle;
        }

        public void FlipToggle()
        {
            toggle = !defaultToggle;
        }
    }
}