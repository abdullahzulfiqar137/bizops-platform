# 🎨 AI Business Operations System (Angular 19 Frontend UI)

An enterprise single-page application built with **Angular 19 Standalone Components, RxJS Signals, Modern SCSS**, and featuring an **Interactive 3D Mouse-Tracking Robot Mascot** and **ChatGPT AI Chat Assistant**.

---

## 🌟 Key Frontend Features

### 🤖 1. Interactive 3D Mouse-Tracking Robot Mascot
- **3D Head Rotation & Tilt**: Calculates mouse cursor coordinates (`perspective(600px) rotateX(...) rotateY(...)`) so the robot's head and pupils rotate in 3D perspective to track mouse movements.
- **Dynamic Mood & Reactions**:
  - `happy` / `welcome`: Friendly greetings & guided instructions.
  - `thinking`: Pulse animation during authentication.
  - `sad` (Failed Login): Red alert state (`🤖 🥺`) with direct quick account registration trigger.
  - `excited` (Registration Success): Celebration state (`🤖 🎉`) with direct sign-in button.
- **Animated CSS Micro-Interactions**: Blinking eyes (`@keyframes blinkEye`), pulsing glowing antenna (`@keyframes pulseGlow`), waving arm (`👋`).

### 💬 2. ChatGPT-Style Universal AI Assistant
- Multi-prompt AI chat interface with instant typing indicators, pre-set suggestion chips, and full database conversation history persistence (`GET /api/ai/history` & `POST /api/ai/chat`).

### 📱 3. Responsive Mobile & Desktop Layouts
- Mobile sidebar drawer with hamburger menu (`☰`), backdrop overlay, and sticky responsive topbar.
- Clamped responsive modal forms preventing overflow on mobile screens.

---

## ⚡ Development Commands

### Start Development Server
```bash
npm start
# or
ng serve
```
Navigate to `http://localhost:4200/`.

### Production Build
```bash
npm run build
```
Generates production bundle in `dist/business-operations-ui`.

---

## 🔑 Default Credentials
- **Username**: `admin`
- **Password**: `Admin123!`
