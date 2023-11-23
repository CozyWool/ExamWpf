using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace ExamWpfApp.Models.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum LanguageTypes
{
    None,
    Russian,
    English,
    German,
    //....
}
