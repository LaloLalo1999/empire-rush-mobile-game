# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 🚧 PROJECT STATUS: DEVELOPMENT IN PROGRESS

❌ **BUILD ISSUES**: Unity version mismatch causing compilation failures  
❌ **MISSING ASSETS**: UI sprites, audio files, prefabs not implemented  
❌ **INCOMPLETE SYSTEMS**: Scripts reference non-existent assets and components  
❌ **BUILD FAILURES**: Multiple dependency conflicts prevent successful builds  

**Current Reality**: Core scripts exist but project requires substantial completion work before deployment

## Project Overview

Empire Rush is a hybrid idle/board mobile game combining elements from Monopoly GO, Adventure Capitalist, and Clash of Clans. Players spin wheels to earn resources, purchase businesses for passive income, and progress through themed worlds.

**Actual Status**: Script framework exists, but missing critical assets and has build errors preventing deployment.

## 📦 Missing Assets Inventory (CRITICAL FOR COMPLETION)

### **Visual Assets** (MUST IMPLEMENT BEFORE BUILDS)
- [ ] **Spin wheel sprite/image** - Referenced in SpinWheelController.cs:15 but doesn't exist
- [ ] **UI button sprites and backgrounds** - Currently using placeholder colors
- [ ] **Business icons** - 5 different business types need unique visuals
- [ ] **Particle effect prefabs** - RewardParticles referenced but missing
- [ ] **Background sprites** - Game currently has solid color backgrounds
- [ ] **Energy bar UI graphics** - Energy system uses basic text display
- [ ] **Coin/currency icons** - Using default Unity text for currency display

### **Audio Assets** (HIGH PRIORITY FOR GAME FEEL)
- [ ] **Spin wheel audio clips** - spinStartClip, spinLoopClip, spinEndClip, rewardClip all missing
- [ ] **Background music tracks** - No audio manager implementation
- [ ] **UI sound effects** - Button clicks, notifications, celebration sounds
- [ ] **Business environment sounds** - Different ambient sounds per business type
- [ ] **Achievement/milestone sounds** - Reward feedback audio missing

### **Prefab Assets** (CRITICAL FOR FUNCTIONALITY)
- [ ] **Spin wheel prefab** - Complete UI layout with proper anchoring
- [ ] **Business card/panel prefabs** - 5 business types need individual UI panels
- [ ] **Popup/modal prefabs** - Reward displays, purchase confirmations
- [ ] **Energy regeneration feedback** - Visual indicators for energy system
- [ ] **Offline earnings popup** - Welcome back screen with earnings summary

### **Configuration Assets** (REQUIRED FOR GAME BALANCE)
- [ ] **Business configuration ScriptableObjects** - Cost, income, upgrade data
- [ ] **Wheel reward configuration** - Probability weights, reward amounts
- [ ] **Game balance configuration** - Economic formulas, progression curves
- [ ] **Localization files** - Multi-language support framework

## 🔧 Build Fix Requirements (IMMEDIATE PRIORITY)

### **STEP 1: Fix Unity Project Compilation**
```bash
# Current Unity version mismatch
cat ProjectSettings/ProjectVersion.txt
# Shows: m_EditorVersion: 6000.1.6f1
# But project built with: Unity 2023.2.20f1

# Fix version compatibility
# Update ProjectVersion.txt to match installed Unity version

# Clean problematic cache
rm -rf Library/ Temp/

# Fix package dependencies in Packages/manifest.json
# Current issues:
# - com.unity.multiplayer.center@1.0.0 (package not found)
# - com.unity.test-framework@1.5.1 (version conflict) 
# - com.unity.ai.navigation@2.0.7 (invalid unity property)
```

### **STEP 2: Create Missing Build Scripts**
```bash
# SimpleBuildScript.cs is referenced but doesn't exist
# Must create: Assets/Editor/SimpleBuildScript.cs

# Build command references that currently fail:
# - BuildScript.BuildiOS (should be iOSBuildScript.BuildiOS)
# - SimpleBuildScript.BuildiOSSimple (script missing entirely)
```

