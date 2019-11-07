using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour {
    #region Public Fields
    static public PlayerInputHandler S;

    #endregion

    #region Private Fields
    [SerializeField] private Vector3 startPosition = Vector3.zero;
    [SerializeField] private Vector3 currentTouchPosition;
    [SerializeField] private float _rayFreeLength = 5f;
    [SerializeField] private float _rayReflectedLength = 8.5f;
    private Camera _mainCam;
    private LineDrawer _lineDrawer;
    private PlayerBall _currentPlayerBall;
    #endregion

    #region Constants
    public const float SCREEN_BOUND_X = 2.8f;
    public const float SCREEN_BOUND_Y = 9.5f;
    private const float SCREEN_BOUND_X_SQR = SCREEN_BOUND_X * SCREEN_BOUND_X;
    private const float MAX_WALL_HIT_Y = 6.5f;
    private const float INITIAL_REFLECTED_RAY_LENGTH = 0.4f;
    private const float EARTH_HIT_REFLECTION_HEIGHT = 4.5f;
    private const float REFLECTION_RAY_MODIFIER = 10f;
    private const float MAX_REFLECTED_RAY_LENGTH = 0.3f;
    
    #endregion

    #region Monobehaviour Callbacks
    private void Awake()
    {
        if (S != null)
            Destroy(gameObject);
        S = this;
    }

    private void OnEnable()
    {
        PlayerBall.OnAsteroidHit += ClearCurrentBall;
        PlayerBall.OnLastAsteroidHit += ClearCurrentBall;
    }

    private void OnDisable()
    {
        PlayerBall.OnAsteroidHit -= ClearCurrentBall;
        PlayerBall.OnLastAsteroidHit -= ClearCurrentBall;
    }

    private void Start()
    {
        if (!_mainCam)
            _mainCam = Camera.main;
        if (!_lineDrawer)
            _lineDrawer = GetComponent<LineDrawer>();
    }

    private void Update()
    {
        if(GameManager.S.gameMode == GameMode.Play && _currentPlayerBall && _currentPlayerBall.transform.position == Vector3.zero)
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE
            
        if (Input.touchCount > 0)
            {
                //Debug.Log("y: " + Input.GetTouch(0).position.y);
                if (Input.GetTouch(0).position.y > Screen.height * 0.15f)
                    OnTouchDown();
                if (Input.GetTouch(0).phase == TouchPhase.Ended && Input.GetTouch(0).position.y > Screen.height * 0.15f)
                    OnTouchUp();
                else if (Input.GetTouch(0).position.y <= Screen.height * 0.15f)
                    _lineDrawer.ClearLine();
            }
         

#elif UNITY_EDITOR || UNITY_STANDALONE
             if (Input.mousePosition.y > 40f)
             {
                 if (Input.GetMouseButton(0))
                     MouseDown();
                 else if (Input.GetMouseButtonUp(0))
                     MouseUp();
                else if (Input.mousePosition.y <= 40f)
                    _lineDrawer.ClearLine();
            }

#endif
        }

    }
#endregion

#region Public Methods
    public void SetCurrentBall(PlayerBall pb)
    {
        _currentPlayerBall = pb;
    }

    public PlayerBall GetCurrentBall()
    {
        return _currentPlayerBall;
    }
#endregion

