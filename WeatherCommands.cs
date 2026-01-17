using System;
using Server.Network;

namespace Server.Commands;

public static class WeatherCommands
{
    public static void Configure()
    {
        CommandSystem.Register("SetFacetWeather", AccessLevel.GameMaster, SetFacetWeather_OnCommand);
    }

    [Usage("SetFacetWeather <rain|storm|snow|clear> [density]")]
    [Description("Changes the weather for all players on the current facet. Density is 0-70 (default 40).")]
    private static void SetFacetWeather_OnCommand(CommandEventArgs e)
    {
        var from = e.Mobile;
        var facet = from.Map;

        if (facet == null || facet == Map.Internal)
        {
            from.SendMessage("You must be on a valid facet to change the weather.");
            return;
        }

        if (e.Length == 0)
        {
            from.SendMessage("Usage: SetFacetWeather <rain|storm|snow|clear> [density]");
            from.SendMessage("  rain  - Sets rainy weather (blue drops)");
            from.SendMessage("  storm - Sets storm weather (blue drops with thunder)");
            from.SendMessage("  snow  - Sets snowy weather (white flakes)");
            from.SendMessage("  clear - Clears weather effects");
            from.SendMessage("  density - Optional, 0-70 (default 40)");
            return;
        }

        var weatherType = e.GetString(0).ToLower();
        var density = e.Length > 1 ? e.GetInt32(1) : 40;
        density = Math.Clamp(density, 0, 70);

        byte type;
        byte temp;
        string weatherName;

        // Weather types from ClassicUO:
        // 0 = WT_RAIN (blue drops)
        // 1 = WT_STORM_APPROACH (blue drops with thunder)
        // 2 = WT_SNOW (white flakes)
        // 3 = WT_STORM_BREWING
        // 0xFE/0xFF = Clear/Invalid
        switch (weatherType)
        {
            case "rain":
                type = 0;
                temp = 15;
                weatherName = "rain";
                break;
            case "storm":
                type = 1;
                temp = 15;
                weatherName = "storm";
                break;
            case "snow":
                type = 2;
                temp = 230; // Negative temperature represented as byte
                weatherName = "snow";
                break;
            case "clear":
                type = 0xFE;
                density = 0;
                temp = 0;
                weatherName = "clear skies";
                break;
            default:
                from.SendMessage("Invalid weather type. Use: rain, storm, snow, or clear");
                return;
        }

        var count = 0;
        foreach (var ns in NetState.Instances)
        {
            if (ns.Mobile?.Map == facet)
            {
                // First clear existing weather to force ClassicUO to regenerate
                if (type != 0xFE)
                {
                    ns.SendWeather(0xFE, 0, 0);
                }
                ns.SendWeather(type, (byte)density, temp);
                count++;
            }
        }

        from.SendMessage($"Weather changed to {weatherName}{(density > 0 ? $" with density {density}" : "")} for {count} player(s) on {facet.Name}.");
    }
}
