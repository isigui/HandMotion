using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HandMotion
{
    public static class Helper
    {
        public static void Save<T>(T obj, string destinationPath)
        {
            var content = JsonConvert.SerializeObject(obj);
            File.WriteAllText(destinationPath, content);
        }
        public static T Load<T>(string destinationPath)
        {
            var content = File.ReadAllText(destinationPath);
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}
