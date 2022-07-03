
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using XLua;
using XFX.Core.Render;

namespace XFX.Game {
    public partial class LuaApi {

        public static class LuaApiConfigNavMesh {
            [LuaCallCSharp]
            private static List<Type> LuaCallCSharp = new List<Type>() {
                typeof(NavMeshAgent),
                typeof(NavMeshObstacle),
            };
        }

        [LuaCallCSharp]
        public static class NavMesh {
            private static void InitNavMeshAgengt(NavMeshAgent agent) {
                agent.acceleration = 10000;
                agent.angularSpeed = 10000;
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
                agent.updateRotation = true;
                agent.autoTraverseOffMeshLink = false;
                agent.autoRepath = true;
                agent.autoBraking = true;
                agent.enabled = false;
            }

            public static NavMeshAgent AddNavMeshAgent(RenderObject renderObject) {
                if (renderObject != null && renderObject.gameObject != null) {
                    NavMeshAgent agent = renderObject.gameObject.GetComponent<NavMeshAgent>();
                    if (agent == null) {
                        agent = renderObject.gameObject.AddComponent<NavMeshAgent>();
                    }
                    InitNavMeshAgengt(agent);
                    return agent;
                }
                return null;
            }

            public static NavMeshObstacle AddNavMeshObstacle(RenderObject renderObject, int shape = 0) {
                if (renderObject != null && renderObject.gameObject != null) {
                    var obstacle = renderObject.gameObject.AddComponent<NavMeshObstacle>();
                    obstacle.shape = (NavMeshObstacleShape) shape;
                    obstacle.enabled = false;
                    obstacle.carving = false;
                    return obstacle;
                }
                return null;
            }

            public static bool IsInNavMesh(Vector3 pos, int areaMask) {
                NavMeshHit hit;
                return UnityEngine.AI.NavMesh.SamplePosition(pos, out hit, 0.1f, areaMask);
            }

            public static Vector3 AdjustToMesh(Vector3 pos, int navmask, float radius = 10F) {
                NavMeshHit mesh_hit;
                if (UnityEngine.AI.NavMesh.SamplePosition(pos, out mesh_hit, radius, navmask))
                    return mesh_hit.position;
                else {
                    Log.Debug("============== adjust to mesh error, {0}", pos.ToString());
                    return pos;
                }
            }

            static NavMeshPath path = new NavMeshPath();
            public static bool CalculatePath(Vector3 source, Vector3 target, int areaMask) {
                if (UnityEngine.AI.NavMesh.CalculatePath(source, target, areaMask, path)) {
                    return path.status == NavMeshPathStatus.PathComplete;
                }
                return false;
            }
        }
    }
}