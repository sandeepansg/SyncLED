<h1 align="center"> SyncLED </h1> <br>
<p align="center">
<img alt="SyncLED Board Layout" title="SyncLED Board Layout" src="https://github.com/SandeepanSengupta/SyncLED/blob/master/Demos/brd.png" width="250">
</p>

## Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [Feedback](#feedback)
- [Contributors](#contributors)
- [Acknowledgments](#acknowledgments)


## Features
- Real-time temperature monitoring of CPU and GPU
- Temperature dependent LED control
- Independent Host software
- Open Source hardware and Software - Make your own!

## Introduction
[![Build Platforms](https://img.shields.io/badge/build_platform-visual_studio-865FC5.svg)](https://www.visualstudio.com/vs/)
[![Build Platforms](https://img.shields.io/badge/build_platform-arduino-10A2AE.svg)](https://www.arduino.cc)
[![Dependency](https://img.shields.io/badge/dependency-open_hardware_monitor-4EC820.svg)](https://github.com/openhardwaremonitor/openhardwaremonitor)
[![Contributors](https://img.shields.io/badge/all_contributors-2-yellow.svg)](#contributors)
[![PCB](https://img.shields.io/badge/PCB_design-EagleCAD-EE8822.svg)](https://www.autodesk.com/products/eagle/overview)

SyncLED is a implementation of a basic secure communiaction framework to bridge communication between a host software(running in a PC) and a slave LED controller. The host software grabs the bus reported temperature data using [Open Hardware Monitor Library](https://github.com/SandeepanSengupta/SyncLED/blob/master/Sources/Application/Bridge/New/OpenHardwareMonitorLib.dll) and sends it over the usb to the salve embedded led controller using the self-tailored secure communication format.
<p align="center">
  <a href="https://gitpoint.co/">
    <img alt="Windows_App" title="Host Software" src="https://raw.githubusercontent.com/SandeepanSengupta/SyncLED/master/Demos/Application.JPG" width="350">
  </a>
</p>

<p align="center">
  Screenshot of host software.
</p>

## Feedback
Feel free to send us feedback on facebook:
 - [![Build Platforms](https://img.shields.io/badge/id-Tamojit-54C7EC.svg?style=for-the-badge)](https://www.facebook.com/saha.tamojit/)
 - [![Build Platforms](https://img.shields.io/badge/id-Sandeepan-54C7EC.svg?style=for-the-badge)](https://www.facebook.com/sengupta.sandeepan/)
<p> 
or [file an issue](https://github.com/SandeepanSengupta/SyncLED/issues/new). Feature requests are always welcome.</p>


## Acknowledgments

Thanks to [Google](https://www.google.com) and various developers for supporting us with a tons of developments... :wink:


