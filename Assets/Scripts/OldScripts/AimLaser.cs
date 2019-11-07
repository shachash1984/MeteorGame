using UnityEngine;
 
public class AimLaser : MonoBehaviour {

    private LineRenderer _line;
	public float laserWidth = 1.0f;

	private float maxStepDistance;
	public float noise = 1.0f;
    public float maxLength = 50.0f;
    public Color color = Color.red;
	//private Ray _ray;
    //[SerializeField] private float _reflectDistance = 0.2f;

    private void Start()
	 {
		 //_ray = new Ray(transform.position, transform.forward);
        _line = GetComponent<LineRenderer>();
        //_line.SetVertexCount(2);
		maxStepDistance = 200f;
		_line.enabled = false;
    }
 
    private void Update() {
        // RaycastHit hit;
        // if (Physics.Raycast(_ray, out hit)) 
		// {
		// 	Debug.Log(hit);
        //     if (hit.collider != null)
		// 	 {
		// 		Vector3 reflectAngle = Vector3.Reflect(_ray.direction, hit.normal) ;
        //    		_line.SetPositions(new[] {transform.position, MobileInput.Instance.swipeDelta, reflectAngle});
		// 		   _line.	
		// 	 }

        //} 
		if(!MobileInput.Instance.IsBreakingStuff())
		{
		DrawPredictedReflectionPattern(transform.position + transform.forward * 0.75f, transform.forward,2);     
		SetBallState();
		}
    }

	void SetBallState()
	{
		if(!MobileInput.Instance.IsBreakingStuff())
		{
			_line.enabled = true;
		}
		else
		{
			_line.enabled = false;	
		}
	}
	 private void DrawPredictedReflectionPattern(Vector3 position, Vector3 direction, int reflectionsRemaining)
    {
        if (reflectionsRemaining == 0) {
            return;
        }

        //Vector3 startingPosition = position;
		
        Ray ray = new Ray(position, direction);
        RaycastHit hit;
		// float x,y,z;
		// float angle = Vector3.Angle(MobileInput.Instance.swipeDelta, startingPosition);
		// z = Mathf.Cos(angle) * 4; 
		// y = Mathf.Sqrt(Mathf.Pow(z,2) - Mathf.Pow(4,2));
		// x = 4;

		//Vector3 coliderHit = new Vector3(x,y,z);
		
        if (Physics.Raycast(ray, out hit, maxStepDistance))
        {
            direction = Vector3.Reflect(position, hit.normal);
            position = hit.point;	
        }
        else
        {
            position += direction * maxStepDistance;
        }
        _line.SetPositions(new[] {transform.position, MobileInput.Instance.swipeDelta, direction});
		
        DrawPredictedReflectionPattern(position, direction, reflectionsRemaining - 1);
    }
}