using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ColoredToggleUI
{
    /// <summary>
    /// A standard toggle that has an on / off state, with a twist that state is represented by colour.
    /// </summary>

    [RequireComponent(typeof(RectTransform))]
    public class ColoredToggle : MonoBehaviour
    {
        public enum ToggleTransition
        {
            None,
            Fade
        }

        [Serializable]
        public class ToggleEvent : UnityEvent<bool>
        { }

        /// <summary>
        /// Transition type.
        /// </summary>
        public ToggleTransition toggleTransition = ToggleTransition.Fade;

        /// <summary>
        /// Graphic the toggle should be working with.
        /// </summary>
        public Graphic graphic;

        // group that this toggle can belong to
        //        [SerializeField]
        //        private ToggleGroup m_Group;

        //        public ToggleGroup group
        //        {
        //            get { return m_Group; }
        //            set
        //            {
        //                m_Group = value;
        //#if UNITY_EDITOR
        //                if (Application.isPlaying)
        //#endif
        //        {
        //            SetToggleGroup(m_Group, true);
        //            PlayEffect(true);
        //        }
        //    }
        //}

        /// <summary>
        /// Allow for delegate-based subscriptions for faster events than 'eventReceiver', and allowing for multiple receivers.
        /// </summary>
        public ToggleEvent onValueChanged = new ToggleEvent();

        // Whether the toggle is on
        [FormerlySerializedAs("m_IsActive")]
        [Tooltip("Is the toggle currently on or off?")]
        [SerializeField]
        private bool m_IsOn;

    }
}
