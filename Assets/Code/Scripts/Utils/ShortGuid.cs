using System;

namespace Code.Scripts.Utils
{
    public static class ShortGuid
    {
        public static string Generate()
        {
            var guid = Guid.NewGuid();
        
            var encoded = Convert.ToBase64String(guid.ToByteArray())
                .Replace("/", "_")  // Заменяем не URL-safe символы
                .Replace("+", "-")
                .Substring(0, 22);  // Обрезаем до 22 символов (это дает 16 байт данных как в оригинальном GUID)
        
            return encoded;
        }

        public static Guid Decode(string shortGuid)
        {
            var base64 = shortGuid.Replace("_", "/").Replace("-", "+") + "==";
            var bytes = Convert.FromBase64String(base64);
            return new Guid(bytes);
        }
    }
}