#region Private Methods
    private void OnTouchDown()
    {
        //Getting the current touch position
        currentTouchPosition = _mainCam.ScreenToWorldPoint(Input.GetTouch(0).position);
        currentTouchPosition.z = 0f;
        Vector3 angleCalculationVector = new Vector3(currentTouchPosition.x, 0f, 0f);
        _rayReflectedLength = INITIAL_REFLECTED_RAY_LENGTH;

        //calculating the first hit point
        float angle = Vector3.SignedAngle(angleCalculationVector, currentTouchPosition, Vector3.forward);
        float cos = Mathf.Cos(Mathf.Deg2Rad * angle);
        float hypotenuse = SCREEN_BOUND_X / cos;
        float oppositeY = Mathf.Sqrt(Mathf.Pow(hypotenuse, 2) - SCREEN_BOUND_X_SQR);

        Vector3 wallHitPoint = Vector3.zero;
        if (currentTouchPosition.x > 0)
        {
            //if wallHitPoint.y is above the MAX_WALL_HIT_Y, the line will not hit the wall
            if (oppositeY > MAX_WALL_HIT_Y)
            {
                wallHitPoint = new Vector3(SCREEN_BOUND_X, oppositeY, startPosition.z);
                wallHitPoint = Vector3.ClampMagnitude(wallHitPoint, _rayFreeLength);
                _lineDrawer.DrawLine(startPosition, wallHitPoint, wallHitPoint);
                //_lineDrawer.AnimateLine();
                return;
            }

            //if wallHitPoint.y is below the MAX_WALL_HIT_Y, the line hits the wall and breaks
            wallHitPoint = new Vector3(SCREEN_BOUND_X, oppositeY, startPosition.z);
        }
        else
        {
            if (oppositeY > MAX_WALL_HIT_Y)
            {
                //if wallHitPoint.y is above the MAX_WALL_HIT_Y, the line will not hit the wall
                wallHitPoint = new Vector3(-SCREEN_BOUND_X, oppositeY, startPosition.z);
                wallHitPoint = Vector3.ClampMagnitude(wallHitPoint, _rayFreeLength);
                _lineDrawer.DrawLine(startPosition, wallHitPoint, wallHitPoint);
                return;

            }
            //if wallHitPoint.y is below the MAX_WALL_HIT_Y, the line hits the wall and breaks
            wallHitPoint = new Vector3(-SCREEN_BOUND_X, oppositeY, startPosition.z);
        }

        //calculating the second hit point
        float reflectionAngle = 0f;
        if (currentTouchPosition.x > 0)
            reflectionAngle = 90f - angle;
        else
            reflectionAngle = -1 * (90f + angle);

        float ceilY = SCREEN_BOUND_Y - wallHitPoint.y;
        float tan = Mathf.Tan(Mathf.Deg2Rad * reflectionAngle);
        float ceilX = 0f;

        if (currentTouchPosition.x > 0)
            ceilX = SCREEN_BOUND_X - Mathf.Abs(tan * ceilY);
        else
            ceilX = -SCREEN_BOUND_X + Mathf.Abs(tan * ceilY);

        Vector3 ceilHitPoint = new Vector3(ceilX, SCREEN_BOUND_Y, 0f);

        //Adjusting ray length to not hit earth
        if (wallHitPoint.y < EARTH_HIT_REFLECTION_HEIGHT)
        {
            _rayReflectedLength -= (EARTH_HIT_REFLECTION_HEIGHT - wallHitPoint.y) / REFLECTION_RAY_MODIFIER;
        }
        else
        {
            _rayReflectedLength = INITIAL_REFLECTED_RAY_LENGTH + MAX_REFLECTED_RAY_LENGTH;//Mathf.Max(MAX_REFLECTED_RAY_LENGTH, (wallHitPoint.y - (REFLECTION_RAY_MODIFIER/2f)));
        }


        ceilHitPoint = (ceilHitPoint - wallHitPoint) * _rayReflectedLength + wallHitPoint;

        //drawing the line
        _lineDrawer.DrawLine(startPosition, wallHitPoint, ceilHitPoint);
        //_lineDrawer.AnimateLine();
    }

    private void MouseDown()
    {
        //Getting the current mouse position
        currentTouchPosition = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        currentTouchPosition.z = 0f;
        Vector3 angleCalculationVector = new Vector3(currentTouchPosition.x, 0f, 0f);
        _rayReflectedLength = INITIAL_REFLECTED_RAY_LENGTH;

        //calculating the first hit point
        float angle = Vector3.SignedAngle(angleCalculationVector, currentTouchPosition, Vector3.forward);
        float cos = Mathf.Cos(Mathf.Deg2Rad * angle);
        float hypotenuse = SCREEN_BOUND_X / cos;
        float oppositeY = Mathf.Sqrt(Mathf.Pow(hypotenuse, 2) - SCREEN_BOUND_X_SQR);

        Vector3 wallHitPoint = Vector3.zero;
        if (currentTouchPosition.x > 0)
        {
            //if wallHitPoint.y is above the MAX_WALL_HIT_Y, the line will not hit the wall
            if (oppositeY > MAX_WALL_HIT_Y)
            {
                wallHitPoint = new Vector3(SCREEN_BOUND_X, oppositeY, startPosition.z);
                wallHitPoint = Vector3.ClampMagnitude(wallHitPoint, _rayFreeLength);
                _lineDrawer.DrawLine(startPosition, wallHitPoint, wallHitPoint);
                //_lineDrawer.AnimateLine();
                return;
            }

            //if wallHitPoint.y is below the MAX_WALL_HIT_Y, the line hits the wall and breaks
            wallHitPoint = new Vector3(SCREEN_BOUND_X, oppositeY, startPosition.z);
        }
        else
        {
            if (oppositeY > MAX_WALL_HIT_Y)
            {
                //if wallHitPoint.y is above the MAX_WALL_HIT_Y, the line will not hit the wall
                wallHitPoint = new Vector3(-SCREEN_BOUND_X, oppositeY, startPosition.z);
                wallHitPoint = Vector3.ClampMagnitude(wallHitPoint, _rayFreeLength);
                _lineDrawer.DrawLine(startPosition, wallHitPoint, wallHitPoint);
                return;

            }
            //if wallHitPoint.y is below the MAX_WALL_HIT_Y, the line hits the wall and breaks
            wallHitPoint = new Vector3(-SCREEN_BOUND_X, oppositeY, startPosition.z);
        }

        //calculating the second hit point
        float reflectionAngle = 0f;
        if (currentTouchPosition.x > 0)
            reflectionAngle = 90f - angle;
        else
            reflectionAngle = -1 * (90f + angle);

        float ceilY = SCREEN_BOUND_Y - wallHitPoint.y;
        float tan = Mathf.Tan(Mathf.Deg2Rad * reflectionAngle);
        float ceilX = 0f;

        if (currentTouchPosition.x > 0)
            ceilX = SCREEN_BOUND_X - Mathf.Abs(tan * ceilY);
        else
            ceilX = -SCREEN_BOUND_X + Mathf.Abs(tan * ceilY);

        Vector3 ceilHitPoint = new Vector3(ceilX, SCREEN_BOUND_Y, 0f);
        
        //Adjusting ray length to not hit earth
        if(wallHitPoint.y < EARTH_HIT_REFLECTION_HEIGHT)
        {
            _rayReflectedLength -= (EARTH_HIT_REFLECTION_HEIGHT - wallHitPoint.y)/REFLECTION_RAY_MODIFIER;
        }
        else 
        {
            _rayReflectedLength = INITIAL_REFLECTED_RAY_LENGTH + MAX_REFLECTED_RAY_LENGTH;
        }
        

        ceilHitPoint = (ceilHitPoint - wallHitPoint) * _rayReflectedLength + wallHitPoint;

        //drawing the line
        _lineDrawer.DrawLine(startPosition, wallHitPoint, ceilHitPoint);
        //_lineDrawer.AnimateLine();
    }

    private void OnTouchUp()
    {
        if (_currentPlayerBall)
        {
            _currentPlayerBall.Shoot(_lineDrawer.GetPoints());
        }
           
        _lineDrawer.ClearLine();
    }

    private void MouseUp()
    {
        if (_currentPlayerBall)
        {
            _currentPlayerBall.Shoot(_lineDrawer.GetPoints());
        }
            
        _lineDrawer.ClearLine();
    }

    private void ClearCurrentBall()
    {
        _currentPlayerBall = null;
    }
#endregion


}