### **STEP 3: Asset Directory Structure Setup**
```bash
# Create required asset directories before implementing
mkdir -p Assets/Sprites/UI
mkdir -p Assets/Sprites/Businesses  
mkdir -p Assets/Audio/SFX
mkdir -p Assets/Audio/Music
mkdir -p Assets/Prefabs/UI
mkdir -p Assets/Resources/Data
mkdir -p Assets/Resources/Audio
mkdir -p Assets/Resources/Sprites
mkdir -p Assets/Resources/Prefabs

# These directories must exist and contain assets before scripts will work
```

## 🛠️ Complete Development Workflow (REALISTIC TIMELINE)

### **Phase 1: Fix Compilation Issues** (Week 1 - IMMEDIATE)
1. **Unity Version Alignment**
   - Update ProjectVersion.txt to match installed Unity (currently 6000.1.6f1 vs 2023.2.20f1)
   - Resolve package dependency conflicts in manifest.json
   - Clean and regenerate Unity library files

2. **Missing Script Dependencies**
   - Create SimpleBuildScript.cs (referenced in build commands but missing)
   - Fix namespace issues causing compilation errors
   - Add missing using statements for UI systems

3. **Build Pipeline Fixes**
   - Correct build method names in command line calls
   - Add error handling in build scripts
   - Create platform detection for iOS/Android/macOS builds

### **Phase 2: Asset Creation** (Week 2-3 - BEFORE ANY BUILDS WORK)
1. **UI Assets Implementation**
   ```bash
   # Required assets that scripts expect:
   Assets/Resources/Sprites/spin_wheel.png          # SpinWheelController reference
   Assets/Resources/Sprites/ui_button_normal.png    # Button backgrounds
   Assets/Resources/Sprites/ui_button_pressed.png   # Button states
   Assets/Resources/Sprites/business_*.png          # 5 business type icons
   Assets/Resources/Sprites/coin_icon.png           # Currency display
   Assets/Resources/Sprites/energy_bar.png          # Energy UI graphics
   ```

2. **Audio System Implementation**
   ```bash
   # Audio files referenced in scripts but missing:
   Assets/Resources/Audio/spin_start.mp3    # SpinWheelController.spinStartClip
   Assets/Resources/Audio/spin_loop.mp3     # SpinWheelController.spinLoopClip  
   Assets/Resources/Audio/spin_end.mp3      # SpinWheelController.spinEndClip
   Assets/Resources/Audio/reward.mp3        # SpinWheelController.rewardClip
   Assets/Resources/Audio/button_click.mp3  # UI button feedback
   Assets/Resources/Audio/background.mp3    # Main game music
   ```

3. **Prefab Creation**
   ```bash
   # Prefabs that scripts try to instantiate:
   Assets/Resources/Prefabs/RewardParticles.prefab   # SpinWheelController.rewardParticles
   Assets/Resources/Prefabs/BusinessPanel.prefab     # Business UI management
   Assets/Resources/Prefabs/OfflineEarnings.prefab   # Offline calculation popup
   Assets/Resources/Prefabs/EnergyRegenUI.prefab     # Energy system feedback
   ```

### **Phase 3: System Integration** (Week 4 - TESTING & DEBUG)
1. **Script-Asset Connection**
   - Link created assets to script references
   - Test all systems work in Unity Editor
   - Debug integration issues between systems

2. **Mobile Optimization**
   - Configure UI for different screen sizes
   - Test touch input responsiveness
   - Optimize performance for mobile devices

3. **Platform Configuration**
   - Set up iOS build settings correctly
   - Configure Android build pipeline
   - Test builds on actual devices

### **Phase 4: Platform Deployment** (Week 5 - FINAL)
1. **iOS Deployment** (Only after all above completed)
   ```bash
   # Build will only work after all assets created and errors fixed
   Unity -quit -batchmode -projectPath . -buildTarget iOS -executeMethod iOSBuildScript.BuildiOS -logFile build_ios.log
   
   # Check for errors (currently guaranteed to fail)
   grep -i "error\|failed\|exception" build_ios.log
   ```

2. **Android Build Setup**
   - Configure Android platform settings
   - Set up keystore and signing
   - Test APK generation and installation

