using System;
using System.Numerics;


namespace ThreeDimensional
{
    public enum CellType
    {
        Empty,
        Integer,
        Operator
    }

    public class Cell
    {
        public CellType Type { get; set; }
        public BigInteger? IntegerValue { get; set; }
        public char? OperatorValue { get; set; }

        public override string ToString()
        {
            switch (Type)
            {
                case CellType.Empty:
                    return ".";
                case CellType.Integer:
                    return IntegerValue.ToString();
                case CellType.Operator:
                    return OperatorValue.ToString();
                default:
                    return "?";
            }
        }

        public Cell Clone()
        {
            Cell clone = new Cell
            {
                Type = Type,
                IntegerValue = IntegerValue,
                OperatorValue = OperatorValue
            };

            return clone;
        }

        public static bool operator ==(Cell left, Cell right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            if (left.Type != right.Type)
            {
                return false;
            }

            switch (left.Type)
            {
                case CellType.Empty:
                    return true;
                case CellType.Integer:
                    return left.IntegerValue == right.IntegerValue;
                case CellType.Operator:
                    return left.OperatorValue == right.OperatorValue;
                default:
                    return false;
            }
        }

        public static bool operator !=(Cell left, Cell right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj is Cell cell)
            {
                return this == cell;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Type.GetHashCode();
                hash = hash * 23 + (IntegerValue?.GetHashCode() ?? 0);
                hash = hash * 23 + (OperatorValue?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }
}
