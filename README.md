# Weather Commands System

A GM command to control weather effects across facets in ModernUO.

## Server Command (ModernUO)

### Usage

```
[SetFacetWeather <type> [density]
```

### Parameters

| Parameter | Required | Description |
|-----------|----------|-------------|
| `type` | Yes | Weather type: `rain`, `storm`, `snow`, or `clear` |
| `density` | No | Intensity of weather effect (0-70, default: 40) |

### Weather Types

| Type | Effect | Description |
|------|--------|-------------|
| `rain` | Blue rain drops | Standard rainfall |
| `storm` | Blue rain + thunder | Fierce storm with thunder sounds |
| `snow` | White snowflakes | Snowfall with wind sounds |
| `clear` | No weather | Clears all weather effects |

### Examples

```
[SetFacetWeather rain          -- Normal rain (density 40)
[SetFacetWeather rain 70       -- Heavy rain (max density)
[SetFacetWeather rain 20       -- Light rain
[SetFacetWeather storm         -- Storm with thunder
[SetFacetWeather snow          -- Snowfall
[SetFacetWeather clear         -- Clear skies
```

### Access Level

- **GameMaster** or higher required

### Behavior

- Affects **all players** currently on the same facet/map as the GM
- Weather is a **client-side visual effect** only
- Weather will **fade after ~30 seconds** (client-controlled timer)
- New players joining after the command won't see the weather
- Does not persist across server restarts

## ClassicUO Client Fix

The default ClassicUO client renders rain as dark/black due to shader processing. A fix was applied to make rain visible:

### File Modified
`ClassicUO/src/ClassicUO.Client/Game/Weather.cs`

### Change
```csharp
// Original (dark/invisible rain)
SolidColorTextureCache.GetTexture(Color.Blue)

// Fixed (visible blue rain)
SolidColorTextureCache.GetTexture(Color.DodgerBlue)
```

### Rebuild Required
After modifying `Weather.cs`, rebuild ClassicUO for changes to take effect.

## Technical Details

### Network Packet
The command uses packet `0x65` (Weather) with format:
```
Byte 0: 0x65 (packet ID)
Byte 1: Type (0=rain, 1=storm, 2=snow, 0xFE=clear)
Byte 2: Density (0-70)
Byte 3: Temperature
```

### Weather Type Values
```csharp
WT_RAIN = 0           // Rain
WT_STORM_APPROACH = 1 // Storm with thunder
WT_SNOW = 2           // Snow
WT_STORM_BREWING = 3  // Storm brewing
WT_INVALID_0 = 0xFE   // Clear weather
WT_INVALID_1 = 0xFF   // Clear weather
```

## Files

| File | Location | Purpose |
|------|----------|---------|
| `WeatherCommands.cs` | `Projects/UOContent/Commands/` | Server command implementation |
| `Weather.cs` | `ClassicUO/src/ClassicUO.Client/Game/` | Client rendering (color fix) |

## Author

Custom command for ModernUO server with ClassicUO client support.
