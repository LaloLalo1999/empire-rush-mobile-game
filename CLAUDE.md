# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Empire Rush is a hybrid idle/board mobile game combining elements from Monopoly GO, Adventure Capitalist, and Clash of Clans. Players spin wheels to earn resources, purchase businesses for passive income, and progress through themed worlds.

## Development Commands

### Unity Development
```bash
# Build for Android
Unity -batchmode -quit -projectPath . -buildTarget Android -executeMethod BuildScript.BuildAndroid

# Build for iOS  
Unity -batchmode -quit -projectPath . -buildTarget iOS -executeMethod BuildScript.BuildiOS

# Run unit tests
Unity -batchmode -nographics -runTests -testPlatform PlayMode -testResults TestResults.xml

# Generate documentation
doxygen Doxyfile
```

### Package Management
```bash
# Install Unity packages via Package Manager
# Firebase SDK, Unity Ads, Unity IAP, DOTween are core dependencies
```

## Architecture Overview

### Core Systems Hierarchy
- **GameManager**: Singleton managing overall game state, scene transitions, and application lifecycle
- **EconomyManager**: Handles all business logic, income calculations, and resource management
- **SaveSystem**: JSON-based persistence with Firebase cloud save integration
- **AnalyticsManager**: Firebase Analytics integration for telemetry and A/B testing
- **MonetizationManager**: IAP and rewarded ads integration

### Key Design Patterns
- **Singleton Pattern**: Used for core managers (GameManager, EconomyManager, SaveSystem)
- **Observer Pattern**: Event system for UI updates and game state changes
- **Object Pooling**: For UI effects, particles, and frequently instantiated objects
- **State Machine**: For business upgrade states and player progression

### Data Flow
1. **Spin Wheel** → EconomyManager updates resources → UI reflects changes → SaveSystem persists data
2. **Offline Calculation** → SaveSystem loads last save time → EconomyManager calculates earnings → UI shows offline earnings popup
3. **Business Purchase** → EconomyManager validates cost → Business unlocked → Analytics event fired → Save triggered

### Economic Formulas (Critical for Balance)
```csharp
// Business cost scaling: baseCost * (1.15^ownedCount)
// Energy regeneration: 1 energy per 5 minutes, max 10
// Offline earnings: totalIncomePerSecond * min(offlineSeconds, 14400) // 4 hour cap
```

### Monetization Integration Points
- **Energy Depletion**: Primary rewarded ad trigger (38.1% conversion rate expected)
- **IAP Timing**: 77% of conversions happen within first 2 weeks
- **Ad Placements**: Between levels, when energy depleted, in IAP store

### Analytics Events (Firebase)
Essential tracking for KPI monitoring:
- `tutorial_complete`, `first_purchase`, `business_purchased`, `spin_wheel`, `offline_earnings`
- Retention tracking: D1, D7, D30 automated via Firebase

### Mobile Optimization Considerations
- Coroutine-based offline calculations to prevent frame drops
- Texture compression and atlas optimization for memory
- Battery-efficient background state handling
- Touch input optimization for various screen sizes

## Key Files Structure
```
Scripts/
├── Core/           # GameManager, SaveSystem, SceneManager
├── Economy/        # Business logic, offline calculations
├── UI/            # All UI controllers and managers  
├── Monetization/  # IAP and ads integration
├── Analytics/     # Firebase analytics wrapper
└── Utils/         # Helper classes and extensions
```

## Development Workflow
1. Core spin mechanic must feel satisfying before adding complexity
2. Economy balance requires playtesting - use A/B testing framework for key variables
3. Always run analytics validation after implementing new features
4. Mobile performance testing required on low-end devices (Android API 21+)
5. IAP and ads integration should be tested in sandbox environments first

## Critical Success Metrics
- Session length (target: 5+ minutes average)
- D1 retention (target: 40%+)  
- Energy to ad conversion rate (target: 38%+)
- Time to first purchase (target: <2 weeks for 77% of converters)