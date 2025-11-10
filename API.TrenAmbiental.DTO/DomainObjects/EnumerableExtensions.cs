using System;
using System.Collections.Generic;

namespace API.TrenAmbiental.DTO.DomainObjects
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> itemAction)
        {
            foreach (var item in items) itemAction(item);
        }
    }
}