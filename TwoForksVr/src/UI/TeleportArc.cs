using TwoForksVr.Settings;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace TwoForksVr.UI
{
    //-------------------------------------------------------------------------
    public class TeleportArc : MonoBehaviour
    {
        public int segmentCount = 60;
        public float thickness = 0.01f;

        [Tooltip("The amount of time in seconds to predict the motion of the projectile.")]
        public float arcDuration = 3.0f;

        public float arcVelocity = 10f;

        private Material material;

        public static Transform hitMarker; // TODO not static

        public LayerMask traceLayerMask = 0;

        //Private data
        private LineRenderer[] lineRenderers;
        private float arcTimeOffset = 0.0f;
        private float prevThickness = 0.0f;
        private int prevSegmentCount = 0;
        private bool showArc = true;
        private Vector3 startPos;
        private Vector3 projectileVelocity;
        private bool useGravity = true;
        private Transform arcObjectsTransfrom;
        private bool arcInvalid = false;
        private float scale = 1;

        public static vgPlayerNavigationController navigationController;


        //-------------------------------------------------
        void Start()
        {
            arcTimeOffset = Time.time;
            Show();
            hitMarker.SetParent(transform.parent);
            material = hitMarker.GetComponent<Renderer>().material;
        }

        public static bool IsNextToTeleportMarker(Transform transform, float minSquareDistance = 0.3f)
        {
            if (!hitMarker.gameObject.activeInHierarchy) return false;
            var targetPoint = hitMarker.position;
            targetPoint.y = transform.position.y;
            var squareDistance = Vector3.SqrMagnitude(targetPoint - transform.position);
            return squareDistance < minSquareDistance;
        }
        
        public static bool IsTeleporting()
        {
            return VrSettings.Teleport.Value && SteamVR_Actions.default_Teleport.state && navigationController && navigationController.enabled;
        }

        //-------------------------------------------------
        void Update()
        {
            if (!IsTeleporting())
            {
                hitMarker.gameObject.SetActive(false);
                Hide();
                return;
            }

            Show();
            if (thickness != prevThickness || segmentCount != prevSegmentCount)
            {
                CreateLineRendererObjects();
                prevThickness = thickness;
                prevSegmentCount = segmentCount;
            }
            
            RaycastHit hitInfo;
            SetArcData(transform.position, transform.forward * arcVelocity, true, false);
            var hit = DrawArc(out hitInfo);
            if (hit)
            {
                hitMarker.gameObject.SetActive(true);
                hitMarker.position = hitInfo.point;
            }
            else
            {
                hitMarker.gameObject.SetActive(false);
            }
        }



        //-------------------------------------------------
        private void CreateLineRendererObjects()
        {
            //Destroy any existing line renderer objects
            if (arcObjectsTransfrom != null)
            {
                Destroy(arcObjectsTransfrom.gameObject);
            }

            var arcObjectsParent = new GameObject("ArcObjects");
            arcObjectsTransfrom = arcObjectsParent.transform;
            arcObjectsTransfrom.SetParent(this.transform);

            //Create new line renderer objects
            lineRenderers = new LineRenderer[segmentCount];
            for (var i = 0; i < segmentCount; ++i)
            {
                var newObject = new GameObject("LineRenderer_" + i);
                newObject.transform.SetParent(arcObjectsTransfrom);

                lineRenderers[i] = newObject.AddComponent<LineRenderer>();

                lineRenderers[i].receiveShadows = false;
                lineRenderers[i].reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                lineRenderers[i].lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                lineRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                lineRenderers[i].material = material;
                lineRenderers[i].startWidth = thickness * scale;
				lineRenderers[i].endWidth = thickness * scale;
                lineRenderers[i].enabled = false;
            }
        }


        //-------------------------------------------------
        public void SetArcData(Vector3 position, Vector3 velocity, bool gravity, bool pointerAtBadAngle)
        {
            startPos = position;
            projectileVelocity = velocity;
            useGravity = gravity;

            if (arcInvalid && !pointerAtBadAngle)
            {
                arcTimeOffset = Time.time;
            }
            arcInvalid = pointerAtBadAngle;
        }


        //-------------------------------------------------
        public void Show()
        {
            showArc = true;
            if (lineRenderers == null)
            {
                CreateLineRendererObjects();
            }
        }


        //-------------------------------------------------
        public void Hide()
        {
            //Hide the line segments if they were previously being shown
            if (showArc)
            {
                HideLineSegments(0, segmentCount);
            }
            showArc = false;
        }


        //-------------------------------------------------
        // Draws each segment of the arc individually
        //-------------------------------------------------
        public bool DrawArc(out RaycastHit hitInfo)
        {
            var timeStep = arcDuration / segmentCount;

            var currentTimeOffset = 0f;

            var segmentStartTime = currentTimeOffset;

            var arcHitTime = FindProjectileCollision(out hitInfo);

            if (arcInvalid)
            {
                //Only draw first segment
                lineRenderers[0].enabled = true;
                lineRenderers[0].SetPosition(0, GetArcPositionAtTime(0.0f));
                lineRenderers[0].SetPosition(1, GetArcPositionAtTime(arcHitTime < timeStep ? arcHitTime : timeStep));

                HideLineSegments(1, segmentCount);
            }
            else
            {
                //Draw the first segment outside the loop if needed
                var loopStartSegment = 0;

                var stopArc = false;
                var currentSegment = 0;
                if (segmentStartTime < arcHitTime)
                {
                    for (currentSegment = loopStartSegment; currentSegment < segmentCount; ++currentSegment)
                    {
                        //Clamp the segment end time to the arc duration
                        var segmentEndTime = segmentStartTime + timeStep;
                        if (segmentEndTime >= arcDuration)
                        {
                            segmentEndTime = arcDuration;
                            stopArc = true;
                        }

                        if (segmentEndTime >= arcHitTime)
                        {
                            segmentEndTime = arcHitTime;
                            stopArc = true;
                        }

                        DrawArcSegment(currentSegment, segmentStartTime, segmentEndTime);

                        segmentStartTime += timeStep;

                        //If the previous end time or the next start time is beyond the duration then stop the arc
                        if (stopArc || segmentStartTime >= arcDuration || segmentStartTime >= arcHitTime)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    currentSegment--;
                }

                //Hide the rest of the line segments
                HideLineSegments(currentSegment + 1, segmentCount);
            }

            return arcHitTime != float.MaxValue;
        }


        //-------------------------------------------------
        private void DrawArcSegment(int index, float startTime, float endTime)
        {
            lineRenderers[index].enabled = true;
            lineRenderers[index].SetPosition(0, GetArcPositionAtTime(startTime));
            lineRenderers[index].SetPosition(1, GetArcPositionAtTime(endTime));
        }


        //-------------------------------------------------
        public void SetColor(Color color)
        {
            for (var i = 0; i < segmentCount; ++i)
            {
                lineRenderers[i].startColor = color;
				lineRenderers[i].endColor = color;
            }
        }


        //-------------------------------------------------
        private float FindProjectileCollision(out RaycastHit hitInfo)
        {
            var timeStep = arcDuration / segmentCount;
            var segmentStartTime = 0.0f;

            hitInfo = new RaycastHit();

            var segmentStartPos = GetArcPositionAtTime(segmentStartTime);
            for (var i = 0; i < segmentCount; ++i)
            {
                var segmentEndTime = segmentStartTime + timeStep;
                var segmentEndPos = GetArcPositionAtTime(segmentEndTime);

                if (Physics.Linecast(segmentStartPos, segmentEndPos, out hitInfo, traceLayerMask))
                {
                    if (hitInfo.collider.GetComponent<IgnoreTeleportTrace>() == null)
                    {
                        Util.DrawCross(hitInfo.point, Color.red, 0.5f);
                        var segmentDistance = Vector3.Distance(segmentStartPos, segmentEndPos);
                        var hitTime = segmentStartTime + (timeStep * (hitInfo.distance / segmentDistance));
                        return hitTime;
                    }
                }

                segmentStartTime = segmentEndTime;
                segmentStartPos = segmentEndPos;
            }

            return float.MaxValue;
        }


        //-------------------------------------------------
        public Vector3 GetArcPositionAtTime(float time)
        {
            var gravity = useGravity ? Physics.gravity : Vector3.zero;

            var arcPos = startPos + ((projectileVelocity * time) + (0.5f * time * time) * gravity) * scale;
            return arcPos;
        }


        //-------------------------------------------------
        private void HideLineSegments(int startSegment, int endSegment)
        {
            if (lineRenderers != null)
            {
                for (var i = startSegment; i < endSegment; ++i)
                {
                    lineRenderers[i].enabled = false;
                }
            }
        }
    }
}
