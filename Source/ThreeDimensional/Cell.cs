using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public int? IntegerValue { get; set; }
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
    }
}
