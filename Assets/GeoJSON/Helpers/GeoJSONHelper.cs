using System.Collections.Generic;
using LibTessDotNet;
using UnityEngine;

namespace GeoJSON
{
    public static class GeoJSONHelper
    {
        public static Vector3 CoordinatesToVector3(List<double> coords)
        {
            if (coords == null || coords.Count < 2)
            {
                return Vector3.zero;
            }
            float x = (float)coords[0];
            float y = coords.Count > 2 ? (float)coords[2] : 0f;
            float z = (float)coords[1];
            return new Vector3(x, y, z);
        }

        public static GameObject CreateLineStringObject(LineString lineString, float lineThickness, Color color)
        {
            GameObject lineObj = new GameObject("LineString");

            if (lineString.coordinates != null && lineString.coordinates.Count > 0)
            {
                LineRenderer lr = lineObj.AddComponent<LineRenderer>();
                lr.positionCount = lineString.coordinates.Count;
                lr.startWidth = lineThickness;
                lr.endWidth = lineThickness;
                lr.useWorldSpace = true;
                lr.material = new Material(Shader.Find("Sprites/Default"));
                lr.startColor = color;
                lr.endColor = color;

                for (int i = 0; i < lineString.coordinates.Count; i++)
                {
                    lr.SetPosition(i, CoordinatesToVector3(lineString.coordinates[i]));
                }
            }

            return lineObj;
        }

        public static GameObject CreateMultiLineStringObject(MultiLineString multiLineString, float lineThickness, Color color)
        {
            GameObject container = new GameObject("MultiLineString");

            if (multiLineString.coordinates != null)
            {
                for (int i = 0; i < multiLineString.coordinates.Count; i++)
                {
                    LineString ls = new LineString();
                    ls.coordinates = multiLineString.coordinates[i];
                    GameObject lineObj = CreateLineStringObject(ls, lineThickness, color);
                    lineObj.name = $"LineString_{i}";
                    lineObj.transform.parent = container.transform;
                }
            }

            return container;
        }

        public static GameObject CreatePolygonObject(Polygon polygon, float lineThickness, Color color, Color lineColor)
        {
            GameObject polygonObj = new GameObject("Polygon");

            if (polygon.coordinates == null || polygon.coordinates.Count == 0)
            {
                return polygonObj;
            }

            var tess = new Tess();
            List<List<double>> outerRing = polygon.coordinates[0];
            ContourVertex[] outerContour = new ContourVertex[outerRing.Count];
            for (int i = 0; i < outerRing.Count; i++)
            {
                Vector3 v = CoordinatesToVector3(outerRing[i]);
                outerContour[i].Position = new Vec3 { X = v.x, Y = v.y, Z = v.z };
            }
            tess.AddContour(outerContour, ContourOrientation.Original);

            for (int r = 1; r < polygon.coordinates.Count; r++)
            {
                List<List<double>> hole = polygon.coordinates[r];
                ContourVertex[] holeContour = new ContourVertex[hole.Count];
                for (int i = 0; i < hole.Count; i++)
                {
                    Vector3 v = CoordinatesToVector3(hole[i]);
                    holeContour[i].Position = new Vec3 { X = v.x, Y = v.y, Z = v.z };
                }
                tess.AddContour(holeContour, ContourOrientation.Original);
            }

            tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3);

            if (tess.Vertices != null && tess.Vertices.Length > 0)
            {
                Vector3[] vertices = new Vector3[tess.Vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    var p = tess.Vertices[i].Position;
                    vertices[i] = new Vector3(p.X, p.Y, p.Z);
                }

                int[] triangles = tess.Elements;
                Material mat = new Material(Shader.Find("Sprites/Default"));
                mat.color = color;

                UnityEngine.Mesh mesh = new UnityEngine.Mesh();
                mesh.name = "PolygonMesh";
                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();

                polygonObj.AddComponent<MeshFilter>().mesh = mesh;
                polygonObj.AddComponent<MeshRenderer>().material = mat;
            }

