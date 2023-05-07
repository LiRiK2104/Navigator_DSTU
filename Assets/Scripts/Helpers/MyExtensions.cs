using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Helpers
{
    public static class MyExtensions
    {
        public static TweenerCore<Vector3, Path, PathOptions> DOPath(
            this MovableMask target,
            Vector3[] path,
            float duration,
            PathType pathType = PathType.Linear,
            PathMode pathMode = PathMode.Full3D,
            int resolution = 10,
            Color? gizmoColor = null)
        {
            if (resolution < 1)
                resolution = 1;
            TweenerCore<Vector3, Path, PathOptions> tweenerCore = DOTween.To(PathPlugin.Get(), 
                () => target.MaskPosition, 
                x => target.MaskPosition = x, 
                new Path(pathType, path, resolution, gizmoColor), duration).SetTarget(target);
            tweenerCore.plugOptions.mode = pathMode;
            return tweenerCore;
        }

        public static int ToInt(this bool target)
        {
            return target ? 1 : 0;
        }
        
        public static bool ToBool(this int value)
        {
            value = Math.Clamp(value, 0, 1);
            return value == 1;
        }
        
        /// <summary>
        /// Determines whether the collection is null or contains no elements.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type.</typeparam>
        /// <param name="enumerable">The enumerable, which may be null or empty.</param>
        /// <returns>
        ///     <c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable switch
            {
                null => true,
                ICollection<T> collection => collection.Count < 1,
                _ => !enumerable.Any()
            };
        }
    }
}
