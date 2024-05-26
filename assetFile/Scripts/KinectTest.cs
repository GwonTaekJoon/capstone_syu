using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Azure Kinect SDK
using Microsoft.Azure.Kinect.Sensor;

public class KinectTest : MonoBehaviour
{
    // Kinect 변수
    Device kinect;

    Texture2D kinectColorTexture;

    [SerializeField]
    UnityEngine.UI.RawImage rawColorImage;

    private void Start()
    {
        InitKinect();
    }

    public void InitKinect()
    {
        kinect = Device.Open(0);

        kinect.StartCameras(new DeviceConfiguration
        {
            ColorFormat = ImageFormat.ColorBGRA32,
            ColorResolution = ColorResolution.R720p,
            DepthMode = DepthMode.NFOV_2x2Binned,
            SynchronizedImagesOnly = true,
            CameraFPS = FPS.FPS30
        });

        int width = kinect.GetCalibration().ColorCameraCalibration.ResolutionWidth;
        int height = kinect.GetCalibration().ColorCameraCalibration.ResolutionHeight;
    
        // 컬러 텍스쳐 생성
        kinectColorTexture = new Texture2D(width, height);
    }

    void Update()
    {
        Capture capture = kinect.GetCapture();

        // 컬러 이미지를 얻어옴
        Image colorImg = capture.Color;

        // 픽셀값을 얻어옴
        Color32[] pixels = colorImg.GetPixels<Color32>().ToArray();

        // BGRA 데이터를 RGBA로 변환
        for (int i = 0; i < pixels.Length; i++)
        {
            Color32 pixel = pixels[i];
            pixels[i] = new Color32(pixel.b, pixel.g, pixel.r, pixel.a);
        }

        // 픽셀값 적용
        kinectColorTexture.SetPixels32(pixels);
        kinectColorTexture.Apply();

        // 컬러 텍스쳐 적용
        rawColorImage.texture = kinectColorTexture;
    }

    private void OnDestroy() 
    {
        kinect.StopCameras();
        kinect.Dispose();
    }
}
