using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Calculator
{
    class EquationHandler
    {
        private List<EquationElement> elements = new List<EquationElement>();

        public RichTextBlock equationText;

        private int cursorIndex = -1;
        private bool cursorDisplay = false;

        public bool isDegrees = true;
        public bool isSecond = false;

        public EquationHandler(RichTextBlock equationText)
        {
            this.equationText = equationText;
        }

        public void ClearAll()
        {
            elements.Clear();

            cursorIndex = -1;

            CursorUpdate();
        }

        public void CursorTick()
        {
            cursorDisplay = !cursorDisplay;

            CursorUpdate();
        }

        public void CursorUpdate()
        {
            equationText.Blocks.Clear();

            Paragraph paragraph = new Paragraph();
            Run run = new Run { Text = GetElementsDisplayString() };

            paragraph.Inlines.Add(run);
            
            equationText.Blocks.Add(paragraph);
        }

        public void MoveCursor(int relativeIndex)
        {
            int index = cursorIndex + relativeIndex;

            if (index >= -1 && index < elements.Count)
            {
                cursorIndex = index;

                CursorTick();
            }
        }

        public void AddElement(EquationElement.ElementType type, String element)
        {
            int index = cursorIndex + 1;

            /*
            if (index >= 0 && index < elements.Count && elements.ElementAt(index).GetCharacter() == '□')
            {
                elements.RemoveAt(cursorIndex + 1);
            }
            */

            AddElement(type, element, element.Length == 1);

            if (element.Contains("□"))
            {
                cursorIndex = cursorIndex - 1 - (element.Length - 1 - element.IndexOf("□"));

                CursorUpdate();
            }
        }

        public void AddElement(EquationElement.ElementType type, String character, bool removeEmpty)
        {
            EquationElement equationElement = new EquationElement(type, character);

            elements.Insert(cursorIndex + 1, equationElement);

            cursorIndex++;

            CursorUpdate();
        }

        public void InsertElement(int index, EquationElement.ElementType type, String character)
        {
            elements.Insert(index, new EquationElement(type, character));

            cursorIndex++;

            CursorUpdate();
        }

        public void SetElement(int index, EquationElement.ElementType type, String character)
        {
            elements[index] = new EquationElement(type, character);

            CursorUpdate();
        }

        public void RemoveElement(int index)
        {
            if (index >= 0 && index < elements.Count)
            {
                elements.RemoveAt(index);

                cursorIndex--;

                CursorUpdate();
            }
        }

        public void RemoveLastElement()
        {
            RemoveElement(cursorIndex);

            CursorUpdate();
        }

        public String GetElementsDisplayString()
        {
            String returnString = elements.Count == 0 ? (cursorDisplay ? "|" : "") : (cursorIndex < 0 ? (cursorDisplay ? "|" : "") : "");

            for (int i = 0; i < elements.Count; i++)
            {
                returnString += elements[i].GetCharacter();

                if (i == cursorIndex && cursorDisplay)
                {
                    returnString += "|";
                }
            }

            return returnString;
        }

        public String GetElementsString()
        {
            String returnString = "";

            for (int i = 0; i < elements.Count; i++)
            {
                returnString += elements[i].GetCharacter();
            }

            return returnString;
        }

        public Equation ToEquation()
        {
            return new Equation(GetElementsString(), this);
        }
    }
}
