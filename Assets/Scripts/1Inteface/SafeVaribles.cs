using System;
using UnityEngine;

namespace HeroLeft.Interfaces {

    [Serializable]
    public struct SafeFloat {
        private float offset;
        [UnityEngine.SerializeField]private float value;

        public SafeFloat(float value = 0) {
            offset = UnityEngine.Random.Range(-10000, 10000);
            this.value = value + offset;
        }

        public float GetValue() {
            return value - offset;
        }

        public void Dispose() {
            offset = 0;
            value = 0;
        }
        public override string ToString() {
            return GetValue().ToString();
        }
        public static SafeFloat operator +(SafeFloat i1, SafeFloat i2) {
            return new SafeFloat(i1.GetValue() + i2.GetValue());
        }
        public static SafeFloat operator -(SafeFloat i1, SafeFloat i2) {
            return new SafeFloat(i1.GetValue() - i2.GetValue());
        }
        public static bool operator ==(SafeFloat i1, SafeFloat i2) {
            if (i1.GetValue() == i2.GetValue()) {
                return true;
            }
            return false;
        }
        public static bool operator !=(SafeFloat i1, SafeFloat i2) {
            if (i1.GetValue() != i2.GetValue()) {
                return true;
            }
            return false;
        }
        public static bool operator <(SafeFloat i1, SafeFloat i2) {
            if (i1.GetValue() < i2.GetValue()) {
                return true;
            }
            return false;
        }
        public static bool operator >(SafeFloat i1, SafeFloat i2) {
            if (i1.GetValue() > i2.GetValue()) {
                return true;
            }
            return false;
        }
        public static bool operator <=(SafeFloat i1, SafeFloat i2) {
            if (i1.GetValue() <= i2.GetValue()) {
                return true;
            }
            return false;
        }
        public static bool operator >=(SafeFloat i1, SafeFloat i2) {
            if (i1.GetValue() >= i2.GetValue()) {
                return true;
            }
            return false;
        }
        public static implicit operator float(SafeFloat i) {
            return i.GetValue();
        }
        public static implicit operator SafeFloat(float i) {
            return new SafeFloat(i);
        }
    }

    [Serializable]
    public struct SafeInt {
        private int offset;
        [UnityEngine.SerializeField]private int value;

        public SafeInt(int value = 0) {
            offset = UnityEngine.Random.Range(-10000, 10000);
            this.value = value + offset;
        }

        public int GetValue() {
            return value - offset;
        }

        public void Dispose() {
            offset = 0;
            value = 0;
        }
        public override string ToString() {
            return GetValue().ToString();
        }
        public static SafeInt operator +(SafeInt i1, SafeInt i2) {
            return new SafeInt(i1.GetValue() + i2.GetValue());
        }
        public static SafeInt operator -(SafeInt i1, SafeInt i2) {
            return new SafeInt(i1.GetValue() - i2.GetValue());
        }
        public static bool operator ==(SafeInt i1, SafeInt i2) {
            if (i1.GetValue() == i2.GetValue()) {
                return true;
            }
            return false;
        }
        public static bool operator !=(SafeInt i1, SafeInt i2) {
            if (i1.GetValue() != i2.GetValue()) {
                return true;
            }
            return false;
        }
        public static bool operator <(SafeInt i1, SafeInt i2) {
            if (i1.GetValue() < i2.GetValue()) {
                return true;
            }
            return false;
        }
        public static bool operator >(SafeInt i1, SafeInt i2) {
            if (i1.GetValue() > i2.GetValue()) {
                return true;
            }
            return false;
        }
        public static bool operator <=(SafeInt i1, SafeInt i2) {
            if (i1.GetValue() <= i2.GetValue()) {
                return true;
            }
            return false;
        }
        public static bool operator >=(SafeInt i1, SafeInt i2) {
            if (i1.GetValue() >= i2.GetValue()) {
                return true;
            }
            return false;
        }
        public static implicit operator int(SafeInt i) {
            return i.GetValue();
        }

        public static implicit operator SafeInt(int i) {
            return new SafeInt(i);
        }
    }

    public class RangeAttribute : PropertyAttribute {
        public float min;
        public float max;

        public RangeAttribute(float min, float max) {
            this.min = min;
            this.max = max;
        }
    }
}
