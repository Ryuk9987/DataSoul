"""
DataSoul — Aldenmere Ambient Audio Generator
Generates a 45-second loopable ambient WAV for the starting town.
Layers:
  - Low wind drone (suboscillator)
  - Gentle wind whoosh (filtered noise)
  - Distant crowd murmur (modulated noise)
  - Occasional footstep transients
  - Subtle medieval bell overtones
Pure Python + numpy — no external audio libs required.
"""

import numpy as np
import struct
import os

OUT_DIR = os.path.dirname(os.path.abspath(__file__))
SAMPLE_RATE = 44100
DURATION    = 45.0  # seconds (loop-friendly)
N_SAMPLES   = int(SAMPLE_RATE * DURATION)

rng = np.random.default_rng(42)

def write_wav(filepath, samples, sr=44100):
    """Write a mono float32 array as 16-bit PCM WAV."""
    # Normalise to [-0.92, 0.92]
    peak = np.max(np.abs(samples))
    if peak > 0:
        samples = samples / peak * 0.92
    # Convert to int16
    pcm = (samples * 32767).astype(np.int16)
    n   = len(pcm)
    data_size = n * 2
    with open(filepath, "wb") as f:
        # RIFF header
        f.write(b"RIFF")
        f.write(struct.pack("<I", 36 + data_size))
        f.write(b"WAVE")
        # fmt chunk
        f.write(b"fmt ")
        f.write(struct.pack("<I", 16))       # chunk size
        f.write(struct.pack("<H", 1))        # PCM
        f.write(struct.pack("<H", 1))        # mono
        f.write(struct.pack("<I", sr))       # sample rate
        f.write(struct.pack("<I", sr * 2))   # byte rate
        f.write(struct.pack("<H", 2))        # block align
        f.write(struct.pack("<H", 16))       # bits per sample
        # data chunk
        f.write(b"data")
        f.write(struct.pack("<I", data_size))
        f.write(pcm.tobytes())
    size_kb = os.path.getsize(filepath) // 1024
    print(f"  ✓ {os.path.basename(filepath)} ({size_kb} KB, {DURATION:.0f}s, 44.1kHz mono)")

def make_sin(freq, amp, phase=0.0):
    t = np.linspace(0, DURATION, N_SAMPLES, endpoint=False)
    return amp * np.sin(2 * np.pi * freq * t + phase)

def lowpass_simple(signal, cutoff_hz, sr=44100, order=4):
    """Simple first-order IIR low-pass (repeated for order)."""
    rc = 1.0 / (2 * np.pi * cutoff_hz)
    dt = 1.0 / sr
    alpha = dt / (rc + dt)
    out = np.zeros_like(signal)
    out[0] = signal[0]
    for i in range(1, len(signal)):
        out[i] = out[i-1] + alpha * (signal[i] - out[i-1])
    return out

def bandpass_simple(signal, lo, hi, sr=44100):
    """Crude bandpass via difference of two low-passes."""
    return lowpass_simple(signal, hi, sr) - lowpass_simple(signal, lo, sr)

# ── Layer 1: Low wind drone (50 Hz + harmonics) ───────────────────────────
wind_drone = (
    make_sin(50,  0.18) +
    make_sin(75,  0.08, 0.3) +
    make_sin(100, 0.05, 1.1) +
    make_sin(30,  0.12, 2.1)
)
# Slow amplitude modulation (breathing ~0.07 Hz)
t = np.linspace(0, DURATION, N_SAMPLES, endpoint=False)
wind_mod = 0.7 + 0.3 * np.sin(2 * np.pi * 0.07 * t)
wind_drone = wind_drone * wind_mod

# ── Layer 2: Wind noise (low-pass filtered white noise) ───────────────────
white = rng.uniform(-1, 1, N_SAMPLES)
wind_noise = lowpass_simple(white, 350) * 0.35
# Modulate wind strength
wind_strength = 0.5 + 0.5 * np.sin(2 * np.pi * 0.04 * t + 1.2)
wind_noise = wind_noise * wind_strength

# ── Layer 3: Distant market murmur (narrow-band noise 300–1200 Hz) ────────
market_white = rng.uniform(-1, 1, N_SAMPLES)
market = bandpass_simple(market_white, 300, 1200) * 0.15
# Subtle crowd swell
crowd_mod = 0.6 + 0.4 * np.sin(2 * np.pi * 0.022 * t + 0.8)
market = market * crowd_mod

# ── Layer 4: Occasional footsteps (transient bursts) ─────────────────────
footsteps = np.zeros(N_SAMPLES)
# ~8 footstep groups spread across 45 seconds
step_times = [3.2, 7.8, 12.1, 18.5, 23.7, 30.2, 36.4, 41.0]
step_duration_samples = int(0.08 * SAMPLE_RATE)
for st in step_times:
    start = int(st * SAMPLE_RATE)
    if start + step_duration_samples < N_SAMPLES:
        # Transient: sharp attack, fast decay
        env = np.exp(-np.linspace(0, 12, step_duration_samples))
        noise_burst = rng.uniform(-1, 1, step_duration_samples)
        footsteps[start:start+step_duration_samples] += noise_burst * env * 0.22

# ── Layer 5: Subtle medieval bell overtones (distant, very quiet) ─────────
# Bell frequencies (partial series for a medieval-ish bell)
bell_freqs = [440, 880, 1056, 1320, 1760]
bell_amps  = [0.04, 0.025, 0.015, 0.010, 0.006]
bells = np.zeros(N_SAMPLES)
for f, a in zip(bell_freqs, bell_amps):
    # Very slow amplitude decay to simulate a distant bell struck once at ~t=8s
    strike_t = 8.0
    strike_idx = int(strike_t * SAMPLE_RATE)
    decay_samples = N_SAMPLES - strike_idx
    decay_env = np.exp(-np.linspace(0, 8, decay_samples))
    bell_wave = a * np.sin(2 * np.pi * f * np.linspace(0, DURATION - strike_t,
                                                         decay_samples, endpoint=False))
    bells[strike_idx:] += bell_wave * decay_env

# Second distant bell at ~t=28s
for f, a in zip(bell_freqs, bell_amps):
    strike_t = 28.0
    strike_idx = int(strike_t * SAMPLE_RATE)
    decay_samples = N_SAMPLES - strike_idx
    decay_env = np.exp(-np.linspace(0, 8, decay_samples))
    bell_wave = a * 0.7 * np.sin(2 * np.pi * f * np.linspace(0, DURATION - strike_t,
                                                               decay_samples, endpoint=False))
    bells[strike_idx:] += bell_wave * decay_env

# ── Mix all layers ─────────────────────────────────────────────────────────
mix = wind_drone + wind_noise + market + footsteps + bells

# ── Loop smoothing: crossfade first/last 2s to ensure seamless loop ────────
fade_len = int(2.0 * SAMPLE_RATE)
fade_in  = np.linspace(0, 1, fade_len)
fade_out = np.linspace(1, 0, fade_len)
mix[:fade_len]  *= fade_in
mix[-fade_len:] *= fade_out

# Apply a final gentle low-pass to smooth out any harshness
mix = lowpass_simple(mix, 8000)

# ── Output ─────────────────────────────────────────────────────────────────
out_path = os.path.join(
    os.path.dirname(OUT_DIR),  # one up from script dir (audio/music)
    # script is already in audio/music
    "ambient_aldenmere.wav"
)
# Script is IN audio/music/ already
out_path = os.path.join(OUT_DIR, "ambient_aldenmere.wav")

print("DataSoul — Ambient Audio Generator")
print("=" * 40)
write_wav(out_path, mix)
print("Done!")
