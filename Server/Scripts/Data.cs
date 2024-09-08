using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace DevelopersHub.RealtimeNetworking.Server
{
    public static class Data // oyunun veri yönetimini sağlar.
    {
        public class Player // Oyuncu verilerini tutar.
        {
            public int gold = 0;
            public int food = 0;
            public int wood = 0;
            public int stone = 0;
            public int gems = 0;
            public List<Building> buildings = new List<Building>(); // Oyuncunun binalarını tutar.
        }
        public class Building // Bina verilerini tutar.
        {
            public string id = "";
            public int level = 0;
            public long databaseID = 0;
            public int x = 0;
            public int y = 0;
            public int columns = 0;
            public int rows = 0;
        }
        public class ServerBuilding // Sunucu tarafındaki bina verilerini tutar
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
        public async static Task<string> Serialize<T>(this T target)
        {
            Task<string> task = Task.Run(() =>
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                StringWriter writer = new StringWriter();
                xml.Serialize(writer, target);
                return writer.ToString();
            });
            return await task;
        }

        public async static Task<T> Deserialize<T>(this string target)
        {
            Task<T> task = Task.Run(() =>
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                StringReader reader = new StringReader(target);
                return (T)xml.Deserialize(reader);
            });
            return await task;
        }
    }
}
