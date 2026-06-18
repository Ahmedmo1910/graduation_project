import cv2
import mediapipe as mp
import numpy as np


class FaceDetector:

    #! Eye landmarks
    LEFT_EYE_POINTS = [33, 133]
    RIGHT_EYE_POINTS = [362, 263]

    #! Face width landmarks
    LEFT_FACE_EDGE = 234
    RIGHT_FACE_EDGE = 454

    #! Nose bridge
    NOSE_BRIDGE = 168

    def __init__(self):

        self.mp_face_mesh = mp.solutions.face_mesh
        self.mp_face_detection = mp.solutions.face_detection

        self.face_detection = self.mp_face_detection.FaceDetection(
            model_selection=1, min_detection_confidence=0.5
        )

        self.face_mesh = self.mp_face_mesh.FaceMesh(
            static_image_mode=True,
            max_num_faces=1,
            refine_landmarks=True,
            min_detection_confidence=0.5,
        )

    def detect(self, image):

        rgb = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)

        detections = self.face_detection.process(rgb)

        if not detections.detections:
            raise ValueError("No face detected")

        h, w = image.shape[:2]

        # ? Select largest face

        largest_face = None
        largest_area = 0

        for detection in detections.detections:

            bbox = detection.location_data.relative_bounding_box

            bw = int(bbox.width * w)
            bh = int(bbox.height * h)

            area = bw * bh

            if area > largest_area:
                largest_area = area
                largest_face = bbox

        bbox = largest_face

        x = int(bbox.xmin * w)
        y = int(bbox.ymin * h)

        bw = int(bbox.width * w)
        bh = int(bbox.height * h)

        # ? Add padding

        padding = int(max(bw, bh) * 0.30)

        x = max(0, x - padding)
        y = max(0, y - padding)

        x2 = min(w, x + bw + (padding * 2))
        y2 = min(h, y + bh + (padding * 2))

        face_crop = image[y:y2, x:x2]

        crop_h, crop_w = face_crop.shape[:2]

        crop_rgb = cv2.cvtColor(face_crop, cv2.COLOR_BGR2RGB)

        mesh_result = self.face_mesh.process(crop_rgb)

        if not mesh_result.multi_face_landmarks:
            raise ValueError("Face landmarks not detected")

        landmarks = mesh_result.multi_face_landmarks[0]

        # ? Eyes

        left_eye_points = []
        right_eye_points = []

        for idx in self.LEFT_EYE_POINTS:

            lm = landmarks.landmark[idx]

            left_eye_points.append((int(lm.x * crop_w), int(lm.y * crop_h)))

        for idx in self.RIGHT_EYE_POINTS:

            lm = landmarks.landmark[idx]

            right_eye_points.append((int(lm.x * crop_w), int(lm.y * crop_h)))

        left_eye = np.mean(left_eye_points, axis=0)

        right_eye = np.mean(right_eye_points, axis=0)

        # ? Face Width

        left_edge = landmarks.landmark[self.LEFT_FACE_EDGE]

        right_edge = landmarks.landmark[self.RIGHT_FACE_EDGE]

        left_edge_x = int(left_edge.x * crop_w)

        right_edge_x = int(right_edge.x * crop_w)

        face_width = abs(right_edge_x - left_edge_x)

        # ? Nose Bridge

        nose = landmarks.landmark[self.NOSE_BRIDGE]

        nose_x = int(nose.x * crop_w)

        nose_y = int(nose.y * crop_h)

        # ? Rotation Angle

        dx = right_eye[0] - left_eye[0]
        dy = right_eye[1] - left_eye[1]

        angle = np.degrees(np.arctan2(dy, dx))

        eye_distance = np.sqrt(dx**2 + dy**2)

        # ? Convert crop coordinates
        # ? back to original image

        left_eye_original = (int(left_eye[0]) + x, int(left_eye[1]) + y)

        right_eye_original = (int(right_eye[0]) + x, int(right_eye[1]) + y)

        nose_original = (nose_x + x, nose_y + y)

        return {
            "left_eye": left_eye_original,
            "right_eye": right_eye_original,
            "nose_bridge": nose_original,
            "angle": float(angle),
            "eye_distance": float(eye_distance),
            "face_width": int(face_width),
            "crop_box": {"x1": x, "y1": y, "x2": x2, "y2": y2},
        }
