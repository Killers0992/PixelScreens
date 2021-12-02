namespace LedDisplay
{
    using AdminToys;
    using Exiled.API.Features;
    using LedDisplay.Enums;
    using LedDisplay.Models;
    using Mirror;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    public class LedDisplayController : MonoBehaviour
    {
        public static Dictionary<int, LedDisplayController> LedDisplays { get; set; } = new Dictionary<int, LedDisplayController>();

        private static PrimitiveObjectToy primitiveBaseObject = null;

        public static PrimitiveObjectToy PrimitiveBaseObject
        {
            get
            {
                if (primitiveBaseObject == null)
                {
                    foreach (var gameObject in NetworkClient.prefabs.Values)
                    {
                        if (gameObject.TryGetComponent<PrimitiveObjectToy>(out var component))
                            primitiveBaseObject = component;
                    }
                }

                return primitiveBaseObject;
            }
        }


        public Color ColorOn = Color.green;                     
        public Color ColorOff = new Color(0f, 0f, 0f, 0f);

        public LedMatrixSymbolFontCollection FontsCollection { get; set; } = new LedMatrixSymbolFontCollection();
        public List<LedMatrixItem> Items = new List<LedMatrixItem>();

        public List<List<LedPixel>> Pixels = new List<List<LedPixel>>();

        bool initialized = false;

        public int DisplayId;
        public int NextId;

        private DisplayObjectType _displaytype = DisplayObjectType.Plane;

        public DisplayObjectType DisplayType
        {
            get
            {
                return _displaytype;
            }
            set
            {
                _displaytype = value;
                GeneratePixels();
            }
        }


        public int _sizex;
        public int _sizey;

        public float _pizelsize;
        public float _spacing;

        public int SizeX
        { 
            get
            {
                return _sizex;
            }
            set
            {
                _sizex = value;
                GeneratePixels();
            }
        }

        public int SizeY
        {
            get
            {
                return _sizey;
            }
            set
            {
                _sizey = value;
                GeneratePixels();
            }
        }

        public float PixelSize
        {
            get
            {
                return _pizelsize;
            }
            set
            {
                _pizelsize = value;
                GeneratePixels();
            }
        }

        public float Spacing
        {
            get
            {
                return _spacing;
            }
            set
            {
                _spacing = value;
                GeneratePixels();
            }
        }

        uint CurrentTicks = 0; 

        public float Interval = 0.1f;
        public bool IsMoving = false;

        private void Awake()
        {
            LedDisplays.Add(DisplayId, this);
            coroutine = StartCoroutine(Ticker());
        }

        private void OnDestroy()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            DestroyPixels(true);
            if (LedDisplays.ContainsKey(DisplayId))
                LedDisplays.Remove(DisplayId);
        }

        public Coroutine coroutine;

        public IEnumerator Ticker()
        {
            while (true)
            {
                yield return new WaitForSeconds(Interval);
                if (!IsMoving)
                    continue;

                try
                {

                    MoveItems(CurrentTicks);
                    CurrentTicks += 1;
                }
                catch (Exception) { }
            }
        }


        private void Update()
        {
            if (Pixels.Count == 0)
                return;

            for (int iIdxLine = 0; iIdxLine < Pixels.Count; iIdxLine++)
            {
                for (int iIdxRow = 0; iIdxRow < Pixels[0].Count; iIdxRow++)
                {
                    bool bLedOn = false;

                    Color lastColor = ColorOn;
                    foreach (LedMatrixItem diDisplayItem in Items)
                    {
                        int iLinePtForItem = iIdxLine - diDisplayItem.GetLineOffset();
                        int iRowPtForItem = iIdxRow - diDisplayItem.GetRowOffset();

                        if ((iLinePtForItem < 0) || (iLinePtForItem >= diDisplayItem.GetMasterLedOn().GetLength(0)) ||
                           (iRowPtForItem < 0) || (iRowPtForItem >= diDisplayItem.GetMasterLedOn().GetLength(1)))
                            continue;

                        bLedOn |= diDisplayItem.GetMasterLedOn()[iLinePtForItem, iRowPtForItem];
                        if (bLedOn)
                            lastColor = diDisplayItem.Color;
                    }

                    var targetPixel = Pixels[Pixels[iIdxLine][iIdxRow].Position.x - Pixels[iIdxLine][iIdxRow].Radius][Pixels[iIdxLine][iIdxRow].Position.y - Pixels[iIdxLine][iIdxRow].Radius];

                    if (bLedOn)
                    {
                        if (targetPixel.Color != lastColor)
                            targetPixel.Color = lastColor;
                    }

                    else
                    {
                        if (targetPixel.Color != ColorOff)
                            targetPixel.Color = ColorOff;
                    }

                }
            }
        }

        public void DestroyPixels(bool hard = false)
        {
            foreach (var pixelArray in Pixels)
            {
                foreach (var pixel in pixelArray)
                {
                    NetworkServer.UnSpawn(pixel.gameObject);
                }
            }

            if (hard)
                Pixels.Clear();
        }

        public void GeneratePixels(bool first = false)
        {
            if (first)
                initialized = true;

            if (!initialized)
                return;

            DestroyPixels();

            float offsetX = 0f;
            float offsetY = 0f;

            var newPixels = new List<List<LedPixel>>();

            for (int y = 0; y < SizeY; y++)
            {
                List<LedPixel> pixelsY = new List<LedPixel>();
                for (int x = 0; x < SizeX; x++)
                {
                    var primitive = Instantiate<PrimitiveObjectToy>(PrimitiveBaseObject);

                    var pixel = primitive.gameObject.AddComponent<LedPixel>();
                    pixel.ObjectToy = primitive;
                    pixel.Position = new Vector2Int(y, x);

                    primitive.NetworkPrimitiveType = DisplayType == DisplayObjectType.Plane ?
                        PrimitiveType.Plane :
                        PrimitiveType.Sphere;

                    primitive.SetPrimitive(PrimitiveType.Sphere, primitive.NetworkPrimitiveType);

                    primitive.transform.localScale = new Vector3(PixelSize, PixelSize, PixelSize);

                    primitive.transform.position = new Vector3((primitive._renderer.bounds.size.x * (transform.position.x - x)) - offsetX, (primitive._renderer.bounds.size.z * (transform.position.y - y)) + offsetY, transform.position.z);

                    primitive.transform.eulerAngles = transform.eulerAngles;

                    NetworkServer.Spawn(primitive.gameObject);

                    offsetX += Spacing;
                    pixelsY.Add(pixel);
                }
                newPixels.Add(pixelsY);
                offsetX = 0f;
                offsetY += Spacing;
            }

            Pixels = newPixels;
        }

        public static LedDisplayController CreateDisplay(int id, Vector3 position, Vector3 rotation, int sizeX, int sizeY, DisplayObjectType objectType, float pixelSize = 0.1f, float spacing = 0f, string fontPath = null)
        {
            if (LedDisplays.TryGetValue(id, out LedDisplayController oldDisplay))
            {
                UnityEngine.Object.Destroy(oldDisplay.gameObject);
            }

            var gm = new GameObject($"LedDisplay_{id}");
            gm.transform.position = position;
            gm.transform.localEulerAngles = rotation;
            var display = gm.AddComponent<LedDisplayController>();
            display.DisplayId = id;

            display.SizeX = sizeX;
            display.SizeY = sizeY;

            display.PixelSize = pixelSize;
            display.Spacing = spacing;

            display.DisplayType = objectType;

            var defaultFont = Utf8Json.JsonSerializer.Deserialize<LedMatrixSymbolFont>(PixelScreens.Properties.Resources.Default);

            if (!string.IsNullOrEmpty(fontPath))
                defaultFont = Utf8Json.JsonSerializer.Deserialize<LedMatrixSymbolFont>(File.ReadAllText(fontPath));

            display.FontsCollection.Fonts.Add(defaultFont);

            display.GeneratePixels(true);
            return display;
        }

        private int GetItemIdxFromId(int utemId)
        {
            int iIdx = 0;

            foreach (LedMatrixItem lmiItem in Items)
            {
                if (lmiItem.Id == utemId)
                {
                    return iIdx;
                }
                iIdx++;
            }

            return -1;
        }

        public int AddItem(bool[,] ledsOn,
                           Vector2Int location,
                           Color color,
                           ItemDirection direction,
                           ItemSpeed speed)
        {
            NextId++;

            LedMatrixItem lmiMyNewItem = new LedMatrixItem(
                NextId,
                ledsOn,
                location,
                color,
                direction,
                speed);

            Items.Add(lmiMyNewItem);

            return NextId;
        }

        public int AddTextItem(string text,
                               Vector2Int location,
                               Color color,
                               ItemDirection direction,
                               ItemSpeed speed)
        {
            NextId++;

            LedMatrixItem lmiMyNewItem = new LedMatrixItem(
                NextId,
                GetLedOnFromString(text),
                location,
                color,
                direction,
                speed);

            Items.Add(lmiMyNewItem);

            return NextId;
        }

        public void SetItemLocation(int itemId,
                                    Vector2Int position)
        {
            int iItemIdx = -1;

            iItemIdx = GetItemIdxFromId(itemId);
            if (iItemIdx == -1)
                return;

            Items[iItemIdx].CurrentLocation = position;
        }

        public void SetItemDirection(int itemId,
                                     ItemDirection direction)
        {
            int iItemIdx = -1;

            iItemIdx = GetItemIdxFromId(itemId);
            if (iItemIdx == -1)
                return;

            Items[iItemIdx].Direction = direction;
        }

        public void SetItemSpeed(int itemId,
                                 ItemSpeed speed)
        {
            int iItemIdx = -1;

            iItemIdx = GetItemIdxFromId(itemId);
            if (iItemIdx == -1)
                return;

            Items[iItemIdx].Speed = speed;
        }

        public void SetItemText(int itemId,
                                string text)
        {
            bool[,] bLedOn;
            int iItemIdx = -1;

            iItemIdx = GetItemIdxFromId(itemId);
            if (iItemIdx == -1)
                return;

            bLedOn = GetLedOnFromString(text);

            Items[iItemIdx].SetLedOnArray(bLedOn);
        }

        private void MoveItems(uint ticks)
        {
            foreach (LedMatrixItem lmiItem in Items)
                lmiItem.MoveItem(ticks, Pixels.Count, Pixels[0].Count);
        }

        public void StartMove(float interval)
        {
            Interval = interval;
            IsMoving = true;
        }

        public void StopMove()
        {
            IsMoving = false;
        }

        private bool[,] GetLedOnFromChar(char p_cSourceChar)
        {
            bool[,] bExclaim = new bool[4, 4];

            foreach (LedMatrixSymbol lmsSymbol in FontsCollection.Fonts[0].Symbols)
            {
                if (p_cSourceChar == lmsSymbol.SymbolCode)
                    return lmsSymbol.LedsOnMatrix;
            }

            bExclaim[0, 0] = true;
            bExclaim[0, 2] = true;
            bExclaim[1, 0] = true;
            bExclaim[1, 2] = true;
            bExclaim[3, 0] = true;
            bExclaim[3, 2] = true;
            return bExclaim;
        }

        private bool[,] GetLedOnFromString(string p_sSourceString)
        {
            int iMaxHeightChar = 0;
            int iNbOfLedUsed = 0;
            int iLedCount = 0;
            List<bool[,]> lstLedOnChar = new List<bool[,]>();

            for (int iIdxChar = 0; iIdxChar < p_sSourceString.Length; iIdxChar++)
            {
                lstLedOnChar.Add(GetLedOnFromChar(p_sSourceString[iIdxChar]));

                if (lstLedOnChar[lstLedOnChar.Count - 1].GetLength(0) > iMaxHeightChar)
                    iMaxHeightChar = lstLedOnChar[lstLedOnChar.Count - 1].GetLength(0);

                iNbOfLedUsed += lstLedOnChar[lstLedOnChar.Count - 1].GetLength(1);
            }

            bool[,] bReturnLedOn = new bool[iMaxHeightChar, iNbOfLedUsed];

            foreach (bool[,] bCurrentLedOn in lstLedOnChar)
            {
                for (int iIdxLine = 0; iIdxLine < bCurrentLedOn.GetLength(0); iIdxLine++)
                {
                    for (int iIdxRow = 0; iIdxRow < bCurrentLedOn.GetLength(1); iIdxRow++)
                    {
                        bReturnLedOn[iIdxLine, iLedCount + iIdxRow] = bCurrentLedOn[iIdxLine, iIdxRow];
                    }
                }

                iLedCount += bCurrentLedOn.GetLength(1);
            }

            return bReturnLedOn;
        }
    }
}