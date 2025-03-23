using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks.Triggers;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace YARG
{
    public enum JudgementMode
    {
        Disabled,
        Judge,
        TimingOnly,
    }
    public class JudgementText : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _judgementTopText;
        [SerializeField]
        private TextMeshProUGUI _judgementBottomText;
        [SerializeField]
        private TextMeshProUGUI _judgementMidText;
        [SerializeField]
        private CanvasGroup     _judgementCanvasGroup;
        
        public float displayTime = 1.0f;
        private float _displayTime;
        public float fadeTime = 0.5f;
        private float _fadeTime;
        public string[] judgeLabels;
        private string defaultPerfect = "<color=green>PERFECT</color>";
        public string overhitText = "<color=red>OVER</color>";
        public string missText = "<color=blue>MISS</color>";
        public string earlyText = "<color=blue>EARLY</color>";
        public string lateText = "<color=red>LATE</color>";
        public float truePerfectRatio = 0.05f;
        
        // hit window in ms, default YARG engine is 140ms between early/late so the window is 140/2
        public int hitWindowSize = 70; 
        
        void Start()
        {
            if (judgeLabels.Length == 0)
            {
                judgeLabels.Append(defaultPerfect);
            }
            _displayTime = displayTime;
            _fadeTime = fadeTime;   
        }
        void Update()
        {
            
            if (_displayTime < displayTime * 0.9)
            {
                _fadeTime = _displayTime / 3;
                _judgementCanvasGroup.alpha = _fadeTime;
                double scaleVal = math.max(_displayTime * 2,.8);
                _judgementMidText.transform.localScale = Vector3.one * (float)scaleVal;
            } else {
                _judgementMidText.transform.localScale = Vector3.one;
            }
            _displayTime -= 1 * Time.deltaTime;
            if ( _displayTime < 0 )
            {
                _displayTime = 0;
                this.gameObject.SetActive(false);
            }
            
     
        }

        public void Ping(bool wasOverhit = false, float timingRatio = 0.0f) // positive means further away from zero ms
        {   
            _displayTime = displayTime;
            _fadeTime = fadeTime;
            _judgementCanvasGroup.alpha = 1;

            float timeVal = math.abs(timingRatio);
            float timeSig = math.sign(timingRatio);

            bool wasTruePerfect = timeVal <= truePerfectRatio;

            int grade;
            
            if (wasTruePerfect) {
                grade = 0;
            } else {
                grade = (int)(timeVal * judgeLabels.Length);
            }

            if (timeVal <= 1.0f) 
            {   
                _judgementMidText.text = judgeLabels[grade];
            } else {
                _judgementMidText.text = missText;
            }

            if (wasTruePerfect || timeVal > 1.0f)          
            { 
                _judgementBottomText.text = "";
                _judgementTopText.text = "";
            } else {
                if (timeSig < 0) {
                _judgementBottomText.text = earlyText;
                } else {
                _judgementBottomText.text = lateText;
                }
                _judgementTopText.text = ((int)(timeVal * timeSig * 100)).ToString();
            }

            if (wasOverhit)
            {
                _judgementMidText.text = overhitText;
                _judgementBottomText.text = "OOF";
                _judgementTopText.text = "";
            }

            this.gameObject.SetActive(true);
        }
    }

    
}
