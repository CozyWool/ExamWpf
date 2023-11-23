using System;

namespace ExamWpfApp.Models.Enums;

public enum MenuTypes
{
    None = -1,
    Exit,
    CreateDictionary,
    Add,
    Delete,
    Replace,
    Find,
    Save,
    Load,
    DeleteDictionary,
    ExportWord,
    Show
}

public static class MenuTypesHelper
{
    public static MenuTypes Parse(string value)
    {
        return Enum.Parse<MenuTypes>(value);
    }
}
