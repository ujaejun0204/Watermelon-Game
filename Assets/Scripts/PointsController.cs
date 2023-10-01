using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using Watermelon_Game.Skills;

namespace Watermelon_Game
{
    internal sealed class PointsController : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] private TextMeshProUGUI points;
        [SerializeField] private TextMeshProUGUI multiplier;
        [SerializeField] private float multiplierDuration = 1f;
        [SerializeField] private float multiplierWaitTime = .1f;
        [SerializeField] private float pointsWaitTime = .05f;
        [SerializeField] private List<Color> multiplierColors;
        #endregion
        
        #region Fields
        private SpriteRenderer multiplierBackground;
        private uint currentPoints;
        private uint pointsDelta;
        private uint currentMultiplier;
        private float currentMultiplierDuration;

        [CanBeNull] private IEnumerator multiplierCoroutine;
        [CanBeNull] private IEnumerator pointsCoroutine;
        private WaitForSeconds multiplierWaitForSeconds;
        private WaitForSeconds pointsWaitForSeconds;
        #endregion

        #region Properties
        public static PointsController Instance { get; private set; }
        #endregion

        #region Methods
        private void Awake()
        {
            Instance = this;

            this.multiplierBackground = this.multiplier.GetComponentInChildren<SpriteRenderer>();
            this.multiplierWaitForSeconds = new WaitForSeconds(this.multiplierWaitTime);
            this.pointsWaitForSeconds = new WaitForSeconds(this.pointsWaitTime);
        }

        public void AddPoints(Fruit.Fruit _Fruit)
        {
            this.currentMultiplierDuration = this.multiplierDuration;
            
            var _multiplier = this.currentMultiplier + 1;
            this.SetMultiplier(_multiplier);

            if (this.multiplierCoroutine == null)
            {
                this.multiplierCoroutine = MultiplierDuration();
                this.StartCoroutine(this.multiplierCoroutine);
            }
            
            var _points = (int)((int)_Fruit + this.currentMultiplier);
            this.SetPoints(_points);
        }

        public void SubtractPoints(uint _PointsToSubtract) 
        {
            //TODO
            //var _points = (uint)Mathf.Clamp(this.currentPoints - _PointsToSubtract, 0, uint.MaxValue);
            this.SetPoints(-(int)_PointsToSubtract);
        }
        
        private IEnumerator MultiplierDuration()
        {
            while (this.currentMultiplierDuration > 0)
            {
                yield return this.multiplierWaitForSeconds;
                this.currentMultiplierDuration -= multiplierWaitTime;
            }
            
            this.SetMultiplier(0);
            this.StopCoroutine(this.multiplierCoroutine);
            this.multiplierCoroutine = null;
            
            this.multiplier.gameObject.SetActive(false);
        }
        
        private void SetMultiplier(uint _CurrentMultiplier)
        {
            this.currentMultiplier = _CurrentMultiplier;
            this.multiplier.text = string.Concat("x", this.currentMultiplier);
            this.multiplierBackground.color = this.GetMultiplierColor(_CurrentMultiplier);
            this.multiplier.gameObject.SetActive(true);
        }

        private Color GetMultiplierColor(uint _CurrentMultiplier)
        {
            if (_CurrentMultiplier == 0)
            {
                return Color.white;
            }
            if (_CurrentMultiplier > this.multiplierColors.Count)
            {
                return this.multiplierColors[^1];
            }

            return this.multiplierColors[(int)_CurrentMultiplier - 1];
        }
        
        private void SetPoints(int _Points)
        {
            this.currentPoints = (uint)Mathf.Clamp(this.currentPoints + _Points, 0, uint.MaxValue);

            if (this.pointsCoroutine == null)
            {
                this.pointsCoroutine = SetPoints();
                StartCoroutine(this.pointsCoroutine);
            }
            
            SkillController.Instance.PointsChanged(this.currentPoints);
        }

        /// <summary>
        /// Gradually increase/decreases the points over time
        /// </summary>
        /// <returns></returns>
        private IEnumerator SetPoints()
        {
            while (this.pointsDelta != this.currentPoints)
            {
                if (this.pointsDelta < this.currentPoints)
                {
                    this.pointsDelta++;
                }
                else if (this.pointsDelta > this.currentPoints)
                {
                    this.pointsDelta--;
                }
                
                this.points.text = string.Concat(this.pointsDelta, "P");
                
                yield return this.pointsWaitForSeconds;
            }
            
            StopCoroutine(this.pointsCoroutine);
            this.pointsCoroutine = null;
        }
        #endregion
    }
}