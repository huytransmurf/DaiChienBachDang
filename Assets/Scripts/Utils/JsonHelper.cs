using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class JsonHelper
    {
        public static string[] GetJsonArray(string json)
        {
            if (!json.StartsWith("["))
                return new[] { json };
            json = "{\"items\":" + json + "}";
            Wrapper wrapper = JsonUtility.FromJson<Wrapper>(json);
            return wrapper.items;
        }

        [Serializable]
        private class Wrapper
        {
            public string[] items;
        }
    }
}
