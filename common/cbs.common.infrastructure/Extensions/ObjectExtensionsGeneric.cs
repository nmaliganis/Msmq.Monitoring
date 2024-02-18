using System;
using System.Dynamic;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cbs.common.infrastructure.Extensions;

public static class ObjectExtensionsGeneric 
{
    /// <summary>
    /// How to use
    /// employee.IfType<Manager>(x => x.ActBusy());
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <param name="action"></param>
    public static void IfType<T>(this object item, Action<T> action) where T : class {
        if (item is T item1) {
            action(item1);
        }
    }
    /// <summary>
    /// Is faster that == null
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static bool IsNull(this object item) {
        return item is null;
    }

    public static string AsJson(this object item) {
        return JsonConvert.SerializeObject(item);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <returns></returns>
    public static T DeserializeObject<T>(this string item) where T : class {
        return JsonConvert.DeserializeObject<T>(item);
    }

    public static JToken AsJToken(this object item) {
        return JToken.FromObject(item);
    }

    public static string DynamicAsJson(this ExpandoObject item) {
        return JsonConvert.SerializeObject(item);
    }

    public static bool TryParseJson<T>(this string @this, out T result) {
        bool success = true;
        var settings = new JsonSerializerSettings {
            Error = (sender, args) => {
                success = false;
                args.ErrorContext.Handled = true;
            },
            MissingMemberHandling = MissingMemberHandling.Error
        };
        result = JsonConvert.DeserializeObject<T>(@this, settings);
        return success;
    }

    public static Expression<TDelegate> AndAlso<TDelegate>(this Expression<TDelegate> left, Expression<TDelegate> right) {
        return Expression.Lambda<TDelegate>(Expression.AndAlso(left, right), left.Parameters);
    }

    public static string ObjectToJson<T>(this T that) {
        return JsonConvert.SerializeObject(that);
    }

    public static T JsonToObject<T>(this string that) {
        return JsonConvert.DeserializeObject<T>(that);
    }
}