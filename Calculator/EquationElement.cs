using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calculator
{
    class EquationElement
    {
        private ElementType type = ElementType.NORMAL;

        private String character;

        public EquationElement(ElementType type, String character)
        {
            this.type = type;

            this.character = character;
        }

        public enum ElementType
        {
            CURSOR,
            NORMAL,
            RAISE,
            LOWER
        }

        public ElementType GetElementType()
        {
            return type;
        }

        public String GetCharacter()
        {
            return character;
        }
    }
}
