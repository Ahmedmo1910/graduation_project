# 🕶️ Virtual Glasses Try-On API

A Flask-based REST API that uses **MediaPipe** and **OpenCV** to overlay glasses onto a user's photo in real time — built as part of a graduation project for an e-commerce eyewear platform.

---

## 📌 Overview

This API accepts a user's photo and a glasses image URL (from the product catalog), detects the user's face using MediaPipe, and returns a new image with the glasses accurately placed on the face — aligned to eye position, face width, and head tilt angle.

---

## 🧠 How It Works

```
User Photo (file upload)
        +
Glasses URL (from product data e.g. twoDImageUrl)
        ↓
┌─────────────────────────────┐
│        Flask API            │
│                             │
│  1. Detect face             │
│     (MediaPipe Detection)   │
│                             │
│  2. Extract landmarks       │
│     - Eye positions         │
│     - Face width            │
│     - Nose bridge           │
│     - Head tilt angle       │
│                             │
│  3. Download glasses PNG    │
│     from product URL        │
│                             │
│  4. Resize & rotate         │
│     glasses to fit face     │
│                             │
│  5. Alpha blend overlay     │
└─────────────────────────────┘
        ↓
  Result Image (PNG)
```

---

## 🗂️ Project Structure

```
virtual-try-on-Flask-API-Uploaded-Photo/
├── app.py                  # Flask app & API routes
├── requirements.txt        # Python dependencies
├── .gitignore
├── README.md
├── services/
│   ├── face_detector.py    # MediaPipe face detection & landmark extraction
│   └── glasses_overlay.py  # Glasses resizing, rotation & alpha blending
└── utils/
    └── image_utils.py      # Image reading & format utilities
```

---

## 🚀 Getting Started

### Prerequisites

- Python 3.9+
- pip

### Installation

```bash
# 1. Clone the repository
git clone https://github.com/Ahmedmo1910/virtual-try-on-Flask-API-Uploaded-Photo-.git
cd virtual-try-on-Flask-API-Uploaded-Photo-

# 2. Create a virtual environment
python -m venv venv

# Activate on Windows
venv\Scripts\activate

# Activate on macOS/Linux
source venv/bin/activate

# 3. Install dependencies
pip install -r requirements.txt

# 4. Run the server
python app.py
```

The API will be running at `http://localhost:5000`

---

## 📡 API Endpoints

### `GET /health`

Check if the API is running.

**Response:**
```json
{
  "status": "healthy"
}
```

---

### `POST /try-on`

Overlay glasses onto a user's face photo.

**Request Type:** `multipart/form-data`

| Field | Type | Required | Description |
|---|---|---|---|
| `face_image` | File | ✅ | User's photo (JPG, JPEG, PNG, WEBP) |
| `glasses_url` | Text | ✅ | URL of the glasses PNG from product data |

**Example Request (Postman):**

```
POST http://localhost:5000/try-on
Body → form-data:
  face_image  → [File]  user_photo.jpg
  glasses_url → [Text]  https://backendgraduationproject1.runasp.net/api/files/images2d/The Architect.png
```

**Example Request (cURL):**

```bash
curl -X POST http://localhost:5000/try-on \
  -F "face_image=@/path/to/photo.jpg" \
  -F "glasses_url=https://backendgraduationproject1.runasp.net/api/files/images2d/The Architect.png"
```

**Success Response:**
- Status: `200 OK`
- Content-Type: `image/png`
- Body: The result image with glasses overlaid

**Error Responses:**

| Status | Error | Reason |
|---|---|---|
| `400` | `face_image missing` | No face image was provided |
| `400` | `glasses_url missing` | No glasses URL was provided |
| `400` | `No face detected` | MediaPipe couldn't find a face in the photo |
| `400` | `Unsupported image format` | File type not in JPG, JPEG, PNG, WEBP |
| `500` | `Internal server error` | Unexpected server-side failure |

---

## 🔗 Integration with Backend

The backend should send a request to this API when the user clicks **"Try On"** on a product page.

**Expected product data structure:**
```json
{
  "productId": "123",
  "name": "The Architect",
  "twoDImageUrl": "https://backendgraduationproject1.runasp.net/api/files/images2d/The Architect.png"
}
```

**Backend integration flow:**
```
1. User selects a glasses product
2. User uploads their photo & clicks "Try On"
3. Backend receives:
     - face_image  → from user upload
     - glasses_url → from product's twoDImageUrl field
4. Backend forwards both to this Flask API (POST /try-on)
5. Flask returns the result image
6. Backend sends result image to Frontend
```

---

## ⚙️ Configuration

| Variable | Default | Description |
|---|---|---|
| `MAX_IMAGE_SIZE` | `2000px` | Max image dimension before auto-resize |
| `WIDTH_FACTOR` | `1.08` | Glasses width relative to face width |
| `MIN_DETECTION_CONFIDENCE` | `0.5` | MediaPipe face detection threshold |

---

## 📦 Dependencies

| Package | Purpose |
|---|---|
| `flask` | Web framework & API routing |
| `opencv-python` | Image processing & alpha blending |
| `mediapipe` | Face detection & landmark extraction |
| `numpy` | Array & matrix operations |
| `Pillow` | Image format conversion |
| `requests` | Download glasses image from URL |

---

## 🧪 Testing with Postman

1. Open Postman
2. Create a new `POST` request to `http://localhost:5000/try-on`
3. Go to **Body** → select **form-data**
4. Add:
   - Key: `face_image` | Type: **File** | Value: your photo
   - Key: `glasses_url` | Type: **Text** | Value: glasses image URL
5. Click **Send**
6. In the response, click **Save Response** → **Save to a file** to see the result image

---

## 🏗️ Built With

- [Flask](https://flask.palletsprojects.com/) — Python web framework
- [MediaPipe](https://mediapipe.dev/) — Face detection & mesh landmarks
- [OpenCV](https://opencv.org/) — Computer vision & image processing
- [NumPy](https://numpy.org/) — Numerical computing

---

## 👨‍💻 Author

Built as part of a graduation project — Eyewear E-Commerce Platform with Virtual Try-On feature.
