using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System;

namespace DevelopersHub.RealtimeNetworking
{
    // oyunun verileri için bir yapı sağlar.
    public static class Data
    {
        // Player: Oyuncu verileri için
        public class Player
        {
            public int gold = 0;
            public int food = 0;
            public int wood = 0;
            public int stone = 0;
            public int gems = 0;
            public List<Building> buildings = new List<Building>(); // buildings: Oyuncunun sahip olduğu binaların listesi
        }
        // Building: Bina verileri için
        public class Building // Bina verilerini tutar.
        {
            public string id = "";
            public int level = 0;
            public long databaseID = 0;
            public int x = 0;
            public int y = 0;
            public int columns = 0;
            public int rows = 0;
            public int storage = 0;
            public DateTime boost;
            public float gold_protection = 0;
            public float food_protection = 0;
            public float wood_protection = 0;
            public float stone_protection = 0;
            public int capacity = 0;
            public float speed = 0;
            public float radius = 0;
        }
        // ServerBuilding: Sunucu tarafında kullanılan bina verileri için
        public class ServerBuilding
        {
            public string id = "";
            public int level = 0;
            public long databaseID = 0;
            public int requiredGold = 0;
            public int requiredFood = 0;
            public int requiredWood = 0;
            public int columns = 0;
            public int rows = 0;
        }
        // Verileri XML formatında serialize eder
        public static string Serialize<T>(this T target)
        {
            XmlSerializer xml = new XmlSerializer(typeof(T));
            StringWriter writer = new StringWriter();
            xml.Serialize(writer, target);
            return writer.ToString();
        }
        // XML formatındaki verileri deserialize eder
        public static T Deserialize<T>(this string target)
        {
            XmlSerializer xml = new XmlSerializer(typeof(T));
            StringReader reader = new StringReader(target);
            return (T) xml.Deserialize(reader);
        }
    }    
}
