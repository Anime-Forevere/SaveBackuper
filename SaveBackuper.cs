using MSCLoader;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SaveBackuper
{
    public class SaveBackuper : Mod
    {
        public override string ID => "SaveBackuper"; // Your (unique) mod ID 
        public override string Name => "SaveBackuper"; // Your mod name
        public override string Author => "Anime-Forevere"; // Name of the Author (your name)
        public override string Version => "1.1"; // Version
        public override string Description => "Automatically backs up your saves on each save load"; // Short description of your mod

        public override void ModSetup()
        {
            SetupFunction(Setup.OnLoad, Mod_OnLoad);
            SetupFunction(Setup.ModSettings, Mod_Settings);
        }
        readonly string MSCSaves = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace("Roaming", "LocalLow") + "/Amistech";

        private void DoBackup()
        {
            try
            {
                string Date = DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss");

                Directory.CreateDirectory(MSCSaves + "/Backup/" + Date);

                string[] Files = Directory.GetFiles(MSCSaves + "/My Summer Car");
                foreach (string file in Files)
                {
                    string FileName = Path.GetFileName(file);
                    File.Copy(file, MSCSaves + "/Backup/" + Date + "/" + FileName);
                }
                ModConsole.Log("<color=#ffffff>[SaveBackuper] Backup done successfully</color>");
            }
            catch (Exception ex)
            {
                ModConsole.Error("[SaveBackuper] Error happened during backing up files\n" + ex.Message);
            }
        }

        SettingsDropDownList SavesList;
        private void Mod_Settings()
        {
            Settings.AddHeader(this, "How it works");
            Settings.AddText(this, "Basically, this mod just backs your save up everytime you load into it.");


            Settings.AddHeader(this, "Backups");
            if (!Directory.Exists(MSCSaves + "/Backup"))
            {
                Settings.AddText(this, "You don't have any backup yet.");
                return;
            }

            string[] SaveDirectories = Directory.GetDirectories(MSCSaves + "/Backup/");
            if (SaveDirectories.Length < 1)
            {
                Settings.AddText(this, "You don't have any backup yet.");
                return;
            }
            SaveDirectories.Reverse();
            for (int i = 0; i < SaveDirectories.Length; i++)
            {
                SaveDirectories[i] = Path.GetFileName(SaveDirectories[i]);
            }

            SavesList = Settings.AddDropDownList(this, "saveselect", "Save file to load (THIS WILL REPLACE YOUR CURRENT SAVE!)", SaveDirectories, 0, OnSaveSelect);
        }

        private void OnSaveSelect()
        {
            string SaveName = SavesList.GetSelectedItemName();
            if (SaveName == null) return;

            string[] CurrentSave = Directory.GetFiles(MSCSaves + "/My Summer Car/");
            foreach (string file in CurrentSave)
            {
                File.Delete(file);
            }

            string[] FilesToCopy = Directory.GetFiles(MSCSaves + "/Backup/" + SaveName);
            foreach (string file in FilesToCopy) { 
                string FileName = Path.GetFileName(file);
                File.Copy(file, MSCSaves + "/My Summer Car/" + FileName);
            }
        }

        private void Mod_OnLoad()
        {
            if(!Directory.Exists(MSCSaves + "/Backup"))
            {
                Directory.CreateDirectory(MSCSaves + "/Backup");
            }

            ModConsole.Log("<color=#ffffff>[SaveBackuper] Backing up SaveFile...</color>");

            DoBackup();
        }
    }
}
