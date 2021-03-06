﻿using System;

namespace Platform_Creator_CS.Sources.UI {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class UI : Attribute {
        public string Name { get; }
        public string Description { get; }
        public bool IsTag { get; }

        public float MinValue { get; }
        public float MaxValue { get; }

        public UI(string name = null, string description = null, bool isTag = false, float minValue = 0,
            float maxValue = 0) {
            Name = name;
            Description = description;
            IsTag = isTag;
            MinValue = minValue;
            MaxValue = maxValue;
        }
    }
}