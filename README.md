
# Bello Horizonte RV - Guía de Importación

Este repositorio contiene activos de alta resolución para Realidad Virtual. Para asegurar que todos los archivos (texturas, modelos y bakes) se descarguen correctamente, es **obligatorio** seguir estos pasos utilizando Git LFS.

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
3. Espera a que Unity importe los assets y abra el proyecto (suele demorarse un poco al ser la primera vez).

---
