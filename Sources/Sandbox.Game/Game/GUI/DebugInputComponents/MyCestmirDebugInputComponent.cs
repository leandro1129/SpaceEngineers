﻿using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Definitions;
using Sandbox.Engine.Physics;
using Sandbox.Engine.Utils;
using Sandbox.Game.AI;
using Sandbox.Game.AI.Pathfinding;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using VRage;
using VRage.Input;
using VRage.Utils;
using VRageMath;
using Sandbox.ModAPI;
using VRage.Library.Utils;
using System.Linq;

namespace Sandbox.Game.Gui
{
    class MyGuiScreenDialogPrefabCheat : MyGuiScreenBase
    {
        List<MyPrefabDefinition> m_prefabDefinitions = new List<MyPrefabDefinition>();

        MyGuiControlButton m_confirmButton;
        MyGuiControlButton m_cancelButton;
        MyGuiControlCombobox m_prefabs;

        public MyGuiScreenDialogPrefabCheat() :
            base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null)
        {
            CanHideOthers = false;
            EnabledBackgroundFade = true;
            RecreateControls(true);
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDialogPrefabCheat";
        }

        public override void RecreateControls(bool contructor)
        {
            base.RecreateControls(contructor);

            this.Controls.Add(new MyGuiControlLabel(new Vector2(0.0f, -0.10f), text: "Select the name of the prefab that you want to spawn", originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER));
            m_prefabs = new MyGuiControlCombobox(new Vector2(0.2f, 0.0f), new Vector2(0.3f, 0.05f), null, null, 10, null);
            m_confirmButton = new MyGuiControlButton(new Vector2(0.21f, 0.10f), Common.ObjectBuilders.Gui.MyGuiControlButtonStyleEnum.Default, new Vector2(0.2f, 0.05f), null, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, null, new System.Text.StringBuilder("Confirm"));
            m_cancelButton = new MyGuiControlButton(new Vector2(-0.21f, 0.10f), Common.ObjectBuilders.Gui.MyGuiControlButtonStyleEnum.Default, new Vector2(0.2f, 0.05f), null, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, null, new System.Text.StringBuilder("Cancel"));

            foreach (var prefab in MyDefinitionManager.Static.GetPrefabDefinitions())
            {
                int key = m_prefabDefinitions.Count;
                m_prefabDefinitions.Add(prefab.Value);
                m_prefabs.AddItem(key, new StringBuilder(prefab.Key));
            }

            this.Controls.Add(m_prefabs);
            this.Controls.Add(m_confirmButton);
            this.Controls.Add(m_cancelButton);

            m_confirmButton.ButtonClicked += confirmButton_OnButtonClick;
            m_cancelButton.ButtonClicked += cancelButton_OnButtonClick;
        }

        public override void HandleUnhandledInput(bool receivedFocusInThisUpdate)
        {
            base.HandleUnhandledInput(receivedFocusInThisUpdate);
        }

        void confirmButton_OnButtonClick(MyGuiControlButton sender)
        {
            var prefabDefinition = m_prefabDefinitions[(int)m_prefabs.GetSelectedKey()];

            var pos = MySector.MainCamera.Position;
            var fwd = MySector.MainCamera.ForwardVector;
            var up = MySector.MainCamera.UpVector;
           
            MyPrefabManager.Static.SpawnPrefab(
                prefabDefinition.Id.SubtypeName,
                pos + fwd * 70.0f,
                fwd,
                up,
                Vector3.Zero,
                Vector3.Zero,
                prefabDefinition.Id.SubtypeName,
                Sandbox.ModAPI.SpawningOptions.None,
                true);

            CloseScreen();
        }

