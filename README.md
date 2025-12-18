
# Bello Horizonte VR

Reconstrucción virtual de la playa **Bello Horizonte (Santa Marta, Colombia)** a partir de **datos geoespaciales reales**, optimizada para **Meta Quest standalone**.
<p align="center">
  <img src="https://github.com/user-attachments/assets/000a5c68-c17a-4318-81da-7406a36b327f" width="500" />
</p>

[Video demostración y exploración de la playa](https://www.youtube.com/watch?v=yUo5kCnFulg)


## Objetivo
Crear una experiencia VR ligera y coherente que represente:
- Terreno y costa con DEM real.
- Edificaciones con alturas aproximadas.
- Interacción estable y buen rendimiento en Quest.

## Datos y herramientas
- **Terreno**: GMRT (batimetría + topografía)
- **Edificios**: Google Open Buildings (huellas + alturas)
- **Procesamiento**: QGIS, Google Earth Engine, GDAL
- **Motor**: Unity (URP, XR Interaction Toolkit)
- **VR**: Meta Quest 2 / 3
- **Agua**:  (URP) [WaterWorks](https://assetstore.unity.com/packages/3d/environments/waterworks-simple-water-ocean-river-system-for-urp-reflection-re-206909?srsltid=AfmBOoqkrymmMM_R5XjfPinfuTs9vRpfAc1N3Zbdp-SeS25PQXB-Bovm)

---
## Guía de Importación

Este repositorio contiene archivos pesados para Realidad Virtual. Para asegurar que todos los archivos (texturas, modelos y prefabs) se descarguen correctamente, es **obligatorio** seguir estos pasos utilizando Git LFS.

## Requisitos Previos

Antes de comenzar, asegúrate de tener instalado:

* [Git](https://git-scm.com/)
* [Git LFS (Large File Storage)](https://git-lfs.github.com/)
* Unity Hub y la versión de Unity (6000.2.9f1).

---

## Cómo importar en Unity (forma correcta)

Sigue estos comandos en tu terminal para clonar el repositorio y gestionar los archivos pesados de manera eficiente:

### 1) Clonar y configurar Git LFS

Ejecuta los siguientes comandos paso a paso:

```bash
# Inicializar Git LFS en tu sistema
git lfs install

# Clonar el repositorio
git clone https://github.com/Jcedielb/BelloHorizonteRV.git

# Entrar al directorio del proyecto
cd BelloHorizonteRV

# Forzar la descarga de los archivos pesados (LFS)
git lfs pull

```

### 2) Apertura en Unity

1. Abre **Unity Hub**.
2. Haz clic en **Add** (Añadir) y selecciona la carpeta `BelloHorizonteRV`.
3. Espera a que Unity importe los assets y abra el proyecto (suele demorarse entre 5 a 10 minutos aproximadamente en la primera vez).

---



