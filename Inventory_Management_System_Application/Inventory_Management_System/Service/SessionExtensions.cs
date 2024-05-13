using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.AspNetCore.Http;

public static class SessionExtensions
{
    public static void SetObject<T>(this ISession session, string key, T value)
    {
        var formatter = new BinaryFormatter();
        using (var stream = new MemoryStream())
        {
            formatter.Serialize(stream, value);
            session.Set(key, stream.ToArray());
        }
    }

    public static T GetObject<T>(this ISession session, string key)
    {
        var data = session.Get(key);
        if (data == null)
            return default;

        var formatter = new BinaryFormatter();
        using (var stream = new MemoryStream(data))
        {
            return (T)formatter.Deserialize(stream);
        }
    }
}