const API_BASE = "http://localhost:5183";

// Send sample wear data
async function sendTestDrive() {
  const payload = {
    sensorRecords: [
      {
        timestamp: "2026-03-25T13:00:00Z",
        lat: -26.2041,
        lon: 28.0473,
        speedKmh: 60,
        accelZ: 0.12
      },
      {
        timestamp: "2026-03-25T13:00:01Z",
        lat: -26.2042,
        lon: 28.0474,
        speedKmh: 61,
        accelZ: 0.15
      },
      {
        timestamp: "2026-03-25T13:00:02Z",
        lat: -26.2043,
        lon: 28.0475,
        speedKmh: 59,
        accelZ: 0.10
      }
    ]
  };

  const res = await fetch(`${API_BASE}/wear-index/calculate`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload)
  });

  const data = await res.json();
  document.getElementById("wearResult").textContent =
    JSON.stringify(data, null, 2);
}

// Fetch degradation + cost forecast
async function getForecast() {
  const segmentId = document.getElementById("segmentId").value;

  const res = await fetch(
    `${API_BASE}/roads/${segmentId}/degradation-forecast`
  );

  const data = await res.json();
  document.getElementById("forecastResult").textContent =
    JSON.stringify(data, null, 2);
}