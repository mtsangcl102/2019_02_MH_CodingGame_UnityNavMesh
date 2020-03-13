using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalAnimation : MonoBehaviour
{
    [SerializeField] private Transform _obj;
    [SerializeField] private Vector3 _rotSpeed;
    [SerializeField] private float _jumpAmp, _jumpFeq;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _obj.localPosition = new Vector3(0f, Mathf.Abs(Mathf.Sin(Time.time * _jumpFeq)) * _jumpAmp, 0f);
        
        _obj.Rotate(_rotSpeed);
    }
}
