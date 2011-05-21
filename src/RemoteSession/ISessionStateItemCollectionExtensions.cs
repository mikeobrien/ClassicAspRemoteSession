using System.Collections.Generic;
using System.Linq;
using System.Web.SessionState;

namespace RemoteSession
{
    public static class ISessionStateItemCollectionExtensions
    {
        public static IDictionary<string, object> ToDictionary(this ISessionStateItemCollection items)
        {
            return items.Cast<string>().ToDictionary(x => x, x => items[x]);
        }

        public static ISessionStateItemCollection AddItems(this ISessionStateItemCollection items, IDictionary<string, object> newItems)
        {
            foreach (var newItem in newItems)
            {
                items[newItem.Key] = newItem.Value;
            }
            return items;
        }
    }
}