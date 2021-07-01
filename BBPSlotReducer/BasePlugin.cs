using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Net;
using System.IO;
//BepInEx stuff
using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using HarmonyLib; //god im hoping i got the right version of harmony
using BepInEx.Configuration;
using BBPlusNameAPI;
using System.Collections.Generic;

namespace BBPSlotReducer
{
    [BepInPlugin("mtm101.rulerp.bbplus.bbpsr", "BB+ Slot Reducer", "1.0.0.0")]

    public class BaldiSlotReducer : BaseUnityPlugin
    {
        public const string CrossImage = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAMAAABEpIrGAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAzUExURf8AAOgAAL0AAMQAAKQAAIsAAJcAALMAANUAAGoAAFgAAEwAAEUAAPUAADgAAHkAAAAAAJb/ev4AAAARdFJOU/////////////////////8AJa2ZYgAAAAlwSFlzAAAOwwAADsMBx2+oZAAAARFJREFUOE+NklmSgzAMRL0FARYT7n/aaS3GNqQq6Q+h5SELmXB+0Y9ACDFG9UwhpJzdVQNlSDMQwvJaFvM9ESJRWjWl4bZvuwVmIYKuYK3Mm0VqJ0J8Zq5aaMAhbxkg9VorW6EBmk5KCArADuiAHZISyRN1P+AGrIkiHstQ74C1sAb7R0AJGXY84AFA44TQADgxN5gAJTbsyEPVo4Mto+s5Q5mJD0MSFb9WUQekuMtnosPQ4gKknrjCHlQoeXYGDnzAem/RAGmAn4TVGQkHJO0bgJcBtDkNkHpG/eX+0KIDqNsK4R8AfE4FprpG/vtJ5JnxCiQupXigBhvgP41FArzffuU+g9pLspLG30pPfQHO8x9Wdi5f/2lu+wAAAABJRU5ErkJggg==";
        public static Sprite CrossSprite;

        private static Texture TextureFromBase64(string base64)
        {
            byte[] array = Convert.FromBase64String(base64);
            Texture2D texture2D = new Texture2D(1, 1);
            ImageConversion.LoadImage(texture2D, array);
            texture2D.filterMode = 0;
            return texture2D;
        }

        private static bool HasAlreadyLoaded = false;
        private static void Load()
        {
            if (HasAlreadyLoaded)
            {
                return;
            }
            HasAlreadyLoaded = true;
            string pathtoload = Path.Combine(Application.persistentDataPath, "Mods", "BBSlotReducer", "data.dat");
            if (File.Exists(pathtoload))
            {
                FileStream stream = File.OpenRead(pathtoload);
                BinaryReader reader = new BinaryReader(stream);
                try
                {
                    SlotsPriv = reader.ReadByte();
                }
                catch (Exception E)
                {
                    UnityEngine.Debug.LogError(E.Message);
                }
                reader.Close();
            }
            else
            {
                Save();
            }
        }

        private static void Save()
        {
            string pathtosave = Path.Combine(Application.persistentDataPath, "Mods", "BBSlotReducer");
            if (!Directory.Exists(pathtosave))
            {
                Directory.CreateDirectory(pathtosave);
            }
            DirectoryInfo fo = new DirectoryInfo(pathtosave);
            FileInfo[] filefo = fo.GetFiles("data.dat");
            if (filefo.Length != 0)
            {
                filefo[0].Delete();
            }
            FileStream stream = File.Create(Path.Combine(pathtosave, "data.dat"));
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write((byte)SlotsPriv);

            writer.Close();
        }

        public static byte Slots
        {
            get
            {
                Load();
                return SlotsPriv;
            }

            set
            {
                SlotsPriv = value;
                Save();
            }


        }

        private static byte SlotsPriv = 1;
        public static object ChangeValue()
        {
            SlotsPriv++;
            if (SlotsPriv == 6)
            {
                SlotsPriv = 1;
            }
            return SlotsPriv;
        }
        void Awake()
        {
            Harmony harmony = new Harmony("mtm101.rulerp.bbplus.bbpsr");
            NameMenuManager.AddPage("bbsroptions","options");
            NameMenuManager.AddToPage("bbsroptions", new Name_MenuOption("changeslots", "Slots Available", Slots, ChangeValue));
            NameMenuManager.AddToPage("options", new Name_MenuFolder("tobbrsoptions", "Slot Reducer", "bbsroptions"));
            Load();
            Texture tex = BaldiSlotReducer.TextureFromBase64(CrossImage);
            CrossSprite = Sprite.Create((Texture2D)tex, new Rect(0f, 0f, tex.width, tex.height), Vector2.zero);
            harmony.PatchAll();
        }
    }
}
