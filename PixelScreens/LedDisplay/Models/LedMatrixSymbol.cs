using System.Collections.Generic;

namespace LedDisplay.Models
{
    public class LedMatrixSymbol
    {
        public uint SymbolCode { get; set; }

        public string Description { get; set; }

        public List<string> LedLine { get; set; } = new List<string>();

        private bool[,] _ledsOnMatrix;

        public bool[,] LedsOnMatrix
        { 
            get
            {
                if (_ledsOnMatrix != null)
                    return _ledsOnMatrix;
                _ledsOnMatrix = ConvertLedOnLineToLedOnMatrix(LedLine);
                return _ledsOnMatrix;
            }
        }

        private bool[,] ConvertLedOnLineToLedOnMatrix(List<string> lines)
        {
            int iMaxLineSize = 0;
            int iMaxLineNb = 0;
            bool[,] bReturnLedOnMatrix;

            for(int x = 0; x < lines.Count; x++)
            {
                if (lines[x].Length > iMaxLineSize)
                {
                    iMaxLineSize = lines[x].Length;
                }

                if (x > iMaxLineNb)
                {
                    iMaxLineNb = x;
                }
            }

            bReturnLedOnMatrix = new bool[iMaxLineNb + 1, iMaxLineSize];

            for (int x = 0; x < lines.Count; x++)
            {
                for (int iIdxChar = 0; iIdxChar < lines[x].Length; iIdxChar++)
                {
                    if (lines[x][iIdxChar] == '#')
                    {
                        bReturnLedOnMatrix[x, iIdxChar] = true;
                    }
                }
            }

            return bReturnLedOnMatrix;
        }
    }
}