3. **Store Preparation**
   - Configure app icons and splash screens
   - Set up store metadata and screenshots
   - Prepare privacy policy and app descriptions

## 📋 Development Commands (CORRECTED)

### **Current Build Status** (Will Fail Until Fixed)
```bash
# These commands WILL FAIL until issues above are resolved:

# Primary build attempt (currently fails)
Unity -quit -batchmode -projectPath . -buildTarget iOS -executeMethod iOSBuildScript.BuildiOS -logFile build_ios.log

# Check why build failed
cat build_ios.log | grep -A 5 -B 5 "error"
# Expected errors: Package resolution, missing scripts, asset references

# Verify Unity version mismatch
cat ProjectSettings/ProjectVersion.txt
# Shows: 6000.1.6f1 but Unity installation is likely different

# Check asset existence (currently empty)
find Assets/ -name "*.png" -o -name "*.jpg" | wc -l  # Will show 0
find Assets/ -name "*.mp3" -o -name "*.wav" | wc -l  # Will show 0
find Assets/ -name "*.prefab" | wc -l                # Will show 0
```

### **Diagnostic Commands**
```bash
# Check compilation errors
cat build_ios.log | grep -E "(error|Error|ERROR)"

# Verify script compilation
Unity -batchmode -quit -projectPath . -executeMethod EditorApplication.Exit -logFile compile_test.log

# Check package status
cat Packages/manifest.json
# Will show invalid packages causing resolution failures

# Unity installation verification
/Applications/Unity/Unity.app/Contents/MacOS/Unity -version
ls /Applications/Unity/Hub/Editor/

# Clean project for fresh build attempt
rm -rf Library/ Temp/
# Note: This won't fix asset/script issues, only clears cache
```

## Architecture Overview (THEORETICAL - NEEDS IMPLEMENTATION)

### Core Systems (Scripts Exist, Assets Missing)
- **GameManager**: Script exists, needs scene integration and UI connections
- **EconomyManager**: Business logic implemented, needs configuration ScriptableObjects
- **SaveSystem**: Framework exists, needs testing and data validation
- **AnalyticsManager**: Firebase integration prepared, needs configuration files
- **MonetizationManager**: IAP/Ads framework ready, needs Unity Services setup

### Current Script Status (17 C# Files)
```
Assets/Scripts/                    # Scripts exist but reference missing assets
├── Core/                         
│   ├── GameManager.cs           # ✅ Implemented, needs UI connection
│   ├── SaveSystem.cs            # ✅ Implemented, needs testing
│   └── GameInitializer.cs       # ✅ Implemented, needs scene setup
├── Economy/
│   ├── EconomyManager.cs        # ✅ Implemented, needs balance data
│   └── Business.cs              # ✅ Implemented, needs visual assets
├── UI/
│   ├── UIManager.cs             # ⚠️ References missing prefabs
│   ├── SpinWheelController.cs   # ❌ References missing wheel sprite & audio
│   ├── EnergyUI.cs              # ⚠️ References missing UI graphics
│   └── BusinessUI.cs            # ❌ References missing business icons
├── Monetization/
│   ├── MonetizationManager.cs   # ✅ Framework ready, needs Unity Services
│   ├── IAPManager.cs            # ✅ Ready, needs product configuration
│   └── AdManager.cs             # ✅ Ready, needs ad network setup
├── Analytics/
│   └── AnalyticsManager.cs      # ✅ Ready, needs Firebase config
├── Utils/
│   ├── AudioManager.cs          # ❌ References missing audio clips
│   ├── ExtensionMethods.cs      # ✅ Utility functions implemented
│   └── ObjectPool.cs            # ✅ Performance optimization ready
└── EmpireRushGame.cs            # ⚠️ Basic implementation, needs full UI
```

## Key Files Structure (ACTUAL STATE)

