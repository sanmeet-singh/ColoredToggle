using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ColoredToggleUI
{
    /// <summary>
    /// A standard toggle that has an on / off state, with a twist that state is represented by colour.
    /// </summary>

    [RequireComponent(typeof(RectTransform))]
    public class ColoredToggle : Selectable, IPointerClickHandler, ISubmitHandler, ICanvasElement
    {
        public enum ToggleTransition
        {
            None,
            Fade
        }

        [Serializable]
        public class ToggleEvent : UnityEvent<bool>
        { }

        public Color onColor;
        public Color offColor;

        /// <summary>
        /// Transition type.
        /// </summary>
        public ToggleTransition toggleTransition = ToggleTransition.Fade;

        /// <summary>
        /// Graphic the toggle should be working with.
        /// </summary>
        public Graphic graphic;

        // group that this toggle can belong to
        [SerializeField]
        private ColoredToggleGroup m_Group;

        public ColoredToggleGroup group
        {
            get { return m_Group; }
            set
            {
                m_Group = value;
#if UNITY_EDITOR
                if (Application.isPlaying)
#endif
                {
                    SetToggleGroup(m_Group, true);
                    //PlayEffect(true);
                }
            }
        }

        /// <summary>
        /// Allow for delegate-based subscriptions for faster events than 'eventReceiver', and allowing for multiple receivers.
        /// </summary>
        public ToggleEvent onValueChanged = new ToggleEvent();

        // Whether the toggle is on
        [FormerlySerializedAs("m_IsActive")]
        [Tooltip("Is the toggle currently on or off?")]
        [SerializeField]
        private bool m_IsOn;

        /// <summary>
        /// Toggle state
        /// </summary>
        public bool isOn
        {
            get { return m_IsOn; }
            set
            {
                Set(value);
            }
        }

        void Set(bool value)
        {
            Set(value, true);
        }

        void Set(bool value, bool sendCallback)
        {
            if (m_IsOn == value)
            {
                UpdateColor();
                return;
            }

            // if we are in a group and set to true, do group logic
            m_IsOn = value;
            if (m_Group != null && IsActive())
            {
                if (m_IsOn || (!m_Group.AnyTogglesOn() && !m_Group.allowSwitchOff))
                {
                    m_IsOn = true;
                    m_Group.NotifyToggleOn(this);
                }
            }

            // Always send event when toggle is clicked, even if value didn't change
            // due to already active toggle in a toggle group being clicked.
            // Controls like SelectionList rely on this.
            // It's up to the user to ignore a selection being set to the same value it already was, if desired.
            //PlayEffect(toggleTransition == ToggleTransition.None);

            UpdateColor();

            if (sendCallback)
                onValueChanged.Invoke(m_IsOn);
        }

        private void InternalToggle()
        {
            if (!IsActive() || !IsInteractable())
                return;

            isOn = !isOn;
        }


        /// <summary>
        /// React to clicks.
        /// </summary>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            InternalToggle();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            InternalToggle();
        }

        public void Rebuild(CanvasUpdate executing)
        {

#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
                onValueChanged.Invoke(m_IsOn);
#endif
        }

        private void SetToggleGroup(ColoredToggleGroup newGroup, bool setMemberValue)
        {
            ColoredToggleGroup oldGroup = m_Group;

            // Sometimes IsActive returns false in OnDisable so don't check for it.
            // Rather remove the toggle too oftem than too little.
            if (m_Group != null)
                m_Group.UnregisterToggle(this);

            // At runtime the group variable should be set but not when calling this method from OnEnable or OnDisable.
            // That's why we use the setMemberValue parameter.
            if (setMemberValue)
                m_Group = newGroup;

            // Only register to the new group if this Toggle is active.
            if (m_Group != null && IsActive())
                m_Group.RegisterToggle(this);

            // If we are in a new group, and this toggle is on, notify group.
            // Note: Don't refer to m_Group here as it's not guaranteed to have been set.
            if (newGroup != null && newGroup != oldGroup && isOn && IsActive())
                m_Group.NotifyToggleOn(this);
        }



#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            Set(m_IsOn, false);
            //PlayEffect(toggleTransition == ToggleTransition.None);

            var prefabType = UnityEditor.PrefabUtility.GetPrefabType(this);
            if (prefabType != UnityEditor.PrefabType.Prefab && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

#endif // if UNITY_EDITOR


        public void LayoutComplete()
        {
            //throw new NotImplementedException();
        }

        public void GraphicUpdateComplete()
        {
            //throw new NotImplementedException();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetToggleGroup(m_Group, false);
        }

        protected override void OnDisable()
        {
            SetToggleGroup(null, false);
            base.OnDisable();
        }

        private void UpdateColor()
        {
            this.transform.GetChild(0).GetComponent<Text>().color = m_IsOn ? onColor : offColor;
        }
    }
}
