using System;
using System.Collections.Generic;
using System.Text;

namespace Block_Blast.Services;

/// <summary>
/// Хранит имя текущего залогиненного игрока между сессиями.
/// Использует Preferences (работает на Android/iOS/Windows).
/// </summary>
public class AccountService
{
    private const string CurrentPlayerKey = "current_player_name";

    // ── Текущий игрок ─────────────────────────────────────────

    /// <summary>Возвращает сохранённое имя или null если аккаунта нет.</summary>
    public string? GetCurrentName()
    {
        var name = Preferences.Get(CurrentPlayerKey, "");
        return string.IsNullOrWhiteSpace(name) ? null : name;
    }

    /// <summary>Сохраняет имя игрока как «текущий аккаунт».</summary>
    public void SetCurrentName(string name)
    {
        Preferences.Set(CurrentPlayerKey, name.Trim());
    }

    /// <summary>Выход из аккаунта — очищает сохранённое имя.</summary>
    public void Logout()
    {
        Preferences.Remove(CurrentPlayerKey);
    }

    /// <summary>True если есть сохранённый аккаунт.</summary>
    public bool IsLoggedIn => GetCurrentName() != null;
}
