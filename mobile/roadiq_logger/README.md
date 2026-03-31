# RoadIQ Mobile Logger

A Flutter app that collects real-time sensor data from your phone to calculate road wear indices.

## Features

- 📱 **Real-time accelerometer data** collection
- 🗺️ **GPS location tracking** with speed calculation
- 📊 **Live wear index display** with color-coded road quality
- 🌐 **Automatic API sync** every 10 seconds
- 🎨 **Beautiful UI** with start/stop controls

## Setup

1. **Install Flutter** (3.19.6+)
2. **Clone repository**
3. **Install dependencies:**
   ```bash
   cd mobile/roadiq_logger
   flutter pub get
   ```

## Running

### Android Emulator
```bash
flutter run
```
API URL automatically configured for Android emulator.

### iOS (requires Mac)
```bash
flutter run -d iphone
```

### Web (for testing)
```bash
flutter run -d chrome
```

## Permissions

The app requires:
- **Location access** for GPS tracking
- **Motion sensors** for accelerometer data

## Architecture

```
📱 Flutter App
    ↓
🔄 Sensor Collection (Accelerometer + GPS)
    ↓
📡 HTTP API Calls (every 10 seconds)
    ↓
🖥️ .NET API Backend
    ↓
🗄️ SQLite Database
    ↓
📊 Real-time Wear Index Display
```

## Wear Index Scale

- 🟢 **0-2.0**: Excellent Road
- 🟡 **2.0-4.0**: Good Road
- 🟡 **4.0-6.0**: Fair Road
- 🟠 **6.0-8.0**: Poor Road
- 🔴 **8.0+**: Very Poor Road

## Testing

1. Start the backend API (`dotnet run`)
2. Run the mobile app
3. Tap "Start Logging"
4. Drive or shake phone to simulate road conditions
5. Watch wear index update in real-time
6. View data in web dashboard

## Cloud Build

This project includes GitHub Actions for automated iOS builds on macOS runners.
