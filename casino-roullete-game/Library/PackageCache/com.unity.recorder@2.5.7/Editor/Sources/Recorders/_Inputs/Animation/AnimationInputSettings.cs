using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor.Recorder.Input
{
    /// <inheritdoc/>
    /// <summary>
    /// Use this class to manage all the information required to record an Animation from a given GameObject.
    /// </summary>
    [Serializable]
    [DisplayName("Animation")]
    public class AnimationInputSettings : RecorderInputSettings
    {
        [SerializeField] string m_BindingId = null;

        /// <summary>
        /// Indicates the GameObject to record from.
        /// </summary>
        public GameObject gameObject
        {
            get
            {
                if (string.IsNullOrEmpty(m_BindingId))
                    return null;

                return BindingManager.Get(m_BindingId) as GameObject;
            }

            set
            {
                if (string.IsNullOrEmpty(m_BindingId))
                    m_BindingId = GenerateBindingId();

                BindingManager.Set(m_BindingId, value);
            }
        }

        /// <summary>
        /// Use this property to record all the gameObject hierarchy (True) or not (False).
        /// </summary>
        public bool Recursive
        {
            get => recursive;
            set => recursive = value;
        }

        [SerializeField] bool recursive = true;
        [SerializeField] bool clampedTangents = true;

        /// <summary>
        /// If true, the Recorder sets the generated animation key tangents to ClampedAuto, else to Auto (legacy). Clamped tangents are useful to prevent curve overshoots when the animation data is discontinuous.
        /// </summary>
        public bool ClampedTangents
        {
            get => clampedTangents;
            set => clampedTangents = value;
        }

        /// <summary>
        /// Sets the keyframe reduction level to use to compress the recorded animation curve data.
        /// </summary>
        public CurveSimplificationOptions SimplyCurves
        {
            get => simplifyCurves;
            set => simplifyCurves = value;
        }

        /// <summary>
        /// Available options for animation curve data compression.
        /// </summary>
        public enum CurveSimplificationOptions
        {
            /// <summary>
            /// Overall keyframe reduction. The Recorder removes animation keys based on a relative tolerance of 0.5 percent, to overall simplify the curve. This reduces the file size but directly affects the original curve accuracy.
            /// </summary>
            Lossy = 0,
            /// <summary>
            /// Keyframe reduction of constant curves. The Recorder removes all unnecessary keys when the animation curve is a straight line, but keeps all recorded keys as long as the animation is not constant.
            /// </summary>
            Lossless = 1,
            /// <summary>
            /// No compression. The Recorder saves all animation keys throughout the recording, even when the animation curve is a straight line. This might result in large files and slow playback.
            /// </summary>
            Disabled = 2
        }

        [SerializeField] CurveSimplificationOptions simplifyCurves = CurveSimplificationOptions.Lossy;

        static readonly CurveFilterOptions DefaultCurveFilterOptions = new CurveFilterOptions()
        {
            keyframeReduction = true,
            positionError = 0.5f,
            rotationError = 0.5f,
            scaleError = 0.5f,
            floatError = 0.5f
        };

        static readonly CurveFilterOptions RegularCurveFilterOptions = new CurveFilterOptions
        {
            keyframeReduction = true
        };

        static readonly CurveFilterOptions NoCurveFilterOptions = new CurveFilterOptions
        {
            keyframeReduction = false
        };

        internal CurveFilterOptions CurveFilterOptions
        {
            get
            {
                switch (SimplyCurves)
                {
                    case CurveSimplificationOptions.Lossy:
                        return DefaultCurveFilterOptions;
                    case CurveSimplificationOptions.Lossless:
                        return RegularCurveFilterOptions;
                    case CurveSimplificationOptions.Disabled:
                        return NoCurveFilterOptions;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// Adds a Component to record from the GameObject.
        /// </summary>
        /// <param name="componentType">The type of the Component.</param>
        public void AddComponentToRecord(Type componentType)
        {
            if (componentType == null)
                return;

            var typeName = componentType.AssemblyQualifiedName;
            if (!bindingTypeNames.Contains(typeName))
                bindingTypeNames.Add(typeName);
        }

        [SerializeField]
        internal List<string> bindingTypeNames = new List<string>();

        internal List<Type> bindingType
        {
            get
            {
                var ret = new List<Type>(bindingTypeNames.Count);
                foreach (var t in bindingTypeNames)
                {
                    ret.Add(Type.GetType(t));
                }
                return ret;
            }
        }

        /// <inheritdoc/>
        protected internal override Type InputType
        {
            get { return typeof(AnimationInput); }
        }

        /// <inheritdoc/>
        protected internal override bool ValidityCheck(List<string> errors)
        {
            var ok = true;

            if (bindingType.Count > 0 && bindingType.Any(x => typeof(ScriptableObject).IsAssignableFrom(x))
            )
            {
                ok = false;
                errors.Add("ScriptableObjects are not supported inputs.");
            }

            return ok;
        }

        static string GenerateBindingId()
        {
            return GUID.Generate().ToString();
        }

        /// <summary>
        /// Duplicates the existing Scene binding key under a new unique key (useful when duplicating the Recorder input).
        /// </summary>
        public void DuplicateExposedReference()
        {
            if (string.IsNullOrEmpty(m_BindingId))
                return;

            var src = m_BindingId;
            var dst = GenerateBindingId();

            m_BindingId = dst;

            BindingManager.Duplicate(src, dst);
        }

        /// <summary>
        /// Removes the binding value for the current key.
        /// </summary>
        public void ClearExposedReference()
        {
        }
    }
}
