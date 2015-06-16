
// @tuantd 2015-02-01

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MouseStatus : MonoBehaviour
{
    public bool isEnableRange;
    private DrawController _drawController;

    private Transform _myTrans;
    private Transform _parentTrans;
    private Camera _camera;
    private Dictionary<int, List<Vector3>> _vertexArr;
    private Vector3 _vertex;
    private Vector3 _preVertex;
    private Vector3 _mousePos;
    private Status _status;
    private Status _preStatus;
    private bool _isDrawing;
    private bool _isMoving;
    private int _drawID;
    private int _preDrawID;

    public Dictionary<int, List<Vector3>> VertexArr {
        get { return _vertexArr; }
    }

    public Status CurStatus {
        get { return _status; }
    }
    
    public Status PreStatus {
        get { return _preStatus; }
    }

    public bool IsDrawing {
        get { return _isDrawing; }
        set { _isDrawing = value; }
    }

    public bool IsMoving {
        get { return _isMoving; }
    }

    public int DrawID {
        get { return _drawID; }
    }

    public int PreDrawID {
        get { return _preDrawID; }
    }

    public enum Status
    {
        Down,
        Up,
        HeldDown,
        Over
    }

    // Use this for initialization
    void Start ()
    {
        _camera = Camera.main;

        _vertexArr = new Dictionary<int, List<Vector3>> ();
        _preStatus = _status = Status.Over;
        _isDrawing = _isMoving = false;
        _drawID = _preDrawID = -1;

        _drawController = GetComponentInChildren<DrawController> ();

        _myTrans = this.transform;
        _parentTrans = _myTrans.parent;
    }
    
    // Update is called once per frame
    void Update ()
    {
        _preStatus = _status;
        _preDrawID = _drawID;

        if (Input.GetButtonDown (Constants.BTN_NAME)) {

            _status = Status.Down;
            _isDrawing = true;
            _drawID++;

        } else if (Input.GetButtonUp (Constants.BTN_NAME)) {

            MouseUpCallback ();

        } else if (Input.GetButton (Constants.BTN_NAME)) {

            _status = Status.HeldDown;

        } else {

            _status = Status.Over;
        }

        AddDrawnVertex ();
    }

    private void AddDrawnVertex ()
    {
        _mousePos = _camera.ScreenToWorldPoint (Input.mousePosition);

        if (_isDrawing == false) // if not drawing, dont do anything
            return;

        if (CheckRaycastHitOrTouchOutSide ()) {
            if (IsNewDraw ()) {
                _drawID--;
            }
            return;
        }
            
        _vertex = new Vector3 (_mousePos.x, _mousePos.y, 0);
            
        if (IsNewDraw ()) {

            List<Vector3> newDrawList = new List<Vector3> ();
            newDrawList.Add (_vertex);
            _vertexArr [_drawID] = newDrawList; // add new list to store all points in a drawn

            _preVertex = _vertex;
            _isMoving = true;
            
        } else if (Vector3.Distance (_preVertex, _vertex) >= Constants.VERTEXS_DISTANCE_MIN) {

            _vertexArr [_drawID].Add (_vertex);

            _preVertex = _vertex;
            _isMoving = true;
        } else {
            _isMoving = false;
        }
    }

    private bool CheckRaycastHitOrTouchOutSide ()
    {
        if (IsNewDraw ()) {
            float eps = Time.deltaTime * 0.01f;
            _vertex = _mousePos + new Vector3 (0, eps, 0);
        }

        if (isEnableRange) {
            // check if user touched outside game area
            if (_mousePos.x * _parentTrans.localScale.x < Constants.BORDER_LEFT || _mousePos.x * _parentTrans.localScale.x > Constants.BORDER_RIGHT || _mousePos.y * _parentTrans.localScale.y < Constants.BORDER_BOTTOM) {
	            
                if (IsNewDraw ()) {
                    MouseUpCallback ();
                }
                return true;
            }
        }
        Vector2 delta = _mousePos - _vertex;
        
        RaycastHit2D hit = Physics2D.Raycast (
            new Vector2 (_vertex.x, _vertex.y),
            delta,
            delta.magnitude + Constants.VERTEX_CIRCLE_RADIUS);

        if (hit) {

            if (!IsWall (hit.transform.name) || IsNewDraw ()) {
                MouseUpCallback ();
            }
            return true;
        }

        // check raycast in two parallel lines 
        Vector2 rayOriPoint1, rayOriPoint2;
        if (delta.x == 0) {

            rayOriPoint1 = new Vector2 (_vertex.x + Constants.VERTEX_CIRCLE_RADIUS, _vertex.y);
            rayOriPoint2 = new Vector2 (_vertex.x - Constants.VERTEX_CIRCLE_RADIUS, _vertex.y);
        } else if (delta.y == 0) {

            rayOriPoint1 = new Vector2 (_vertex.x, _vertex.y + Constants.VERTEX_CIRCLE_RADIUS);
            rayOriPoint2 = new Vector2 (_vertex.x, _vertex.y - Constants.VERTEX_CIRCLE_RADIUS);
        } else {

            float alpha1 = Mathf.Atan (- delta.x / delta.y);
            float alpha2 = alpha1 + Mathf.PI;

            rayOriPoint1 = new Vector2 ();
            rayOriPoint2 = new Vector2 ();

            rayOriPoint1.x = _vertex.x + Constants.VERTEX_CIRCLE_RADIUS * Mathf.Cos (alpha1);
            rayOriPoint2.x = _vertex.x + Constants.VERTEX_CIRCLE_RADIUS * Mathf.Cos (alpha2);

            rayOriPoint1.y = _vertex.y + Constants.VERTEX_CIRCLE_RADIUS * Mathf.Sin (alpha1);
            rayOriPoint2.y = _vertex.y + Constants.VERTEX_CIRCLE_RADIUS * Mathf.Sin (alpha2);
        }

        RaycastHit2D hit1 = Physics2D.Raycast (
            rayOriPoint1,
            delta,
            delta.magnitude + Constants.VERTEX_CIRCLE_RADIUS);

        RaycastHit2D hit2 = Physics2D.Raycast (
            rayOriPoint2,
            delta,
            delta.magnitude + Constants.VERTEX_CIRCLE_RADIUS);

        if (hit1) {

            if (!IsWall (hit1.transform.name) || IsNewDraw ()) {
                MouseUpCallback ();
            }
            return true;
        }
        if (hit2) {
            
            if (!IsWall (hit2.transform.name) || IsNewDraw ()) {
                MouseUpCallback ();
            }
            return true;
        }

        return false;
    }

    private void MouseUpCallback ()
    {
        if (!_isDrawing)
            return;

        _status = Status.Up;
        _isDrawing = false;
        
        // callback to DrawController to add collider to drawn object
        _drawController.AddColliderAndRigid ();
    }

    private bool IsNewDraw ()
    {
        return _preDrawID != _drawID;
    }

    private bool IsWall (string name)
    {
        if (name == "Left" || name == "Right" || name == "Bottom")
            return true;

        return false;
    }
}
