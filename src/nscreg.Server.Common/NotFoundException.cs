using System;

namespace nscreg.Server.Common
{
    /// <summary>
    /// Класс обработчик "Не найдено" исключения
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message)
        : base(message)
        {
        }

        public NotFoundException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}