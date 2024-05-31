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
            Null,
            TimesZero
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
                switch (Type)
                {
                    case NumberType.Common:
                        if (Value < 0) return string.Format("({0})", Value);
                        return Value.ToString();
                    case NumberType.Signum:
                        return string.Format("(±{0})", Math.Abs(Value));
                    case NumberType.Infinitive:
                        return "∞";
                    case NumberType.Double:
                        return "[×2]";
                    case NumberType.Unknown:
                        return "x";
                    case NumberType.Null:
                        return "Null";
                    case NumberType.TimesZero:
                        return "[×0]";
                    default:
                        return "NaN";
                }
            }
            set => throw new ReadOnlyException();
        }
    }
}
