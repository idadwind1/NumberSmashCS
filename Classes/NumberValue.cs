using System;
using System.Data;

namespace NumberGamePlus.Classes
{
    public class NumberValue
    {
        public enum NumberType
        {
            Common,
            Signum,
            Infinitive,
            Double,
            Unknown,
            Null
        }

        public readonly NumberType Type = NumberType.Common;

        public readonly int Value = 0;

        public NumberValue(NumberType type, int value)
        {
            Type = type;
            Value = value;
        }

        public NumberValue()
        {

        }

        public string ValueString
        {
            get
            {
                if (Type == NumberType.Common)
                {
                    if (Value < 0) return string.Format("({0})", Value);
                    return Value.ToString();
                }
                if (Type == NumberType.Signum)
                    return string.Format("(±{0})", Math.Abs(Value));
                if (Type == NumberType.Infinitive)
                    return "∞";
                if (Type == NumberType.Unknown)
                    return "x";
                if (Type == NumberType.Double)
                    return "[×2]";
                if (Type == NumberType.Null)
                    return "Null";
                return "NaN";
            }
            set => throw new ReadOnlyException();
        }
    }
}
