using NerdAppV3.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NerdAppV3
{
    public class JsonConverter
    {
        private NerdModel _model;
        public JsonConverter()
        {
            var jsonString = File.ReadAllText(@"C:\Users\puszynski\source\repos\NerdAppV3\NerdAppV3\JSON.txt");
            _model = new NerdModel();
            _model.Notes = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
        }

        public NerdModel GetAll() => _model;

        public IEnumerable<string> GetMenu()
        {
            if (_model.Notes == null && !_model.Notes.Any())
                return null;
            else
                return YieldMenu();
        }

        public string GetContentByKey(string key)
        {
            return _model.Notes.Where(x => x.Key == key).FirstOrDefault().Value;
        }

        private IEnumerable<string> YieldMenu()
        {
            foreach (var note in _model.Notes)
                yield return note.Key;
        }
    }
}
