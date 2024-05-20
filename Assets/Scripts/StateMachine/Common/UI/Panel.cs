using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace com.tksr.statemachine
{
    [RequireComponent(typeof(LayoutAnchor))]
    public class Panel : MonoBehaviour
    {
        [Serializable]
        public class GUITilePosition
        {
            public string PosName;
            public TextAnchor MyAnchor;
            public TextAnchor ParentAnchor;
            public Vector2 Offset;

            public GUITilePosition(string posName)
            {
                this.PosName = posName;
            }

            public GUITilePosition(string posName, TextAnchor myAnchor, TextAnchor parentAnchor) : this(posName)
            {
                this.MyAnchor = myAnchor;
                this.ParentAnchor = parentAnchor;
            }

            public GUITilePosition(string posName, TextAnchor myAnchor, TextAnchor parentAnchor, Vector2 offset) : this(posName, myAnchor, parentAnchor)
            {
                this.Offset = offset;
            }
        }

        [SerializeField]
        private List<GUITilePosition> positionList;

        private Dictionary<string, GUITilePosition> positionMap;
        private LayoutAnchor anchor;

        public GUITilePosition CurrentPosition { get; private set; }
        public Tweener Transition { get; private set; }
        public bool InTransition { get { return Transition != null; } }

        public GUITilePosition this[string posName]
        {
            get
            {
                if (positionMap.ContainsKey(posName))
                    return positionMap[posName];
                return null;
            }
        }

        void Awake()
        {
            anchor = GetComponent<LayoutAnchor>();
            positionMap = new Dictionary<string, GUITilePosition>(positionList.Count);
            for (int i = positionList.Count - 1; i >= 0; --i)
                AddPosition(positionList[i]);
        }

        void Start()
        {
            if (CurrentPosition == null && positionList.Count > 0)
                SetPosition(positionList[0], false);
        }

        public void AddPosition(GUITilePosition p)
        {
            positionMap[p.PosName] = p;
        }

        public void RemovePosition(GUITilePosition p)
        {
            if (positionMap.ContainsKey(p.PosName))
                positionMap.Remove(p.PosName);
        }

        public Tweener SetPosition(string positionName, bool animated)
        {
            return SetPosition(this[positionName], animated);
        }

        public Tweener SetPosition(GUITilePosition p, bool animated)
        {
            CurrentPosition = p;
            if (CurrentPosition == null)
                return null;

            if (InTransition)
                Transition.Stop();

            if (animated)
            {
                Transition = anchor.MoveToAnchorPosition(p.MyAnchor, p.ParentAnchor, p.Offset);
                return Transition;
            }
            else
            {
                anchor.SnapToAnchorPosition(p.MyAnchor, p.ParentAnchor, p.Offset);
                return null;
            }
        }
    }
}