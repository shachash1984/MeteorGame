using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LineDrawer : MonoBehaviour {

    #region Private Fields
    private LineRenderer _line;
    private LineRenderer _line2;
    #endregion

    #region Monobehaviour Callbacks
    private void Start()
    {
        if (!_line)
            _line = GetComponent<LineRenderer>();
        if (!_line2)
            _line2 = transform.GetChild(0).GetComponent<LineRenderer>();
    }
    #endregion

    #region Public Methods
    public void DrawLine(params Vector3[] points)
    {
        _line.SetPositions(points);
        _line2.SetPositions(points);
        _line.enabled = true;
        _line2.enabled = true;
    }

    public void ClearLine()
    {
        _line.enabled = false;
        _line2.enabled = false;
    }

    public Vector3[] GetPoints()
    {
        Vector3[] points = new Vector3[_line.positionCount];
        for (int i = 0; i < _line.positionCount; i++)
        {
            points[i] = _line.GetPosition(i);
        }
        return points;
    }

    #endregion


}
