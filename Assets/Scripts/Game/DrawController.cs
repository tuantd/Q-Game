
// @tuantd 2015-02-01

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawController : MonoBehaviour
{
    private Transform _myTrans;
    private Transform _parentTrans;
    private GameObject _drawContainerGO;
    private GameObject _drawGO;
    private MeshFilter _curDrawMF;
    private Material _mat;
    private PhysicsMaterial2D _phyMat;
    private PolygonCollider2D _polyCol2D;
    private CircleCollider2D _circleCol2D;
    private MouseStatus _mouseStatus;

    private int _drawnVertNum; // num of drawn vertexs in a draw
    private float _degRad;
    private float _preDim; // pre draw dimension
    private float _dim;
    private Vector3 _preVertex;

    private Dictionary<int, List<Vector2>> _polyColPaths;
    private List<Vector2> _polyColPath2;
    private List<Vector2> _centers;
    private List<Vector3> _vertexs; // store all vertexs for mesh in a draw
    private List<int> _triangles; // store all triangles for mesh in a draw
    private Color[] _colors;
    
    // Use this for initialization
    void Start ()
    {
        _mat = new Material ("Shader \"Lines/Colored Blended\" {" +
            "SubShader { Tags { \"RenderType\"=\"Transparent\" } Pass { " +
            "    Blend SrcAlpha OneMinusSrcAlpha " +
            "    ZWrite On ZTest LEqual Cull Off Fog { Mode Off } " +
            "    BindChannels {" +
            "      Bind \"vertex\", vertex Bind \"color\", color }" +
            "} } }"
        );

        _phyMat = Resources.Load ("PhysicsMaterial/Draw2D") as PhysicsMaterial2D;

        _degRad = Mathf.PI / 180;

        _mouseStatus = GetComponentInChildren<MouseStatus> ();

        _myTrans = this.transform;
        _parentTrans = _myTrans.parent;

        _drawContainerGO = new GameObject ();
        _drawContainerGO.name = Constants.DRAW_CONTAINER_GO_NAME;
        _drawContainerGO.layer = Constants.LAYER_GAME;
        _drawContainerGO.transform.parent = _myTrans;
        _drawContainerGO.transform.localPosition = Vector3.zero;
        _drawContainerGO.transform.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update ()
    {
        if (!_mouseStatus.IsDrawing || !_mouseStatus.IsMoving) {
            return;
        }

        // loop through all the vertices in the vertexArr array.
        int drawIDMax = _mouseStatus.VertexArr.Count - 1;
        if (drawIDMax == -1)
            return;
        List<Vector3> vertexsInDraw = _mouseStatus.VertexArr [drawIDMax];

        int vertexIDMax = vertexsInDraw.Count - 1;
        if (vertexIDMax == -1)
            return;
        Vector3 vertex = vertexsInDraw [vertexIDMax] / _parentTrans.localScale.x;

        bool isFirstVertex = (vertexIDMax == 0) ? true : false; // first vertex in draw

        if (isFirstVertex) { // draw first vertex

            _drawGO = CreateDrawGO (drawIDMax);

            if (_centers != null) {
                _centers.Clear ();
            }
            if (_vertexs != null) {
                _vertexs.Clear ();
            }
            if (_triangles != null) {
                _triangles.Clear ();
            }
            _centers = new List<Vector2> ();  // contains all center points for a draw
            _vertexs = new List<Vector3> ();  // contains all vertexs for a draw
            _triangles = new List<int> ();  // contains all triangles for a draw

            //if (_polyColPaths != null) {
            //    _polyColPaths.Clear ();
            //}
            //_polyColPaths = new Dictionary<int, List<Vector2>> ();

            if (_polyColPath2 != null) {
                _polyColPath2.Clear ();
            }
            _polyColPath2 = new List<Vector2> ();

            _drawnVertNum = 0;
            _preDim = 10;

            UpdateMeshAtVertex (vertex);

            _preVertex = vertex;

            return;
        }

        // from second vertex
        bool isDrawn = CheckVertexsDistanceAndDraw (_preVertex, vertex); // check if the distance between two vertexs is longer than threshold, add more circles

        if (isDrawn) {
            //_preVertex = vertex;
        }
    }

    private bool CheckVertexsDistanceAndDraw (Vector3 preVertex, Vector3 vertex)
    {
        float dist = Vector3.Distance (preVertex, vertex);

        if (dist < Constants.VERTEXS_DISTANCE_MAX)
            return false;

        float deltaY = vertex.y - preVertex.y;
        float deltaX = vertex.x - preVertex.x;

        float angle = Mathf.Atan2 (deltaY, deltaX);

        int addedVertexsNum = (int)(dist / Constants.VERTEXS_DISTANCE_MAX);

        for (int i = 1; i <= addedVertexsNum; i++) {

            Vector3 addedVertex = new Vector3 (
                preVertex.x + Constants.VERTEXS_DISTANCE_MAX * i * Mathf.Cos (angle),
                preVertex.y + Constants.VERTEXS_DISTANCE_MAX * i * Mathf.Sin (angle),
                preVertex.z
            );

            UpdateMeshAtVertex (addedVertex);

            _preVertex = addedVertex;
        }

        //UpdateMeshAtVertex (vertex);

        return true;
    }

    private void UpdateMeshAtVertex (Vector3 vertex)
    {
        Mesh mesh = _curDrawMF.sharedMesh;
        if (mesh == null) {
            _curDrawMF.mesh = new Mesh ();
            mesh = _curDrawMF.sharedMesh;
            _colors = mesh.colors;
        }
        mesh.Clear ();

        _centers.Add (new Vector2 (
            vertex.x,
            vertex.y));
        
        _vertexs.Add (new Vector3 (
            vertex.x,
            vertex.y,
            vertex.z));

        //List<Vector2> polyColPath = new List<Vector2> ();

        int c = 0; // counter
        for (float theta = 0.0f; theta < (2 * Mathf.PI); theta += Constants.VERTEX_CIRCLE_STEP) {

            c++;
            Vector3 ci = new Vector3 (
                (Mathf.Cos (theta) * Constants.VERTEX_CIRCLE_RADIUS + vertex.x),
                (Mathf.Sin (theta) * Constants.VERTEX_CIRCLE_RADIUS + vertex.y),
                vertex.z
            );
            
            _vertexs.Add (ci);

            //if (c % 2 == 0 && !CheckVertIsInnerOtherVerts (ci.x, ci.y)) {
            //    polyColPath.Add (new Vector2 (ci.x, ci.y));
            //}
        }

        //_polyColPaths [_polyColPaths.Count] = polyColPath; // used in case of add poly col after

        // used in case of add poly col before
        //_polyCol2D.pathCount++;
        //_polyCol2D.SetPath (_polyCol2D.pathCount - 1, polyColPath.ToArray ());
        
        // triangles
        int v1, v2, v3;
        v1 = _vertexs.Count - c - 1;
        v2 = v1 + 1;
        v3 = v1 + 2;

        int max = 3 * c;
        for (int i = 0; i < max; i+=3) {
            _triangles.Add (v1);
            _triangles.Add (v2);
            
            v3 = (v3 == v1 + c + 1) ? v1 + 1 : v3;
            _triangles.Add (v3);
            
            v2++;
            v3++;
        }
        
        
        // assign data to mesh
        mesh.vertices = _vertexs.ToArray ();
        //mesh.uv = uvs;
        mesh.triangles = _triangles.ToArray ();
        
        // recalculations
        //mesh.RecalculateNormals ();
        //mesh.RecalculateBounds ();  
        //mesh.Optimize ();

        mesh.colors = _colors;

        _curDrawMF.mesh = mesh;

        _drawnVertNum++;
    }

    private GameObject CreateDrawGO (int drawID)
    {
        GameObject drawGO = new GameObject ();
        drawGO.name = Constants.DRAW_GO_NAME + drawID;
        drawGO.layer = Constants.LAYER_GAME;
        drawGO.tag = GameConstant.POLYGON_TAG;
        drawGO.transform.parent = _drawContainerGO.transform;
        drawGO.transform.localScale = Vector3.one;

        // add needed components
        _curDrawMF = drawGO.AddComponent ("MeshFilter") as MeshFilter;
        
        MeshRenderer mr = drawGO.AddComponent ("MeshRenderer") as MeshRenderer;
        mr.material = _mat;

        drawGO.AddComponent ("Draw");

        // used in case of add poly col before
        //_polyCol2D = drawGO.AddComponent ("PolygonCollider2D") as PolygonCollider2D;
        //_polyCol2D.sharedMaterial = _phyMat;

        return drawGO;
    }

    public void AddColliderAndRigid ()
    {
        if (!_drawGO)
            return;

        //MeshCollider meshCol = _drawGO.AddComponent ("MeshCollider") as MeshCollider;
        //meshCol.material = _phyMat;
        //meshCol.mesh = _curDrawMF.mesh;

        // used in case of add poly col after
        if (_centers.Count == 1) {
            _circleCol2D = _drawGO.AddComponent ("CircleCollider2D") as CircleCollider2D;
            _circleCol2D.sharedMaterial = _phyMat;
        } else {
            _polyCol2D = _drawGO.AddComponent ("PolygonCollider2D") as PolygonCollider2D;
            _polyCol2D.sharedMaterial = _phyMat;

            //_polyCol2D.pathCount = _polyColPaths.Count;
            //foreach (KeyValuePair<int, List<Vector2>> polyColPath in _polyColPaths) {
            //    _polyCol2D.SetPath (polyColPath.Key, polyColPath.Value.ToArray ());
            //}

            CalPolyColPath ();
            _polyCol2D.pathCount = 1;
            _polyCol2D.SetPath (0, _polyColPath2.ToArray ());
        }

        Rigidbody2D myRigidBody2D = _drawGO.AddComponent ("Rigidbody2D") as Rigidbody2D;
        myRigidBody2D.gravityScale = Constants.GRAVITY_SCALE;
        myRigidBody2D.drag = Constants.LINEAR_DRAG;
        myRigidBody2D.mass = Constants.MASS_UNIT * (2 * Constants.VERTEX_CIRCLE_RADIUS + (_centers.Count - 1) * Constants.VERTEXS_DISTANCE_MAX) / (2 * Constants.VERTEX_CIRCLE_RADIUS);
        myRigidBody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        // reset
        _drawGO = null;
    }

    private void CalPolyColPath ()
    {
        Vector2[] points;
        Vector2 preVert = _centers [0];

        int count = _centers.Count;

        List<Vector2> points1 = new List<Vector2> ();
        List<Vector2> points2 = new List<Vector2> ();

        for (int i=1; i<count; i++) {

            // smooth draw 
            if (i == 1) {

                for (int j=Constants.DRAW_SMOOTH_NUM; j>=1; j--) {

                    Vector2 delta = _centers [i] - preVert;
                    float dist = delta.magnitude;

                    Vector2 point = new Vector2 ();
                    point.x = preVert.x - delta.x * j * Constants.VERTEX_CIRCLE_RADIUS / (Constants.DRAW_SMOOTH_NUM * dist);
                    point.y = preVert.y - delta.y * j * Constants.VERTEX_CIRCLE_RADIUS / (Constants.DRAW_SMOOTH_NUM * dist);

                    float scale = (float)(Constants.DRAW_SMOOTH_NUM - j) / Constants.DRAW_SMOOTH_NUM;
                    if (j == 2) {
                        scale += Constants.EPS;
                    } else if (j == 1) {
                        scale *= 1.73f;
                    }
                    points = Cal2PointsOn2Sides (point, preVert, scale);
                    
                    points1.Add (points [0]);
                    points2.Add (points [1]);
                }
            }

            points = Cal2PointsOn2Sides (preVert, _centers [i]);

            points1.Add (points [0]);
            points2.Add (points [1]);

            // smooth draw 
            if (i == count - 1) {

                Vector2 temp = _centers [i];
                for (int j=1; j<=Constants.DRAW_SMOOTH_NUM + 1; j++) {
                    
                    Vector2 delta = _centers [i] - preVert;
                    float dist = delta.magnitude;
                    
                    Vector2 point = new Vector2 ();
                    point.x = _centers [i].x + delta.x * j * Constants.VERTEX_CIRCLE_RADIUS / (Constants.DRAW_SMOOTH_NUM * dist);
                    point.y = _centers [i].y + delta.y * j * Constants.VERTEX_CIRCLE_RADIUS / (Constants.DRAW_SMOOTH_NUM * dist);

                    float scale = (float)(Constants.DRAW_SMOOTH_NUM - j + 1) / Constants.DRAW_SMOOTH_NUM;
                    if (j == 2) {
                        scale *= 1.73f;
                    } else if (j == Constants.DRAW_SMOOTH_NUM + 1) {
                        scale += Constants.EPS;
                    }
                    
                    points = Cal2PointsOn2Sides (temp, point, scale);
                    
                    points1.Add (points [0]);
                    points2.Add (points [1]);

                    temp = point;
                }
            }

            if (_preDim != 10) {

                if (Mathf.Abs (_dim - _preDim) > (Mathf.PI / 2)) {

                    for (int j=Constants.DRAW_SMOOTH_NUM; j>=1; j--) {
                        
                        Vector2 delta = _centers [i] - preVert;
                        float dist = delta.magnitude;
                        
                        Vector2 point = new Vector2 ();
                        point.x = preVert.x - delta.x * j * Constants.VERTEX_CIRCLE_RADIUS / (Constants.DRAW_SMOOTH_NUM * dist);
                        point.y = preVert.y - delta.y * j * Constants.VERTEX_CIRCLE_RADIUS / (Constants.DRAW_SMOOTH_NUM * dist);
                        
                        float scale = (float)(Constants.DRAW_SMOOTH_NUM - j) / Constants.DRAW_SMOOTH_NUM;
                        if (j == 2) {
                            scale += Constants.EPS;
                        } else if (j == 1) {
                            scale *= 1.73f;
                        }
                        points = Cal2PointsOn2Sides (point, preVert, scale);
                        
                        points1.Add (points [0]);
                        points2.Add (points [1]);
                    }
                }
            }

            _preDim = _dim;

            preVert = _centers [i];
        }

        count = points1.Count;
        for (int i=0; i<count; i++) {

            _polyColPath2.Add (points1 [i]);
        }

        count = points2.Count;
        for (int i=count-1; i>=0; i--) {
            
            _polyColPath2.Add (points2 [i]);
        }
    }

    private Vector2[] Cal2PointsOn2Sides (Vector2 preVert, Vector2 vertex, float scale = 1)
    {
        Vector2 delta = vertex - preVert;

        Vector2[] points = new Vector2[2];
        if (delta.x == 0) {
            
            points [0] = new Vector2 (preVert.x + Constants.VERTEX_CIRCLE_RADIUS * scale, preVert.y);
            points [1] = new Vector2 (preVert.x - Constants.VERTEX_CIRCLE_RADIUS * scale, preVert.y);

            _dim = Mathf.PI / 2;

            if (delta.y < 0) {
                Vector2 temp = points [0];
                points [0] = points [1];
                points [1] = temp;

                _dim = - Mathf.PI / 2;
            }
        } else if (delta.y == 0) {

            points [0] = new Vector2 (preVert.x, preVert.y + Constants.VERTEX_CIRCLE_RADIUS * scale);
            points [1] = new Vector2 (preVert.x, preVert.y - Constants.VERTEX_CIRCLE_RADIUS * scale);

            _dim = Mathf.PI;

            if (delta.x > 0) {
                Vector2 temp = points [0];
                points [0] = points [1];
                points [1] = temp;

                _dim = 0;
            }
        } else {
            
            float alpha1 = Mathf.Atan (- delta.x / delta.y);
            float alpha2 = alpha1 + Mathf.PI;

            _dim = Mathf.PI / 2 - Mathf.Abs (alpha1);
            if (delta.x > 0) {
                if (delta.y < 0) {
                    _dim = - _dim;
                }
            } else if (delta.x < 0) {
                if (delta.y > 0) {
                    _dim = Mathf.PI - _dim;
                } else if (delta.y < 0) {
                    _dim = _dim - Mathf.PI;
                }
            }
            
            points [0] = new Vector2 ();
            points [1] = new Vector2 ();
            
            points [0].x = preVert.x + Constants.VERTEX_CIRCLE_RADIUS * scale * Mathf.Cos (alpha1);
            points [1].x = preVert.x + Constants.VERTEX_CIRCLE_RADIUS * scale * Mathf.Cos (alpha2);
            
            points [0].y = preVert.y + Constants.VERTEX_CIRCLE_RADIUS * scale * Mathf.Sin (alpha1);
            points [1].y = preVert.y + Constants.VERTEX_CIRCLE_RADIUS * scale * Mathf.Sin (alpha2);

            if (delta.y < 0) {
                Vector2 temp = points [0];
                points [0] = points [1];
                points [1] = temp;
            }
        }

        return points;
    }

    private bool CheckVertIsInnerOtherVerts (float x, float y)
    {
        bool oddNodes = false;

        foreach (KeyValuePair<int, List<Vector2>> polyColPath in _polyColPaths) {
            List<Vector2> vertexs = polyColPath.Value;
            int vertexNum = vertexs.Count;
            int i, j = vertexNum - 1;

            for (i=0; i<vertexNum; i++) {
                if ((vertexs [i].y < y && vertexs [j].y >= y
                    || vertexs [j].y < y && vertexs [i].y >= y)
                    && (vertexs [i].x <= x || vertexs [j].x <= x)) {
                    if (vertexs [i].x + (y - vertexs [i].y) / (vertexs [j].y - vertexs [i].y) * (vertexs [j].x - vertexs [i].x) < x) {
                        oddNodes = !oddNodes;
                    }
                }
                j = i;
            }

            if (oddNodes)
                return oddNodes;
        }

        return oddNodes;
    }

    // not use
    private void DrawCircleAtVertex (Vector3 vertex)
    {
        GL.PushMatrix ();
        _mat.SetPass (0);
        GL.LoadOrtho ();
        GL.Begin (GL.LINES);
        GL.Color (Color.white);
        
        for (float theta = 0.0f; theta < (2 * Mathf.PI); theta += Constants.VERTEX_CIRCLE_STEP) {
            
            GL.Vertex (new Vector3 (
                vertex.x / Screen.width,
                vertex.y / Screen.height,
                vertex.z));
            
            Vector3 ci = new Vector3 (
                (Mathf.Cos (theta) * Constants.VERTEX_CIRCLE_RADIUS + vertex.x) / Screen.width,
                (Mathf.Sin (theta) * Constants.VERTEX_CIRCLE_RADIUS + vertex.y) / Screen.height,
                vertex.z
            );
            
            GL.Vertex (ci);
        }
        
        GL.End ();
        GL.PopMatrix ();
    }
}
