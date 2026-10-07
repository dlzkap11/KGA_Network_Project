using UnityEngine;
using UnityEngine.UI;
using TMPro;

// DelaySlider / DelayLabel / InterpolationToggle을
// FakeNetworkChannel과 RemoteCubeReceiver에 배선하는 UI 스크립트.
public class LatencyLabUI : MonoBehaviour
{
    public FakeNetworkChannel channel;
    public RemoteCubeReceiver receiver;

    public Slider delaySlider;
    public TMP_Text delayLabel;
    public Toggle interpolationToggle;

    private void Start()
    {
        // 슬라이더/토글의 현재 값으로 초기화
        if (delaySlider != null)
        {
            delaySlider.onValueChanged.AddListener(OnDelayChanged);
            OnDelayChanged(delaySlider.value);
        }

        if (interpolationToggle != null)
        {
            interpolationToggle.onValueChanged.AddListener(OnInterpolationChanged);
            OnInterpolationChanged(interpolationToggle.isOn);
        }
    }

    private void OnDelayChanged(float value)
    {
        if (channel != null)
        {
            channel.delaySeconds = value;
        }

        if (delayLabel != null)
        {
            delayLabel.text = $"Delay: {value:F2}s";
        }
    }

    private void OnInterpolationChanged(bool isOn)
    {
        if (receiver != null)
        {
            receiver.useInterpolation = isOn;
        }
    }
}