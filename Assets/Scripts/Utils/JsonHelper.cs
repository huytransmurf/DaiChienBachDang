using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class JsonHelper
    {
        public static string[] GetJsonArray(string rawJsonArray)
        {
            try
            {
                return JsonConvert.DeserializeObject<string[]>(rawJsonArray);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning("⚠ Lỗi parse JSON array: " + ex.Message);
                return Array.Empty<string>();
            }
        }
    }
}
