Imports System.Runtime.InteropServices

''' <summary>
''' Helper class containing Gdi32 API functions
''' </summary>
Public Class GDI32

  ''' <summary>
  '''    Performs a bit-block transfer of the color data corresponding to a
  '''    rectangle of pixels from the specified source device context into
  '''    a destination device context.
  ''' </summary>
  ''' <param name="hdc">Handle to the destination device context.</param>
  ''' <param name="nXDest">The leftmost x-coordinate of the destination rectangle (in pixels).</param>
  ''' <param name="nYDest">The topmost y-coordinate of the destination rectangle (in pixels).</param>
  ''' <param name="nWidth">The width of the source and destination rectangles (in pixels).</param>
  ''' <param name="nHeight">The height of the source and the destination rectangles (in pixels).</param>
  ''' <param name="hdcSrc">Handle to the source device context.</param>
  ''' <param name="nXSrc">The leftmost x-coordinate of the source rectangle (in pixels).</param>
  ''' <param name="nYSrc">The topmost y-coordinate of the source rectangle (in pixels).</param>
  ''' <param name="dwRop">A raster-operation code.</param>
  ''' <returns>
  '''    <c>true</c> if the operation succeeded, <c>false</c> otherwise.
  ''' </returns>
  <DllImport("gdi32.dll")> _
  Public Shared Function BitBlt(ByVal hdc As IntPtr, ByVal nXDest As Integer, ByVal nYDest As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal hdcSrc As IntPtr, ByVal nXSrc As Integer, ByVal nYSrc As Integer, ByVal dwRop As TernaryRasterOperations) As Boolean
  End Function

  ''' <summary>
  '''     Specifies a raster-operation code. These codes define how the color data for the
  '''     source rectangle is to be combined with the color data for the destination
  '''     rectangle to achieve the final color.
  ''' </summary>
  Enum TernaryRasterOperations As UInteger
    ''' <summary>dest = source</summary>
    SRCCOPY = &HCC0020
    ''' <summary>dest = source OR dest</summary>
    SRCPAINT = &HEE0086
    ''' <summary>dest = source AND dest</summary>
    SRCAND = &H8800C6
    ''' <summary>dest = source XOR dest</summary>
    SRCINVERT = &H660046
    ''' <summary>dest = source AND (NOT dest)</summary>
    SRCERASE = &H440328
    ''' <summary>dest = (NOT source)</summary>
    NOTSRCCOPY = &H330008
    ''' <summary>dest = (NOT src) AND (NOT dest)</summary>
    NOTSRCERASE = &H1100A6
    ''' <summary>dest = (source AND pattern)</summary>
    MERGECOPY = &HC000CA
    ''' <summary>dest = (NOT source) OR dest</summary>
    MERGEPAINT = &HBB0226
    ''' <summary>dest = pattern</summary>
    PATCOPY = &HF00021
    ''' <summary>dest = DPSnoo</summary>
    PATPAINT = &HFB0A09
    ''' <summary>dest = pattern XOR dest</summary>
    PATINVERT = &H5A0049
    ''' <summary>dest = (NOT dest)</summary>
    DSTINVERT = &H550009
    ''' <summary>dest = BLACK</summary>
    BLACKNESS = &H42
    ''' <summary>dest = WHITE</summary>
    WHITENESS = &HFF0062
    ''' <summary>
    ''' Capture window as seen on screen.  This includes layered windows
    ''' such as WPF windows with AllowsTransparency="true"
    ''' </summary>
    CAPTUREBLT = &H40000000
  End Enum

  ' Public Const SRCCOPY As Integer = &HCC0020

  <DllImport("gdi32.dll", SetLastError:=True)> _
  Public Shared Function GetPixel(ByVal hdc As IntPtr, _
              ByVal nXPos As Integer, _
              ByVal nYPos As Integer) As UInteger
  End Function
  <DllImport("gdi32.dll", SetLastError:=True)> _
  Public Shared Function CreateDC(ByVal strDriver As String,
      ByVal strDevice As String, ByVal strOutput As String, ByVal pData As IntPtr) As IntPtr
  End Function

  <DllImport("gdi32.dll")> _
  Public Shared Function CreateCompatibleBitmap(ByVal hDC As IntPtr, ByVal nWidth As Integer, ByVal nHeight As Integer) As IntPtr
  End Function
  <DllImport("gdi32.dll")> _
  Public Shared Function CreateCompatibleDC(ByVal hDC As IntPtr) As IntPtr
  End Function
  <DllImport("gdi32.dll")> _
  Public Shared Function DeleteDC(ByVal hDC As IntPtr) As Boolean
  End Function
  <DllImport("gdi32.dll")> _
  Public Shared Function DeleteObject(ByVal hObject As IntPtr) As Boolean
  End Function
  <DllImport("gdi32.dll")> _
  Public Shared Function SelectObject(ByVal hDC As IntPtr, ByVal hObject As IntPtr) As IntPtr
  End Function

  <DllImport("gdi32.dll")> _
  Public Shared Function CreateDIBSection( _
    ByVal hdc As Int32, _
    ByRef pbmi As BITMAPINFO, _
    ByVal iUsage As System.UInt32, _
    ByRef ppvBits As Int32, _
    ByVal hSection As Int32, _
    ByVal dwOffset As System.UInt32) As Int32
  End Function

  <StructLayout(LayoutKind.Sequential)> _
  Public Structure BITMAPINFO
    Dim bmiheader As BITMAPINFOHEADER
    Dim bmiColors As RGBQUAD
  End Structure

  <StructLayout(LayoutKind.Sequential)> _
  Public Class BITMAPINFOHEADER
    Public biSize As Int32
    Public biWidth As Int32
    Public biHeight As Int32
    Public biPlanes As Int16
    Public biBitCount As Int16
    Public biCompression As Int32
    Public biSizeImage As Int32
    Public biXPelsPerMeter As Int32
    Public biYPelsPerMeter As Int32
    Public biClrUsed As Int32
    Public biClrImportant As Int32
  End Class

  <StructLayout(LayoutKind.Sequential)> _
  Public Structure RGBQUAD
    Dim rgbBlue As Byte
    Dim rgbGreen As Byte
    Dim rgbRed As Byte
    Dim rgbReserved As Byte
  End Structure



  ' Declare the Windows API function CopyMemory
  ' Note that all variables are ByVal. pDst is passed ByVal because we want
  ' CopyMemory to go to that location and modify the data that is pointed to
  ' by the IntPtr, and not the pointer itself.
  Public Declare Sub CopyMemory Lib "kernel32" Alias "RtlMoveMemory" (ByVal pDst As IntPtr, _
                                                               ByVal pSrc As String, _
                                                               ByVal ByteLen As Long)

  Public Declare Function VarPtrArray Lib "msvbvm60.dll" Alias "VarPtr" (ByRef Ptr() As Byte) As Integer
End Class
