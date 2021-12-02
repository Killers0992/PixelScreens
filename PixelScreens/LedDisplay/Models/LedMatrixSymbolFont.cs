namespace LedDisplay.Models
{
    using System.Collections.Generic;

    public class LedMatrixSymbolFont
    {
        public string FontName { get; set; }
        public List<LedMatrixSymbol> Symbols { get; set; } = new List<LedMatrixSymbol>();
    }
}