        void cancelButton_OnButtonClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }
    }

    class MyGuiScreenDialogRemoveTriangle : MyGuiScreenBase
    {
        MyGuiControlTextbox m_textbox;
        MyGuiControlButton m_confirmButton;
        MyGuiControlButton m_cancelButton;

        public MyGuiScreenDialogRemoveTriangle() :
            base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null)
        {
            CanHideOthers = false;
            EnabledBackgroundFade = true;
            RecreateControls(true);
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDialogRemoveTriangle";
        }

        public override void RecreateControls(bool contructor)
        {
            base.RecreateControls(contructor);

            this.Controls.Add(new MyGuiControlLabel(new Vector2(0.0f, -0.10f), text: "Enter the number of a navmesh triangle to remove", originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER));
            m_textbox = new MyGuiControlTextbox(new Vector2(0.2f, 0.0f), type: MyGuiControlTextboxType.DigitsOnly);
            m_confirmButton = new MyGuiControlButton(new Vector2(0.21f, 0.10f), Common.ObjectBuilders.Gui.MyGuiControlButtonStyleEnum.Default, new Vector2(0.2f, 0.05f), null, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, null, new System.Text.StringBuilder("Confirm"));
            m_cancelButton = new MyGuiControlButton(new Vector2(-0.21f, 0.10f), Common.ObjectBuilders.Gui.MyGuiControlButtonStyleEnum.Default, new Vector2(0.2f, 0.05f), null, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, null, new System.Text.StringBuilder("Cancel"));

            this.Controls.Add(m_textbox);
            this.Controls.Add(m_confirmButton);
            this.Controls.Add(m_cancelButton);

            m_confirmButton.ButtonClicked += confirmButton_OnButtonClick;
            m_cancelButton.ButtonClicked += cancelButton_OnButtonClick;
        }

        public override void HandleUnhandledInput(bool receivedFocusInThisUpdate)
        {
            base.HandleUnhandledInput(receivedFocusInThisUpdate);
        }

        void confirmButton_OnButtonClick(MyGuiControlButton sender)
        {
            int index = Convert.ToInt32(m_textbox.Text);
            MyAIComponent.Static.Pathfinding.VoxelPathfinding.RemoveTriangle(index);
            CloseScreen();
        }

        void cancelButton_OnButtonClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }
    }

    class MyGuiScreenDialogViewEdge : MyGuiScreenBase
    {
        MyGuiControlTextbox m_textbox;
        MyGuiControlButton m_confirmButton;
        MyGuiControlButton m_cancelButton;

        public MyGuiScreenDialogViewEdge() :
            base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null)
        {
            CanHideOthers = false;
            EnabledBackgroundFade = true;
            RecreateControls(true);
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDialogViewEdge";
        }

        public override void RecreateControls(bool contructor)
        {
            base.RecreateControls(contructor);

            this.Controls.Add(new MyGuiControlLabel(new Vector2(0.0f, -0.10f), text: "Enter the number of winged-edge mesh edge to view", originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER));
            m_textbox = new MyGuiControlTextbox(new Vector2(0.2f, 0.0f), type: MyGuiControlTextboxType.DigitsOnly);
            m_confirmButton = new MyGuiControlButton(new Vector2(0.21f, 0.10f), Common.ObjectBuilders.Gui.MyGuiControlButtonStyleEnum.Default, new Vector2(0.2f, 0.05f), null, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, null, new System.Text.StringBuilder("Confirm"));
            m_cancelButton = new MyGuiControlButton(new Vector2(-0.21f, 0.10f), Common.ObjectBuilders.Gui.MyGuiControlButtonStyleEnum.Default, new Vector2(0.2f, 0.05f), null, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, null, new System.Text.StringBuilder("Cancel"));

            this.Controls.Add(m_textbox);
            this.Controls.Add(m_confirmButton);
            this.Controls.Add(m_cancelButton);

            m_confirmButton.ButtonClicked += confirmButton_OnButtonClick;
            m_cancelButton.ButtonClicked += cancelButton_OnButtonClick;
        }

        public override void HandleUnhandledInput(bool receivedFocusInThisUpdate)
        {
            base.HandleUnhandledInput(receivedFocusInThisUpdate);
        }

        void confirmButton_OnButtonClick(MyGuiControlButton sender)
        {
            int index = Convert.ToInt32(m_textbox.Text);
            MyWingedEdgeMesh.DebugDrawEdgesAdd(index);
            CloseScreen();
        }

        void cancelButton_OnButtonClick(MyGuiControlButton sender)
        {
            CloseScreen();
        }
    }

    public class MyCestmirDebugInputComponent : MyDebugComponent
    {
        private bool m_drawSphere = false;
        private BoundingSphere m_sphere;
        private Matrix m_sphereMatrix;
        private string m_string;
        private Vector3D m_point1;
        private Vector3D m_point2;

        public static int FaceToRemove;
        public static int BinIndex = -1;

        public static event Action TestAction;
        public static event Action<Vector3D, MyEntity> PlacedAction;

        private struct DebugDrawPoint
        {
            public Vector3D Position;
            public Color Color;
        }
        private static List<DebugDrawPoint> DebugDrawPoints = new List<DebugDrawPoint>();

        private static MyWingedEdgeMesh DebugDrawMesh = null;
        private static List<MyPolygon> DebugDrawPolys = new List<MyPolygon>();

        public static List<BoundingBoxD> Boxes = null;

        public MyCestmirDebugInputComponent()
        {
            AddShortcut(MyKeys.NumPad0, true, false, false, false, () => "Add prefab...", AddPrefab);
            AddShortcut(MyKeys.NumPad2, true, false, false, false, () => "Copy target grid position to clipboard", CaptureGridPosition);
            if (MyPerGameSettings.EnableAi)
            {
                AddShortcut(MyKeys.L, true, false, false, false, () => "Test polygon intersection", Test2);

                AddShortcut(MyKeys.Multiply, true, false, false, false, () => "Next navmesh connection helper bin", NextBin);
                AddShortcut(MyKeys.Divide, true, false, false, false, () => "Prev navmesh connection helper bin", PrevBin);

                AddShortcut(MyKeys.NumPad3, true, false, false, false, () => "Add bot", AddBot);
                AddShortcut(MyKeys.NumPad4, true, false, false, false, () => "Remove bot", RemoveBot);
                AddShortcut(MyKeys.NumPad5, true, false, false, false, () => "Find path for first bot", FindBotPath);
                AddShortcut(MyKeys.NumPad6, true, false, false, false, () => "Find path between points", FindPath);
                AddShortcut(MyKeys.NumPad7, true, false, false, false, () => "All bots go to random beacon", AllBotsToRandomBeacon);
                AddShortcut(MyKeys.NumPad8, true, false, false, false, () => "All bots aim at me", AllBotsAimAtMe);
                AddShortcut(MyKeys.NumPad9, true, false, false, false, () => "All bots look ahead", AllBotsLookAhead);
                AddShortcut(MyKeys.Add, true, false, false, false, () => "Next funnel segment",
                    delegate { Sandbox.Game.AI.Pathfinding.MyNavigationMesh.m_debugFunnelIdx++; return true; }
                );
                AddShortcut(MyKeys.Subtract, true, false, false, false, () => "Previous funnel segment",
                    delegate
                    {
                        if (Sandbox.Game.AI.Pathfinding.MyNavigationMesh.m_debugFunnelIdx > 0)
                            Sandbox.Game.AI.Pathfinding.MyNavigationMesh.m_debugFunnelIdx--;
                        return true;
                    }
                );

                AddShortcut(MyKeys.O, true, false, false, false, () => "Remove navmesh tri...",
                    delegate
                    {
                        var dialog = new MyGuiScreenDialogRemoveTriangle();
                        MyGuiSandbox.AddScreen(dialog);
                        return true;
                    }
                );

                AddShortcut(MyKeys.M, true, false, false, false, () => "View all navmesh edges",
                    delegate
                    {
                        MyWingedEdgeMesh.DebugDrawEdgesReset();
                        return true;
                    }
                );

                AddShortcut(MyKeys.L, true, false, false, false, () => "View single navmesh edge...",
                    delegate
                    {
                        var dialog = new MyGuiScreenDialogViewEdge();
                        MyGuiSandbox.AddScreen(dialog);
                        return true;
                    }
                );
            }
        }

        private bool AddPrefab()
        {
            var dialog = new MyGuiScreenDialogPrefabCheat();
            MyGuiSandbox.AddScreen(dialog);
            return true;
        }

        private bool CaptureGridPosition()
        {
            Vector3D from = MySector.MainCamera.Position;
            Vector3D to = MySector.MainCamera.Position + MySector.MainCamera.ForwardVector * 1000.0f;
            List<MyPhysics.HitInfo> hitList = new List<MyPhysics.HitInfo>();

            MyPhysics.CastRay(from, to, hitList);

            bool success = false;

            for (int i = 0; i < hitList.Count; ++i)
            {
                var hitGrid = hitList[i].HkHitInfo.Body.GetEntity() as MyCubeGrid;
                if (hitGrid != null)
                {
                    var builder = hitGrid.GetObjectBuilder() as MyObjectBuilder_CubeGrid;
                    if (builder != null)
                    {
                        m_sphere = builder.CalculateBoundingSphere();
                        m_sphere = m_sphere.Transform(hitGrid.WorldMatrix);
                        m_sphereMatrix = hitGrid.WorldMatrix;
                        m_sphereMatrix.Translation = m_sphere.Center;

                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat(
                            "<Position x=\"{0}\" y=\"{1}\" z=\"{2}\" />\n<Forward x=\"{3}\" y=\"{4}\" z=\"{5}\" />\n<Up x=\"{6}\" y=\"{7}\" z=\"{8}\" />",
                            new Object[] { m_sphereMatrix.Translation.X, m_sphereMatrix.Translation.Y, m_sphereMatrix.Translation.Z,
                                       m_sphereMatrix.Forward.X, m_sphereMatrix.Forward.Y, m_sphereMatrix.Forward.Z,
                                       m_sphereMatrix.Up.X, m_sphereMatrix.Up.Y, m_sphereMatrix.Up.Z });
                        m_string = sb.ToString();

                        Thread thread = new Thread(() => System.Windows.Forms.Clipboard.SetText(m_string));
                        thread.SetApartmentState(ApartmentState.STA);
                        thread.Start();
                        thread.Join();

                        success = true;
                        break;
                    }
                }
            }

            m_drawSphere = success;
            return success;
        }

        private class Vector3Comparer : IComparer<Vector3>
        {
            private Vector3 m_right;
            private Vector3 m_up;

            public Vector3Comparer(Vector3 right, Vector3 up)
            {
                m_right = right;
                m_up = up;
            }

            public int Compare(Vector3 x, Vector3 y)
            {
                float yDot, xDot;
                Vector3.Dot(ref x, ref m_right, out xDot);
                Vector3.Dot(ref y, ref m_right, out yDot);

                float diff = xDot - yDot;
                if (diff < 0) return -1;
                if (diff > 0) return 1;

                Vector3.Dot(ref x, ref m_up, out xDot);
                Vector3.Dot(ref y, ref m_up, out yDot);

                diff = xDot - yDot;
                if (diff < 0) return -1;
                if (diff > 0) return 1;

                return 0;
            }
        }

        private bool Test()
        {
            for (int test = 0; test < 1; test++)
            {
                ClearDebugPoints();
                DebugDrawPolys.Clear();

                float dist = 8.0f;

                Vector3D normal = MySector.MainCamera.ForwardVector;
                Vector3D pos = MySector.MainCamera.Position + normal * dist;
                Vector3D right = MySector.MainCamera.WorldMatrix.Right;
                Vector3D up = MySector.MainCamera.WorldMatrix.Up;

                Matrix tform = MySector.MainCamera.WorldMatrix;
                tform.Translation += normal * dist;
                //tform = Matrix.CreateRotationX(MyUtils.GetRandomRadian()) * tform;
                //tform = Matrix.CreateRotationY(MyUtils.GetRandomRadian()) * tform;

                Plane p = new Plane(pos, normal);

                DebugDrawPoints.Add(new DebugDrawPoint() { Position = pos, Color = Color.Pink });
                DebugDrawPoints.Add(new DebugDrawPoint() { Position = pos + normal, Color = Color.Pink });

                bool intersecting = true;
                bool incorrectWinding = true;

                List<Vector3> points = new List<Vector3>();

                while (intersecting || incorrectWinding)
                {
                    intersecting = false;
                    incorrectWinding = true;
                    points.Clear();

                    for (int i = 0; i < 6; ++i)
                    {
                        Vector3 point = MyUtils.GetRandomDiscPosition(ref pos, 4.5f, ref right, ref up);
                        points.Add(point);
                    }

                    /*var cmp = new Vector3Comparer(right, up);

                    points.Sort(cmp);*/

                    for (int i = 0; i < points.Count; ++i)
                    {
                        Line l = new Line(points[i], points[(i + 1) % points.Count]);
                        Vector3 lNormal = Vector3.Normalize(l.Direction);

                        for (int j = 0; j < points.Count; ++j)
                        {
                            if (Math.Abs(j - i) <= 1 || (j == 0 && i == points.Count - 1) || (i == 0 && j == points.Count - 1)) continue;

                            Vector3 p1 = points[j] - points[i];
                            Vector3 p2 = points[(j + 1) % points.Count] - points[i];
                            if (Vector3.Dot(Vector3.Cross(p1, lNormal), Vector3.Cross(p2, lNormal)) >= 0) continue;

                            float p1t = Vector3.Dot(p1, lNormal);
                            float p2t = Vector3.Dot(p2, lNormal);

                            float p1r = Vector3.Reject(p1, lNormal).Length();
                            float p2r = Vector3.Reject(p2, lNormal).Length();
                            float r = p1r + p2r;
                            p1r /= r;
                            p2r /= r;

                            float t = p1t * p2r + p2t * p1r;

                            if (t <= l.Length && t >= 0)
                            {
                                intersecting = true;
                                break;
                            }
                        }

                        if (intersecting) break;
                    }

                    float area = 0;
                    for (int i = 0; i < points.Count; ++i)
                    {
                        var p1 = points[i];
                        var p2 = points[(i + 1) % points.Count];
                        area += (p2.X - p1.X) * (p2.Y + p1.Y);
                    }
                    if (area < 0)
                    {
                        incorrectWinding = false;
                    }
                }

                foreach (var point in points)
                {
                    AddDebugPoint(point, Color.Yellow);
                }

                var intersectionMesh = MyDefinitionManager.Static.GetCubeBlockDefinition(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "StoneCube")).NavigationDefinition.Mesh.Mesh.Copy();
                intersectionMesh.Transform(tform);

                // While not all edges were iterated over:

                // edge, prevInt := Find an intersected edge
                // Find intersected vertex loop:
                // edge2, int := Find other intersected edge
                // list.Add(prevInt, int)
                // while (edge != edge2):
                // prevInt := int;
                // edge2, int := Find other intersected edge
                // list.Add(prevInt, int)
                // CheckListDirection(list)

                HashSet<int> m_visitedEdges = new HashSet<int>();
                MyWingedEdgeMesh.EdgeEnumerator edgeEnum = intersectionMesh.GetEdges();
                List<Vector3> loop = new List<Vector3>();

                while (edgeEnum.MoveNext())
                {
                    int edgeIndex = edgeEnum.CurrentIndex;
                    if (m_visitedEdges.Contains(edgeIndex))
                        continue;

                    MyWingedEdgeMesh.Edge edge = intersectionMesh.GetEdge(edgeIndex);
                    loop.Clear();
                    Vector3 intersection;

                    if (intersectionMesh.IntersectEdge(ref edge, ref p, out intersection))
                    {
                        loop.Add(intersection);
                        int firstEdgeIndex = edgeIndex;
                        int face = edge.LeftFace;

                        edgeIndex = edge.GetNextFaceEdge(face);
                        edge = intersectionMesh.GetEdge(edgeIndex);

                        while (edgeIndex != firstEdgeIndex)
                        {
                            if (intersectionMesh.IntersectEdge(ref edge, ref p, out intersection))
                            {
                                face = edge.OtherFace(face);
                                if (Vector3.DistanceSquared(loop[loop.Count - 1], intersection) > 0.000001f)
                                    loop.Add(intersection);
                            }

                            edgeIndex = edge.GetNextFaceEdge(face);
                            edge = intersectionMesh.GetEdge(edgeIndex);
                        }

                        break; // TODO: So far, only intersect one loop
                    }
                };
                edgeEnum.Dispose();

                // Find an intersecting edge
                /*Vector3? intersection = null;
                Ray ray = default(Ray);
                MyWingedEdgeMesh.Edge edge = new MyWingedEdgeMesh.Edge();
                MyWingedEdgeMesh.EdgeEnumerator edgeEnum = intersectionMesh.GetEdges();
                while (edgeEnum.MoveNext())
                {
                    edge = edgeEnum.Current;
                    edge.ToRay(intersectionMesh, ref ray);
                    double? intDist = ray.Intersects(p);

                    // If edge is intersecting:
                    if (intDist.HasValue && intDist.Value >= 0.0 && intDist.Value <= 1.0)
                    {
                        intersection = ray.Position + ray.Direction * (float)intDist.Value;
                        DebugDrawPoints.Add(new DebugDrawPoint() { Position = intersection.Value, Color = Color.Red });
                        int face = edge.LeftFace;

                        int otherEdge = edge.GetNextFaceEdge(face);
                        while (otherEdge != edge.Index)
                        {
                            var itEdge = intersectionMesh.GetEdge(otherEdge);
                            itEdge.ToRay(intersectionMesh, ref ray);
                            intDist = ray.Intersects(p);
                            if (intDist.HasValue && intDist.Value >= 0.0 && intDist.Value <= 1.0)
                            {
                                face = itEdge.OtherFace(face);
                                break;
                            }

                            otherEdge = itEdge.GetNextFaceEdge(face);
                        }

                        break;
                    }
                }
                edgeEnum.Dispose();*/

                List<int> edges = new List<int>();
                DebugDrawMesh = intersectionMesh;
                //DebugDrawMesh = new MyWingedEdgeMesh();
                //DebugDrawMesh.MakeNewPoly(null, points, edges);

                edges.Clear();
                var poly1 = new MyPolygon(p);
                poly1.AddLoop(loop);
                DebugDrawPolys.Add(poly1);
                var poly2 = new MyPolygon(p);
                poly2.AddLoop(points);
                DebugDrawPolys.Add(poly2);

                var result = MyPolygonBoolOps.Static.Difference(poly1, poly2);
                Matrix tr = Matrix.CreateTranslation(normal * -1.0f);
                result.Transform(ref tr);
                DebugDrawPolys.Add(result);

                /*if (intersection.HasValue)
                {
                    int firstFace = edge.RightFace;

                    MyWingedEdgeMesh.Face face = intersectionMesh.GetFace(firstFace);

                    var faceEdgeEnum = face.GetEnumerator();
                    while (faceEdgeEnum.MoveNext())
                    {
                        MyWingedEdgeMesh.Edge nextEdge = intersectionMesh.GetEdge(faceEdgeEnum.Current);

                    }
                }*/

                if (TestAction != null)
                    TestAction();
            }
            return true;
        }

        private static List<Tuple<Vector2[], Vector2[]>> m_testList = null;
        private static int m_testIndex = 0;
        private static int m_testOperation = 0;
        private static int m_prevTestIndex = 0;
        private static int m_prevTestOperation = 0;
        private bool Test2()
        {
            Plane testPlane = new Plane(Vector3.Forward, 0.0f);

            if (m_testList == null)
            {
                m_testList = new List<Tuple<Vector2[], Vector2[]>>();

                /*m_testList.Add(new Tuple<Vector2[], Vector2[]>(
                    new Vector2[] {
                        new Vector2(-1.0f, -2.0f),
                        new Vector2(0.0f, 0.0f),
                        new Vector2(1.0f, -3.0f),
                    },
                    new Vector2[] {
                        new Vector2(0.0f, -2.0f),
                        new Vector2(0.0f, 0.0f),
                        new Vector2(2.0f, -3.0f),
                    }
                ));*/

                m_testList.Add(new Tuple<Vector2[], Vector2[]>(
                    new Vector2[] { 
                        new Vector2(0.0f, 0.0f),
                        new Vector2(0.0f, 2.0f),
                        new Vector2(2.0f, 2.0f),
                        new Vector2(2.0f, 0.0f),
                    },
                    new Vector2[] {
                        new Vector2(1.0f, 1.0f),
                        new Vector2(1.0f, 3.0f),
                        new Vector2(3.0f, 3.0f),
                        new Vector2(3.0f, 1.0f),
                    }
                ));

                m_testList.Add(new Tuple<Vector2[], Vector2[]>(
                    new Vector2[] { 
                        new Vector2(0.0f, 0.0f),
                        new Vector2(0.0f, 2.0f),
                        new Vector2(2.0f, 2.0f),
                        new Vector2(2.0f, 0.0f),
                    },
                    new Vector2[] {
                        new Vector2(-1.0f, 1.0f),
                        new Vector2(-1.0f, 3.0f),
                        new Vector2(1.0f, 3.0f),
                        new Vector2(1.0f, 1.0f),
                    }
                ));

                m_testList.Add(new Tuple<Vector2[], Vector2[]>(
                    new Vector2[] { 
                        new Vector2(0.0f, 0.0f),
                        new Vector2(0.0f, 2.0f),
                        new Vector2(2.0f, 2.0f),
                        new Vector2(2.0f, 0.0f),
                    },
                    new Vector2[] {
                        new Vector2(-1.0f, -1.0f),
                        new Vector2(-1.0f, 1.0f),
                        new Vector2(1.0f, 1.0f),
                        new Vector2(1.0f, -1.0f),
                    }
                ));

                m_testList.Add(new Tuple<Vector2[], Vector2[]>(
                    new Vector2[] { 
                        new Vector2(0.0f, 0.0f),
                        new Vector2(0.0f, 2.0f),
                        new Vector2(2.0f, 2.0f),
                        new Vector2(2.0f, 0.0f),
                    },
                    new Vector2[] {
                        new Vector2(1.0f, -1.0f),
                        new Vector2(1.0f, 1.0f),
                        new Vector2(3.0f, 1.0f),
                        new Vector2(3.0f, -1.0f),
                    }
                ));

                m_testList.Add(new Tuple<Vector2[], Vector2[]>(
                    new Vector2[] { 
                        new Vector2(0.0f, 0.0f),
                        new Vector2(0.0f, 2.0f),
                        new Vector2(2.0f, 2.0f),
                        new Vector2(2.0f, 0.0f),
                    },
                    new Vector2[] {
                        new Vector2(-1.0f, 0.0f),
                        new Vector2(-1.0f, 2.0f),
                        new Vector2(1.0f, 2.0f),
                        new Vector2(1.0f, 0.0f),
                    }
                ));

                m_testList.Add(new Tuple<Vector2[], Vector2[]>(
                    new Vector2[] { 
                        new Vector2(0.0f, 0.0f),
                        new Vector2(0.0f, 2.0f),
                        new Vector2(2.0f, 2.0f),
                        new Vector2(2.0f, 0.0f),
                    },
                    new Vector2[] {
                        new Vector2(1.0f, 0.0f),
                        new Vector2(1.0f, 2.0f),
                        new Vector2(3.0f, 2.0f),
                        new Vector2(3.0f, 0.0f),
                    }
                ));

                m_testList.Add(new Tuple<Vector2[], Vector2[]>(
                    new Vector2[] { 
                        new Vector2(0.0f, 0.0f),
                        new Vector2(0.0f, 2.0f),
                        new Vector2(2.0f, 2.0f),
                        new Vector2(2.0f, 0.0f),
                    },
                    new Vector2[] {
                        new Vector2(0.0f, 1.0f),
                        new Vector2(0.0f, 3.0f),
                        new Vector2(2.0f, 3.0f),
                        new Vector2(2.0f, 1.0f),
                    }
                ));

                m_testList.Add(new Tuple<Vector2[], Vector2[]>(
                    new Vector2[] { 
                        new Vector2(0.0f, 0.0f),
                        new Vector2(0.0f, 2.0f),
                        new Vector2(2.0f, 2.0f),
                        new Vector2(2.0f, 0.0f),
                    },
                    new Vector2[] {
                        new Vector2(0.0f, -1.0f),
                        new Vector2(0.0f, 1.0f),
                        new Vector2(2.0f, 1.0f),
                        new Vector2(2.0f, -1.0f),
                    }
                ));

                m_testList.Add(new Tuple<Vector2[], Vector2[]>(
                    new Vector2[] {
                        new Vector2(0.0f, 0.0f),
                        new Vector2(0.0f, 2.0f),
                        new Vector2(2.0f, 2.0f),
                        new Vector2(2.0f, 0.0f),
                    },
                    new Vector2[] {
                        new Vector2(0.0f, 0.0f),
                        new Vector2(0.0f, 2.0f),
                        new Vector2(2.0f, 2.0f),
                        new Vector2(2.0f, 0.0f),
                    }
                ));

                m_testList.Add(new Tuple<Vector2[], Vector2[]>(
                    new Vector2[] { 
                        new Vector2(0.0f, 0.0f),
                        new Vector2(-1.0f, 1.0f),
                        new Vector2(0.0f, 2.0f),
                        new Vector2(1.0f, 1.0f),
                    },
                    new Vector2[] { 
                        new Vector2(-2.0f, 1.3f),
                        new Vector2(-2.0f, 2.3f),
                        new Vector2(2.0f, 2.7f),
                        new Vector2(2.0f, 1.7f),
                    }
                ));

                m_testList.Add(new Tuple<Vector2[], Vector2[]>(
                    new Vector2[] { 
                        new Vector2(0.0f, 0.0f),
                        new Vector2(1.0f, 5.0f),
                        new Vector2(3.0f, 2.0f),
                        new Vector2(4.0f, 4.0f),
                        new Vector2(5.0f, 1.0f),
                    },
                    new Vector2[] { 
                        new Vector2(-1.0f, 4.0f),
                        new Vector2(1.0f, 7.0f),
                        new Vector2(6.0f, 4.0f),
                        new Vector2(5.0f, 3.0f),
                    }
                ));
                
                m_testList.Add(new Tuple<Vector2[], Vector2[]>(
                     new Vector2[] { 
                        new Vector2(0.0f, 3.0f),
                        new Vector2(4.0f, 7.0f),
                        new Vector2(9.0f, 8.0f),
                        new Vector2(5.0f, 2.0f),
                        new Vector2(2.0f, 0.0f),
                    },
                     new Vector2[] { 
                        new Vector2(0.0f, 9.0f),
                        new Vector2(4.0f, 12.0f),
                        new Vector2(7.0f, 9.0f),
                        new Vector2(9.0f, 1.0f),
                        new Vector2(4.0f, 9.0f),
                        new Vector2(2.0f, 4.0f),
                    }
                ));

                m_testList.Add(new Tuple<Vector2[], Vector2[]>(
                    new Vector2[] { 
                        new Vector2(0.0f, 0.0f),
                        new Vector2(0.0f, 4.1f),
                        new Vector2(4.0f, 4.0f),
                        new Vector2(4.0f, 0.1f),
                    },
                    new Vector2[] { 
                        new Vector2(2.0f, 1.0f),
                        new Vector2(1.0f, 2.0f),
                        new Vector2(2.0f, 3.0f),
                        new Vector2(3.0f, 2.0f),
                    }
                ));

                m_testList.Add(new Tuple<Vector2[], Vector2[]>(
                    new Vector2[] { 
                        new Vector2(3.0f, 0.0f),
                        new Vector2(0.0f, 3.0f),
                        new Vector2(3.0f, 6.0f),
                        new Vector2(6.0f, 3.0f),
                    },
                    new Vector2[] { 
                        new Vector2(6.0f, 7.0f),
                        new Vector2(8.0f, 5.0f),
                        new Vector2(5.0f, 2.0f),
                        new Vector2(3.0f, 4.0f),
                    }
                ));

                m_testList.Add(new Tuple<Vector2[], Vector2[]>(
                    new Vector2[] { 
                        new Vector2(3.0f, 0.0f),
                        new Vector2(0.0f, 3.0f),
                        new Vector2(3.0f, 6.0f),
                        new Vector2(6.0f, 3.0f),
                    },
                    new Vector2[] { 
                        new Vector2(6.0f, 3.0f),
                        new Vector2(3.0f, 6.0f),
                        new Vector2(6.0f, 7.0f),
                    }
                ));

                m_testList.Add(new Tuple<Vector2[], Vector2[]>(
                    new Vector2[] { 
                        new Vector2(0.0f, 0.0f),
                        new Vector2(-2.0f, 2.0f),
                        new Vector2(0.0f, 4.0f),
                        new Vector2(2.0f, 2.0f),
                    },
                    new Vector2[] { 
                        new Vector2(0.0f, 0.0f),
                        new Vector2(-1.0f, 1.0f),
                        new Vector2(0.0f, 2.0f),
                        new Vector2(1.0f, 1.0f),
                    }
                ));

                m_testList.Add(new Tuple<Vector2[], Vector2[]>(
                    new Vector2[] { 
                        new Vector2(0.0f, 0.0f),
                        new Vector2(-2.0f, 2.0f),
                        new Vector2(0.0f, 4.0f),
                        new Vector2(2.0f, 2.0f),
                    },
                    new Vector2[] {
                        new Vector2(1.0f, 1.0f),
                        new Vector2(-0.0f, 2.0f),
                        new Vector2(1.0f, 3.0f),
                        new Vector2(2.0f, 2.0f),
                    }
                ));

                m_testList.Add(new Tuple<Vector2[], Vector2[]>(
                    new Vector2[] { 
                        new Vector2(0.0f, 0.0f),
                        new Vector2(-2.0f, 2.0f),
                        new Vector2(0.0f, 4.0f),
                        new Vector2(2.0f, 2.0f),
                    },
                    new Vector2[] { 
                        new Vector2(0.0f, 2.0f),
                        new Vector2(-1.0f, 3.0f),
                        new Vector2(0.0f, 4.0f),
                        new Vector2(1.0f, 3.0f),
                    }
                ));

                m_testList.Add(new Tuple<Vector2[], Vector2[]>(
                    new Vector2[] { 
                        new Vector2(0.0f, 0.0f),
                        new Vector2(-2.0f, 2.0f),
                        new Vector2(0.0f, 4.0f),
                        new Vector2(2.0f, 2.0f),
                    },
                    new Vector2[] {
                        new Vector2(-1.0f, 1.0f),
                        new Vector2(-2.0f, 2.0f),
                        new Vector2(-1.0f, 3.0f),
                        new Vector2(0.0f, 2.0f),
                    }
                ));

                m_testList.Add(new Tuple<Vector2[], Vector2[]>(
                    new Vector2[] { 
                        new Vector2(2.0f, 0.0f),
                        new Vector2(0.0f, 2.0f),
                        new Vector2(4.0f, 6.0f),
                        new Vector2(0.0f, 10.0f),
                        new Vector2(2.0f, 12.0f),
                        new Vector2(4.0f, 10.0f),
                        new Vector2(0.0f, 6.0f),
                        new Vector2(4.0f, 2.0f),
                    },
                    new Vector2[] {
                        new Vector2(1.0f, 2.0f),
                        new Vector2(1.0f, 8.0f),
                        new Vector2(3.0f, 10.0f),
                        new Vector2(3.0f, 4.0f),
                    }
                ));
            }

            DebugDrawPolys.Clear();

            /*m_testIndex = 13;
            m_testOperation = 2;*/

            m_prevTestIndex = m_testIndex;
            m_prevTestOperation = m_testOperation;

            var loop1 = new Vector2[] {
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 4.0f),
                new Vector2(4.0f, 4.0f),
                new Vector2(4.0f, 0.0f),
            };

            var loop2 = new Vector2[] {
                new Vector2(1.0f, 2.0f),
                new Vector2(2.0f, 1.0f),
                new Vector2(3.0f, 2.0f),
                new Vector2(2.0f, 3.0f),
            };

            var loop3 = new Vector2[] {
                new Vector2(-1.0f, 2.0f),
                new Vector2(-1.0f, 5.0f),
                new Vector2(5.0f, 5.0f),
                new Vector2(5.0f, 2.0f),
            };

            var arrays = m_testList[m_testIndex];
            MyPolygon poly1 = new MyPolygon(testPlane);
            MyPolygon poly2 = new MyPolygon(testPlane);
            poly1.AddLoop(new List<Vector3>(loop1.Select(i => new Vector3(i.X, i.Y, 0.0f))));
            poly1.AddLoop(new List<Vector3>(loop2.Select(i => new Vector3(i.X, i.Y, 0.0f))));
            poly2.AddLoop(new List<Vector3>(loop3.Select(i => new Vector3(i.X, i.Y, 0.0f))));
            /*poly1.AddLoop(new List<Vector3>(arrays.Item1.Select(i => new Vector3(i.X, i.Y, 0.0f))));
            poly2.AddLoop(new List<Vector3>(arrays.Item2.Select(i => new Vector3(i.X, i.Y, 0.0f))));*/
            DebugDrawPolys.Add(poly1);
            DebugDrawPolys.Add(poly2);

            TimeSpan time1000 = new TimeSpan();

            MyPolygon resultPoly = null;
            /*for (int i = 0; i < 1000; ++i)
            {*/
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                if (m_testOperation == 0)
                {
                    resultPoly = MyPolygonBoolOps.Static.Intersection(poly1, poly2);
                }
                else if (m_testOperation == 1)
                {
                    resultPoly = MyPolygonBoolOps.Static.Union(poly1, poly2);
                }
                else if (m_testOperation == 2)
                {
                    resultPoly = MyPolygonBoolOps.Static.Difference(poly1, poly2);
                }
                else if (m_testOperation == 3)
                {
                    resultPoly = MyPolygonBoolOps.Static.Intersection(poly2, poly1);
                }
                else if (m_testOperation == 4)
                {
                    resultPoly = MyPolygonBoolOps.Static.Union(poly2, poly1);
                }
                else
                {
                    resultPoly = MyPolygonBoolOps.Static.Difference(poly2, poly1);
                }
                var time = stopwatch.Elapsed;
                time1000 += time;
            //}

            Matrix tform = Matrix.CreateTranslation(Vector3.Right * 12.0f);
            resultPoly.Transform(ref tform);
            DebugDrawPolys.Add(resultPoly);

            m_testIndex = m_testIndex + 1;
            /*if (m_testIndex >= m_testList.Count)
            {*/
                m_testIndex = 0;
                m_testOperation = (m_testOperation + 1) % 6;
            //}

            return true;
        }

        private bool EmitPlacedAction(Vector3D position, IMyEntity entity)
        {
            if (PlacedAction != null)
                PlacedAction(position, entity as MyEntity);

            return true;
        }

        private bool NextBin()
        {
            BinIndex++;
            return true;
        }

        private bool PrevBin()
        {
            BinIndex--;
            if (BinIndex < -1) BinIndex = -1;
            return true;
        }

        private bool AddBot()
        {
            var barbarianBehavior = MyDefinitionManager.Static.GetBotDefinition(new MyDefinitionId(typeof(MyObjectBuilder_BotDefinition), "NormalBarbarian")) as MyAgentDefinition;
            MyAIComponent.Static.SpawnNewBot(barbarianBehavior);

            return true;
        }

        private bool RemoveBot()
        {
            int highestExistingPlayer = -1;

            var players = Sync.Players.GetOnlinePlayers();
            foreach (var player in players)
            {
                if (player.Id.SteamId == Sync.MyId)

                    highestExistingPlayer = Math.Max(highestExistingPlayer, player.Id.SerialId);
            }

            if (highestExistingPlayer > 0)
            {
                var player = Sync.Players.TryGetPlayerById(new MyPlayer.PlayerId(Sync.MyId, highestExistingPlayer));
                Sync.Players.RemovePlayer(player);
            }

            return true;
        }

        private bool FindPath()
        {
            Vector3D? firstHit;
            ModAPI.IMyEntity entity;
            Raycast(out firstHit, out entity);

            if (firstHit.HasValue)
            {
                m_point1 = m_point2;
                m_point2 = firstHit.Value;
                MyAIComponent.Static.Pathfinding.FindPathLowlevel(m_point1, m_point2);
            }
            return true;
        }

        private bool FindBotPath()
        {
            Vector3D? firstHit;
            ModAPI.IMyEntity entity;
            Raycast(out firstHit, out entity);

            if (firstHit.HasValue)
            {
                EmitPlacedAction(firstHit.Value, entity);
            }

            return true;
        }

        private bool AllBotsLookAhead()
        {
            for (int j = 1; j < 100; ++j)
            {
                MySandboxBot bot = MyAIComponent.Static.Bots.TryGetBot<MySandboxBot>(j);
                if (bot != null)
                    bot.Navigation.ResetAiming(true);
                else break;
            }
            return true;
        }

        private bool AllBotsAimAtMe()
        {
            for (int j = 1; j < 100; ++j)
            {
                MySandboxBot bot = MyAIComponent.Static.Bots.TryGetBot<MySandboxBot>(j);
                if (bot != null)
                    bot.Navigation.AimAt(MySession.LocalCharacter);
                else break;
            }
            return true;
        }

        private bool AllBotsToRandomBeacon()
        {
            var ctrlEntity = MySession.ControlledEntity;
            if (ctrlEntity != null)
            {
                var grid = ctrlEntity.Entity.Hierarchy.GetTopMostParent().Entity as MyCubeGrid;
                if (grid != null)
                {
                    List<MyCubeBlock> blocks = new List<MyCubeBlock>();
                    foreach (var block in grid.CubeBlocks)
                    {
                        if (block.FatBlock == null) continue;
                        if (!(block.FatBlock is MyBeacon)) continue;
                        blocks.Add(block.FatBlock);
                    }

                    if (blocks.Count != 0)
                    {
                        int i = MyRandom.Instance.Next();
                        if (i < 0) i = -i;
                        i = i % blocks.Count;

                        for (int j = 1; j < 100; ++j)
                        {
                            MySandboxBot bot = MyAIComponent.Static.Bots.TryGetBot<MySandboxBot>(j);
                            if (bot != null)
                                bot.Navigation.Goto(blocks[i].WorldMatrix.Translation, 0.0f, blocks[i]);
                            else break;
                        }
                    }
                }
            }
            return true;
        }

        public override string GetName()
        {
            return "Cestmir";
        }

        public override bool HandleInput()
        {
            if (MySession.Static == null)
                return false;

            if (MyScreenManager.GetScreenWithFocus() is MyGuiScreenDialogPrefabCheat) return false;
            if (MyScreenManager.GetScreenWithFocus() is MyGuiScreenDialogRemoveTriangle) return false;
            if (MyScreenManager.GetScreenWithFocus() is MyGuiScreenDialogViewEdge) return false;

            return base.HandleInput();
        }

        private static void Raycast(out Vector3D? firstHit, out ModAPI.IMyEntity entity)
        {
            var cam = MySector.MainCamera;
            var hitList = new List<Sandbox.Engine.Physics.MyPhysics.HitInfo>();

            MyPhysics.CastRay(cam.Position, cam.Position + cam.ForwardVector * 1000.0f, hitList);
            if (hitList.Count > 0)
            {
                firstHit = hitList[0].Position;
                entity = hitList[0].HkHitInfo.Body.GetEntity();
            }
            else
            {
                firstHit = null;
                entity = null;
            }
        }

        public static void AddDebugPoint(Vector3D point, Color color)
        {
            DebugDrawPoints.Add(new DebugDrawPoint() { Position = point, Color = color });
        }

        public static void ClearDebugPoints()
        {
            DebugDrawPoints.Clear();
        }

        public override void Draw()
        {
            base.Draw();

            if (!MyDebugDrawSettings.ENABLE_DEBUG_DRAW) return;

            var src = MyScreenManager.GetScreenWithFocus();

            if (MyScreenManager.GetScreenWithFocus() == null || MyScreenManager.GetScreenWithFocus().DebugNamePath != "MyGuiScreenGamePlay") return;

            if (m_drawSphere)
            {
                VRageRender.MyRenderProxy.DebugDrawSphere(m_sphere.Center, m_sphere.Radius, Color.Red, 1.0f, false, cull: true);
                VRageRender.MyRenderProxy.DebugDrawAxis(m_sphereMatrix, 50.0f, false);
                VRageRender.MyRenderProxy.DebugDrawText2D(new Vector2(200.0f, 0.0f), m_string, Color.Red, 0.5f);
            }

            /*Vector3? prevPoint = null;
            if (m_path != null && m_pathGrid != null)
            {
                foreach (var point in m_path)
                {
                    Vector3 pointWorld = Vector3.Transform(point, m_pathGrid.WorldMatrix);
                    if (prevPoint.HasValue)
                    {
                        VRageRender.MyRenderProxy.DebugDrawLine3D(prevPoint.Value + Vector3.Up * 0.2f, pointWorld + Vector3.Up * 0.2f, Color.Orange, Color.Orange, true);
                    }

                    prevPoint = pointWorld;
                }
            }*/
            /*Vector3? prevPoint = null;
            if (m_path2 != null && m_pathGrid != null)
            {
                foreach (var point in m_path2)
                {
                    Vector3 pointWorld = Vector3.Transform(point * m_pathGrid.GridSize, m_pathGrid.WorldMatrix);
                    if (prevPoint.HasValue)
                    {
                        VRageRender.MyRenderProxy.DebugDrawLine3D(prevPoint.Value + Vector3.Up * 0.2f, pointWorld + Vector3.Up * 0.2f, Color.Violet, Color.Violet, true);
                    }

                    prevPoint = pointWorld;
                }
            }*/

            VRageRender.MyRenderProxy.DebugDrawSphere(m_point1, 0.5f, Color.Orange.ToVector3(), 1.0f, true);
            VRageRender.MyRenderProxy.DebugDrawSphere(m_point2, 0.5f, Color.Orange.ToVector3(), 1.0f, true);

            foreach (var point in DebugDrawPoints)
            {
                //VRageRender.MyRenderProxy.DebugDrawSphere(point.Position, 0.05f, point.Color.ToVector3(), 1.0f, false);
                VRageRender.MyRenderProxy.DebugDrawSphere(point.Position, 0.03f, point.Color, 1.0f, false);
            }

            VRageRender.MyRenderProxy.DebugDrawText2D(new Vector2(300.0f, 0.0f), "Test index: " + m_prevTestIndex.ToString() + "/" + (m_testList == null ? "-" : m_testList.Count.ToString()) + ", Test operation: " + m_prevTestOperation.ToString(), Color.Red, 1.0f);
            if (m_prevTestOperation % 3 == 0)
            {
                VRageRender.MyRenderProxy.DebugDrawText2D(new Vector2(300.0f, 20.0f), "Intersection", Color.Red, 1.0f);
            }
            else if (m_prevTestOperation % 3 == 1)
            {
                VRageRender.MyRenderProxy.DebugDrawText2D(new Vector2(300.0f, 20.0f), "Union", Color.Red, 1.0f);
            }
            else if (m_prevTestOperation % 3 == 2)
            {
                VRageRender.MyRenderProxy.DebugDrawText2D(new Vector2(300.0f, 20.0f), "Difference", Color.Red, 1.0f);
            }

            if (DebugDrawMesh != null)
            {
                Matrix identity = Matrix.Identity;
                DebugDrawMesh.DebugDraw(ref identity, MyDebugDrawSettings.DEBUG_DRAW_NAVMESHES);
            }

            foreach (var poly in DebugDrawPolys)
            {
                MatrixD identity = MatrixD.Identity;
                poly.DebugDraw(ref identity);
            }

            MyPolygonBoolOps.Static.DebugDraw(MatrixD.Identity);

            if (Boxes != null)
            {
                foreach (var box in Boxes)
                {
                    VRageRender.MyRenderProxy.DebugDrawAABB(box, Color.Red, 1.0f, 1.0f, true);
                }
            }
        }
    }
}
