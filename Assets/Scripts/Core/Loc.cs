using System;
using System.Collections.Generic;

/// <summary>
/// Hệ thống đa ngôn ngữ MVP (Hard-code).
/// Gọi Loc.Get("key") để lấy chuỗi theo ngôn ngữ hiện tại.
/// Gọi Loc.Get("key", arg0, arg1...) cho chuỗi có tham số {0}, {1}.
/// </summary>
public static class Loc
{
    public enum Lang { VI, EN }

    public static Lang Current { get; private set; } = Lang.VI;
    public static event Action OnLanguageChanged;

    public static void SetLanguage(Lang lang)
    {
        if (Current == lang) return;
        Current = lang;
        OnLanguageChanged?.Invoke();
    }

    public static void Toggle()
    {
        SetLanguage(Current == Lang.VI ? Lang.EN : Lang.VI);
    }

    /// <summary>Tra cứu chuỗi theo key. Trả về key nếu không tìm thấy.</summary>
    public static string Get(string key)
    {
        var dict = Current == Lang.VI ? vi : en;
        return dict.TryGetValue(key, out string value) ? value : key;
    }

    /// <summary>Tra cứu chuỗi có tham số: Loc.Get("hud_day", dayNumber)</summary>
    public static string Get(string key, params object[] args)
    {
        var dict = Current == Lang.VI ? vi : en;
        if (dict.TryGetValue(key, out string template))
        {
            return string.Format(template, args);
        }
        return key;
    }

    // ===================== BẢNG CHUỖI TIẾNG VIỆT =====================
    private static readonly Dictionary<string, string> vi = new Dictionary<string, string>
    {
        // ── HUD ──
        { "hud_stamina",        "The luc: {0} / {1}" },
        { "hud_oxygen",         "Duong khi: {0} / {1}" },
        { "hud_gold",           "Vang: {0}" },
        { "hud_day",            "Ngay {0}" },
        { "hud_tax",            "Thue sau {0} ngay" },

        // ── Crafting UI ──
        { "craft_title",        "Gia kim MVP" },
        { "craft_hint",         "Chon nguyen lieu trong tui de tha vao vac." },
        { "craft_inv_title",    "Nguyen lieu trong tui" },
        { "craft_inv_empty",    "Tui do dang trong." },
        { "craft_temp",         "Nhiet do: {0}C" },
        { "craft_fire",         "Bep lua: {0}" },
        { "craft_fire_on",      "Dang bat" },
        { "craft_fire_off",     "Dang tat" },
        { "craft_cauldron_title","Nguyen lieu trong vac" },
        { "craft_cauldron_empty","Vac dang trong." },
        { "craft_cauldron_no_mgr","Chua co vac hoac AlchemyManager chua khoi tao." },
        { "craft_footer",       "Dat vac o layer Interactable de bam E mo man hinh nay." },

        // ── Buttons ──
        { "btn_toggle_fire",    "Bat / tat lua" },
        { "btn_fire_on",        "Tat lua" },
        { "btn_fire_off",       "Bat lua" },
        { "btn_return_explore", "Quay lai tham hiem" },
        { "btn_end_day",        "Ket thuc ngay" },

        // ── End of Day ──
        { "end_header",         "Tong ket ngay {0}" },
        { "end_gold",           "Vang hien co: {0}" },
        { "end_tax",            "Con {0} ngay toi han dong thue." },
        { "end_no_incident",    "Khong co su co nao hom nay." },
        { "btn_next_day",       "Bat dau ngay moi" },

        // ── Game Over ──
        { "gameover_title",     "GAME OVER" },
        { "gameover_reason",    "Khong du vang de dong thue." },

        // ── Language Switch ──
        { "lang_switch",        "English" },
    };

    // ===================== BẢNG CHUỖI TIẾNG ANH =====================
    private static readonly Dictionary<string, string> en = new Dictionary<string, string>
    {
        // ── HUD ──
        { "hud_stamina",        "Stamina: {0} / {1}" },
        { "hud_oxygen",         "Oxygen: {0} / {1}" },
        { "hud_gold",           "Gold: {0}" },
        { "hud_day",            "Day {0}" },
        { "hud_tax",            "Tax in {0} days" },

        // ── Crafting UI ──
        { "craft_title",        "Alchemy MVP" },
        { "craft_hint",         "Select ingredients from bag to drop into cauldron." },
        { "craft_inv_title",    "Bag Ingredients" },
        { "craft_inv_empty",    "Bag is empty." },
        { "craft_temp",         "Temperature: {0}C" },
        { "craft_fire",         "Fire: {0}" },
        { "craft_fire_on",      "On" },
        { "craft_fire_off",     "Off" },
        { "craft_cauldron_title","Cauldron Ingredients" },
        { "craft_cauldron_empty","Cauldron is empty." },
        { "craft_cauldron_no_mgr","No cauldron or AlchemyManager not initialized." },
        { "craft_footer",       "Place cauldron on Interactable layer, press E to open." },

        // ── Buttons ──
        { "btn_toggle_fire",    "Toggle Fire" },
        { "btn_fire_on",        "Turn Off" },
        { "btn_fire_off",       "Turn On" },
        { "btn_return_explore", "Return to Exploration" },
        { "btn_end_day",        "End Day" },

        // ── End of Day ──
        { "end_header",         "Day {0} Summary" },
        { "end_gold",           "Current Gold: {0}" },
        { "end_tax",            "{0} days until tax due." },
        { "end_no_incident",    "No incidents today." },
        { "btn_next_day",       "Start New Day" },

        // ── Game Over ──
        { "gameover_title",     "GAME OVER" },
        { "gameover_reason",    "Not enough gold to pay tax." },

        // ── Language Switch ──
        { "lang_switch",        "Tieng Viet" },
    };
}
