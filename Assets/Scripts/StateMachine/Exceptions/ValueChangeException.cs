﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace com.tksr.statemachine
{
    public class ValueChangeException : BaseException
    {

        public readonly float fromValue;
        public readonly float toValue;
        public float delta { get { return toValue - fromValue; } }
        List<ValueModifier> modifiers;

        public ValueChangeException(float fromValue, float toValue) : base(true)
        {
            this.fromValue = fromValue;
            this.toValue = toValue;
        }

        public void AddModifier(ValueModifier m)
        {
            if (modifiers == null)
                modifiers = new List<ValueModifier>();
            modifiers.Add(m);
        }

        public float GetModifiedValue()
        {
            if (modifiers == null)
                return toValue;

            float value = toValue;
            modifiers.Sort(Compare);
            for (int i = 0; i < modifiers.Count; ++i)
                value = modifiers[i].Modify(fromValue, value);

            return value;
        }

        int Compare(ValueModifier x, ValueModifier y)
        {
            return x.sortOrder.CompareTo(y.sortOrder);
        }
    }
}