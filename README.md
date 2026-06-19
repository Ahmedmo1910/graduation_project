# Optivio — AI-Powered Virtual Try-On Platform for Eyewear E-Commerce

Optivio is an AI-powered platform that brings the in-store eyewear try-on experience online. Users can upload a photo or use their camera to get their face shape classified by a machine learning model, receive personalized frame recommendations based on that classification, virtually try on glasses in 3D, and shop for eyewear — all in one place.

This project was developed as a graduation project for the **Faculty of Computers & Information Technology, Egyptian E-Learning University** (Fayoum, 2026), supervised by **Dr. Manal Shaaban** (Assistant: Eng. Ziad Mohamed).

## Team

| Name | ID |
|------|-----|
| Ahmed Mohamed Abd El Rahem | 2202105 |
| Verona Safwat Eid | 2202016 |
| Marya Medhat Fayez | 2201995 |
| Mariam Hanna Farhat | 2202007 |
| Nader Abd Elhameed | 2200135 |
| Alaa Adel Ibrahim | 2202130 |
| Bilal Ashraf Kamal | 2202062 |
| Hager Adel Abuzaid | 2201979 |
| Mariam Hany Zarif | 2200679 |

## The Problem

Online eyewear shopping suffers from a core limitation: customers can't physically try on frames before buying. This leads to uncertainty, low purchase confidence, and high return rates. Existing platforms (Glasses.com, Zenni Optical, Warby Parker, Magrabi) offer pieces of the solution — virtual try-on *or* e-commerce *or* basic recommendations — but none combine AI-driven face shape analysis, realistic AR try-on, personalized recommendations, and a full shopping experience in a single system, particularly in the Egyptian market.

## What Optivio Does

- **Face Shape Classification** — Classifies a user's face into one of 5 categories (Heart, Oblong, Oval, Round, Square) using a VGG16-based CNN, achieving ~92.5% validation accuracy.
- **Facial Landmark Detection** — Uses MediaPipe Face Mesh (468 landmarks) and OpenCV to track facial geometry in real time for accurate eyewear alignment.
- **3D Virtual Try-On** — Renders realistic, real-time 3D glasses models on the user's face using Three.js and WebGL, with accurate scaling, rotation, and positioning based on facial landmarks.
- **Personalized Recommendations** — Matches the user's classified face shape against a tagged eyewear catalog to suggest the most suitable frames.
- **Online Vision Test (optional)** — A basic visual acuity screening tool for preliminary self-assessment (not a replacement for a professional eye exam).
- **E-Commerce** — Product browsing, search, filtering, shopping cart, and checkout.

## How It's Built

The system follows a modular client–server architecture, split across four branches in this repository:

| Branch | Responsibility | Stack |
|--------|----------------|-------|
| [`AI`](https://github.com/Ahmedmo1910/graduation_project/tree/AI) | Face shape classification model, inference API & virtual try-on from uploaded photo | Python, TensorFlow, VGG16 (transfer learning), MediaPipe, OpenCV |
| [`Backend`](https://github.com/Ahmedmo1910/graduation_project/tree/Backend) | Business logic, auth, product/order management, API layer | .NET, SQL Server |
| [`frontend`](https://github.com/Ahmedmo1910/graduation_project/tree/frontend) | User interface — browsing, try-on UI, recommendations, vision test, cart/checkout | Angular, TypeScript, HTML/CSS |
| [`3D`](https://github.com/Ahmedmo1910/graduation_project/tree/3D) | Real-time 3D eyewear rendering and try-on alignment | Three.js, WebGL |

**Data flow:** A user's image/video is captured in the **frontend**. Facial landmarks are extracted (MediaPipe/OpenCV) and passed to the **AI** model, which classifies face shape. That result is sent to the **Backend**, which filters the product catalog and returns recommended frames. The **3D** module takes the landmark data to position and render the eyewear model in real time over the user's face. All components communicate over RESTful APIs, exchanging data as JSON.

## Tech Stack Summary

- **Frontend:** Angular, TypeScript, HTML, CSS
- **Backend:** .NET, SQL Server
- **AI / Computer Vision:** Python, TensorFlow, VGG16, MediaPipe Face Mesh, OpenCV
- **3D / AR:** Three.js, WebGL
- **Tooling:** Google Colab (model training), Visual Studio, Visual Studio Code, Git & GitHub

## Model Performance

The face shape classification model (VGG16, transfer learning) was evaluated on 5 classes:

| Face Shape | Precision | Recall | F1-Score |
|------------|-----------|--------|----------|
| Heart | 0.93 | 0.92 | 0.92 |
| Oblong | 0.95 | 0.93 | 0.94 |
| Oval | 0.91 | 0.90 | 0.90 |
| Round | 0.89 | 0.91 | 0.90 |
| Square | 0.92 | 0.94 | 0.93 |
| **Weighted Avg** | **0.92** | **0.92** | **0.92** |

Overall validation accuracy: **~92.47%**

## Project Status

Most core modules — face shape classification, facial landmark tracking, 3D try-on rendering, and personalized recommendations — are functional. E-commerce features (browsing, filtering, cart) and the optional vision test module are also in place. The system continues to be refined for accuracy, dataset diversity, and real-time performance.

## Getting Started

Each branch contains the code for its respective component. Check out the branch you need and refer to its own setup instructions:

```bash
git clone https://github.com/Ahmedmo1910/graduation_project.git
cd graduation_project

# Switch to the component you want to work on
git checkout AI         # Face shape classification model + API
git checkout Backend    # .NET API and business logic
git checkout frontend   # Angular application
git checkout 3D         # Three.js try-on rendering
```

## Known Limitations

- The face shape dataset originally had a gender imbalance (majority female images); additional male images and augmentation were added, but further diversification would improve robustness.
- Oval and Round face shapes share similar geometric features and can occasionally be misclassified.
- Face detection and landmark accuracy can be affected by poor lighting, camera quality, and extreme head angles.
- Real-time 3D rendering may be resource-intensive on low-end devices.
- The vision test module is for preliminary screening only and does not replace a professional eye exam.
- Recommendations are currently based on face shape alone; factors like skin tone, style preference, and purchase history are planned for future versions.

## Future Work

- More advanced classification architectures (EfficientNet, ResNet, Vision Transformers)
- Larger, more diverse training dataset
- Recommendation engine incorporating style, skin tone, and user behavior (collaborative filtering)
- Full 3D face reconstruction for more realistic fitting
- Markerless AR via WebXR
- AI-assisted prescription/vision guidance
- Native mobile apps (iOS/Android)
- Multi-accessory try-on (sunglasses, contact lenses, watches, jewelry)

## References

- Niten19, "Face Shape Dataset," Kaggle, 2022
- Simonyan, K., and Zisserman, A., "Very Deep Convolutional Networks for Large-Scale Image Recognition," ICLR, 2015
- Bazarevsky, V., et al., "MediaPipe Face Mesh: Real-Time Face Geometry Estimation Using Machine Learning," 2020
- Bradski, G., "The OpenCV Library," Dr. Dobb's Journal of Software Tools, 2000
- TensorFlow, MediaPipe, OpenCV, Three.js, WebGL, Angular, and .NET official documentation
