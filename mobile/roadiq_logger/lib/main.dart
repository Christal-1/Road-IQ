import 'package:flutter/material.dart';
import 'package:sensors_plus/sensors_plus.dart';
import 'package:geolocator/geolocator.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'dart:async';

void main() {
  runApp(const RoadIQLoggerApp());
}

class RoadIQLoggerApp extends StatelessWidget {
  const RoadIQLoggerApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'RoadIQ Logger',
      theme: ThemeData(
        primarySwatch: Colors.blue,
        useMaterial3: true,
      ),
      home: const SensorLoggerScreen(),
    );
  }
}

class SensorLoggerScreen extends StatefulWidget {
  const SensorLoggerScreen({super.key});

  @override
  State<SensorLoggerScreen> createState() => _SensorLoggerScreenState();
}

class _SensorLoggerScreenState extends State<SensorLoggerScreen> {
  bool _isLogging = false;
  List<Map<String, dynamic>> _sensorData = [];
  double _currentWearIndex = 0.0;
  Position? _lastPosition;
  StreamSubscription<AccelerometerEvent>? _accelerometerSubscription;
  StreamSubscription<Position>? _positionSubscription;
  Timer? _apiTimer;

  @override
  void initState() {
    super.initState();
    _requestPermissions();
  }

  @override
  void dispose() {
    _stopLogging();
    super.dispose();
  }

  Future<void> _requestPermissions() async {
    LocationPermission permission = await Geolocator.checkPermission();
    if (permission == LocationPermission.denied) {
      permission = await Geolocator.requestPermission();
    }
  }

  void _startLogging() async {
    setState(() {
      _isLogging = true;
      _sensorData = [];
    });

    // Start accelerometer listening
    _accelerometerSubscription = accelerometerEvents.listen((event) {
      _addSensorReading(event);
    });

    // Start GPS listening
    _positionSubscription = Geolocator.getPositionStream(
      locationSettings: const LocationSettings(
        accuracy: LocationAccuracy.high,
        distanceFilter: 1,
      ),
    ).listen((position) {
      _lastPosition = position;
    });

    // Send data to API every 10 seconds
    _apiTimer = Timer.periodic(const Duration(seconds: 10), (timer) {
      _sendDataToAPI();
    });
  }

  void _stopLogging() {
    _accelerometerSubscription?.cancel();
    _positionSubscription?.cancel();
    _apiTimer?.cancel();

    setState(() {
      _isLogging = false;
    });

    // Send final batch
    _sendDataToAPI();
  }

  void _addSensorReading(AccelerometerEvent event) {
    if (_lastPosition == null) return;

    final reading = {
      'timestamp': DateTime.now().toIso8601String(),
      'lat': _lastPosition!.latitude,
      'lon': _lastPosition!.longitude,
      'speedKmh': _calculateSpeed(),
      'accelZ': event.z,
    };

    setState(() {
      _sensorData.add(reading);
      if (_sensorData.length > 100) {
        _sensorData.removeAt(0); // Keep last 100 readings
      }
    });
  }

  double _calculateSpeed() {
    // Simple speed calculation (would need proper GPS tracking for accuracy)
    return _lastPosition?.speed != null ? _lastPosition!.speed! * 3.6 : 0.0;
  }

  Future<void> _sendDataToAPI() async {
    if (_sensorData.isEmpty) return;

    try {
      final response = await http.post(
        Uri.parse('http://10.10.62.40:5183/wear-index/calculate'), // Computer IP for iPhone
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({'sensorRecords': _sensorData}),
      );

      if (response.statusCode == 200) {
        final data = jsonDecode(response.body);
        setState(() {
          _currentWearIndex = data['wearIndex'] ?? 0.0;
        });
      }
    } catch (e) {
      // Handle error
    }

    // Clear sent data
    setState(() {
      _sensorData = [];
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('RoadIQ Logger'),
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Card(
              child: Padding(
                padding: const EdgeInsets.all(16.0),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Wear Index: ${_currentWearIndex.toStringAsFixed(2)}',
                      style: Theme.of(context).textTheme.headlineMedium,
                    ),
                    const SizedBox(height: 8),
                    Text(
                      _getWearDescription(_currentWearIndex),
                      style: TextStyle(
                        color: _getWearColor(_currentWearIndex),
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 20),
            Row(
              children: [
                Expanded(
                  child: ElevatedButton(
                    onPressed: _isLogging ? null : _startLogging,
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Colors.green,
                      padding: const EdgeInsets.symmetric(vertical: 16),
                    ),
                    child: const Text('Start Logging'),
                  ),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: ElevatedButton(
                    onPressed: _isLogging ? _stopLogging : null,
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Colors.red,
                      padding: const EdgeInsets.symmetric(vertical: 16),
                    ),
                    child: const Text('Stop Logging'),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 20),
            Text(
              'Status: ${_isLogging ? "Logging Active" : "Stopped"}',
              style: Theme.of(context).textTheme.titleMedium,
            ),
            const SizedBox(height: 10),
            Text('Data Points: ${_sensorData.length}'),
            const SizedBox(height: 20),
            if (_lastPosition != null) ...[
              Text('Current Location:'),
              Text('${_lastPosition!.latitude.toStringAsFixed(6)}, ${_lastPosition!.longitude.toStringAsFixed(6)}'),
              Text('Speed: ${_calculateSpeed().toStringAsFixed(1)} km/h'),
            ],
          ],
        ),
      ),
    );
  }

  String _getWearDescription(double index) {
    if (index < 2) return 'Excellent Road';
    if (index < 4) return 'Good Road';
    if (index < 6) return 'Fair Road';
    if (index < 8) return 'Poor Road';
    return 'Very Poor Road';
  }

  Color _getWearColor(double index) {
    if (index < 2) return Colors.green;
    if (index < 4) return Colors.lightGreen;
    if (index < 6) return Colors.yellow;
    if (index < 8) return Colors.orange;
    return Colors.red;
  }
}