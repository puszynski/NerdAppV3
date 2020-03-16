using NerdAppV3.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NerdAppV3
{
    public class JsonConverter
    {
        //TODO - ADD ADDITIONAL SAVE TO TARGETED PASTH (E.G. GOOGLE DRIVE, MS DRIVE..)

        private NerdModel _model;
        private string _appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);  // eg. C:\\Users\\EF-L-0034\\AppData\\Roaming   
        private const string AppDirectory = "\\NerdNote\\backup\\";
        private const string NotesFile = "NerdNotes.txt";
        private string _fullPath;

        public JsonConverter()
        {
            _fullPath =  _appDataFolder + AppDirectory + NotesFile;
            
            string jsonString;

            if (!File.Exists(_fullPath))
                InitializeBackupFile();

            jsonString = File.ReadAllText(_fullPath);                
            _model = new NerdModel();
            _model.Notes = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
        }

        private void InitializeBackupFile()
        {
            Directory.CreateDirectory(_appDataFolder + AppDirectory); 
            var initModel = new NerdModel();
            initModel.Notes = new Dictionary<string, string>();
            initModel.Notes.Add( "MyNote", "Start typing here!");
            File.WriteAllText(_fullPath, JsonConvert.SerializeObject(initModel.Notes));
        }

        public string GetFilePath() => _fullPath;

        public NerdModel GetAll() => _model;

        public IEnumerable<IGrouping<string,string>> GetMenu()
        {
            if (_model.Notes == null && !_model.Notes.Any())
                return null;

            var result =  YieldMenu().GroupBy(x => x.Split(';', System.StringSplitOptions.RemoveEmptyEntries).First());
            return result;
        }

        public void AddNewMenu(string TagsToAppend, string newMenuName = null)
        {
            newMenuName = string.IsNullOrWhiteSpace(newMenuName) ? "NewTitle" : newMenuName;

            string keyToSet;
            var value = "Start typing here";

            if (string.IsNullOrWhiteSpace(TagsToAppend))
                keyToSet = $"{newMenuName};";
            else
                keyToSet = $"{TagsToAppend};{newMenuName};";


            _model.Notes.Add(keyToSet, value);
            SaveAll();
        }

        public string GetContentByKey(string key)
        {
            return _model.Notes.Where(x => x.Key == key).FirstOrDefault().Value;
        }

        public bool UpdateContentByKey(string key, string value)
        {
            _model.Notes[key] = value;
            File.WriteAllText(_fullPath, JsonConvert.SerializeObject(_model.Notes));

            return true;
        }

        public bool SaveAll()
        {
            File.WriteAllText(_fullPath, JsonConvert.SerializeObject(_model.Notes));
            return true;
        }

        private IEnumerable<string> YieldMenu()
        {
            foreach (var note in _model.Notes.OrderBy(x => x.Key))
                yield return note.Key;
        }
    }
}
