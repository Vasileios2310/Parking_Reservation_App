using System.ComponentModel.DataAnnotations;
using System.Reflection;


namespace ParkingReservationApp.Models;
/// <summary>
/// 
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="enumValue"></param>
    /// <returns></returns>
    public static string GetDisplayName(this Enum enumValue)
    {
        return enumValue.GetType()
            .GetMember(enumValue.ToString())
            .First()
            .GetCustomAttribute<DisplayAttribute>()?
            .Name ?? enumValue.ToString();
    }
}