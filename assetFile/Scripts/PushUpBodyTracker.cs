using UnityEngine;
using UnityEngine.UI;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.BodyTracking;
using System.Threading.Tasks;
using System.Collections;

public class PushUpBodyTracker : MonoBehaviour
{
    Device kinect; // Kinect 디바이스를 참조하는 변수
    Texture2D kinectColorTexture; // Kinect의 컬러 이미지를 저장하는 텍스처

    [SerializeField]
    UnityEngine.UI.RawImage rawColorImg; // Unity UI RawImage를 참조하는 변수

    Tracker tracker; // Kinect Body Tracking을 위한 추적기

    [SerializeField]
    GameObject Pelvis; // Pelvis 관절을 나타내는 게임 오브젝트

    [SerializeField]
    GameObject rightKnee; // Right Knee 관절을 나타내는 게임 오브젝트

    [SerializeField]
    GameObject leftKnee; // Left Knee 관절을 나타내는 게임 오브젝트

    [SerializeField]
    GameObject Nose; // Nose 관절을 나타내는 게임 오브젝트

    [SerializeField]
    GameObject rightHand; // Right Hand 관절을 나타내는 게임 오브젝트

    [SerializeField]
    GameObject leftHand; // Left Hand 관절을 나타내는 게임 오브젝트

    [SerializeField]
    GameObject RightShoulder; // Right Shoulder 관절을 나타내는 게임 오브젝트

    [SerializeField]
    GameObject LeftShoulder; // Left Shoulder 관절을 나타내는 게임 오브젝트

    [SerializeField]
    private Text countText; // UI Text 요소를 참조하는 변수

    [SerializeField]
    private Text SetInitialPosText; // UI Text 요소를 참조하는 변수

    [SerializeField]
    private UnityEngine.UI.Image countImage; // UI Image 요소를 참조하는 변수

    private int count = 0; // 특정 조건을 만족할 때 증가하는 변수

    // 각 관절의 초기 위치를 저장할 변수들
    private Vector3 initialPelvisPosition;
    private Vector3 initialRightKneePosition;
    private Vector3 initialLeftKneePosition;
    private Vector3 initialNosePosition;
    private Vector3 initialRightHandPosition;
    private Vector3 initialLeftHandPosition;
    private Vector3 initialRightShoulderPosition;
    private Vector3 initialLeftShoulderPosition;

    private bool initialPositionsSet = false; // 초기 위치가 설정되었는지 확인하는 변수
    private bool noseBelowThreshold = false; // Nose가 아래로 내려갔는지 확인하는 변수

    private void Start()
    {
        InitKinect(); // Kinect 초기화
        KinectLoop(); // Kinect 데이터 처리 루프 시작
    }

    public void Update()
    {
        SetColor(); // Kinect 컬러 이미지 설정
    }

    public void InitKinect()
    {
        // Kinect 디바이스 열기
        kinect = Device.Open(0);

        // Kinect 카메라 시작
        kinect.StartCameras(new DeviceConfiguration
        {
            ColorFormat = ImageFormat.ColorBGRA32,
            ColorResolution = ColorResolution.R720p,
            DepthMode = DepthMode.NFOV_2x2Binned,
            SynchronizedImagesOnly = true,
            CameraFPS = FPS.FPS30
        });

        // 컬러 카메라의 해상도 가져오기
        int width = kinect.GetCalibration().ColorCameraCalibration.ResolutionWidth;
        int height = kinect.GetCalibration().ColorCameraCalibration.ResolutionHeight;

        // 컬러 텍스처 초기화
        kinectColorTexture = new Texture2D(width, height);

        // Body Tracking 추적기 생성
        tracker = Tracker.Create(kinect.GetCalibration(), new TrackerConfiguration { });
    }

    private async void KinectLoop()
    {
        while (true)
        {
            // 캡처 데이터 가져오기
            using (Capture capture = await Task.Run(() => this.kinect.GetCapture()).ConfigureAwait(true))
            {
                // 캡처 데이터를 추적기에 추가
                tracker.EnqueueCapture(capture);
                using (var frame = tracker.PopResult())
                {
                    // 하나 이상의 바디가 인식된 경우
                    if (frame.NumberOfBodies > 0)
                    {
                        // 초기 위치가 설정되지 않은 경우 초기 위치 설정
                        if (!initialPositionsSet)
                        {
                            SetInitialPositions(frame);
                            initialPositionsSet = true;
                        }

                        // 각 관절의 위치 설정
                        this.SetMarkPos(this.Pelvis, JointId.Pelvis, frame);
                        this.SetMarkPos(this.rightKnee, JointId.KneeRight, frame);
                        this.SetMarkPos(this.leftKnee, JointId.KneeLeft, frame);
                        this.SetMarkPos(this.Nose, JointId.Nose, frame);
                        this.SetMarkPos(this.rightHand, JointId.HandRight, frame);
                        this.SetMarkPos(this.leftHand, JointId.HandLeft, frame);
                        this.SetMarkPos(this.RightShoulder, JointId.ShoulderRight, frame);
                        this.SetMarkPos(this.LeftShoulder, JointId.ShoulderLeft, frame);
                    }
                    else
                    {
                        // 관절을 찾을 수 없을 시 초기 위치 값을 초기화
                        initialPositionsSet = false;
                        noseBelowThreshold = false;
                    }
                }
            }
        }
    }

