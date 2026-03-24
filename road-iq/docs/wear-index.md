# Wear Index

# Wear Index v1 – RoadIQ

## 1. Purpose

The Wear Index quantifies the relative mechanical stress imposed on a vehicle
when traveling over a road segment.

It is designed to:
- Compare road quality between routes
- Estimate suspension and tyre wear risk
- Enable damage-aware route planning

This is a **relative index**, not a guarantee of damage.

---

## 2. Input Data Contract

Each sensor record represents a single timestamped reading.

### Required Fields

| Field Name   | Type    | Description |
|-------------|---------|-------------|
| timestamp   | datetime | Time of sensor reading |
| lat         | float   | Latitude |
| lon         | float   | Longitude |
| speed_kmh   | float   | Vehicle speed in km/h |
| accel_z     | float   | Vertical acceleration (m/s²) |

---

## 3. Feature Engineering

Sensor data is aggregated per road segment or trip window.

### Extracted Features

#### 3.1 Average Absolute Vertical Acceleration
```text
avg_abs_accel = mean(|accel_z|)