from flask import Flask, request, send_file, jsonify

import cv2
import io
import logging
import numpy as np
import requests

from PIL import Image

from services.face_detector import FaceDetector
from services.glasses_overlay import GlassesOverlay
from utils.image_utils import read_image

app = Flask(__name__)

logging.basicConfig(
    level=logging.INFO, format="%(asctime)s - %(levelname)s - %(message)s"
)
logger = logging.getLogger(__name__)

detector = FaceDetector()

MAX_IMAGE_SIZE = 2000


def resize_if_needed(image):
    h, w = image.shape[:2]
    largest_side = max(h, w)
    if largest_side <= MAX_IMAGE_SIZE:
        return image
    scale = MAX_IMAGE_SIZE / largest_side
    new_w = int(w * scale)
    new_h = int(h * scale)
    return cv2.resize(image, (new_w, new_h), interpolation=cv2.INTER_AREA)


def load_image_from_url(url):
    response = requests.get(url, timeout=10)
    if response.status_code != 200:
        raise ValueError(f"Failed to download glasses image from URL")
    np_buffer = np.frombuffer(response.content, dtype=np.uint8)
    image = cv2.imdecode(np_buffer, cv2.IMREAD_UNCHANGED)
    if image is None:
        raise ValueError("Failed to decode glasses image from URL")
    return image


@app.route("/health", methods=["GET"])
def health():
    return {"status": "healthy"}


@app.route("/try-on", methods=["POST"])
def try_on():
    try:

        # ? Validate face_image file
        if "face_image" not in request.files:
            return jsonify({"error": "face_image missing"}), 400

        # ? Validate glasses_url string
        glasses_url = request.form.get("glasses_url")
        if not glasses_url:
            return jsonify({"error": "glasses_url missing"}), 400

        # ? Read face image
        face_image = read_image(request.files["face_image"])
        if face_image is None:
            return jsonify({"error": "Invalid face image"}), 400

        # ? Load glasses image from URL
        glasses_image = load_image_from_url(glasses_url)

        # ? Convert Grayscale
        if len(face_image.shape) == 2:
            face_image = cv2.cvtColor(face_image, cv2.COLOR_GRAY2BGR)

        if len(glasses_image.shape) == 2:
            glasses_image = cv2.cvtColor(glasses_image, cv2.COLOR_GRAY2BGR)

        # ? Resize Large Images
        face_image = resize_if_needed(face_image)
        glasses_image = resize_if_needed(glasses_image)

        logger.info(f"Face image shape: {face_image.shape}")
        logger.info(f"Glasses image shape: {glasses_image.shape}")

        # ? Detect Face
        face_data = detector.detect(face_image)
        logger.info(f"Face detected successfully")
        logger.info(f"Face width: {face_data['face_width']}")
        logger.info(f"Angle: {face_data['angle']:.2f}")

        # ? Overlay Glasses
        result = GlassesOverlay.overlay(face_image, glasses_image, face_data)

        # ? Convert To PNG
        rgb = cv2.cvtColor(result, cv2.COLOR_BGR2RGB)
        image = Image.fromarray(rgb)
        img_io = io.BytesIO()
        image.save(img_io, format="PNG")
        img_io.seek(0)

        return send_file(
            img_io, mimetype="image/png", download_name="virtual_tryon.png"
        )

    except ValueError as e:
        logger.error(f"Validation Error: {str(e)}")
        return jsonify({"error": str(e)}), 400

    except Exception as e:
        logger.exception("Unhandled Exception")
        return jsonify({"error": "Internal server error", "details": str(e)}), 500


if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000, debug=True)