    private void SetColor()
    {
        // 널 참조를 확인하여 Kinect이 초기화되지 않았는지 확인합니다.
        if (kinect == null)
        {
            Debug.LogWarning("Kinect이 초기화되지 않았습니다. 컬러 이미지를 가져올 수 없습니다.");
            return;
        }

        // 컬러 이미지를 캡처합니다.
        using (Capture capture = kinect.GetCapture())
        {
            // 캡처가 널인지 확인하여 사용할 수 있는지 확인합니다.
            if (capture == null)
            {
                Debug.LogWarning("캡처가 널입니다. 컬러 이미지를 가져올 수 없습니다.");
                return;
            }

            Microsoft.Azure.Kinect.Sensor.Image colorImg = capture.Color;

            // 컬러 이미지가 널인지 확인하여 사용할 수 있는지 확인합니다.
            if (colorImg == null)
            {
                Debug.LogWarning("컬러 이미지가 널입니다. 컬러 이미지를 가져올 수 없습니다.");
                return;
            }

            Color32[] pixels = colorImg.GetPixels<Color32>().ToArray();

            // 픽셀 배열이 널인지 확인하여 사용할 수 있는지 확인합니다.
            if (pixels == null)
            {
                Debug.LogWarning("픽셀 배열이 널입니다. 컬러 이미지를 가져올 수 없습니다.");
                return;
            }

            // BGRA32 데이터를 RGBA32로 변환합니다.
            for (int i = 0; i < pixels.Length; i++)
            {
                Color32 pixel = pixels[i];
                pixels[i] = new Color32(pixel.b, pixel.g, pixel.r, pixel.a);
            }

            // 컬러 텍스처를 설정합니다.
            kinectColorTexture.SetPixels32(pixels);
            kinectColorTexture.Apply();

            // rawColorImg가 널인지 확인하여 텍스처를 할당할 수 있는지 확인합니다.
            if (rawColorImg != null)
            {
                rawColorImg.texture = kinectColorTexture;
            }
            else
            {
                Debug.LogWarning("RawColorImg가 널입니다. 텍스처를 할당할 수 없습니다.");
            }
        }
    }

    private IEnumerator ShowCountImage()
    {
        // 이미지 활성화
        countImage.gameObject.SetActive(true);

        // 0.5초 대기
        yield return new WaitForSeconds(0.5f);

        // 이미지 비활성화
        countImage.gameObject.SetActive(false);
    }

    private IEnumerator HideInitialPosText()
    {
        // 1초 대기
        yield return new WaitForSeconds(2f);

        // 텍스트 숨기기
        SetInitialPosText.gameObject.SetActive(false);
    }


    private void SetMarkPos(GameObject effectPrefab, JointId jointId, Frame frame)
    {
        // 특정 관절의 위치를 계산하여 게임 오브젝트 위치 설정
        var joint = frame.GetBodySkeleton(0).GetJoint(jointId);
        var offset = 50;
        var pos = new Vector3(joint.Position.X / -offset, joint.Position.Y / -offset, joint.Position.Z / offset);

        effectPrefab.transform.localPosition = pos;

        // 초기 위치 기준으로 Nose 관절의 Y축 위치가 5 이상 내려갔다가 초기 위치로 돌아오면 count 변수 증가
        if (jointId == JointId.Nose && initialPositionsSet)
        {
            if (pos.y < initialNosePosition.y - 5)
            {
                noseBelowThreshold = true;
            }
            else if (noseBelowThreshold && pos.y >= initialNosePosition.y - 5)
            {
                count++;
                noseBelowThreshold = false;
                Debug.Log("Count increased: " + count);
                countText.text = "<color=black> Reps \n" + count + "</color>"; // count 변수를 텍스트로 변환하여 countText에 할당
            
                // 코루틴 시작
                StartCoroutine(ShowCountImage());
            }
        }
    }

    private void SetInitialPositions(Frame frame)
    {
        // 각 관절의 초기 위치 설정
        initialPelvisPosition = GetJointPosition(JointId.Pelvis, frame);
        initialRightKneePosition = GetJointPosition(JointId.KneeRight, frame);
        initialLeftKneePosition = GetJointPosition(JointId.KneeLeft, frame);
        initialNosePosition = GetJointPosition(JointId.Nose, frame);
        initialRightHandPosition = GetJointPosition(JointId.HandRight, frame);
        initialLeftHandPosition = GetJointPosition(JointId.HandLeft, frame);
        initialRightShoulderPosition = GetJointPosition(JointId.ShoulderRight, frame);
        initialLeftShoulderPosition = GetJointPosition(JointId.ShoulderLeft, frame);

        Debug.Log("Initial Positions Set");
        SetInitialPosText.text = "<color=blue>Initial Positions Set</color>";

        // 1초 후에 텍스트를 숨기는 Coroutine 시작
        StartCoroutine(HideInitialPosText());
    }

    private Vector3 GetJointPosition(JointId jointId, Frame frame)
    {
        // 특정 관절의 위치를 계산하여 반환
        var joint = frame.GetBodySkeleton(0).GetJoint(jointId);
        var offset = 50;
        return new Vector3(joint.Position.X / -offset, joint.Position.Y / -offset, joint.Position.Z / offset);
    }

    private void OnDestroy()
    {
        // Kinect 카메라 중지
        if (kinect != null)
        {
            kinect.StopCameras();
            kinect.Dispose(); // Kinect 디바이스 해제
        }
    }

}
