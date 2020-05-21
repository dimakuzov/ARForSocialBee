using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialBeeAR
{

    public class Const
    {
        public static Color FEATURE_POINT_WEAK = new Color32(180, 180, 180, 64);
        public static Color FEATURE_POINT_STRONG = new Color32(87, 0, 181, 255);

        public static Vector3 ANCHOR_Y_OFFSET = new Vector3(0, 0.3f, 0);

        public static int MIN_MAP_SIZE = 300;

        public static readonly string ANCHOR_DATA_JSON_ROOT = "SBAnchorList";

        public static readonly float DISTANCE_TO_ACTIVE_ANCHOR = 1.5f;
        public static readonly float ANCHOR_ROTATION_SPEED = 24f;

        public static readonly string MAP_PREFIX = "SBActColl";

    }

}

