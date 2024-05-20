using UnityEngine;
using System.Collections;

namespace com.tksr.statemachine
{
    public abstract class State : MonoBehaviour
    {
        public virtual void Enter()
        {
            AddListeners();
        }

        public virtual void Exit()
        {
            RemoveListeners();
        }

        protected virtual void OnDestroy()
        {
            RemoveListeners();
        }

        protected virtual void AddListeners()
        {

        }

        protected virtual void RemoveListeners()
        {

        }
    }
}