            GameObject border = new GameObject("Border");
            border.transform.SetParent(polygonObj.transform, false);
            LineRenderer lr = border.AddComponent<LineRenderer>();
            lr.positionCount = outerRing.Count + 1;
            lr.loop = false;
            lr.useWorldSpace = true;
            lr.startWidth = lineThickness;
            lr.endWidth = lineThickness;
            Material lrMat = new Material(Shader.Find("Sprites/Default"));
            lr.material = lrMat;
            lr.startColor = lineColor;
            lr.endColor = lineColor;

            for (int i = 0; i < outerRing.Count; i++)
            {
                lr.SetPosition(i, CoordinatesToVector3(outerRing[i]));
            }
            lr.SetPosition(outerRing.Count, CoordinatesToVector3(outerRing[0]));

            return polygonObj;
        }

        public static GameObject CreateMultiPolygonObject(MultiPolygon multiPolygon, float lineThickness, Color color, Color lineColor)
        {
            GameObject container = new GameObject("MultiPolygon");

            if (multiPolygon.coordinates != null)
            {
                for (int i = 0; i < multiPolygon.coordinates.Count; i++)
                {
                    Polygon polygon = new Polygon();
                    polygon.coordinates = multiPolygon.coordinates[i];
                    GameObject polygonObj = CreatePolygonObject(polygon, lineThickness, color, lineColor);
                    polygonObj.name = $"Polygon_{i}";
                    polygonObj.transform.parent = container.transform;
                }
            }

            return container;
        }

        public static GameObject ProcessGeometry(Geometry geometry, float lineThickness, Color color, Color lineColor)
        {
            if (geometry == null)
            {
                return new GameObject("Null_Geometry");
            }

            if (geometry is Point point)
            {
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                obj.name = "Point";
                obj.transform.position = CoordinatesToVector3(point.coordinates);
                obj.transform.localScale = Vector3.one * lineThickness * 2f;
                obj.GetComponent<Renderer>().material.color = color;
                return obj;
            }
            else if (geometry is LineString lineString)
            {
                return CreateLineStringObject(lineString, lineThickness, color);
            }
            else if (geometry is MultiLineString multiLineString)
            {
                return CreateMultiLineStringObject(multiLineString, lineThickness, color);
            }
            else if (geometry is Polygon polygon)
            {
                return CreatePolygonObject(polygon, lineThickness, color, lineColor);
            }
            else if (geometry is MultiPolygon multiPolygon)
            {
                return CreateMultiPolygonObject(multiPolygon, lineThickness, color, lineColor);
            }
            else if (geometry is GeometryCollection collection)
            {
                GameObject container = new GameObject("GeometryCollection");
                for (int i = 0; i < collection.geometries.Count; i++)
                {
                    GameObject child = ProcessGeometry(collection.geometries[i], lineThickness, color, lineColor);
                    child.name = $"Geometry_{i}";
                    child.transform.parent = container.transform;
                }
                return container;
            }

            return new GameObject($"Unsupported_{geometry.GetType().Name}");
        }

        public static GameObject ProcessFeatureCollection(FeatureCollection fc, float lineThickness, Color defaultColor, Color defaultLineColor)
        {
            GameObject container = new GameObject("FeatureCollection");

            if (fc.features == null)
            {
                return container;
            }

            foreach (Feature feature in fc.features)
            {
                if (feature.geometry == null)
                {
                    continue;
                }

                Color featureColor = defaultColor;
                if (feature.properties != null && feature.properties.ContainsKey("color"))
                {
                    string colorStr = feature.GetProperty<string>("color");
                    if (!string.IsNullOrEmpty(colorStr))
                    {
                        if (!ColorUtility.TryParseHtmlString(colorStr, out featureColor))
                        {
                            featureColor = defaultColor;
                        }
                    }
                }

                GameObject geomObj = ProcessGeometry(feature.geometry, lineThickness, featureColor, featureColor);
                geomObj.transform.parent = container.transform;
            }

            return container;
        }
    }
}
