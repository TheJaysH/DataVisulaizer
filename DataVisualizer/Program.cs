﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DataVisualizer
{
    class Program
    {
        // this is the file header. it dictates colour pallet, and filesize etc. and other file info
        // I've extracted this from an empty bitmap, so the size etc is static
        private static byte[] BMP_Header = { 0x42, 0x4D, 0x36, 0x04, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x36, 0x04, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x01, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x80, 0x80, 0x00, 0x80, 0x00, 0x00, 0x00, 0x80, 0x00, 0x80, 0x00, 0x80, 0x80, 0x00, 0x00, 0xC0, 0xC0, 0xC0, 0x00, 0xC0, 0xDC, 0xC0, 0x00, 0xF0, 0xCA, 0xA6, 0x00, 0x00, 0x20, 0x40, 0x00, 0x00, 0x20, 0x60, 0x00, 0x00, 0x20, 0x80, 0x00, 0x00, 0x20, 0xA0, 0x00, 0x00, 0x20, 0xC0, 0x00, 0x00, 0x20, 0xE0, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x40, 0x20, 0x00, 0x00, 0x40, 0x40, 0x00, 0x00, 0x40, 0x60, 0x00, 0x00, 0x40, 0x80, 0x00, 0x00, 0x40, 0xA0, 0x00, 0x00, 0x40, 0xC0, 0x00, 0x00, 0x40, 0xE0, 0x00, 0x00, 0x60, 0x00, 0x00, 0x00, 0x60, 0x20, 0x00, 0x00, 0x60, 0x40, 0x00, 0x00, 0x60, 0x60, 0x00, 0x00, 0x60, 0x80, 0x00, 0x00, 0x60, 0xA0, 0x00, 0x00, 0x60, 0xC0, 0x00, 0x00, 0x60, 0xE0, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x80, 0x20, 0x00, 0x00, 0x80, 0x40, 0x00, 0x00, 0x80, 0x60, 0x00, 0x00, 0x80, 0x80, 0x00, 0x00, 0x80, 0xA0, 0x00, 0x00, 0x80, 0xC0, 0x00, 0x00, 0x80, 0xE0, 0x00, 0x00, 0xA0, 0x00, 0x00, 0x00, 0xA0, 0x20, 0x00, 0x00, 0xA0, 0x40, 0x00, 0x00, 0xA0, 0x60, 0x00, 0x00, 0xA0, 0x80, 0x00, 0x00, 0xA0, 0xA0, 0x00, 0x00, 0xA0, 0xC0, 0x00, 0x00, 0xA0, 0xE0, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0xC0, 0x20, 0x00, 0x00, 0xC0, 0x40, 0x00, 0x00, 0xC0, 0x60, 0x00, 0x00, 0xC0, 0x80, 0x00, 0x00, 0xC0, 0xA0, 0x00, 0x00, 0xC0, 0xC0, 0x00, 0x00, 0xC0, 0xE0, 0x00, 0x00, 0xE0, 0x00, 0x00, 0x00, 0xE0, 0x20, 0x00, 0x00, 0xE0, 0x40, 0x00, 0x00, 0xE0, 0x60, 0x00, 0x00, 0xE0, 0x80, 0x00, 0x00, 0xE0, 0xA0, 0x00, 0x00, 0xE0, 0xC0, 0x00, 0x00, 0xE0, 0xE0, 0x00, 0x40, 0x00, 0x00, 0x00, 0x40, 0x00, 0x20, 0x00, 0x40, 0x00, 0x40, 0x00, 0x40, 0x00, 0x60, 0x00, 0x40, 0x00, 0x80, 0x00, 0x40, 0x00, 0xA0, 0x00, 0x40, 0x00, 0xC0, 0x00, 0x40, 0x00, 0xE0, 0x00, 0x40, 0x20, 0x00, 0x00, 0x40, 0x20, 0x20, 0x00, 0x40, 0x20, 0x40, 0x00, 0x40, 0x20, 0x60, 0x00, 0x40, 0x20, 0x80, 0x00, 0x40, 0x20, 0xA0, 0x00, 0x40, 0x20, 0xC0, 0x00, 0x40, 0x20, 0xE0, 0x00, 0x40, 0x40, 0x00, 0x00, 0x40, 0x40, 0x20, 0x00, 0x40, 0x40, 0x40, 0x00, 0x40, 0x40, 0x60, 0x00, 0x40, 0x40, 0x80, 0x00, 0x40, 0x40, 0xA0, 0x00, 0x40, 0x40, 0xC0, 0x00, 0x40, 0x40, 0xE0, 0x00, 0x40, 0x60, 0x00, 0x00, 0x40, 0x60, 0x20, 0x00, 0x40, 0x60, 0x40, 0x00, 0x40, 0x60, 0x60, 0x00, 0x40, 0x60, 0x80, 0x00, 0x40, 0x60, 0xA0, 0x00, 0x40, 0x60, 0xC0, 0x00, 0x40, 0x60, 0xE0, 0x00, 0x40, 0x80, 0x00, 0x00, 0x40, 0x80, 0x20, 0x00, 0x40, 0x80, 0x40, 0x00, 0x40, 0x80, 0x60, 0x00, 0x40, 0x80, 0x80, 0x00, 0x40, 0x80, 0xA0, 0x00, 0x40, 0x80, 0xC0, 0x00, 0x40, 0x80, 0xE0, 0x00, 0x40, 0xA0, 0x00, 0x00, 0x40, 0xA0, 0x20, 0x00, 0x40, 0xA0, 0x40, 0x00, 0x40, 0xA0, 0x60, 0x00, 0x40, 0xA0, 0x80, 0x00, 0x40, 0xA0, 0xA0, 0x00, 0x40, 0xA0, 0xC0, 0x00, 0x40, 0xA0, 0xE0, 0x00, 0x40, 0xC0, 0x00, 0x00, 0x40, 0xC0, 0x20, 0x00, 0x40, 0xC0, 0x40, 0x00, 0x40, 0xC0, 0x60, 0x00, 0x40, 0xC0, 0x80, 0x00, 0x40, 0xC0, 0xA0, 0x00, 0x40, 0xC0, 0xC0, 0x00, 0x40, 0xC0, 0xE0, 0x00, 0x40, 0xE0, 0x00, 0x00, 0x40, 0xE0, 0x20, 0x00, 0x40, 0xE0, 0x40, 0x00, 0x40, 0xE0, 0x60, 0x00, 0x40, 0xE0, 0x80, 0x00, 0x40, 0xE0, 0xA0, 0x00, 0x40, 0xE0, 0xC0, 0x00, 0x40, 0xE0, 0xE0, 0x00, 0x80, 0x00, 0x00, 0x00, 0x80, 0x00, 0x20, 0x00, 0x80, 0x00, 0x40, 0x00, 0x80, 0x00, 0x60, 0x00, 0x80, 0x00, 0x80, 0x00, 0x80, 0x00, 0xA0, 0x00, 0x80, 0x00, 0xC0, 0x00, 0x80, 0x00, 0xE0, 0x00, 0x80, 0x20, 0x00, 0x00, 0x80, 0x20, 0x20, 0x00, 0x80, 0x20, 0x40, 0x00, 0x80, 0x20, 0x60, 0x00, 0x80, 0x20, 0x80, 0x00, 0x80, 0x20, 0xA0, 0x00, 0x80, 0x20, 0xC0, 0x00, 0x80, 0x20, 0xE0, 0x00, 0x80, 0x40, 0x00, 0x00, 0x80, 0x40, 0x20, 0x00, 0x80, 0x40, 0x40, 0x00, 0x80, 0x40, 0x60, 0x00, 0x80, 0x40, 0x80, 0x00, 0x80, 0x40, 0xA0, 0x00, 0x80, 0x40, 0xC0, 0x00, 0x80, 0x40, 0xE0, 0x00, 0x80, 0x60, 0x00, 0x00, 0x80, 0x60, 0x20, 0x00, 0x80, 0x60, 0x40, 0x00, 0x80, 0x60, 0x60, 0x00, 0x80, 0x60, 0x80, 0x00, 0x80, 0x60, 0xA0, 0x00, 0x80, 0x60, 0xC0, 0x00, 0x80, 0x60, 0xE0, 0x00, 0x80, 0x80, 0x00, 0x00, 0x80, 0x80, 0x20, 0x00, 0x80, 0x80, 0x40, 0x00, 0x80, 0x80, 0x60, 0x00, 0x80, 0x80, 0x80, 0x00, 0x80, 0x80, 0xA0, 0x00, 0x80, 0x80, 0xC0, 0x00, 0x80, 0x80, 0xE0, 0x00, 0x80, 0xA0, 0x00, 0x00, 0x80, 0xA0, 0x20, 0x00, 0x80, 0xA0, 0x40, 0x00, 0x80, 0xA0, 0x60, 0x00, 0x80, 0xA0, 0x80, 0x00, 0x80, 0xA0, 0xA0, 0x00, 0x80, 0xA0, 0xC0, 0x00, 0x80, 0xA0, 0xE0, 0x00, 0x80, 0xC0, 0x00, 0x00, 0x80, 0xC0, 0x20, 0x00, 0x80, 0xC0, 0x40, 0x00, 0x80, 0xC0, 0x60, 0x00, 0x80, 0xC0, 0x80, 0x00, 0x80, 0xC0, 0xA0, 0x00, 0x80, 0xC0, 0xC0, 0x00, 0x80, 0xC0, 0xE0, 0x00, 0x80, 0xE0, 0x00, 0x00, 0x80, 0xE0, 0x20, 0x00, 0x80, 0xE0, 0x40, 0x00, 0x80, 0xE0, 0x60, 0x00, 0x80, 0xE0, 0x80, 0x00, 0x80, 0xE0, 0xA0, 0x00, 0x80, 0xE0, 0xC0, 0x00, 0x80, 0xE0, 0xE0, 0x00, 0xC0, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x20, 0x00, 0xC0, 0x00, 0x40, 0x00, 0xC0, 0x00, 0x60, 0x00, 0xC0, 0x00, 0x80, 0x00, 0xC0, 0x00, 0xA0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xE0, 0x00, 0xC0, 0x20, 0x00, 0x00, 0xC0, 0x20, 0x20, 0x00, 0xC0, 0x20, 0x40, 0x00, 0xC0, 0x20, 0x60, 0x00, 0xC0, 0x20, 0x80, 0x00, 0xC0, 0x20, 0xA0, 0x00, 0xC0, 0x20, 0xC0, 0x00, 0xC0, 0x20, 0xE0, 0x00, 0xC0, 0x40, 0x00, 0x00, 0xC0, 0x40, 0x20, 0x00, 0xC0, 0x40, 0x40, 0x00, 0xC0, 0x40, 0x60, 0x00, 0xC0, 0x40, 0x80, 0x00, 0xC0, 0x40, 0xA0, 0x00, 0xC0, 0x40, 0xC0, 0x00, 0xC0, 0x40, 0xE0, 0x00, 0xC0, 0x60, 0x00, 0x00, 0xC0, 0x60, 0x20, 0x00, 0xC0, 0x60, 0x40, 0x00, 0xC0, 0x60, 0x60, 0x00, 0xC0, 0x60, 0x80, 0x00, 0xC0, 0x60, 0xA0, 0x00, 0xC0, 0x60, 0xC0, 0x00, 0xC0, 0x60, 0xE0, 0x00, 0xC0, 0x80, 0x00, 0x00, 0xC0, 0x80, 0x20, 0x00, 0xC0, 0x80, 0x40, 0x00, 0xC0, 0x80, 0x60, 0x00, 0xC0, 0x80, 0x80, 0x00, 0xC0, 0x80, 0xA0, 0x00, 0xC0, 0x80, 0xC0, 0x00, 0xC0, 0x80, 0xE0, 0x00, 0xC0, 0xA0, 0x00, 0x00, 0xC0, 0xA0, 0x20, 0x00, 0xC0, 0xA0, 0x40, 0x00, 0xC0, 0xA0, 0x60, 0x00, 0xC0, 0xA0, 0x80, 0x00, 0xC0, 0xA0, 0xA0, 0x00, 0xC0, 0xA0, 0xC0, 0x00, 0xC0, 0xA0, 0xE0, 0x00, 0xC0, 0xC0, 0x00, 0x00, 0xC0, 0xC0, 0x20, 0x00, 0xC0, 0xC0, 0x40, 0x00, 0xC0, 0xC0, 0x60, 0x00, 0xC0, 0xC0, 0x80, 0x00, 0xC0, 0xC0, 0xA0, 0x00, 0xF0, 0xFB, 0xFF, 0x00, 0xA4, 0xA0, 0xA0, 0x00, 0x80, 0x80, 0x80, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0xFF, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0x00 };
        //private static byte[] BMP_Header = { 0x42, 0x4D, 0x36, 0x04, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x36, 0x04, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x01, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x80, 0x80, 0x00, 0x80, 0x00, 0x00, 0x00, 0x80, 0x00, 0x80, 0x00, 0x80, 0x80, 0x00, 0x00, 0xC0, 0xC0, 0xC0, 0x00, 0xC0, 0xDC, 0xC0, 0x00, 0xF0, 0xCA, 0xA6, 0x00, 0x00, 0x20, 0x40, 0x00, 0x00, 0x20, 0x60, 0x00, 0x00, 0x20, 0x80, 0x00, 0x00, 0x20, 0xA0, 0x00, 0x00, 0x20, 0xC0, 0x00, 0x00, 0x20, 0xE0, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x40, 0x20, 0x00, 0x00, 0x40, 0x40, 0x00, 0x00, 0x40, 0x60, 0x00, 0x00, 0x40, 0x80, 0x00, 0x00, 0x40, 0xA0, 0x00, 0x00, 0x40, 0xC0, 0x00, 0x00, 0x40, 0xE0, 0x00, 0x00, 0x60, 0x00, 0x00, 0x00, 0x60, 0x20, 0x00, 0x00, 0x60, 0x40, 0x00, 0x00, 0x60, 0x60, 0x00, 0x00, 0x60, 0x80, 0x00, 0x00, 0x60, 0xA0, 0x00, 0x00, 0x60, 0xC0, 0x00, 0x00, 0x60, 0xE0, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x80, 0x20, 0x00, 0x00, 0x80, 0x40, 0x00, 0x00, 0x80, 0x60, 0x00, 0x00, 0x80, 0x80, 0x00, 0x00, 0x80, 0xA0, 0x00, 0x00, 0x80, 0xC0, 0x00, 0x00, 0x80, 0xE0, 0x00, 0x00, 0xA0, 0x00, 0x00, 0x00, 0xA0, 0x20, 0x00, 0x00, 0xA0, 0x40, 0x00, 0x00, 0xA0, 0x60, 0x00, 0x00, 0xA0, 0x80, 0x00, 0x00, 0xA0, 0xA0, 0x00, 0x00, 0xA0, 0xC0, 0x00, 0x00, 0xA0, 0xE0, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0xC0, 0x20, 0x00, 0x00, 0xC0, 0x40, 0x00, 0x00, 0xC0, 0x60, 0x00, 0x00, 0xC0, 0x80, 0x00, 0x00, 0xC0, 0xA0, 0x00, 0x00, 0xC0, 0xC0, 0x00, 0x00, 0xC0, 0xE0, 0x00, 0x00, 0xE0, 0x00, 0x00, 0x00, 0xE0, 0x20, 0x00, 0x00, 0xE0, 0x40, 0x00, 0x00, 0xE0, 0x60, 0x00, 0x00, 0xE0, 0x80, 0x00, 0x00, 0xE0, 0xA0, 0x00, 0x00, 0xE0, 0xC0, 0x00, 0x00, 0xE0, 0xE0, 0x00, 0x40, 0x00, 0x00, 0x00, 0x40, 0x00, 0x20, 0x00, 0x40, 0x00, 0x40, 0x00, 0x40, 0x00, 0x60, 0x00, 0x40, 0x00, 0x80, 0x00, 0x40, 0x00, 0xA0, 0x00, 0x40, 0x00, 0xC0, 0x00, 0x40, 0x00, 0xE0, 0x00, 0x40, 0x20, 0x00, 0x00, 0x40, 0x20, 0x20, 0x00, 0x40, 0x20, 0x40, 0x00, 0x40, 0x20, 0x60, 0x00, 0x40, 0x20, 0x80, 0x00, 0x40, 0x20, 0xA0, 0x00, 0x40, 0x20, 0xC0, 0x00, 0x40, 0x20, 0xE0, 0x00, 0x40, 0x40, 0x00, 0x00, 0x40, 0x40, 0x20, 0x00, 0x40, 0x40, 0x40, 0x00, 0x40, 0x40, 0x60, 0x00, 0x40, 0x40, 0x80, 0x00, 0x40, 0x40, 0xA0, 0x00, 0x40, 0x40, 0xC0, 0x00, 0x40, 0x40, 0xE0, 0x00, 0x40, 0x60, 0x00, 0x00, 0x40, 0x60, 0x20, 0x00, 0x40, 0x60, 0x40, 0x00, 0x40, 0x60, 0x60, 0x00, 0x40, 0x60, 0x80, 0x00, 0x40, 0x60, 0xA0, 0x00, 0x40, 0x60, 0xC0, 0x00, 0x40, 0x60, 0xE0, 0x00, 0x40, 0x80, 0x00, 0x00, 0x40, 0x80, 0x20, 0x00, 0x40, 0x80, 0x40, 0x00, 0x40, 0x80, 0x60, 0x00, 0x40, 0x80, 0x80, 0x00, 0x40, 0x80, 0xA0, 0x00, 0x40, 0x80, 0xC0, 0x00, 0x40, 0x80, 0xE0, 0x00, 0x40, 0xA0, 0x00, 0x00, 0x40, 0xA0, 0x20, 0x00, 0x40, 0xA0, 0x40, 0x00, 0x40, 0xA0, 0x60, 0x00, 0x40, 0xA0, 0x80, 0x00, 0x40, 0xA0, 0xA0, 0x00, 0x40, 0xA0, 0xC0, 0x00, 0x40, 0xA0, 0xE0, 0x00, 0x40, 0xC0, 0x00, 0x00, 0x40, 0xC0, 0x20, 0x00, 0x40, 0xC0, 0x40, 0x00, 0x40, 0xC0, 0x60, 0x00, 0x40, 0xC0, 0x80, 0x00, 0x40, 0xC0, 0xA0, 0x00, 0x40, 0xC0, 0xC0, 0x00, 0x40, 0xC0, 0xE0, 0x00, 0x40, 0xE0, 0x00, 0x00, 0x40, 0xE0, 0x20, 0x00, 0x40, 0xE0, 0x40, 0x00, 0x40, 0xE0, 0x60, 0x00, 0x40, 0xE0, 0x80, 0x00, 0x40, 0xE0, 0xA0, 0x00, 0x40, 0xE0, 0xC0, 0x00, 0x40, 0xE0, 0xE0, 0x00, 0x80, 0x00, 0x00, 0x00, 0x80, 0x00, 0x20, 0x00, 0x80, 0x00, 0x40, 0x00, 0x80, 0x00, 0x60, 0x00, 0x80, 0x00, 0x80, 0x00, 0x80, 0x00, 0xA0, 0x00, 0x80, 0x00, 0xC0, 0x00, 0x80, 0x00, 0xE0, 0x00, 0x80, 0x20, 0x00, 0x00, 0x80, 0x20, 0x20, 0x00, 0x80, 0x20, 0x40, 0x00, 0x80, 0x20, 0x60, 0x00, 0x80, 0x20, 0x80, 0x00, 0x80, 0x20, 0xA0, 0x00, 0x80, 0x20, 0xC0, 0x00, 0x80, 0x20, 0xE0, 0x00, 0x80, 0x40, 0x00, 0x00, 0x80, 0x40, 0x20, 0x00, 0x80, 0x40, 0x40, 0x00, 0x80, 0x40, 0x60, 0x00, 0x80, 0x40, 0x80, 0x00, 0x80, 0x40, 0xA0, 0x00, 0x80, 0x40, 0xC0, 0x00, 0x80, 0x40, 0xE0, 0x00, 0x80, 0x60, 0x00, 0x00, 0x80, 0x60, 0x20, 0x00, 0x80, 0x60, 0x40, 0x00, 0x80, 0x60, 0x60, 0x00, 0x80, 0x60, 0x80, 0x00, 0x80, 0x60, 0xA0, 0x00, 0x80, 0x60, 0xC0, 0x00, 0x80, 0x60, 0xE0, 0x00, 0x80, 0x80, 0x00, 0x00, 0x80, 0x80, 0x20, 0x00, 0x80, 0x80, 0x40, 0x00, 0x80, 0x80, 0x60, 0x00, 0x80, 0x80, 0x80, 0x00, 0x80, 0x80, 0xA0, 0x00, 0x80, 0x80, 0xC0, 0x00, 0x80, 0x80, 0xE0, 0x00, 0x80, 0xA0, 0x00, 0x00, 0x80, 0xA0, 0x20, 0x00, 0x80, 0xA0, 0x40, 0x00, 0x80, 0xA0, 0x60, 0x00, 0x80, 0xA0, 0x80, 0x00, 0x80, 0xA0, 0xA0, 0x00, 0x80, 0xA0, 0xC0, 0x00, 0x80, 0xA0, 0xE0, 0x00, 0x80, 0xC0, 0x00, 0x00, 0x80, 0xC0, 0x20, 0x00, 0x80, 0xC0, 0x40, 0x00, 0x80, 0xC0, 0x60, 0x00, 0x80, 0xC0, 0x80, 0x00, 0x80, 0xC0, 0xA0, 0x00, 0x80, 0xC0, 0xC0, 0x00, 0x80, 0xC0, 0xE0, 0x00, 0x80, 0xE0, 0x00, 0x00, 0x80, 0xE0, 0x20, 0x00, 0x80, 0xE0, 0x40, 0x00, 0x80, 0xE0, 0x60, 0x00, 0x80, 0xE0, 0x80, 0x00, 0x80, 0xE0, 0xA0, 0x00, 0x80, 0xE0, 0xC0, 0x00, 0x80, 0xE0, 0xE0, 0x00, 0xC0, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x20, 0x00, 0xC0, 0x00, 0x40, 0x00, 0xC0, 0x00, 0x60, 0x00, 0xC0, 0x00, 0x80, 0x00, 0xC0, 0x00, 0xA0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xC0, 0x00, 0xE0, 0x00, 0xC0, 0x20, 0x00, 0x00, 0xC0, 0x20, 0x20, 0x00, 0xC0, 0x20, 0x40, 0x00, 0xC0, 0x20, 0x60, 0x00, 0xC0, 0x20, 0x80, 0x00, 0xC0, 0x20, 0xA0, 0x00, 0xC0, 0x20, 0xC0, 0x00, 0xC0, 0x20, 0xE0, 0x00, 0xC0, 0x40, 0x00, 0x00, 0xC0, 0x40, 0x20, 0x00, 0xC0, 0x40, 0x40, 0x00, 0xC0, 0x40, 0x60, 0x00, 0xC0, 0x40, 0x80, 0x00, 0xC0, 0x40, 0xA0, 0x00, 0xC0, 0x40, 0xC0, 0x00, 0xC0, 0x40, 0xE0, 0x00, 0xC0, 0x60, 0x00, 0x00, 0xC0, 0x60, 0x20, 0x00, 0xC0, 0x60, 0x40, 0x00, 0xC0, 0x60, 0x60, 0x00, 0xC0, 0x60, 0x80, 0x00, 0xC0, 0x60, 0xA0, 0x00, 0xC0, 0x60, 0xC0, 0x00, 0xC0, 0x60, 0xE0, 0x00, 0xC0, 0x80, 0x00, 0x00, 0xC0, 0x80, 0x20, 0x00, 0xC0, 0x80, 0x40, 0x00, 0xC0, 0x80, 0x60, 0x00, 0xC0, 0x80, 0x80, 0x00, 0xC0, 0x80, 0xA0, 0x00, 0xC0, 0x80, 0xC0, 0x00, 0xC0, 0x80, 0xE0, 0x00, 0xC0, 0xA0, 0x00, 0x00, 0xC0, 0xA0, 0x20, 0x00, 0xC0, 0xA0, 0x40, 0x00, 0xC0, 0xA0, 0x60, 0x00, 0xC0, 0xA0, 0x80, 0x00, 0xC0, 0xA0, 0xA0, 0x00, 0xC0, 0xA0, 0xC0, 0x00, 0xC0, 0xA0, 0xE0, 0x00, 0xC0, 0xC0, 0x00, 0x00, 0xC0, 0xC0, 0x20, 0x00, 0xC0, 0xC0, 0x40, 0x00, 0xC0, 0xC0, 0x60, 0x00, 0xC0, 0xC0, 0x80, 0x00, 0xC0, 0xC0, 0xA0, 0x00, 0xF0, 0xFB, 0xFF, 0x00, 0xA4, 0xA0, 0xA0, 0x00, 0x80, 0x80, 0x80, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0xFF, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0x00 };
        //private static byte[] BMP_Header = { 0x42, 0x4D, 0x36, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x36, 0x00, 0x00, 0x00, 0x28, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x01, 0x00, 0x18, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        // list is a byte list. this is used so i can append byes to the header. 
        // as an array is fixed size once initiated. meaning we cant add to it. 
        private static List<byte> Byte_List = new List<byte>();

        // This is my custom bmp header. (UNUSED)
        // its generated in ConstructFileHeader()
        private static byte[] Header { get; set; }

        private static string File_Output { get; set; }
        private static string File_Input { get; set; }
        private static string Input_Directory { get; set; }

        /// <summary>
        /// Main entry point
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            try
            {
                Input_Directory = args[0];
            }
            catch
            {
                Console.WriteLine($"Input Directory not defined. Generating Random Image... ");
            }
            
            // No comand line args supplied. generate random image in working dir
            if (string.IsNullOrEmpty(Input_Directory))
            {
                byte[] bytes = ByteArray();
                File_Output = $@"{Environment.CurrentDirectory}\Random.bmp";

                // then append it to a file
                AppendFile(bytes);
            }
            else
            {
                // Directory of Files to convert
                DirectoryInfo dir = new DirectoryInfo(Input_Directory);

                // create output directory
                if (!Directory.Exists($@"{dir.FullName}\output"))
                    Directory.CreateDirectory($@"{dir.FullName}\output");

                // Iterate over each file
                foreach (var item in dir.EnumerateFiles())
                {
                    Console.Write($"Processing: {item.Name}\t\t\t\t\t\r");

                    // clear out list
                    Byte_List.Clear();

                    // Generate the custom header.
                    // this isnt fully working yet
                    //ConstructFileHeader();

                    File_Output = $@"{dir.FullName}\output\{item.Name.Substring(0, item.Name.IndexOf("."))}.bmp";

                    // first lets generate the byte array
                    byte[] bytes = ByteArray(item.FullName);

                    // then append it to a file
                    AppendFile(bytes);
                }
            }
            
        }

        /// <summary>
        /// This is not yet fully implimented
        /// Im essentially creating my own file header.
        /// however the size and offset i cant get right yet
        /// so using the static header for now
        /// </summary>
        private static void ConstructFileHeader()
        {
            Header = new byte[54];

            #region BMP Header
            // ID field (0x42 = B, 0x4D = M)
            Header[0] = 0x42;   // 00
            Header[1] = 0x4D;   // 01

            // the size of the BMP file in bytes
            Header[2] = 0x46;   // 02
            Header[3] = 0x00;   // 03
            Header[4] = 0x00;   // 04
            Header[5] = 0x00;   // 05

            // Reserved; actual value depends on the application that creates the image
            Header[6] = 0x00;   // 06
            Header[7] = 0x00;   // 07
            Header[8] = 0x00;   // 08
            Header[9] = 0x00;   // 09

            // The offset, i.e.starting address, of the byte where the bitmap image data(pixel array) can be found.
            Header[10] = 0x36;  // 0A
            Header[11] = 0x00;  // 0B
            Header[12] = 0x00;  // 0C
            Header[13] = 0x00;  // 0D
            #endregion

            #region DIB Header
            // Number of bytes in the DIB header (from this point)
            Header[14] = 0x28;  // 0E
            Header[15] = 0x00;  // 0F
            Header[16] = 0x00;  // 10
            Header[17] = 0x00;  // 11

            // Width of the bitmap in pixels
            Header[18] = 0xFF;  // 12
            Header[19] = 0x02;  // 13        
            Header[20] = 0x00;  // 14
            Header[21] = 0x00;  // 15

            // Height of the bitmap in pixels. Positive for bottom to top pixel order
            Header[22] = 0xFF;  // 16
            Header[23] = 0x02;  // 17
            Header[24] = 0x00;  // 18  
            Header[25] = 0x00;  // 19

            // Number of color planes being used
            Header[26] = 0x01;  // 1A     
            Header[27] = 0x00;  // 1B

            // Number of bits per pixel
            Header[28] = 0x18;  // 1C
            Header[29] = 0x00;  // 1D

            // BI_RGB, no pixel array compression used
            Header[30] = 0x00;  // 1E
            Header[31] = 0x00;  // 1F
            Header[32] = 0x00;  // 20
            Header[33] = 0x00;  // 21

            // Size of the raw bitmap data (including padding)
            Header[34] = 0x10;  // 22
            Header[35] = 0x00;  // 23
            Header[36] = 0x00;  // 24
            Header[37] = 0x00;  // 25

            #region Print resolution of the image
            Header[38] = 0x13;  // 26
            Header[39] = 0x0B;  // 27
            Header[40] = 0x00;  // 28
            Header[41] = 0x00;  // 29

            Header[42] = 0x13;  // 2A
            Header[43] = 0x0B;  // 2B
            Header[44] = 0x00;  // 2C
            Header[45] = 0x00;  // 2D
            #endregion

            // 	Number of colors in the palette
            Header[46] = 0x00;  // 2E
            Header[47] = 0x00;  // 2F
            Header[48] = 0x00;  // 30
            Header[49] = 0x00;  // 31

            // 0 means all colors are important
            Header[50] = 0x00;  // 32
            Header[51] = 0x00;  // 33
            Header[52] = 0x00;  // 34
            Header[53] = 0x00;  // 35
            #endregion

            #region 2x2 Test Pixels
            //Header[54] = 0x00;  // 36
            //Header[55] = 0x00;  // 37
            //Header[56] = 0xFF;  // 38

            //Header[57] = 0xFF;  // 39
            //Header[58] = 0xFF;  // 3A
            //Header[59] = 0xFF;  // 3B

            //Header[60] = 0x00;  // 3C
            //Header[61] = 0x00;  // 3D

            //Header[62] = 0xFF;  // 3E
            //Header[63] = 0x00;  // 3F
            //Header[64] = 0x00;  // 40

            //Header[65] = 0x00;  // 41
            //Header[66] = 0xFF;  // 42
            //Header[67] = 0x00;  // 43

            //Header[68] = 0x00;  // 3C
            //Header[69] = 0x00;  // 3D
            #endregion
        }

        private static void AppendFile(byte[] bytes)
        {
            // delete the file
            File.Delete(File_Output);

            // append our bytes
            File.WriteAllBytes(File_Output, bytes);
        }

        private static byte[] ByteArray(string file = null)
        {
            // first add teh header to the byte list.
            foreach (var byt in BMP_Header)
            {
                Byte_List.Add(byt);
            }

            // We have not specifed an input, generate random data
            if (string.IsNullOrEmpty(file))
            {
                // now lets append some random data to the byte list
                // The strange maths to cound the for length, is to enure we generate enough bytes to foffill the headers size allocation.
                // bsically, if i make a picture thats 720p i need to ensure the bare minimun bytes are populated. otherwise the file is incomplete
                for (int i = 0; i < 36 * (8 * 1024); i++)
                {
                    byte[] random = GenerateBytes();

                    for (int j = 0; j < random.Length; j++)
                    {
                        Byte_List.Add(GenerateBytes()[j]);
                    }
                }
            }
            // Read the bytes in the file, and append them to the list
            else
            {
                byte[] bytes = File.ReadAllBytes(file);
                for (int i = 0; i < bytes.Length; i++)
                {
                    Byte_List.Add(bytes[i]);

                    // Every third byte we need to append 2 empty bytes
                    // this is data about the pixel
                    if ((i % 5) == 0)
                    {
                        //Byte_List.Add(0x00);
                        //Byte_List.Add(0x00);
                    }
                }
            }
           
            // cool our list is full... now return it as an array to be appened to a file
            return Byte_List.ToArray();
        }

        /// <summary>
        /// This will generate a byte array with 3 random bytes.
        /// </summary>
        /// <returns></returns>
        private static byte[] GenerateBytes()
        {
            // first create a random object.
            // it just spits out random data
            Random rnd = new Random();

            // next we cpopulate the byte array wilth random data.
            byte[] bytes = new byte[3];
            rnd.NextBytes(bytes);

            // return the byte array ready to be added to the list
            return bytes;
        }
    }
}
