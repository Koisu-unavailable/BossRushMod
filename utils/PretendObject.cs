using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BossRush.utils
{
    public class PretendObject<T>(T val, T valueToIgnore) where T: IEquatable<T>
    {
        private T value = val;
        private T _valueToIgnore = valueToIgnore;

        public static implicit operator T(PretendObject<T> pretendObject)
        {
            return pretendObject.value.Equals(pretendObject._valueToIgnore) ? throw new Exception("Test") : pretendObject.value;
        }
    }
}