# Empire Rush - Mobile Idle/Board Game

A hybrid idle/board mobile game combining elements from Monopoly GO, Adventure Capitalist, and Clash of Clans. Players spin wheels to earn resources, purchase businesses for passive income, and progress through themed worlds.

## 🎮 Game Overview

Empire Rush is designed to be highly engaging and profitable for mobile platforms (iOS and Android). The game features:

- **Spin Wheel Mechanics**: Casino-style satisfaction with energy system
- **Business Empire Building**: 5 business types with exponential scaling
- **Passive Income Generation**: Earn money even when offline
- **Energy System**: Regenerates over time, drives monetization
- **Progressive Unlocks**: New businesses and worlds as you advance

## 🏗️ Technical Architecture

### Core Systems
- **GameManager**: Singleton managing overall game state and scene transitions
- **EconomyManager**: Handles all business logic, income calculations, and resource management
- **SaveSystem**: JSON-based persistence with Firebase cloud save integration
- **MonetizationManager**: IAP and rewarded ads integration
- **AnalyticsManager**: Firebase Analytics for telemetry and A/B testing
- **UIManager**: Comprehensive UI system with modern design patterns

### Key Features
- Modular, scalable architecture for live ops
- Offline income calculation (up to 4 hours)
- Energy regeneration system (1 energy per 5 minutes)
- Exponential business scaling with satisfying progression
- Complete monetization framework with IAP and ads

## 🚀 Getting Started

### Prerequisites
- Unity 2022.3 LTS or later
- Firebase SDK for Unity
- Unity Ads SDK
- Unity IAP SDK

### Setup Instructions

1. **Clone/Download the project**
2. **Open in Unity 2022.3 LTS+**
3. **Install Required Packages**:
   - Unity Ads (4.4.2+)
   - Unity IAP (4.9.3+)
   - Firebase Analytics
   - TextMeshPro (3.0.6+)

4. **Configure Firebase**:
   - Create Firebase project at https://console.firebase.google.com
   - Download `google-services.json` (Android) and `GoogleService-Info.plist` (iOS)
   - Place in `Assets/StreamingAssets/`

5. **Setup Unity Services**:
   - Enable Unity Ads in Project Settings > Services
   - Configure Unity IAP product catalog
   - Set up Analytics dashboard

## 🎯 Core Game Loop

1. **Player opens app** → Check offline earnings
2. **Spin wheel** → Consume energy, gain resources  
3. **Purchase/upgrade businesses** → Increase passive income
4. **Watch rewarded ads** → Gain energy/multipliers
5. **Energy depletes** → Option to buy energy or wait
6. **Repeat cycle** → Progressive business unlocks

## 💰 Monetization Strategy

### IAP Products
- **Energy Packs**: $0.99 (25), $4.99 (150), $19.99 (750)
- **Coin Packs**: $0.99 (10K), $4.99 (75K), $19.99 (500K)
- **Gem Packs**: $0.99 (50), $4.99 (300), $19.99 (1500)

### Rewarded Ads (Expected 38.1% conversion)
- +5 energy when depleted
- 2x income for 30 minutes
- Double offline earnings
- Skip upgrade timers

## 📊 Key Metrics & Analytics

### Success Metrics
- **D1 Retention**: Target 40%+
- **Session Length**: Target 5+ minutes average
- **Energy→Ad Conversion**: Target 38%+
- **Time to First Purchase**: <2 weeks for 77% of converters

### Tracked Events
```csharp
// Core progression
FirebaseAnalytics.LogEvent("tutorial_complete");
FirebaseAnalytics.LogEvent("business_purchased", "type", "lemonade_stand");
FirebaseAnalytics.LogEvent("spin_wheel", "reward", "coins");

// Monetization
FirebaseAnalytics.LogEvent("first_purchase", "item", "energy_pack_small");
FirebaseAnalytics.LogEvent("ad_watched", "placement", "energy_depleted");
```

## 🏢 Business Types & Economy

### Business Scaling Formula
```csharp
// Cost scaling: baseCost * (1.15^level)
// Income scaling: baseIncome * (1.2^(level-1))
```

### Business Types
1. **Lemonade Stand**: 15 coins → 1/sec income
2. **Pizza Shop**: 100 coins → 8/sec income  
3. **Car Wash**: 1,100 coins → 47/sec income
4. **Bank**: 12,000 coins → 260/sec income
5. **Tech Company**: 130,000 coins → 1,400/sec income

## 🎨 Asset Requirements

### Recommended Unity Asset Store Packages
- **Modern UI Pack** by Michsky (polished mobile UI)
- **Modular Game UI Kit** by ricimi (flexible components)
- **Casino Audio Pack** (slot machine & reward sounds)
- **Particle Effect Pack** (coin collection & upgrade effects)
- **DOTween** (smooth animations)

## 🔧 Development Commands

### Building
```bash
# Android build
Unity -batchmode -quit -projectPath . -buildTarget Android -executeMethod BuildScript.BuildAndroid

# iOS build  
Unity -batchmode -quit -projectPath . -buildTarget iOS -executeMethod BuildScript.BuildiOS
```

### Testing
```bash
# Run unit tests
Unity -batchmode -nographics -runTests -testPlatform PlayMode -testResults TestResults.xml
```

## 📱 Platform Support

- **iOS**: 11.0+ (optimized for iPhone/iPad)
- **Android**: API 21+ (Android 5.0+)
- **Performance**: Targets 60 FPS on mid-range devices
- **Storage**: <100MB initial download

## 🔮 Future Roadmap (Post-MVP)

- **Multiple Worlds**: Different themed business environments
- **Prestige System**: Reset progress for permanent multipliers
- **Social Features**: Visit friends, steal resources
- **Battle Pass**: Seasonal progression rewards
- **Live Events**: Limited-time tournaments and challenges

## 📄 License

This project is proprietary software developed for Empire Rush Studios.

## 🤝 Contributing

This is a commercial project. For collaboration inquiries, please contact the development team.

---

**Built with Unity 2022.3 LTS | Optimized for Mobile | Ready for Live Ops**