import csv
import json
import sys

input_csv = sys.argv[1]
output_json = sys.argv[2]

records = []

with open(input_csv, newline='', encoding='utf-8') as f:
    reader = csv.DictReader(f)

    # Detect column names dynamically
    headers = reader.fieldnames

    lat_key = next(h for h in headers if "lat" in h.lower())
    lon_key = next(h for h in headers if "lon" in h.lower())
    speed_key = next(h for h in headers if "vel" in h.lower() or "speed" in h.lower())
    accel_z_key = next(h for h in headers if "z" in h.lower() and "acc" in h.lower())
    time_key = headers[0]  # usually first column

    for row in reader:
        # ✅ Skip rows with missing GPS
        if not row[lat_key] or not row[lon_key]:
            continue

        try:
            record = {
                "timestamp": row[time_key],
                "lat": float(row[lat_key]),
                "lon": float(row[lon_key]),
                "speedKmh": float(row[speed_key]) * 3.6,
                "accelZ": float(row[accel_z_key])
            }
            records.append(record)
        except ValueError:
            # Skip malformed rows safely
            continue

with open(output_json, "w", encoding="utf-8") as f:
    json.dump({"sensorRecords": records}, f, indent=2)

print(f"✅ Converted {len(records)} valid sensor records")