```
Assets/Scripts/                  # 17 C# scripts (implemented but incomplete)
├── Core/                       # Core systems ready
├── Economy/                    # Business logic complete
├── UI/                        # ❌ Missing sprites, prefabs, audio
├── Monetization/              # Framework ready, needs configuration
├── Analytics/                 # Ready for Firebase integration
├── Utils/                     # Support systems implemented
└── EmpireRushGame.cs          # Main controller (basic implementation)

Assets/Editor/                  # Build scripts
├── iOSBuildScript.cs          # ✅ Exists and functional
└── SimpleBuildScript.cs       # ❌ MISSING - referenced but doesn't exist

Assets/Resources/              # ❌ MISSING ENTIRELY
├── Audio/                     # ❌ No audio files exist
├── Sprites/                   # ❌ No image assets exist  
└── Prefabs/                   # ❌ No prefabs created

Builds/iOS/                    # Generated but broken
└── Unity-iPhone.xcodeproj     # ❌ Contains compilation errors
```

## ⚠️ Current Limitations (MUST FIX BEFORE DEPLOYMENT)

### **Cannot Build Because:**
1. Unity version mismatch (6000.1.6f1 vs 2023.2.20f1)
2. Missing SimpleBuildScript.cs referenced in build commands
3. Package resolution failures in manifest.json
4. Missing asset dependencies throughout scripts

### **Cannot Run Because:**
1. UI scripts reference non-existent sprites and prefabs
2. Audio system has no audio files to play
3. Spin wheel controller missing wheel graphics
4. Business system missing visual assets

### **Cannot Deploy Because:**
1. Build process fails with compilation errors
2. Runtime errors from missing asset references
3. Incomplete UI makes game unplayable
4. No visual assets for App Store submission

## 🎯 Realistic Timeline Estimate

### **Week 1: Fix Build Issues** (40 hours)
- Resolve Unity version compatibility
- Fix package dependencies
- Create missing SimpleBuildScript.cs
- Get basic compilation working

### **Week 2-3: Asset Creation** (60-80 hours)
- Design and create all UI sprites
- Source or create audio files
- Build all required prefabs
- Create configuration ScriptableObjects

### **Week 4: Integration & Testing** (30 hours)
- Connect assets to scripts
- Test all systems in Unity Editor
- Fix integration bugs
- Mobile optimization

### **Week 5: Platform Builds** (20 hours)
- Configure iOS build properly
- Set up Android build pipeline
- Test on actual devices
- Prepare store submission

**Total Estimated Time: 150-170 hours of development work**

## Critical Success Metrics (ASPIRATIONAL)
- Session length (target: 5+ minutes average)
- D1 retention (target: 40%+)  
- Energy to ad conversion rate (target: 38%+)
- Time to first purchase (target: <2 weeks for 77% of converters)

**Note**: These metrics cannot be measured until the game is actually functional and deployed.

## Troubleshooting Current Issues

### **Build Failure Diagnosis**
```bash
# Check compilation errors (guaranteed to exist)
cat build_ios.log | grep -A 5 -B 5 "error"

# Package resolution issues
grep -E "(cannot be found|invalid|failed)" build_ios.log

# Missing asset references
grep -E "(missing|null reference|asset not found)" build_ios.log

# Unity version problems
grep -E "(version|compatibility)" build_ios.log
```

### **Asset Missing Diagnosis**
```bash
# Count existing assets (currently zero)
echo "Images: $(find Assets/ -name '*.png' -o -name '*.jpg' | wc -l)"
echo "Audio: $(find Assets/ -name '*.mp3' -o -name '*.wav' | wc -l)"  
echo "Prefabs: $(find Assets/ -name '*.prefab' | wc -l)"

# Check for missing directories
ls -la Assets/Resources/ 2>/dev/null || echo "Resources directory missing"
ls -la Assets/Sprites/ 2>/dev/null || echo "Sprites directory missing" 
ls -la Assets/Audio/ 2>/dev/null || echo "Audio directory missing"
```

## Next Immediate Actions Required

1. **Fix Unity version in ProjectVersion.txt** to match installed Unity
2. **Create SimpleBuildScript.cs** in Assets/Editor/
3. **Update Packages/manifest.json** to remove problematic packages
4. **Create asset directory structure** as outlined above
5. **Begin asset creation process** before attempting any builds

**Only after completing ALL above steps will builds have a chance of succeeding.**