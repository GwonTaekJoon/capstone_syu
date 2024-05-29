using UnityEngine;
using UnityEngine.UI;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.BodyTracking;
using System.Threading.Tasks;
using System.Collections;

public class LungeBodyTracker : MonoBehaviour
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
    GameObject ChestSpine; // Spine Chest 관절을 나타내는 게임 오브젝트

    [SerializeField]
    private Text countText; // UI Text 요소를 참조하는 변수

    [SerializeField]
    private Text SetInitialPosText; // UI Text 요소를 참조하는 변수

    [SerializeField]
    private UnityEngine.UI.Image countImage; // UI Image 요소를 참조하는 변수

    [SerializeField]
    private UnityEngine.UI.Image handBelowSpineImage; // 추가할 UI Image 요소를 참조하는 변수

    [SerializeField]
    private UnityEngine.UI.Image handUpSpineImage; // 추가할 UI Image 요소를 참조하는 변수

    [SerializeField]
    private Text toastMsg; // Toast 메시지를 표시하는 UI Text 요소
    public Text OuputTimer; // 타이머를 출력할 토스트 메시지
    public RawImage rawImage; // 타이머가 0이 되었을 때 표시할 RawImage
    public Text TimerMsg; // 타이머가 0이 되었을 때 표시할 토스트 메시지
    public Text GoalMsg; // 타이머가 0이 되거나 목표 갯수를 채웠을 때 팝업창에 나올 goal 토스트 메시지
    public Text CountMsg; // 타이머가 0이 되거나 목표 갯수를 채웠을 때 팝업창에 나올 count 토스트 메시지
    public Text restTimeMsg; // 남은 타이머 시간을 표시할 토스트 메시지
    public Button button; // 타이머가 0이 되었을 때 활성화할 버튼

    private int timer; // 타이머 값

    public int count = 0; // 특정 조건을 만족할 때 증가하는 변수
    private int goal; // 목표 횟수

    // 각 관절의 초기 위치를 저장할 변수들
    private Vector3 initialPelvisPosition;
    private Vector3 initialRightKneePosition;
    private Vector3 initialLeftKneePosition;
    private Vector3 initialNosePosition;
    private Vector3 initialRightHandPosition;
    private Vector3 initialLeftHandPosition;
    private Vector3 initialRightShoulderPosition;
    private Vector3 initialLeftShoulderPosition;
    private Vector3 initialChestSpinePosition;

    private bool initialPositionsSet = false; // 초기 위치가 설정되었는지 확인하는 변수
    private bool pelvisBelowThreshold = false; // Pelvis가 아래로 내려갔는지 확인하는 변수

    private void Start()
    {
        goal = PlayerPrefs.GetInt("SetGoal", 12); // 목표 값을 불러옵니다. 값이 없으면 기본값 12를 설정합니다.
        timer = (goal * 5) + 1;
        
        // "Get ready" 메시지를 출력하고 3초 후에 숨깁니다.
        StartCoroutine(ShowAndHideReadyMessage());

        InitKinect(); // Kinect 초기화
        KinectLoop(); // Kinect 데이터 처리 루프 시작

    }

    public void Update()
    {
        SetColor(); // Kinect 컬러 이미지 설정
    }

    // "Get ready" 메시지를 출력하고 3초 뒤에 타이머를 시작하는 코루틴
    private IEnumerator ShowAndHideReadyMessage()
    {
        string[] countdownMessages = {"Get Ready!", "3", "2", "1", "!START!" };
        float[] waitTimes = {3f, 1f, 1f, 1f, 1f };

        // Outline 컴포넌트를 추가합니다.
        Outline outline = TimerMsg.GetComponent<Outline>();
        if (outline == null)
        {
            outline = TimerMsg.gameObject.AddComponent<Outline>();
            outline.effectColor = Color.black; // 테두리 색상
            outline.effectDistance = new Vector2(2, 2); // 테두리 두께
        }

        // Shadow 컴포넌트를 추가합니다.
        Shadow shadow = TimerMsg.GetComponent<Shadow>();
        if (shadow == null)
        {
            shadow = TimerMsg.gameObject.AddComponent<Shadow>();
            shadow.effectColor = Color.black; // 그림자 색상
            shadow.effectDistance = new Vector2(2, -2); // 그림자 위치
        }

        for (int i = 0; i < countdownMessages.Length; i++)
        {
            // 메시지를 출력합니다.
            if (TimerMsg != null)
            {
                TimerMsg.text = countdownMessages[i];
                TimerMsg.color = Color.green;
                TimerMsg.fontSize = 80;
                TimerMsg.fontStyle = FontStyle.Bold;
                TimerMsg.gameObject.SetActive(true);
            }

            // 대기합니다.
            yield return new WaitForSeconds(waitTimes[i]);

            // 메시지를 숨깁니다.
            if (TimerMsg != null)
            {
                TimerMsg.gameObject.SetActive(false);
            }
        }
        // 타이머 값을 매 초 감소시키기 시작합니다.
        StartCoroutine(DecreaseTimer());
    }

    // 매 초마다 타이머 값을 감소시키는 코루틴
    private IEnumerator DecreaseTimer()
    {
        while (timer > 0 || goal == count)
        {
            yield return new WaitForSeconds(1f); // 1초 대기
            timer--; // 타이머 값을 1 감소시킵니다.

            OuputTimer.text = "" + timer;
            OuputTimer.color = Color.black;
            OuputTimer.gameObject.SetActive(true);
        }

        // 타이머가 0에 도달했을 때 추가 작업을 수행합니다.
        if (rawImage != null)
        {
            rawImage.gameObject.SetActive(true); // RawImage 활성화
        }

        if (TimerMsg != null)
        {
            TimerMsg.text = "Timer end!";
            TimerMsg.color = Color.green;
            TimerMsg.gameObject.SetActive(true); // 토스트 메시지 활성화
        }

        if (GoalMsg != null)
        {
            GoalMsg.text = "Your goal : " + goal;
            GoalMsg.color = Color.black;
            GoalMsg.gameObject.SetActive(true); // 토스트 메시지 활성화
        }

        // 타이머가 0에 도달했을 때 추가 작업을 수행합니다.
        if (CountMsg != null)
        {
            CountMsg.text = "Your Count : " + count;
            CountMsg.color = Color.black;
            CountMsg.gameObject.SetActive(true); // 토스트 메시지 활성화
        }

        if (button != null)
        {
            button.gameObject.SetActive(true); // 버튼 활성화
        }
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
                            Debug.Log("Initial Positions Set");
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
                        this.SetMarkPos(this.ChestSpine, JointId.SpineChest, frame);
                    }
                    else
                    {
                        // 관절을 찾을 수 없을 시 초기 위치 값을 초기화
                        initialPositionsSet = false;
                        pelvisBelowThreshold = false;
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

    private IEnumerator ShowHandBelowSpineImage()
    {
        // 이미지 활성화
        handBelowSpineImage.gameObject.SetActive(true);

        // 0.5초 대기
        yield return new WaitForSeconds(0.5f);

        // 이미지 비활성화
        handBelowSpineImage.gameObject.SetActive(false);
    }

    private IEnumerator ShowHandUpSpineImage()
    {
        // 이미지 활성화
        handUpSpineImage.gameObject.SetActive(true);

        // 0.5초 대기
        yield return new WaitForSeconds(0.5f);

        // 이미지 비활성화
        handUpSpineImage.gameObject.SetActive(false);
    }

    private IEnumerator HideInitialPosText()
    {
        // 2초 대기
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

        // 모든 조건이 만족했을 때 count 증가
        if (initialPositionsSet)
        {
            Vector3 leftHandPos = GetJointPosition(JointId.HandLeft, frame);
            Vector3 rightHandPos = GetJointPosition(JointId.HandRight, frame);
            Vector3 chestSpinePos = GetJointPosition(JointId.SpineChest, frame);

            // Pelvis 관절의 Y축 위치가 초기 위치 기준으로 5 이상 내려갔다가 초기 위치로 돌아오고,
            // Left Hand와 Right Hand가 항상 Spine Chest 관절 위에 있어야 count 증가
            if (pos.y < initialPelvisPosition.y - 5 &&
                leftHandPos.y > chestSpinePos.y && rightHandPos.y > chestSpinePos.y)
            {
                pelvisBelowThreshold = true;
            }
            else if (pelvisBelowThreshold && pos.y >= initialPelvisPosition.y - 5)
            {
                pelvisBelowThreshold = false;

                // LeftKnee 또는 RightKnee의 X 위치가 초기 위치에서 2 이상 움직였다가 초기 위치로 돌아오면 count 증가
                if ((jointId == JointId.KneeLeft || jointId == JointId.KneeRight) && Mathf.Abs(pos.x - initialPelvisPosition.x) >= 2)
                {
                    count++;
                    Debug.Log("Count increased: " + count);
                    countText.text = "<color=black> Reps \n" + count + "</color>"; // count 변수를 텍스트로 변환하여 countText에 할당

                    // 코루틴 시작
                    StartCoroutine(ShowCountImage());

                    if (count == goal) 
                    {
                        if (rawImage != null)
                        {
                            rawImage.gameObject.SetActive(true); // RawImage 활성화
                        }

                        toastMsg.text = "Goal Reached!";
                        toastMsg.color = Color.green;
                        toastMsg.gameObject.SetActive(true); // 목표 도달 시 ToastMsg 표시
                        
                        if (GoalMsg != null)
                        {
                            GoalMsg.text = "Your goal : " + goal;
                            GoalMsg.color = Color.black;
                            GoalMsg.gameObject.SetActive(true); // 토스트 메시지 활성화
                        }

                        // 타이머가 0에 도달했을 때 추가 작업을 수행합니다.
                        if (CountMsg != null)
                        {
                            CountMsg.text = "Your Count : " + count;
                            CountMsg.color = Color.black;
                            CountMsg.gameObject.SetActive(true); // 토스트 메시지 활성화
                        }

                        if (restTimeMsg != null)
                        {
                            restTimeMsg.text = "Rest Time : " + timer;
                            restTimeMsg.color = Color.black;
                            restTimeMsg.gameObject.SetActive(true); // 토스트 메시지 활성화 
                        }

                        if (button != null)
                        {
                            button.gameObject.SetActive(true); // 버튼 활성화
                        }
                    }
                }
            }

            // 손 관절 위치에 따른 이미지 출력 변화
            if (leftHandPos.y <= chestSpinePos.y || rightHandPos.y <= chestSpinePos.y)
            {
                StartCoroutine(ShowHandBelowSpineImage());
            }
            else
            {
                StartCoroutine(ShowHandUpSpineImage());
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
        initialChestSpinePosition = GetJointPosition(JointId.SpineChest, frame);

        Debug.Log("Initial Positions Set");
        SetInitialPosText.text = "<color=blue>Initial Positions Set</color>";

        // 2초 후에 텍스트를 숨기는 Coroutine 시작
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
